<?php
declare(strict_types=1);

require_once dirname(__DIR__) . '/includes/config.php';
require_once dirname(__DIR__) . '/includes/Database.php';
require_once dirname(__DIR__) . '/includes/MenuRepository.php';

header('Content-Type: application/json; charset=utf-8');

try {
    if (!file_exists(DB_PATH)) {
        http_response_code(503);
        echo json_encode(['error' => 'La aplicación no está instalada. Visita /install.php']);
        exit;
    }

    Database::initialize();
    $menu = new MenuRepository(Database::getConnection());

    echo json_encode([
        'success' => true,
        'app' => APP_NAME,
        'categories' => $menu->getFullMenu(),
    ], JSON_UNESCAPED_UNICODE);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(['error' => 'Error al cargar el menú.']);
}
