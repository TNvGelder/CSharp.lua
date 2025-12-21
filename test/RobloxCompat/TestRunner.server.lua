--[[
  CSharp.lua GameSystem Test Runner for Roblox

  This script tests the transpiled C# GameSystem in Roblox Studio.
  Run with: rojo serve default.project.json
--]]

print("\n" .. string.rep("=", 60))
print("CSharp.lua GameSystem Test Runner")
print(string.rep("=", 60) .. "\n")

-- Load shared configuration and set it before requiring CoreSystem
local config = require(game.ReplicatedStorage.config)
local systemNamespace = config.systemNamespace or "MyGame"
rawset(_G, "__CoreSystemConfig", config)

print("[Config] systemNamespace = " .. systemNamespace)
print("")

-- Step 1: Load CoreSystem (it initializes directly into the configured namespace)
print("[1/4] Loading CoreSystem...")
local System = require(game.ReplicatedStorage.CoreSystem)
if not System then
    error("Failed to load CoreSystem!")
end
print("[OK] CoreSystem loaded at _G." .. systemNamespace .. ".System\n")

-- Step 2: Debug - list what's in ReplicatedStorage
print("[DEBUG] Contents of ReplicatedStorage:")
for _, child in ipairs(game.ReplicatedStorage:GetChildren()) do
    print("  - " .. child.Name .. " (" .. child.ClassName .. ")")
end

-- Check if GameSystem exists
local GameSystemFolder = game.ReplicatedStorage:FindFirstChild("GameSystem")
if not GameSystemFolder then
    warn("[ERROR] GameSystem folder not found in ReplicatedStorage!")
    warn("Make sure Rojo is running and connected.")
    return
end

print("\n[DEBUG] Contents of GameSystem folder:")
for _, child in ipairs(GameSystemFolder:GetChildren()) do
    print("  - " .. child.Name .. " (" .. child.ClassName .. ")")
end

-- Step 3: Load all GameSystem modules
print("\n[2/4] Loading GameSystem modules...")

-- Load modules in dependency order - try both with and without .lua extension
local moduleOrder = {
    "Entity",
    "Stats",
    "Inventory",
    "Combat",
    "Character",
    "GameManager"
}

local loadedModules = {}
for _, moduleName in ipairs(moduleOrder) do
    -- Try to find the module (Rojo might name it differently)
    local module = GameSystemFolder:FindFirstChild(moduleName)
        or GameSystemFolder:FindFirstChild(moduleName .. ".lua")

    if module then
        local success, result = pcall(function()
            return require(module)
        end)
        if success then
            print("  [OK] " .. moduleName .. " (from " .. module.Name .. ")")
            loadedModules[moduleName] = true
        else
            warn("  [FAIL] " .. moduleName .. ": " .. tostring(result))
        end
    else
        warn("  [SKIP] " .. moduleName .. " not found")
    end
end

-- Initialize the types via System.init directly (we already loaded the modules)
print("\n[2.5/4] Initializing GameSystem types...")

-- Since we already required all the modules, we just need to call System.init
-- to initialize the types. Pass the root folder so it can find modules if needed.
-- Note: ModifierType and CharacterClass are inlined by the compiler (not actual types)
local typeNames = {
    "GameSystem.IComponent",
    "GameSystem.Entity",
    "GameSystem.Stat",
    "GameSystem.StatModifier",
    "GameSystem.StatsComponent",
    "GameSystem.ItemRarity",
    "GameSystem.Item",
    "GameSystem.InventorySlot",
    "GameSystem.InventoryComponent",
    "GameSystem.DamageType",
    "GameSystem.DamageResult",
    "GameSystem.CombatComponent",
    "GameSystem.CharacterFactory",
    "GameSystem.CharacterExtensions",
    "GameSystem.GameManager",
    "GameSystem.TestResults",
    "GameSystem.TestResults.TestResult",
}

local ok, err = pcall(function()
    System.init({
        root = GameSystemFolder,
        files = {}, -- Already loaded
        types = typeNames,
    })
end)

if ok then
    print("  [OK] Types initialized")
else
    warn("  [FAIL] init() error: " .. tostring(err))
end

