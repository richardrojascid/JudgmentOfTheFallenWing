<?php
declare(strict_types=1);

class Database
{
    private static ?PDO $instance = null;

    public static function getConnection(): PDO
    {
        if (self::$instance === null) {
            if (!is_dir(DATA_PATH)) {
                mkdir(DATA_PATH, 0755, true);
            }

            self::$instance = new PDO('sqlite:' . DB_PATH);
            self::$instance->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
            self::$instance->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
            self::$instance->exec('PRAGMA foreign_keys = ON');
        }

        return self::$instance;
    }

    public static function initialize(): void
    {
        $db = self::getConnection();

        $db->exec("
            CREATE TABLE IF NOT EXISTS categories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                sort_order INTEGER NOT NULL DEFAULT 0,
                active INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS menu_items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                category_id INTEGER NOT NULL,
                name TEXT NOT NULL,
                description TEXT,
                price REAL NOT NULL,
                active INTEGER NOT NULL DEFAULT 1,
                sort_order INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS ingredients (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                menu_item_id INTEGER NOT NULL,
                name TEXT NOT NULL,
                removable INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS extras (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                menu_item_id INTEGER NOT NULL,
                name TEXT NOT NULL,
                price REAL NOT NULL DEFAULT 0,
                FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS orders (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                table_number TEXT,
                waiter_name TEXT,
                subtotal REAL NOT NULL,
                total REAL NOT NULL,
                status TEXT NOT NULL DEFAULT 'pending',
                created_at TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS order_items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER NOT NULL,
                menu_item_id INTEGER,
                item_name TEXT NOT NULL,
                unit_price REAL NOT NULL,
                quantity INTEGER NOT NULL DEFAULT 1,
                extras_total REAL NOT NULL DEFAULT 0,
                line_total REAL NOT NULL,
                removed_ingredients TEXT,
                added_extras TEXT,
                notes TEXT,
                FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
            );
        ");

        $count = (int) $db->query('SELECT COUNT(*) FROM categories')->fetchColumn();
        if ($count === 0) {
            self::seedMenu($db);
        }
    }

    private static function seedMenu(PDO $db): void
    {
        $categories = [
            ['Bebidas calientes', 1],
            ['Bebidas frías', 2],
            ['Desayunos', 3],
            ['Alimentos', 4],
            ['Postres', 5],
        ];

        $stmtCat = $db->prepare('INSERT INTO categories (name, sort_order) VALUES (?, ?)');
        foreach ($categories as [$name, $order]) {
            $stmtCat->execute([$name, $order]);
        }

        $items = [
            // Bebidas calientes
            [1, 'Café americano', 'Café negro tradicional', 35.00, 1, ['Café', 'Agua'], [['Leche extra', 8], ['Shot espresso', 12]]],
            [1, 'Capuchino', 'Espresso con leche espumada', 45.00, 2, ['Espresso', 'Leche', 'Espuma'], [['Canela', 0], ['Jarabe vainilla', 10]]],
            [1, 'Latte', 'Espresso con leche vaporizada', 48.00, 3, ['Espresso', 'Leche'], [['Leche de almendra', 12], ['Caramelo', 10]]],
            [1, 'Chocolate caliente', 'Chocolate artesanal', 42.00, 4, ['Chocolate', 'Leche'], [['Malvaviscos', 8]]],
            // Bebidas frías
            [2, 'Frappé de café', 'Café helado batido', 55.00, 1, ['Café', 'Hielo', 'Leche'], [['Crema batida', 10], ['Caramelo', 8]]],
            [2, 'Limonada natural', 'Limones frescos', 38.00, 2, ['Limón', 'Agua', 'Azúcar'], [['Menta', 5]]],
            [2, 'Agua de horchata', 'Receta casera', 35.00, 3, ['Arroz', 'Canela', 'Leche'], []],
            // Desayunos
            [3, 'Chilaquiles', 'Con salsa verde o roja', 85.00, 1, ['Totopos', 'Salsa', 'Queso', 'Crema', 'Cebolla'], [['Huevo extra', 15], ['Pollo', 25]]],
            [3, 'Huevos rancheros', 'Huevos estrellados con salsa', 78.00, 2, ['Huevo', 'Tortilla', 'Frijoles', 'Salsa'], [['Aguacate', 18]]],
            [3, 'Hot cakes', '3 piezas con miel', 72.00, 3, ['Harina', 'Huevo', 'Leche'], [['Frutas', 15], ['Nutella', 20]]],
            // Alimentos
            [4, 'Sándwich club', 'Pollo, jamón, queso y vegetales', 95.00, 1, ['Pan', 'Pollo', 'Jamón', 'Queso', 'Lechuga', 'Tomate'], [['Papas fritas', 20]]],
            [4, 'Ensalada César', 'Lechuga romana con aderezo', 88.00, 2, ['Lechuga', 'Crutones', 'Parmesano', 'Aderezo'], [['Pollo a la plancha', 30], ['Camarones', 45]]],
            [4, 'Quesadillas', '3 piezas con queso fundido', 75.00, 3, ['Tortilla', 'Queso'], [['Champiñones', 12], ['Chorizo', 18]]],
            // Postres
            [5, 'Pastel de chocolate', 'Porción generosa', 55.00, 1, ['Chocolate', 'Harina', 'Huevo'], [['Helado', 15]]],
            [5, 'Cheesecake', 'Con mermelada de frutos rojos', 58.00, 2, ['Queso crema', 'Galleta'], []],
            [5, 'Brownie', 'Con nuez', 48.00, 3, ['Chocolate', 'Nuez'], [['Helado de vainilla', 15]]],
        ];

        $stmtItem = $db->prepare('INSERT INTO menu_items (category_id, name, description, price, sort_order) VALUES (?, ?, ?, ?, ?)');
        $stmtIng = $db->prepare('INSERT INTO ingredients (menu_item_id, name) VALUES (?, ?)');
        $stmtExt = $db->prepare('INSERT INTO extras (menu_item_id, name, price) VALUES (?, ?, ?)');

        foreach ($items as [$catId, $name, $desc, $price, $sort, $ings, $extras]) {
            $stmtItem->execute([$catId, $name, $desc, $price, $sort]);
            $itemId = (int) $db->lastInsertId();

            foreach ($ings as $ing) {
                $stmtIng->execute([$itemId, $ing]);
            }
            foreach ($extras as [$extName, $extPrice]) {
                $stmtExt->execute([$itemId, $extName, $extPrice]);
            }
        }
    }
}
