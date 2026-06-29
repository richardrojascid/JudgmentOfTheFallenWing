# Café Comanda — Sistema de pedidos para meseros

Aplicación web responsiva para que los meseros de una cafetería tomen pedidos desde su celular Android o tablet, con personalización de productos, cálculo de precios y impresión en impresora térmica.

## Características

- **Carta digital** organizada por categorías (bebidas, desayunos, alimentos, postres)
- **Selección múltiple** de productos con cantidad ajustable
- **Personalización**: quitar ingredientes, agregar extras con precio
- **Total en tiempo real** del pedido
- **Diseño responsivo** optimizado para móvil y tablet
- **Impresión térmica** vía Bluetooth (Android/Chrome), impresora de red o navegador
- **Compatible con Hostgator** (PHP + SQLite, sin base de datos MySQL obligatoria)
- **HTTPS** forzado mediante `.htaccess`

## Requisitos del servidor (Hostgator)

| Requisito | Detalle |
|-----------|---------|
| PHP | 7.4 o superior (recomendado 8.x) |
| Extensiones | PDO, PDO_SQLite, JSON |
| Permisos | Carpeta `/data` escribible (chmod 755 o 775) |
| SSL | Certificado Let's Encrypt (gratis en Hostgator) |

## Instalación en Hostgator

### 1. Subir archivos

1. Accede al **cPanel** de Hostgator → **Administrador de archivos**
2. Navega a `public_html` (o subcarpeta de tu dominio, ej. `public_html/comanda`)
3. Sube todos los archivos del proyecto manteniendo la estructura de carpetas

### 2. Activar HTTPS

1. En cPanel → **SSL/TLS Status** → activa Let's Encrypt para tu dominio
2. El archivo `.htaccess` incluido redirige automáticamente HTTP → HTTPS

### 3. Instalar la aplicación

1. Visita `https://tudominio.com/install.php`
2. Haz clic en **Instalar base de datos y menú**
3. Accede a `https://tudominio.com/` para usar la app

### 4. Configurar impresora térmica

#### Opción A: Bluetooth (recomendado para Android)

1. Empareja la impresora térmica con el celular Android
2. Abre la app en **Chrome** (Web Bluetooth requiere HTTPS)
3. Ve a **Configuración** → **Conectar impresora**
4. Selecciona tu impresora de la lista

#### Opción B: Impresora de red (WiFi/Ethernet)

1. Edita `includes/config.php`:
   ```php
   define('PRINTER_ENABLED', true);
   define('PRINTER_HOST', '192.168.1.100'); // IP de tu impresora
   define('PRINTER_PORT', 9100);
   ```
2. En la app, selecciona modo **Impresora de red** en Configuración

> **Nota:** La impresora de red debe estar en la misma red que el servidor, o accesible desde Hostgator (poco común en hosting compartido). Para la mayoría de cafeterías, Bluetooth desde el celular del mesero es la opción más práctica.

#### Opción C: Imprimir desde navegador

Selecciona **Imprimir desde navegador** en Configuración. Android puede enviar a impresoras Bluetooth emparejadas vía el servicio de impresión del sistema.

## Uso para meseros

1. Ingresa **número de mesa** y **nombre del mesero**
2. Navega las categorías y toca un producto
3. Ajusta **cantidad**, quita ingredientes o agrega extras
4. Toca **Agregar al pedido**
5. Repite para más productos
6. Abre el carrito inferior y revisa el **total**
7. Toca **Enviar comanda** para guardar e imprimir

## Estructura del proyecto

```
├── index.php              # Interfaz principal del mesero
├── install.php            # Instalador
├── manifest.json          # PWA para agregar a pantalla de inicio
├── .htaccess              # HTTPS y seguridad
├── api/
│   ├── menu.php           # API del menú
│   ├── orders.php         # API de pedidos
│   └── print.php          # Generación e impresión ESC/POS
├── assets/
│   ├── css/app.css        # Estilos responsivos
│   └── js/
│       ├── app.js         # Lógica del mesero
│       └── printer.js     # Impresión térmica
├── includes/
│   ├── config.php         # Configuración
│   ├── Database.php       # SQLite y menú de ejemplo
│   ├── MenuRepository.php
│   ├── OrderService.php
│   └── EscPosPrinter.php  # Comandos ESC/POS
└── data/
    └── cafe.db            # Base de datos (se crea al instalar)
```

## Personalizar el menú

El menú de ejemplo se carga automáticamente en la instalación. Para modificarlo:

- Edita directamente la base SQLite en `/data/cafe.db` con un gestor como [DB Browser for SQLite](https://sqlitebrowser.org/)
- O modifica los datos de ejemplo en `includes/Database.php` antes de instalar

Tablas: `categories`, `menu_items`, `ingredients`, `extras`

## Seguridad recomendada

- Elimina o protege `install.php` después de instalar
- Mantén HTTPS activo siempre
- La carpeta `/data` está protegida por `.htaccess`
- Considera agregar autenticación básica en cPanel para restringir acceso

## Licencia

MIT
