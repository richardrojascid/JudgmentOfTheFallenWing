<?php
declare(strict_types=1);

define('APP_NAME', 'Café Comanda');
define('APP_VERSION', '1.0.0');
define('BASE_PATH', dirname(__DIR__));
define('DATA_PATH', BASE_PATH . '/data');
define('DB_PATH', DATA_PATH . '/cafe.db');

// Configuración de impresora térmica de red (ESC/POS)
// Ajusta estos valores en Hostgator según tu impresora WiFi/Ethernet
define('PRINTER_ENABLED', false);
define('PRINTER_HOST', '192.168.1.100');
define('PRINTER_PORT', 9100);
define('PRINTER_TIMEOUT', 5);

// Zona horaria (ajusta según tu país)
date_default_timezone_set('America/Mexico_City');

// Cabeceras de seguridad básicas
header('X-Content-Type-Options: nosniff');
header('X-Frame-Options: SAMEORIGIN');
header('Referrer-Policy: strict-origin-when-cross-origin');
