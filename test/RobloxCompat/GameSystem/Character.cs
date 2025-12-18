using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Paladin
    }

    /// <summary>
    /// Factory for creating pre-configured character entities
    /// </summary>
    public static class CharacterFactory
    {
        public static Entity CreateCharacter(string name, CharacterClass characterClass, int level = 1)
        {
            var entity = new Entity(name);

            // Add stats component
            var stats = new StatsComponent();
            entity.AddComponent(stats);

            // Set base stats based on class
            SetBaseStats(stats, characterClass, level);

            // Add inventory
            var inventory = new InventoryComponent(20);
            entity.AddComponent(inventory);

            // Add combat component
            var combat = new CombatComponent(entity);
            entity.AddComponent(combat);

            // Give starting items
            GiveStartingItems(inventory, characterClass);

            return entity;
        }

        private static void SetBaseStats(StatsComponent stats, CharacterClass characterClass, int level)
        {
            // Base stats that scale with level
            float healthBase, manaBase, attackBase, defenseBase;

            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    healthBase = 150;
                    manaBase = 30;
                    attackBase = 15;
                    defenseBase = 12;
                    break;
                case CharacterClass.Mage:
                    healthBase = 80;
                    manaBase = 150;
                    attackBase = 8;
                    defenseBase = 5;
                    break;
                case CharacterClass.Rogue:
                    healthBase = 100;
                    manaBase = 50;
                    attackBase = 12;
                    defenseBase = 7;
                    break;
                case CharacterClass.Paladin:
                    healthBase = 120;
                    manaBase = 80;
                    attackBase = 10;
                    defenseBase = 15;
                    break;
                default:
                    healthBase = 100;
                    manaBase = 50;
                    attackBase = 10;
                    defenseBase = 10;
                    break;
            }

            // Scale with level
            float levelMultiplier = 1 + (level - 1) * 0.1f;

            stats.GetOrCreateStat("Health", healthBase * levelMultiplier);
            stats.GetOrCreateStat("MaxHealth", healthBase * levelMultiplier);
            stats.GetOrCreateStat("Mana", manaBase * levelMultiplier);
            stats.GetOrCreateStat("MaxMana", manaBase * levelMultiplier);
            stats.GetOrCreateStat("AttackPower", attackBase * levelMultiplier);
            stats.GetOrCreateStat("Defense", defenseBase * levelMultiplier);
            stats.GetOrCreateStat("CritChance", characterClass == CharacterClass.Rogue ? 15 : 5);
            stats.GetOrCreateStat("CritMultiplier", 150);
            stats.GetOrCreateStat("DodgeChance", characterClass == CharacterClass.Rogue ? 10 : 3);
            stats.GetOrCreateStat("Level", level);
        }

        private static void GiveStartingItems(InventoryComponent inventory, CharacterClass characterClass)
        {
            // Give a health potion to everyone
            var healthPotion = new Item("Health Potion", "Restores 50 health", ItemRarity.Common, 10)
            {
                StackSize = 3
            };
            inventory.AddItem(healthPotion);

            // Class-specific starting weapon
            Item weapon;
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    weapon = new Item("Iron Sword", "A sturdy iron sword", ItemRarity.Common)
                        .WithStatBonus("AttackPower", 5);
                    break;
                case CharacterClass.Mage:
                    weapon = new Item("Apprentice Staff", "A basic magic staff", ItemRarity.Common)
                        .WithStatBonus("AttackPower", 3)
                        .WithStatBonus("MaxMana", 20);
                    break;
                case CharacterClass.Rogue:
                    weapon = new Item("Steel Dagger", "A sharp dagger", ItemRarity.Common)
                        .WithStatBonus("AttackPower", 4)
                        .WithStatBonus("CritChance", 5);
                    break;
                case CharacterClass.Paladin:
                    weapon = new Item("Blessed Mace", "A holy weapon", ItemRarity.Uncommon)
                        .WithStatBonus("AttackPower", 4)
                        .WithStatBonus("Defense", 3);
                    break;
                default:
                    weapon = new Item("Wooden Stick", "Better than nothing", ItemRarity.Common)
                        .WithStatBonus("AttackPower", 1);
                    break;
            }
            inventory.AddItem(weapon);
        }
    }

    /// <summary>
    /// Extension methods for working with character entities
    /// </summary>
    public static class CharacterExtensions
    {
        public static bool IsAlive(this Entity entity)
        {
            var stats = entity.GetComponent<StatsComponent>();
            if (stats == null) return entity.IsActive;

            return stats.GetStatValue("Health") > 0;
        }

        public static float GetHealthPercent(this Entity entity)
        {
            var stats = entity.GetComponent<StatsComponent>();
            if (stats == null) return 100;

            float health = stats.GetStatValue("Health");
            float maxHealth = stats.GetStatValue("MaxHealth");
            if (maxHealth == 0) return 100;

            return (health / maxHealth) * 100;
        }

        public static void Heal(this Entity entity, float amount)
        {
            var stats = entity.GetComponent<StatsComponent>();
            if (stats == null) return;

            var healthStat = stats.GetStat("Health");
            var maxHealthStat = stats.GetStat("MaxHealth");
            if (healthStat == null || maxHealthStat == null) return;

            healthStat.BaseValue = Math.Min(healthStat.BaseValue + amount, maxHealthStat.Value);
        }

        public static string GetStatusReport(this Entity entity)
        {
            var stats = entity.GetComponent<StatsComponent>();
            var inventory = entity.GetComponent<InventoryComponent>();

            var lines = new List<string>
            {
                $"=== {entity.Name} ===",
                $"Active: {entity.IsActive}"
            };

            if (stats != null)
            {
                lines.Add($"Health: {stats.GetStatValue("Health"):F0}/{stats.GetStatValue("MaxHealth"):F0}");
                lines.Add($"Mana: {stats.GetStatValue("Mana"):F0}/{stats.GetStatValue("MaxMana"):F0}");
                lines.Add($"Attack: {stats.GetStatValue("AttackPower"):F1}");
                lines.Add($"Defense: {stats.GetStatValue("Defense"):F1}");
                lines.Add($"Crit: {stats.GetStatValue("CritChance"):F0}%");
            }

            if (inventory != null)
            {
                lines.Add($"Inventory: {inventory.UsedSlots}/{inventory.Capacity}");
                foreach (var item in inventory.GetAllItems())
                {
                    lines.Add($"  - {item}");
                }
            }

            return string.Join("\n", lines);
        }
    }
}
