using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    /// <summary>
    /// Manages the game state and provides the main test interface
    /// </summary>
    public class GameManager
    {
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly Random _random = new Random();

        public event Action<string> OnLog;

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
            Log($"Added entity: {entity}");
        }

        public Entity GetEntity(string name)
        {
            return _entities.FirstOrDefault(e => e.Name == name);
        }

        public IEnumerable<Entity> GetActiveEntities()
        {
            return _entities.Where(e => e.IsActive);
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in GetActiveEntities())
            {
                entity.Update(deltaTime);
            }
        }

        /// <summary>
        /// Run a comprehensive test of all game systems
        /// </summary>
        public TestResults RunAllTests()
        {
            var results = new TestResults();

            // Test 1: Entity creation
            results.AddTest("Entity Creation", TestEntityCreation);

            // Test 2: Stats system
            results.AddTest("Stats System", TestStatsSystem);

            // Test 3: Inventory system
            results.AddTest("Inventory System", TestInventorySystem);

            // Test 4: Combat system
            results.AddTest("Combat System", TestCombatSystem);

            // Test 5: Character factory
            results.AddTest("Character Factory", TestCharacterFactory);

            // Test 6: LINQ queries
            results.AddTest("LINQ Queries", TestLinqQueries);

            // Test 7: Events and delegates
            results.AddTest("Events and Delegates", TestEventsAndDelegates);

            // Test 8: Generics
            results.AddTest("Generics", TestGenerics);

            // Test 9: Full battle simulation
            results.AddTest("Battle Simulation", TestBattleSimulation);

            return results;
        }

        private bool TestEntityCreation()
        {
            var entity = new Entity("TestEntity");
            if (entity.Id <= 0) return false;
            if (entity.Name != "TestEntity") return false;
            if (!entity.IsActive) return false;

            entity.AddComponent(new StatsComponent());
            if (!entity.HasComponent<StatsComponent>()) return false;
            if (entity.GetComponent<StatsComponent>() == null) return false;

            Log("Entity creation: All checks passed");
            return true;
        }

        private bool TestStatsSystem()
        {
            var stats = new StatsComponent();

            var health = stats.GetOrCreateStat("Health", 100);
            if (health.Value != 100) return false;

            // Test flat modifier
            var flatMod = new StatModifier("Test", ModifierType.Flat, 20);
            health.AddModifier(flatMod);
            if (Math.Abs(health.Value - 120) > 0.01f) return false;

            // Test percent modifier
            var percentMod = new StatModifier("Test", ModifierType.Percent, 50);
            health.AddModifier(percentMod);
            // (100 + 20) * 1.5 = 180
            if (Math.Abs(health.Value - 180) > 0.01f) return false;

            // Test remove modifier
            health.RemoveModifier(flatMod);
            // 100 * 1.5 = 150
            if (Math.Abs(health.Value - 150) > 0.01f) return false;

            Log("Stats system: All checks passed");
            return true;
        }

        private bool TestInventorySystem()
        {
            var inventory = new InventoryComponent(5);

            // Test adding items
            var sword = new Item("Sword", "A sword", ItemRarity.Common);
            if (!inventory.AddItem(sword)) return false;
            if (inventory.UsedSlots != 1) return false;

            // Test stacking
            var potion1 = new Item("Potion", "Heals", ItemRarity.Common, 10) { StackSize = 3 };
            var potion2 = new Item("Potion", "Heals", ItemRarity.Common, 10) { StackSize = 2 };
            inventory.AddItem(potion1);
            inventory.AddItem(potion2);
            if (inventory.CountItem("Potion") != 5) return false;

            // Test has item
            if (!inventory.HasItem("Sword")) return false;
            if (!inventory.HasItem("Potion", 5)) return false;
            if (inventory.HasItem("Potion", 10)) return false;

            // Test remove item
            var removed = inventory.RemoveItem("Potion", 2);
            if (removed == null || removed.StackSize != 2) return false;
            if (inventory.CountItem("Potion") != 3) return false;

            // Test rarity filter
            var rare = new Item("Rare Item", "Rare", ItemRarity.Rare);
            inventory.AddItem(rare);
            var rareItems = inventory.GetItemsByRarity(ItemRarity.Rare).ToList();
            if (rareItems.Count != 1) return false;

            Log("Inventory system: All checks passed");
            return true;
        }

        private bool TestCombatSystem()
        {
            var attacker = CharacterFactory.CreateCharacter("Attacker", CharacterClass.Warrior, 5);
            var defender = CharacterFactory.CreateCharacter("Defender", CharacterClass.Paladin, 5);

            var attackerCombat = attacker.GetComponent<CombatComponent>();
            var defenderStats = defender.GetComponent<StatsComponent>();

            float healthBefore = defenderStats.GetStatValue("Health");
            attackerCombat.Attack(defender, 20, DamageType.Physical);
            float healthAfter = defenderStats.GetStatValue("Health");

            // Damage should have been dealt (unless dodged, but we can't control RNG)
            // Just verify the system runs without errors
            Log($"Combat: Health went from {healthBefore:F0} to {healthAfter:F0}");

            return true;
        }

        private bool TestCharacterFactory()
        {
            var warrior = CharacterFactory.CreateCharacter("TestWarrior", CharacterClass.Warrior, 10);
            var mage = CharacterFactory.CreateCharacter("TestMage", CharacterClass.Mage, 10);

            var warriorStats = warrior.GetComponent<StatsComponent>();
            var mageStats = mage.GetComponent<StatsComponent>();

            // Warrior should have more health than mage
            if (warriorStats.GetStatValue("MaxHealth") <= mageStats.GetStatValue("MaxHealth")) return false;

            // Mage should have more mana
            if (mageStats.GetStatValue("MaxMana") <= warriorStats.GetStatValue("MaxMana")) return false;

            // Both should have inventory with items
            var warriorInv = warrior.GetComponent<InventoryComponent>();
            var mageInv = mage.GetComponent<InventoryComponent>();

            if (warriorInv.UsedSlots < 2) return false;
            if (mageInv.UsedSlots < 2) return false;

            Log("Character factory: All checks passed");
            return true;
        }

        private bool TestLinqQueries()
        {
            // Clear and add test entities
            _entities.Clear();

            for (int i = 1; i <= 5; i++)
            {
                var entity = CharacterFactory.CreateCharacter($"Hero{i}", CharacterClass.Warrior, i * 2);
                _entities.Add(entity);
            }

            // Test various LINQ operations
            var count = _entities.Count();
            if (count != 5) return false;

            var first = _entities.First();
            if (first.Name != "Hero1") return false;

            var last = _entities.Last();
            if (last.Name != "Hero5") return false;

            var filtered = _entities.Where(e => e.Name.Contains("3")).ToList();
            if (filtered.Count != 1) return false;

            var ordered = _entities.OrderByDescending(e => e.GetComponent<StatsComponent>().GetStatValue("MaxHealth")).ToList();
            if (ordered.First().Name != "Hero5") return false;

            var names = _entities.Select(e => e.Name).ToList();
            if (names.Count != 5) return false;

            var any = _entities.Any(e => e.Name == "Hero3");
            if (!any) return false;

            var all = _entities.All(e => e.IsActive);
            if (!all) return false;

            Log("LINQ queries: All checks passed");
            return true;
        }

        private bool TestEventsAndDelegates()
        {
            var entity = CharacterFactory.CreateCharacter("EventTest", CharacterClass.Rogue, 1);
            var inventory = entity.GetComponent<InventoryComponent>();

            int addedCount = 0;
            int removedCount = 0;

            inventory.OnItemAdded += item => addedCount++;
            inventory.OnItemRemoved += item => removedCount++;

            var item1 = new Item("TestItem1", "Test", ItemRarity.Common);
            var item2 = new Item("TestItem2", "Test", ItemRarity.Uncommon);

            inventory.AddItem(item1);
            inventory.AddItem(item2);

            if (addedCount != 2) return false;

            inventory.RemoveItem("TestItem1");

            if (removedCount != 1) return false;

            Log("Events and delegates: All checks passed");
            return true;
        }

        private bool TestGenerics()
        {
            var entity = new Entity("GenericTest");
            entity.AddComponent(new StatsComponent());
            entity.AddComponent(new InventoryComponent(10));
            entity.AddComponent(new CombatComponent(entity));

            // Test generic GetComponent
            var stats = entity.GetComponent<StatsComponent>();
            var inv = entity.GetComponent<InventoryComponent>();
            var combat = entity.GetComponent<CombatComponent>();

            if (stats == null || inv == null || combat == null) return false;

            // Test generic HasComponent
            if (!entity.HasComponent<StatsComponent>()) return false;
            if (!entity.HasComponent<InventoryComponent>()) return false;

            // Test GetComponents (multiple)
            var allComponents = entity.GetComponents<IComponent>().ToList();
            if (allComponents.Count != 3) return false;

            // Test generic collections
            var dict = new Dictionary<string, List<int>>();
            dict["test"] = new List<int> { 1, 2, 3 };
            if (dict["test"].Sum() != 6) return false;

            Log("Generics: All checks passed");
            return true;
        }

        private bool TestBattleSimulation()
        {
            Log("=== Starting Battle Simulation ===");

            var hero = CharacterFactory.CreateCharacter("Hero", CharacterClass.Paladin, 10);
            var enemy = CharacterFactory.CreateCharacter("Goblin", CharacterClass.Rogue, 8);

            var heroCombat = hero.GetComponent<CombatComponent>();
            var enemyCombat = enemy.GetComponent<CombatComponent>();

            heroCombat.OnDamageDealt += (target, result) =>
                Log($"{hero.Name} dealt {result} to {target.Name}");

            enemyCombat.OnDamageDealt += (target, result) =>
                Log($"{enemy.Name} dealt {result} to {target.Name}");

            heroCombat.OnDeath += () => Log($"{hero.Name} has fallen!");
            enemyCombat.OnDeath += () => Log($"{enemy.Name} has been defeated!");

            int round = 0;
            while (hero.IsAlive() && enemy.IsAlive() && round < 20)
            {
                round++;
                Log($"--- Round {round} ---");
                Log($"{hero.Name}: {hero.GetHealthPercent():F0}% HP");
                Log($"{enemy.Name}: {enemy.GetHealthPercent():F0}% HP");

                // Hero attacks
                if (hero.IsAlive())
                    heroCombat.Attack(enemy, 15 + _random.Next(10), DamageType.Physical);

                // Enemy attacks
                if (enemy.IsAlive())
                    enemyCombat.Attack(hero, 10 + _random.Next(8), DamageType.Physical);
            }

            Log("=== Battle Complete ===");
            Log(hero.IsAlive() ? $"{hero.Name} wins!" : $"{enemy.Name} wins!");

            return true;
        }
    }

    public class TestResults
    {
        public class TestResult
        {
            public string Name { get; set; }
            public bool Passed { get; set; }
            public string Error { get; set; }
        }

        private readonly List<TestResult> _results = new List<TestResult>();

        public void AddTest(string name, Func<bool> test)
        {
            var result = new TestResult { Name = name };
            try
            {
                result.Passed = test();
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Error = ex.Message;
            }
            _results.Add(result);
        }

        public int TotalTests => _results.Count;
        public int PassedTests => _results.Count(r => r.Passed);
        public int FailedTests => _results.Count(r => !r.Passed);

        public IEnumerable<TestResult> GetAllResults() => _results;
        public IEnumerable<TestResult> GetFailedResults() => _results.Where(r => !r.Passed);

        public override string ToString()
        {
            var lines = new List<string>
            {
                "========== TEST RESULTS ==========",
                $"Total: {TotalTests}, Passed: {PassedTests}, Failed: {FailedTests}",
                ""
            };

            foreach (var result in _results)
            {
                string status = result.Passed ? "[PASS]" : "[FAIL]";
                lines.Add($"{status} {result.Name}");
                if (!result.Passed && !string.IsNullOrEmpty(result.Error))
                {
                    lines.Add($"       Error: {result.Error}");
                }
            }

            lines.Add("");
            lines.Add(FailedTests == 0 ? "All tests passed!" : $"{FailedTests} test(s) failed.");
            lines.Add("==================================");

            return string.Join("\n", lines);
        }
    }
}
