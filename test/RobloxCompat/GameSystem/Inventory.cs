using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public class Item
    {
        private static int _nextId = 1;

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public ItemRarity Rarity { get; }
        public int StackSize { get; set; } = 1;
        public int MaxStackSize { get; }
        public Dictionary<string, float> StatBonuses { get; } = new Dictionary<string, float>();

        public Item(string name, string description, ItemRarity rarity, int maxStackSize = 1)
        {
            Id = _nextId++;
            Name = name;
            Description = description;
            Rarity = rarity;
            MaxStackSize = maxStackSize;
        }

        public Item WithStatBonus(string stat, float value)
        {
            StatBonuses[stat] = value;
            return this;
        }

        public bool CanStack => MaxStackSize > 1;

        public override string ToString()
        {
            return $"[{Rarity}] {Name} x{StackSize}";
        }
    }

    public class InventorySlot
    {
        public Item Item { get; set; }
        public bool IsEmpty => Item == null;

        public bool TryAddItem(Item item, out int remaining)
        {
            remaining = item.StackSize;

            if (IsEmpty)
            {
                Item = item;
                remaining = 0;
                return true;
            }

            if (Item.Name == item.Name && Item.CanStack)
            {
                int space = Item.MaxStackSize - Item.StackSize;
                int toAdd = Math.Min(space, item.StackSize);
                Item.StackSize += toAdd;
                remaining = item.StackSize - toAdd;
                return remaining == 0;
            }

            return false;
        }

        public Item TakeItem(int count = 1)
        {
            if (IsEmpty) return null;

            if (count >= Item.StackSize)
            {
                var item = Item;
                Item = null;
                return item;
            }

            Item.StackSize -= count;
            return new Item(Item.Name, Item.Description, Item.Rarity, Item.MaxStackSize)
            {
                StackSize = count
            };
        }
    }

    public class InventoryComponent : IComponent
    {
        public string Name => "Inventory";

        private readonly List<InventorySlot> _slots;
        public int Capacity { get; }

        public event Action<Item> OnItemAdded;
        public event Action<Item> OnItemRemoved;

        public InventoryComponent(int capacity)
        {
            Capacity = capacity;
            _slots = new List<InventorySlot>();
            for (int i = 0; i < capacity; i++)
            {
                _slots.Add(new InventorySlot());
            }
        }

        public bool AddItem(Item item)
        {
            // First try to stack with existing items
            foreach (var slot in _slots.Where(s => !s.IsEmpty && s.Item.Name == item.Name && s.Item.CanStack))
            {
                if (slot.TryAddItem(item, out int remaining))
                {
                    OnItemAdded?.Invoke(item);
                    return true;
                }
                if (remaining == 0) break;
            }

            // Then try empty slots
            foreach (var slot in _slots.Where(s => s.IsEmpty))
            {
                if (slot.TryAddItem(item, out _))
                {
                    OnItemAdded?.Invoke(item);
                    return true;
                }
            }

            return false;
        }

        public Item RemoveItem(string itemName, int count = 1)
        {
            var slot = _slots.FirstOrDefault(s => !s.IsEmpty && s.Item.Name == itemName);
            if (slot == null) return null;

            var item = slot.TakeItem(count);
            if (item != null)
            {
                OnItemRemoved?.Invoke(item);
            }
            return item;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _slots.Where(s => !s.IsEmpty).Select(s => s.Item);
        }

        public IEnumerable<Item> GetItemsByRarity(ItemRarity rarity)
        {
            return GetAllItems().Where(i => i.Rarity == rarity);
        }

        public int CountItem(string itemName)
        {
            return _slots
                .Where(s => !s.IsEmpty && s.Item.Name == itemName)
                .Sum(s => s.Item.StackSize);
        }

        public bool HasItem(string itemName, int count = 1)
        {
            return CountItem(itemName) >= count;
        }

        public int UsedSlots => _slots.Count(s => !s.IsEmpty);
        public int FreeSlots => Capacity - UsedSlots;

        public void Update(float deltaTime)
        {
            // Inventory doesn't need regular updates
        }
    }
}