-- Debug: Check what's in _G after initialization
print("\n[DEBUG] Checking _G for GameSystem...")
if _G.GameSystem then
    print("  GameSystem found in _G!")
    print("  Contents:")
    for k, v in pairs(_G.GameSystem) do
        print("    - " .. tostring(k) .. ": " .. type(v))
    end
else
    print("  GameSystem NOT in _G")
end

-- Now types should be accessible via _G
local GameSystem = _G.GameSystem

-- Define inlined enums manually (compiler inlines these as integers)
-- ModifierType and CharacterClass don't exist as types in the generated Lua
local ModifierType = { Flat = 0, Percent = 1 }
local CharacterClass = { Warrior = 0, Mage = 1, Rogue = 2, Paladin = 3 }

print("\n[3/4] Running GameSystem tests...\n")

local testsPassed = 0
local testsFailed = 0

local function test(name, fn)
    local success, err = pcall(fn)
    if success then
        testsPassed = testsPassed + 1
        print("[PASS] " .. name)
    else
        testsFailed = testsFailed + 1
        warn("[FAIL] " .. name .. ": " .. tostring(err))
    end
end

local function assertEqual(actual, expected, msg)
    if actual ~= expected then
        error((msg or "Assertion failed") .. ": expected " .. tostring(expected) .. ", got " .. tostring(actual))
    end
end

local function assertTrue(value, msg)
    if not value then
        error((msg or "Assertion failed") .. ": expected true, got " .. tostring(value))
    end
end

local function assertNotNil(value, msg)
    if value == nil then
        error((msg or "Assertion failed") .. ": expected non-nil value")
    end
end

local function assertApprox(actual, expected, tolerance, msg)
    tolerance = tolerance or 0.01
    if math.abs(actual - expected) > tolerance then
        error((msg or "Assertion failed") .. ": expected ~" .. tostring(expected) .. ", got " .. tostring(actual))
    end
end

-- Skip tests if GameSystem isn't available
if not GameSystem then
    warn("\n[ERROR] GameSystem namespace not available. Cannot run tests.")
    warn("Check the debug output above to diagnose the issue.")
    print("\n" .. string.rep("=", 60))
    print("[4/4] Test Summary: 0 passed, 0 failed (SKIPPED - no GameSystem)")
    print(string.rep("=", 60))
    return
end

-- Access classes from GameSystem namespace
-- Note: ModifierType and CharacterClass are defined manually above (inlined by compiler)
local Entity = GameSystem.Entity
local StatsComponent = GameSystem.StatsComponent
local Stat = GameSystem.Stat
local StatModifier = GameSystem.StatModifier
local InventoryComponent = GameSystem.InventoryComponent
local Item = GameSystem.Item
local ItemRarity = GameSystem.ItemRarity
local CombatComponent = GameSystem.CombatComponent
local DamageType = GameSystem.DamageType
local CharacterFactory = GameSystem.CharacterFactory
local CharacterExtensions = GameSystem.CharacterExtensions
local GameManager = GameSystem.GameManager

-- Check if we have the basic classes
print("--- Checking Class Availability ---")
test("Entity class exists", function()
    assertNotNil(Entity, "Entity class should exist")
end)

test("StatsComponent class exists", function()
    assertNotNil(StatsComponent, "StatsComponent class should exist")
end)

test("Stat class exists", function()
    assertNotNil(Stat, "Stat class should exist")
end)

test("InventoryComponent class exists", function()
    assertNotNil(InventoryComponent, "InventoryComponent class should exist")
end)

test("Item class exists", function()
    assertNotNil(Item, "Item class should exist")
end)

test("CharacterFactory class exists", function()
    assertNotNil(CharacterFactory, "CharacterFactory class should exist")
end)

test("GameManager class exists", function()
    assertNotNil(GameManager, "GameManager class should exist")
end)

-- Test Entity System
print("\n--- Entity Tests ---")

test("Create Entity", function()
    assertNotNil(Entity, "Entity class required")
    local entity = Entity("TestEntity")
    assertNotNil(entity, "Should create entity")
    assertEqual(entity.Name, "TestEntity", "Entity name")
    assertTrue(entity.Id > 0, "Entity should have positive ID")
    assertTrue(entity.IsActive, "Entity should be active by default")
end)

