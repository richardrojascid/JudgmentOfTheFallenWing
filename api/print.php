<?php
declare(strict_types=1);

require_once dirname(__DIR__) . '/includes/config.php';
require_once dirname(__DIR__) . '/includes/Database.php';
require_once dirname(__DIR__) . '/includes/MenuRepository.php';
require_once dirname(__DIR__) . '/includes/OrderService.php';
require_once dirname(__DIR__) . '/includes/EscPosPrinter.php';

header('Content-Type: application/json; charset=utf-8');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Método no permitido.']);
    exit;
}

try {
    if (!file_exists(DB_PATH)) {
        http_response_code(503);
        echo json_encode(['error' => 'La aplicación no está instalada.']);
        exit;
    }

    $input = json_decode(file_get_contents('php://input'), true);
    if (!is_array($input)) {
        throw new InvalidArgumentException('Datos inválidos.');
    }

    Database::initialize();
    $db = Database::getConnection();
    $menu = new MenuRepository($db);
    $orders = new OrderService($db, $menu);

    $order = null;
    $items = [];

    if (!empty($input['order_id'])) {
        $order = $orders->getOrder((int) $input['order_id']);
        if (!$order) {
            throw new InvalidArgumentException('Orden no encontrada.');
        }
        $items = $order['items'];
    } elseif (!empty($input['order'])) {
        $order = $input['order'];
        $items = $order['items'] ?? [];
    } else {
        throw new InvalidArgumentException('Se requiere order_id o datos de order.');
    }

    $cafeName = $input['cafe_name'] ?? APP_NAME;
    $receipt = EscPosPrinter::buildReceipt($order, $items, strtoupper($cafeName));

    $printed = false;
    $printError = null;

    if (PRINTER_ENABLED) {
        $printed = EscPosPrinter::sendToNetwork($receipt, PRINTER_HOST, PRINTER_PORT, PRINTER_TIMEOUT);
        if (!$printed) {
            $printError = 'No se pudo conectar con la impresora de red.';
        }
    }

    echo json_encode([
        'success' => true,
        'printed_network' => $printed,
        'print_error' => $printError,
        'receipt_base64' => base64_encode($receipt),
        'receipt_text' => receiptToPlainText($order, $items, $cafeName),
    ], JSON_UNESCAPED_UNICODE);
} catch (InvalidArgumentException $e) {
    http_response_code(400);
    echo json_encode(['error' => $e->getMessage()]);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(['error' => 'Error al generar la comanda.']);
}

function receiptToPlainText(array $order, array $items, string $cafeName): string
{
    $lines = [];
    $lines[] = strtoupper($cafeName);
    $lines[] = 'COMANDA DE PEDIDO';
    $lines[] = str_repeat('-', 32);
    $lines[] = 'Fecha: ' . date('d/m/Y H:i');

    if (!empty($order['table_number'])) {
        $lines[] = 'Mesa: ' . $order['table_number'];
    }
    if (!empty($order['waiter_name'])) {
        $lines[] = 'Mesero: ' . $order['waiter_name'];
    }
    if (!empty($order['id'])) {
        $lines[] = 'Orden #: ' . $order['id'];
    }

    $lines[] = str_repeat('-', 32);

    foreach ($items as $item) {
        $qty = (int) ($item['quantity'] ?? 1);
        $name = $item['item_name'] ?? $item['name'] ?? 'Producto';
        $lines[] = "{$qty}x {$name}";
        $lines[] = '   Precio unit.: $' . number_format((float) ($item['unit_price'] ?? 0), 2);

        $removed = $item['removed_ingredients'] ?? [];
        if (is_string($removed)) {
            $removed = json_decode($removed, true) ?: [];
        }
        if (!empty($removed)) {
            $lines[] = '   Sin: ' . implode(', ', $removed);
        }

        $extras = $item['added_extras'] ?? [];
        if (is_string($extras)) {
            $extras = json_decode($extras, true) ?: [];
        }
        foreach ($extras as $extra) {
            $extraName = is_array($extra) ? ($extra['name'] ?? '') : $extra;
            $extraPrice = is_array($extra) ? (float) ($extra['price'] ?? 0) : 0;
            $priceStr = $extraPrice > 0 ? ' (+$' . number_format($extraPrice, 2) . ')' : '';
            $lines[] = "   + {$extraName}{$priceStr}";
        }

        if (!empty($item['notes'])) {
            $lines[] = '   Nota: ' . $item['notes'];
        }

        $lines[] = '   Subtotal: $' . number_format((float) ($item['line_total'] ?? 0), 2);
        $lines[] = '';
    }

    $lines[] = str_repeat('-', 32);
    $lines[] = 'TOTAL: $' . number_format((float) ($order['total'] ?? 0), 2);
    $lines[] = str_repeat('-', 32);
    $lines[] = 'Gracias por su preferencia';

    return implode("\n", $lines);
}
