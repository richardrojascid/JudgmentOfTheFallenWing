<?php
declare(strict_types=1);

class MenuRepository
{
    private PDO $db;

    public function __construct(PDO $db)
    {
        $this->db = $db;
    }

    public function getFullMenu(): array
    {
        $categories = $this->db->query("
            SELECT id, name, sort_order
            FROM categories
            WHERE active = 1
            ORDER BY sort_order, name
        ")->fetchAll();

        $itemsStmt = $this->db->query("
            SELECT id, category_id, name, description, price, sort_order
            FROM menu_items
            WHERE active = 1
            ORDER BY sort_order, name
        ");
        $items = $itemsStmt->fetchAll();

        $ingsStmt = $this->db->query('SELECT menu_item_id, id, name, removable FROM ingredients');
        $ingsByItem = [];
        foreach ($ingsStmt->fetchAll() as $ing) {
            $ingsByItem[$ing['menu_item_id']][] = $ing;
        }

        $extrasStmt = $this->db->query('SELECT menu_item_id, id, name, price FROM extras');
        $extrasByItem = [];
        foreach ($extrasStmt->fetchAll() as $extra) {
            $extrasByItem[$extra['menu_item_id']][] = $extra;
        }

        $itemsByCategory = [];
        foreach ($items as $item) {
            $itemId = $item['id'];
            $item['ingredients'] = $ingsByItem[$itemId] ?? [];
            $item['extras'] = $extrasByItem[$itemId] ?? [];
            $item['price'] = (float) $item['price'];
            $itemsByCategory[$item['category_id']][] = $item;
        }

        foreach ($categories as &$cat) {
            $cat['items'] = $itemsByCategory[$cat['id']] ?? [];
        }

        return $categories;
    }

    public function getItemById(int $id): ?array
    {
        $stmt = $this->db->prepare('SELECT * FROM menu_items WHERE id = ? AND active = 1');
        $stmt->execute([$id]);
        $item = $stmt->fetch();
        if (!$item) {
            return null;
        }

        $item['price'] = (float) $item['price'];

        $stmtIng = $this->db->prepare('SELECT id, name, removable FROM ingredients WHERE menu_item_id = ?');
        $stmtIng->execute([$id]);
        $item['ingredients'] = $stmtIng->fetchAll();

        $stmtExt = $this->db->prepare('SELECT id, name, price FROM extras WHERE menu_item_id = ?');
        $stmtExt->execute([$id]);
        $item['extras'] = array_map(function ($e) {
            $e['price'] = (float) $e['price'];
            return $e;
        }, $stmtExt->fetchAll());

        return $item;
    }
}