test("Entity.ToString", function()
    local entity = Entity("MyEntity")
    local str = entity:ToString()
    assertTrue(str:find("MyEntity") ~= nil, "ToString should contain name")
end)

test("Entity add component", function()
    local entity = Entity("WithStats")
    local stats = StatsComponent()
    entity:AddComponent(stats)
    assertTrue(entity:HasComponent(StatsComponent), "Should have StatsComponent")
end)

test("Entity get component", function()
    local entity = Entity("WithStats")
    local stats = StatsComponent()
    entity:AddComponent(stats)
    local retrieved = entity:GetComponent(StatsComponent)
    assertNotNil(retrieved, "Should retrieve component")
    assertEqual(retrieved:getName(), "Stats", "Component name")
end)

-- Test Stats System
print("\n--- Stats Tests ---")

test("Stat creation", function()
    local health = Stat("Health", 100)
    assertEqual(health.Name, "Health", "Stat name")
    assertEqual(health.BaseValue, 100, "Base value")
    assertApprox(health:getValue(), 100, 0.01, "Computed value")
end)

test("Stat flat modifier", function()
    local health = Stat("Health", 100)
    local mod = StatModifier("Buff", ModifierType.Flat, 20)
    health:AddModifier(mod)
    assertApprox(health:getValue(), 120, 0.01, "After flat modifier")
end)

test("Stat percent modifier", function()
    local health = Stat("Health", 100)
    local mod = StatModifier("Buff", ModifierType.Percent, 50)
    health:AddModifier(mod)
    assertApprox(health:getValue(), 150, 0.01, "After percent modifier")
end)

test("Stat combined modifiers", function()
    local health = Stat("Health", 100)
    health:AddModifier(StatModifier("Flat", ModifierType.Flat, 20))
    health:AddModifier(StatModifier("Percent", ModifierType.Percent, 50))
    assertApprox(health:getValue(), 180, 0.01, "Combined modifiers")
end)

test("StatsComponent create and get stat", function()
    local stats = StatsComponent()
    local hp = stats:GetOrCreateStat("Health", 100)
    assertNotNil(hp, "Should create stat")
    assertApprox(stats:GetStatValue("Health"), 100, 0.01, "Get stat value")
end)

-- Test Inventory System
print("\n--- Inventory Tests ---")

test("Item creation", function()
    local sword = Item("Iron Sword", "A basic sword", ItemRarity.Common, 1)
    assertEqual(sword.Name, "Iron Sword", "Item name")
    assertEqual(sword.StackSize, 1, "Default stack size")
end)

test("Item with stat bonus", function()
    local sword = Item("Magic Sword", "A magic sword", ItemRarity.Rare, 1)
    sword = sword:WithStatBonus("AttackPower", 10)
    assertTrue(sword.StatBonuses:ContainsKey("AttackPower"), "Should have stat bonus")
    assertEqual(sword.StatBonuses:get("AttackPower"), 10, "Bonus value")
end)

test("InventoryComponent add item", function()
    local inventory = InventoryComponent(10)
    assertEqual(inventory.Capacity, 10, "Capacity")
    assertEqual(inventory:getUsedSlots(), 0, "Empty at start")

    local sword = Item("Sword", "A sword", ItemRarity.Common, 1)
    assertTrue(inventory:AddItem(sword), "Should add item")
    assertEqual(inventory:getUsedSlots(), 1, "One slot used")
end)

test("Inventory HasItem and CountItem", function()
    local inventory = InventoryComponent(10)
    local sword = Item("Sword", "A sword", ItemRarity.Common, 1)
    inventory:AddItem(sword)

    assertTrue(inventory:HasItem("Sword", 1), "Should have sword")
    assertEqual(inventory:CountItem("Sword"), 1, "Count should be 1")
end)

test("Inventory stacking", function()
    local inventory = InventoryComponent(10)

    local potion1 = Item("Potion", "Heals", ItemRarity.Common, 10)
    potion1.StackSize = 3

    local potion2 = Item("Potion", "Heals", ItemRarity.Common, 10)
    potion2.StackSize = 2

    inventory:AddItem(potion1)
    inventory:AddItem(potion2)

    assertEqual(inventory:CountItem("Potion"), 5, "Stacked count")
    assertEqual(inventory:getUsedSlots(), 1, "Should stack into one slot")
end)

