# Cómo probar localmente

## Requisitos

- **PHP 7.4+** (recomendado 8.x)
- Extensiones: `pdo_sqlite`, `zip`, `json`

### Instalar PHP (si no lo tienes)

**Ubuntu / Debian:**
```bash
sudo apt update
sudo apt install php php-sqlite3 php-zip
```

**macOS (Homebrew):**
```bash
brew install php
```

**Windows:**
Descarga PHP desde [https://windows.php.net/download/](https://windows.php.net/download/) o usa XAMPP/WAMP.

## Iniciar el servidor

Desde la carpeta del proyecto:

```bash
chmod +x scripts/serve-local.sh
./scripts/serve-local.sh
```

O manualmente:

```bash
php -S localhost:8080 -t .
```

Abre en el navegador: **http://localhost:8080**

## Pasos de prueba

### 1. Instalar
1. Ve a `http://localhost:8080/install.php`
2. Define un PIN (ej. `1234`) y confirma
3. Se carga la carta Artemisa 2026

### 2. Login (logo + PIN)
1. Ve a `http://localhost:8080/login.php`
2. Verás el **logo de Artemisa** y el teclado numérico
3. Ingresa tu PIN

### 3. Tomar pedidos (mesero)
1. En `http://localhost:8080/index.php` elige productos
2. En el carrito verás:
   - **Subtotal productos**
   - **Propina 10%** (puedes activar/desactivar)
   - **Total con propina**
3. Envía la comanda (la impresión en local usará modo navegador si no hay Bluetooth)

### 4. Probar en el celular (misma red WiFi)

1. Obtén la IP de tu PC:
   - Linux/macOS: `hostname -I` o `ipconfig getifaddr en0`
   - Windows: `ipconfig`
2. En el celular abre: `http://TU_IP:8080/login.php`
3. Asegúrate de que el firewall permita el puerto 8080

> Web Bluetooth para impresora solo funciona con **HTTPS** en producción, o en `localhost` en algunos casos.

### 5. Reporte de ventas
1. Entra a `http://localhost:8080/admin/`
2. Sección **Reporte de ventas del día**
3. **Ver resumen** — muestra productos, propinas y total
4. **Descargar Excel (CSV)** — archivo compatible con Excel
5. **Enviar por correo** — intenta enviar a `richardrojas.cid@gmail.com`

> En local, `mail()` suele fallar. El reporte se guarda automáticamente en `data/reports/` como respaldo.

### 6. Simular ventas para el reporte

Haz 2-3 pedidos desde la app del mesero con propina activada, luego genera el reporte en admin.

## Producción en Hostgator

1. Sube archivos a `public_html`
2. Activa SSL (HTTPS)
3. Edita `includes/config.php` → `MAIL_FROM` con un correo de tu dominio (ej. `no-reply@tudominio.com`)
4. En admin configura el correo de reportes
5. Opcional: cron diario con `scripts/send-daily-report.php`

## Solución de problemas

| Problema | Solución |
|----------|----------|
| `php: command not found` | Instala PHP (ver arriba) |
| Error permisos `/data` | `chmod 775 data` |
| Correo no llega en local | Normal; revisa `data/reports/` |
| Correo no llega en Hostgator | Configura `MAIL_FROM` con dominio válido en cPanel |
| Sesión expirada en API | Vuelve a `login.php` |
