<?php
declare(strict_types=1);
require_once __DIR__ . '/includes/config.php';
require_once __DIR__ . '/includes/Database.php';

$message = '';
$error = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    try {
        Database::initialize();
        $message = '¡Instalación completada! Ya puedes usar la aplicación.';
    } catch (Throwable $e) {
        $error = 'Error durante la instalación. Verifica permisos de escritura en /data';
    }
}

$installed = file_exists(DB_PATH);
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Instalación — <?= htmlspecialchars(APP_NAME) ?></title>
    <link rel="stylesheet" href="assets/css/app.css">
</head>
<body class="install-page">
    <main class="install-card">
        <h1>Instalación</h1>
        <p>Configura la base de datos y el menú de ejemplo para <strong><?= htmlspecialchars(APP_NAME) ?></strong>.</p>

        <?php if ($message): ?>
            <div class="alert alert-success"><?= htmlspecialchars($message) ?></div>
            <a href="index.php" class="btn btn-primary">Ir a la aplicación</a>
        <?php elseif ($installed): ?>
            <div class="alert alert-info">La aplicación ya está instalada.</div>
            <a href="index.php" class="btn btn-primary">Ir a la aplicación</a>
        <?php else: ?>
            <?php if ($error): ?>
                <div class="alert alert-error"><?= htmlspecialchars($error) ?></div>
            <?php endif; ?>
            <form method="post">
                <button type="submit" class="btn btn-primary">Instalar base de datos y menú</button>
            </form>
        <?php endif; ?>

        <section class="install-requirements">
            <h2>Requisitos Hostgator</h2>
            <ul>
                <li>PHP 7.4 o superior (recomendado PHP 8.x)</li>
                <li>Extensión PDO SQLite habilitada</li>
                <li>Carpeta <code>/data</code> con permisos de escritura (755 o 775)</li>
                <li>Certificado SSL activo para HTTPS</li>
            </ul>
        </section>
    </main>
</body>
</html>