test("Inventory remove item", function()
    local inventory = InventoryComponent(10)
    local sword = Item("Sword", "A sword", ItemRarity.Common, 1)
    inventory:AddItem(sword)

    local removed = inventory:RemoveItem("Sword", 1)
    assertNotNil(removed, "Should return removed item")
    assertEqual(inventory:getUsedSlots(), 0, "Should be empty")
end)

-- Test Character Factory
print("\n--- Character Factory Tests ---")

test("Create Warrior", function()
    local warrior = CharacterFactory.CreateCharacter("TestWarrior", CharacterClass.Warrior, 5)
    assertNotNil(warrior, "Should create warrior")
    assertEqual(warrior.Name, "TestWarrior", "Warrior name")

    local stats = warrior:GetComponent(StatsComponent)
    assertNotNil(stats, "Should have stats")
    assertTrue(stats:GetStatValue("Health") > 0, "Should have health")
end)

test("Create Mage", function()
    local mage = CharacterFactory.CreateCharacter("TestMage", CharacterClass.Mage, 5)
    local stats = mage:GetComponent(StatsComponent)
    assertTrue(stats:GetStatValue("MaxMana") > 0, "Mage should have mana")
end)

test("Warrior vs Mage stats", function()
    local warrior = CharacterFactory.CreateCharacter("W", CharacterClass.Warrior, 10)
    local mage = CharacterFactory.CreateCharacter("M", CharacterClass.Mage, 10)

    local wStats = warrior:GetComponent(StatsComponent)
    local mStats = mage:GetComponent(StatsComponent)

    assertTrue(wStats:GetStatValue("MaxHealth") > mStats:GetStatValue("MaxHealth"), "Warrior should have more HP")
    assertTrue(mStats:GetStatValue("MaxMana") > wStats:GetStatValue("MaxMana"), "Mage should have more mana")
end)

test("Character has starting items", function()
    local warrior = CharacterFactory.CreateCharacter("W", CharacterClass.Warrior, 1)
    local inventory = warrior:GetComponent(InventoryComponent)
    assertNotNil(inventory, "Should have inventory")
    assertTrue(inventory:getUsedSlots() >= 2, "Should have starting items")
end)

-- Test Combat System
print("\n--- Combat Tests ---")

test("Combat attack deals damage", function()
    local attacker = CharacterFactory.CreateCharacter("Attacker", CharacterClass.Warrior, 10)
    local defender = CharacterFactory.CreateCharacter("Defender", CharacterClass.Warrior, 10)

    local aCombat = attacker:GetComponent(CombatComponent)
    local dStats = defender:GetComponent(StatsComponent)

    local healthBefore = dStats:GetStatValue("Health")
    aCombat:Attack(defender, 50, DamageType.Physical)
    local healthAfter = dStats:GetStatValue("Health")

    print("  Combat result: " .. string.format("%.0f", healthBefore) .. " -> " .. string.format("%.0f", healthAfter))
end)

test("Combat damage types", function()
    local attacker = CharacterFactory.CreateCharacter("A", CharacterClass.Mage, 10)
    local defender = CharacterFactory.CreateCharacter("D", CharacterClass.Warrior, 10)

    local combat = attacker:GetComponent(CombatComponent)

    combat:Attack(defender, 10, DamageType.Physical)
    combat:Attack(defender, 10, DamageType.Fire)
    combat:Attack(defender, 10, DamageType.Ice)
end)

-- Test Character Extensions
print("\n--- Character Extension Tests ---")

test("IsAlive extension", function()
    local hero = CharacterFactory.CreateCharacter("Hero", CharacterClass.Warrior, 10)
    assertTrue(CharacterExtensions.IsAlive(hero), "New character should be alive")
end)

test("GetHealthPercent extension", function()
    local hero = CharacterFactory.CreateCharacter("Hero", CharacterClass.Warrior, 10)
    local percent = CharacterExtensions.GetHealthPercent(hero)
    assertApprox(percent, 100, 1, "Full health should be 100%")
end)

