<?php
declare(strict_types=1);

define('APP_NAME', 'Artemisa Salón de Té');
define('APP_VERSION', '1.1.0');
define('BASE_PATH', dirname(__DIR__));
define('DATA_PATH', BASE_PATH . '/data');
define('DB_PATH', DATA_PATH . '/cafe.db');

define('PRINTER_ENABLED', false);
define('PRINTER_HOST', '192.168.1.100');
define('PRINTER_PORT', 9100);
define('PRINTER_TIMEOUT', 5);

date_default_timezone_set('America/Santiago');

header('X-Content-Type-Options: nosniff');
header('X-Frame-Options: SAMEORIGIN');
header('Referrer-Policy: strict-origin-when-cross-origin');
