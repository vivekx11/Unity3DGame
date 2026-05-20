using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Simple grid-based inventory using ScriptableObject items. This is a minimal runtime representation; UI is outside the scope but events are provided.
    /// Editor tips: Create ItemData assets via CreateAssetMenu.
    /// Design notes: No serialization here; SaveSystem should capture inventory.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        public int width = 6;
        public int height = 4;
        public ItemData[,] grid;

        private void Awake()
        {
            grid = new ItemData[width, height];
        }

        public bool AddItem(ItemData item)
        {
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    grid[x, y] = item;
                    return true;
                }
            }
            return false;
        }

        public bool RemoveItemAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return false;
            grid[x, y] = null;
            return true;
        }
    }

    [CreateAssetMenu(menuName = "Game/Items/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public int width = 1;
        public int height = 1;
    }
}