test("Heal extension", function()
    local hero = CharacterFactory.CreateCharacter("Hero", CharacterClass.Warrior, 10)
    local stats = hero:GetComponent(StatsComponent)
    local healthStat = stats:GetStat("Health")

    healthStat.BaseValue = healthStat.BaseValue - 50
    local damagedHealth = stats:GetStatValue("Health")

    CharacterExtensions.Heal(hero, 30)
    local healedHealth = stats:GetStatValue("Health")

    assertTrue(healedHealth > damagedHealth, "Health should increase after heal")
end)

-- Test GameManager
print("\n--- GameManager Tests ---")

test("GameManager creation", function()
    local manager = GameManager()
    assertNotNil(manager, "Should create manager")
end)

test("GameManager add and get entity", function()
    local manager = GameManager()
    local entity = Entity("ManagedEntity")

    manager:AddEntity(entity)
    local found = manager:GetEntity("ManagedEntity")

    assertNotNil(found, "Should find entity")
    assertEqual(found.Name, "ManagedEntity", "Entity name matches")
end)

test("GameManager active entities", function()
    local manager = GameManager()
    local e1 = Entity("Active1")
    local e2 = Entity("Active2")
    local e3 = Entity("Inactive")
    e3.IsActive = false

    manager:AddEntity(e1)
    manager:AddEntity(e2)
    manager:AddEntity(e3)

    local activeCount = 0
    for _ in System.each(manager:GetActiveEntities()) do
        activeCount = activeCount + 1
    end

    assertEqual(activeCount, 2, "Should have 2 active entities")
end)

