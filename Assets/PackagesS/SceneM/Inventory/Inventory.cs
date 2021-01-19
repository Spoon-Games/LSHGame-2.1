using System;
using System.Collections.Generic;

namespace SceneM
{
    public static class Inventory
    {
        public static Action OnInventoryChanged;

        private static Dictionary<InventoryItem, int> content = new Dictionary<InventoryItem, int>();

        public static void Add(InventoryItem item)
        {
            if (content.TryGetValue(item, out int count))
                content[item] = count + 1;
            else
                content.Add(item, 1);
            OnInventoryChanged?.Invoke();
        }

        public static int GetCount(InventoryItem item)
        {
            if (content.TryGetValue(item, out int count))
                return count;
            else
                return 0;
        }

        internal static void DeleteAllLevelData()
        {
            List<InventoryItem> removeItems = new List<InventoryItem>();
            foreach(var c in content)
            {
                if (c.Key.PersistenceType == PersistenceType.LevelOnly)
                    removeItems.Add(c.Key);
            }

            foreach(InventoryItem inventoryItem in removeItems)
            {
                content.Remove(inventoryItem);
            }
        }
    }
}
