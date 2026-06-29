<?php
declare(strict_types=1);
require_once __DIR__ . '/includes/config.php';
require_once __DIR__ . '/includes/Database.php';

$installed = file_exists(DB_PATH);
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">
    <meta name="theme-color" content="#3d2314">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="mobile-web-app-capable" content="yes">
    <title><?= htmlspecialchars(APP_NAME) ?> — Mesero</title>
    <link rel="icon" href="data:image/svg+xml,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'><rect fill='%233d2314' width='100' height='100' rx='12'/><circle cx='50' cy='40' r='18' fill='%23c8860a'/><rect x='42' y='55' width='16' height='22' rx='3' fill='%23f5ebe0'/></svg>">
    <link rel="stylesheet" href="assets/css/app.css">
    <link rel="manifest" href="manifest.json">
</head>
<body>
    <?php if (!$installed): ?>
    <div class="install-banner">
        <p>La aplicación aún no está instalada.</p>
        <a href="install.php" class="btn btn-primary">Instalar ahora</a>
    </div>
    <?php endif; ?>

    <header class="app-header">
        <div class="header-top">
            <h1><?= htmlspecialchars(APP_NAME) ?></h1>
            <button type="button" class="btn-icon" id="btnSettings" aria-label="Configuración">⚙️</button>
        </div>
        <div class="order-meta">
            <label>
                Mesa
                <input type="text" id="tableNumber" placeholder="Ej: 5" inputmode="numeric" autocomplete="off">
            </label>
            <label>
                Mesero
                <input type="text" id="waiterName" placeholder="Tu nombre" autocomplete="name">
            </label>
        </div>
        <div class="category-tabs" id="categoryTabs" role="tablist"></div>
    </header>

    <main class="menu-grid" id="menuGrid" aria-live="polite"></main>

    <aside class="cart-panel" id="cartPanel" aria-label="Carrito de pedido">
        <button type="button" class="cart-toggle" id="cartToggle">
            <span class="cart-count" id="cartCount">0</span>
            <span class="cart-label">Ver pedido</span>
            <span class="cart-total" id="cartTotalPreview">$0.00</span>
        </button>

        <div class="cart-content" id="cartContent">
            <div class="cart-header">
                <h2>Pedido actual</h2>
                <button type="button" class="btn-icon" id="closeCart" aria-label="Cerrar">✕</button>
            </div>
            <ul class="cart-items" id="cartItems"></ul>
            <div class="cart-summary">
                <div class="summary-row total-row">
                    <span>Total</span>
                    <span id="cartTotal">$0.00</span>
                </div>
            </div>
            <div class="cart-actions">
                <button type="button" class="btn btn-secondary" id="btnClearCart">Vaciar</button>
                <button type="button" class="btn btn-primary" id="btnSendOrder">Enviar comanda</button>
            </div>
        </div>
    </aside>

    <!-- Modal personalización de producto -->
    <dialog class="modal" id="itemModal">
        <form method="dialog" id="itemForm">
            <div class="modal-header">
                <h2 id="modalItemName">Producto</h2>
                <button type="button" class="btn-icon modal-close" id="modalClose" aria-label="Cerrar">✕</button>
            </div>
            <p class="modal-description" id="modalItemDesc"></p>
            <p class="modal-price" id="modalItemPrice"></p>

            <div class="form-group">
                <label for="itemQuantity">Cantidad</label>
                <div class="quantity-control">
                    <button type="button" class="qty-btn" id="qtyMinus" aria-label="Menos">−</button>
                    <input type="number" id="itemQuantity" value="1" min="1" max="99" inputmode="numeric">
                    <button type="button" class="qty-btn" id="qtyPlus" aria-label="Más">+</button>
                </div>
            </div>

            <div class="form-group" id="ingredientsGroup" hidden>
                <h3>Quitar ingredientes</h3>
                <div class="chip-list" id="ingredientsList"></div>
            </div>

            <div class="form-group" id="extrasGroup" hidden>
                <h3>Agregados</h3>
                <div class="chip-list" id="extrasList"></div>
            </div>

            <div class="form-group">
                <label for="itemNotes">Notas especiales</label>
                <textarea id="itemNotes" rows="2" placeholder="Ej: poco picante, sin hielo..."></textarea>
            </div>

            <div class="modal-total">
                <span>Subtotal línea:</span>
                <strong id="modalLineTotal">$0.00</strong>
            </div>

            <div class="modal-actions">
                <button type="button" class="btn btn-secondary" id="modalCancel">Cancelar</button>
                <button type="submit" class="btn btn-primary" id="modalAdd">Agregar al pedido</button>
            </div>
        </form>
    </dialog>

    <!-- Modal configuración -->
    <dialog class="modal" id="settingsModal">
        <form method="dialog">
            <div class="modal-header">
                <h2>Configuración</h2>
                <button type="button" class="btn-icon modal-close" id="settingsClose" aria-label="Cerrar">✕</button>
            </div>
            <div class="form-group">
                <label for="cafeName">Nombre del café</label>
                <input type="text" id="cafeName" value="<?= htmlspecialchars(APP_NAME) ?>">
            </div>
            <div class="form-group">
                <label for="printerMode">Modo de impresión</label>
                <select id="printerMode">
                    <option value="bluetooth">Bluetooth (Android)</option>
                    <option value="browser">Imprimir desde navegador</option>
                    <option value="network">Impresora de red (servidor)</option>
                </select>
            </div>
            <div class="form-group" id="btPrinterGroup">
                <label>Impresora Bluetooth</label>
                <button type="button" class="btn btn-secondary" id="btnConnectPrinter">Conectar impresora</button>
                <p class="hint" id="printerStatus">No conectada</p>
            </div>
            <div class="modal-actions">
                <button type="submit" class="btn btn-primary">Guardar</button>
            </div>
        </form>
    </dialog>

    <!-- Área de impresión oculta -->
    <div id="printArea" class="print-area" hidden></div>

    <script src="assets/js/printer.js"></script>
    <script src="assets/js/app.js"></script>
</body>
</html>