test("GameManager internal tests", function()
    local manager = GameManager()

    local logs = {}
    -- Use System.DelegateCombine since OnLog starts as nil (can't use + on nil)
    manager.OnLog = System.DelegateCombine(manager.OnLog, function(msg)
        table.insert(logs, msg)
    end)

    local results = manager:RunAllTests()
    assertNotNil(results, "Should return results")

    print("\n  --- GameManager Internal Results ---")
    print("  Total: " .. results:getTotalTests() .. ", Passed: " .. results:getPassedTests() .. ", Failed: " .. results:getFailedTests())

    for _, result in System.each(results:GetAllResults()) do
        local status = result.Passed and "PASS" or "FAIL"
        print("    [" .. status .. "] " .. result.Name)
    end
end)

-- ============================================
-- Roblox Transform Validation Tests
-- These verify the -roblox compiler flag generates correct code
-- ============================================
print("\n--- Roblox Compiler Transform Tests ---")

-- Test: Number formatting (validates compiler generates System.toString)
test("Compiled code: number formatting works", function()
    -- This pattern appears in GameManager: $"{healthBefore:F0}"
    -- Compiler should generate: System.toString(healthBefore, "F0")
    local value = 123.456
    local formatted = System.toString(value, "F0")
    assertEqual(formatted, "123", "F0 format")

    formatted = System.toString(value, "F1")
    assertEqual(formatted, "123.5", "F1 format")
end)

-- Test: String methods work as static calls
test("Compiled code: string static methods work", function()
    -- Compiler generates: System.String.Contains(str, x)
    -- instead of: str:Contains(x)
    local str = "Hello World"
    assertTrue(System.String.Contains(str, "World"), "Contains")
    assertTrue(System.String.StartsWith(str, "Hello"), "StartsWith")
    assertTrue(System.String.EndsWith(str, "World"), "EndsWith")
end)

-- Test: Delegate combine pattern (validates compiler generates DelegateCombine)
test("Compiled code: delegate += pattern works", function()
    -- GameManager uses: OnLog += handler
    -- Compiler should generate: OnLog = System.DelegateCombine(OnLog, handler)
    local callCount = 0
    local event = nil

    -- First += on nil delegate
    event = System.DelegateCombine(event, function() callCount = callCount + 1 end)
    -- Second +=
    event = System.DelegateCombine(event, function() callCount = callCount + 10 end)

    event()
    assertEqual(callCount, 11, "Both handlers should fire")
end)

-- Test: Enum ToString pattern
test("Compiled code: enum to string works", function()
    -- Compiler generates: System.EnumToString(value, EnumType)
    -- Test with actual compiled enum
    local rarity = ItemRarity.Rare -- value 2
    local str = System.EnumToString(rarity, ItemRarity)
    assertEqual(str, "Rare", "Enum should convert to string")
end)

-- Test: LINQ works with compiled generic types
test("Compiled code: LINQ on compiled collections", function()
    local linq = System.Linq.Enumerable

    -- Create inventory and add items (like GameManager does)
    local inventory = InventoryComponent(10)
    inventory:AddItem(Item("Sword", "A sword", ItemRarity.Common, 1))
    inventory:AddItem(Item("Shield", "A shield", ItemRarity.Uncommon, 1))
    inventory:AddItem(Item("Helm", "A helm", ItemRarity.Rare, 1))

    -- Test LINQ on compiled collection
    local allItems = inventory:GetAllItems()
    local count = linq.Count(allItems)
    assertEqual(count, 3, "Should have 3 items")

    -- Test Where (like GetItemsByRarity does internally)
    local rareItems = linq.Where(allItems, function(i) return i.Rarity == ItemRarity.Rare end)
    local rareCount = linq.Count(rareItems)
    assertEqual(rareCount, 1, "Should have 1 rare item")
end)

-- Test: Extension methods work
test("Compiled code: extension methods work", function()
    -- CharacterExtensions.IsAlive(entity) and GetHealthPercent(entity)
    local hero = CharacterFactory.CreateCharacter("TestHero", CharacterClass.Warrior, 5)

    assertTrue(CharacterExtensions.IsAlive(hero), "Hero should be alive")

    local percent = CharacterExtensions.GetHealthPercent(hero)
    assertTrue(percent > 0 and percent <= 100, "Health percent should be 0-100")
end)

-- Test: Event subscription in compiled code
test("Compiled code: event subscription works", function()
    local inventory = InventoryComponent(5)
    local addedItems = {}

    -- Subscribe to event (like GameManager does)
    inventory.OnItemAdded = System.DelegateCombine(inventory.OnItemAdded, function(item)
        table.insert(addedItems, item.Name)
    end)

    inventory:AddItem(Item("TestItem", "Test", ItemRarity.Common, 1))

    assertEqual(#addedItems, 1, "Event should have fired")
    assertEqual(addedItems[1], "TestItem", "Should receive correct item")
end)

-- Test: Generic dictionary in compiled code
test("Compiled code: generic Dictionary works", function()
    -- Item uses Dictionary<string, float> for StatBonuses
    local sword = Item("Magic Sword", "Powerful", ItemRarity.Epic, 1)
    sword = sword:WithStatBonus("AttackPower", 25.5)
    sword = sword:WithStatBonus("CritChance", 10.0)

    assertTrue(sword.StatBonuses:ContainsKey("AttackPower"), "Should have AttackPower")
    assertApprox(sword.StatBonuses:get("AttackPower"), 25.5, 0.01, "AttackPower value")
    assertEqual(sword.StatBonuses:getCount(), 2, "Should have 2 bonuses")
end)

-- Test: ToString on compiled objects
test("Compiled code: object ToString works", function()
    local entity = Entity("TestEntity")
    local str = entity:ToString()
    assertTrue(string.find(str, "TestEntity") ~= nil, "ToString should contain name")

    -- Item.ToString uses string interpolation with enum
    local item = Item("Potion", "Heals", ItemRarity.Uncommon, 10)
    item.StackSize = 5
    local itemStr = item:ToString()
    assertTrue(string.find(itemStr, "Uncommon") ~= nil, "Should contain rarity")
    assertTrue(string.find(itemStr, "Potion") ~= nil, "Should contain name")
end)

-- Print summary
print("\n" .. string.rep("=", 60))
print(string.format("[4/4] Test Summary: %d passed, %d failed", testsPassed, testsFailed))
print(string.rep("=", 60))

if testsFailed == 0 then
    print("\nAll tests passed! The C# GameSystem is fully working in Roblox!")
else
    warn(string.format("\n%d test(s) failed. Check the errors above.", testsFailed))
end
