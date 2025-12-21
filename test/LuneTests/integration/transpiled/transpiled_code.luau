--[[
  Transpiled Code Integration Tests

  Tests actual transpiled code from TestGame/Output/
  This verifies the full transpilation pipeline works correctly.

  This is a single consolidated test file to avoid duplicate class
  registration issues (CoreSystem's modules table tracks registered classes).
--]]

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertFalse = tf.assertFalse
local assertNotNil = tf.assertNotNil

-- Set up Roblox globals before loading CoreSystem
local robloxGlobals = require("../../lib/roblox_globals")
local game, workspace = robloxGlobals.setupGlobals()

-- Get explicit references to native Roblox types
local Vector3 = robloxGlobals.Vector3
local Color3 = robloxGlobals.Color3
local Instance = robloxGlobals.Instance

-- Load CoreSystem fresh for transpiled code testing
-- Clear cache first to ensure we don't use stale state from unit tests
local csLoader = require("../../lib/coresystem_loader")
csLoader.clearCache()

-- Load CoreSystem with TestGame namespace
-- Note: transpiled code expects _G.TestGame.System and _G.TestGame.Roblox
local System = csLoader.load({ systemNamespace = "TestGame" })

-- Load the lua_loader to load .lua files
local luaLoader = require("../../lib/lua_loader")

-- Load the Lune-compatible roblox types shim
local robloxTypesShim = require("../../lib/roblox_types_shim")

print("\n--- Transpiled Code Integration Tests ---")

-- ============================================================
-- PHASE 1: Setup Roblox Types
-- ============================================================

local Roblox = nil

test("roblox_types_shim installs into namespace", function()
  Roblox = robloxTypesShim("TestGame")
  assertNotNil(Roblox, "Should return Roblox table")
end)

test("_G.TestGame.Roblox is installed", function()
  assertNotNil(_G.TestGame, "TestGame namespace should exist")
  assertNotNil(_G.TestGame.Roblox, "Roblox should be installed in TestGame")
end)

test("Roblox.Vector3 exists with default", function()
  assertNotNil(Roblox.Vector3, "Vector3 should exist")
  assertNotNil(Roblox.Vector3.default, "default should exist")
  -- Note: __clone__ is no longer needed - transpiler skips it for Roblox types
end)

test("Roblox.Vector3.default() returns Vector3.zero", function()
  local defaultVec = Roblox.Vector3.default()
  assertNotNil(defaultVec, "default() should return a value")
  assertEqual(defaultVec.X, 0, "X should be 0")
  assertEqual(defaultVec.Y, 0, "Y should be 0")
  assertEqual(defaultVec.Z, 0, "Z should be 0")
end)

test("Roblox.Vector3() creates Vector3 via __call", function()
  local vec = Roblox.Vector3(5, 10, 15)
  assertNotNil(vec, "Should create Vector3")
  assertEqual(vec.X, 5, "X should be 5")
  assertEqual(vec.Y, 10, "Y should be 10")
  assertEqual(vec.Z, 15, "Z should be 15")
end)

test("Roblox.Color3.default() returns black", function()
  local defaultColor = Roblox.Color3.default()
  assertNotNil(defaultColor, "default() should return a value")
  assertEqual(defaultColor.R, 0, "R should be 0")
  assertEqual(defaultColor.G, 0, "G should be 0")
  assertEqual(defaultColor.B, 0, "B should be 0")
end)

test("Roblox.Color3.fromRGB works via metatable", function()
  local color = Roblox.Color3.fromRGB(255, 128, 64)
  assertNotNil(color, "Should create Color3")
  assertTrue(color.R > 0.9, "R should be ~1")
  assertTrue(color.G > 0.4 and color.G < 0.6, "G should be ~0.5")
end)

test("System.default works with Roblox types", function()
  local vec = System.default(Roblox.Vector3)
  assertNotNil(vec, "System.default should work")
  assertEqual(vec.X, 0, "X should be 0")
end)

-- ============================================================
-- PHASE 2: Load Transpiled Classes
-- ============================================================

test("Load Spawnable.lua", function()
  local result = luaLoader.loadWithCSharpEnv("TestGame/Output/Spawnable.lua")
  assertEqual(result, true, "Spawnable.lua should return true")
end)

test("Load SpawnableBox.lua", function()
  local result = luaLoader.loadWithCSharpEnv("TestGame/Output/SpawnableBox.lua")
  assertEqual(result, true, "SpawnableBox.lua should return true")
end)

test("Load GameScene.lua", function()
  local result = luaLoader.loadWithCSharpEnv("TestGame/Output/GameScene.lua")
  assertEqual(result, true, "GameScene.lua should return true")
end)

-- IMPORTANT: Loading the .lua files only registers them in CoreSystem's internal
-- modules table. We must call System.init() with the types list to actually
-- instantiate the classes and place them in the namespace.
test("Initialize classes via System.init", function()
  local result = System.init({
    types = {
      "TestGame.Spawnable",
      "TestGame.SpawnableBox",
      "TestGame.GameScene"
    }
  })
  assertNotNil(result, "System.init should return assembly")
end)

-- Note: With systemNamespace="TestGame", classes end up at _G.TestGame.TestGame.*
-- because the transpiled code does System.namespace("TestGame", ...) on top of the root

-- Get the actual TestGame namespace where classes are registered
local TestGameNS = _G.TestGame.TestGame

test("TestGame.Spawnable class exists", function()
  assertNotNil(_G.TestGame, "TestGame namespace should exist")
  assertNotNil(_G.TestGame.TestGame, "TestGame.TestGame should exist (nested namespace)")
  assertNotNil(TestGameNS.Spawnable, "Spawnable class should exist")
end)

test("TestGame.SpawnableBox class exists", function()
  assertNotNil(TestGameNS.SpawnableBox, "SpawnableBox class should exist")
end)

test("TestGame.GameScene class exists", function()
  assertNotNil(TestGameNS.GameScene, "GameScene class should exist")
end)

-- ============================================================
-- PHASE 3: Test SpawnableBox Class
-- ============================================================

-- Use native Roblox types directly (no __clone__ wrappers needed)
test("Create SpawnableBox instance", function()
  local pos = Vector3.new(10, 5, 20)
  local size = Vector3.new(2, 2, 2)
  local color = Color3.fromRGB(255, 0, 0)

  local box = TestGameNS.SpawnableBox("RedBox", pos, size, color)
  assertNotNil(box, "Should create SpawnableBox instance")
end)

test("SpawnableBox has correct Name", function()
  local pos = Vector3.new(0, 0, 0)
  local size = Vector3.new(1, 1, 1)
  local color = Color3.new(1, 1, 1)

  local box = TestGameNS.SpawnableBox("TestBox", pos, size, color)
  assertEqual(box.Name, "TestBox", "Name should be 'TestBox'")
end)

test("SpawnableBox has correct Position", function()
  local pos = Vector3.new(100, 50, 200)
  local size = Vector3.new(1, 1, 1)
  local color = Color3.new(1, 1, 1)

  local box = TestGameNS.SpawnableBox("PosBox", pos, size, color)
  assertEqual(box.Position.X, 100, "Position.X should be 100")
  assertEqual(box.Position.Y, 50, "Position.Y should be 50")
  assertEqual(box.Position.Z, 200, "Position.Z should be 200")
end)

test("SpawnableBox has correct Size and Color", function()
  local pos = Vector3.new(0, 0, 0)
  local size = Vector3.new(5, 10, 15)
  local color = Color3.fromRGB(128, 64, 255)

  local box = TestGameNS.SpawnableBox("SizeBox", pos, size, color)
  assertEqual(box.Size.X, 5, "Size.X should be 5")
  assertEqual(box.Size.Y, 10, "Size.Y should be 10")
  assertTrue(box.Color.B > 0.9, "Color.B should be ~1")
end)

-- ============================================================
-- PHASE 4: Test Spawn() Method
-- ============================================================

test("SpawnableBox.Spawn() creates Part", function()
  local pos = Vector3.new(10, 20, 30)
  local size = Vector3.new(4, 4, 4)
  local color = Color3.fromRGB(0, 255, 0)

  local box = TestGameNS.SpawnableBox("GreenBox", pos, size, color)
  local part = box:Spawn()

  assertNotNil(part, "Spawn should return a Part")
  assertTrue(part:IsA("Part"), "Should be a Part")
end)

test("Spawned Part has correct properties", function()
  local pos = Vector3.new(50, 100, 150)
  local size = Vector3.new(3, 6, 9)
  local color = Color3.fromRGB(255, 128, 0)

  local box = TestGameNS.SpawnableBox("PropBox", pos, size, color)
  local part = box:Spawn()

  assertEqual(part.Name, "PropBox", "Part.Name should match")
  assertEqual(part.Size.X, 3, "Part.Size.X should be 3")
  assertEqual(part.Position.X, 50, "Part.Position.X should be 50")
  assertTrue(part.Anchored, "Part should be anchored")
  assertEqual(part.Parent, workspace, "Part.Parent should be workspace")
end)

test("Multiple SpawnableBox instances are independent", function()
  local box1 = TestGameNS.SpawnableBox("Box1", Vector3.new(1, 0, 0), Vector3.new(1, 1, 1), Color3.new(1, 0, 0))
  local box2 = TestGameNS.SpawnableBox("Box2", Vector3.new(0, 1, 0), Vector3.new(2, 2, 2), Color3.new(0, 1, 0))

  assertEqual(box1.Name, "Box1", "box1.Name")
  assertEqual(box2.Name, "Box2", "box2.Name")
  assertEqual(box1.Position.X, 1, "box1.Position.X")
  assertEqual(box2.Position.Y, 1, "box2.Position.Y")
end)

-- ============================================================
-- PHASE 5: Test GameScene
-- ============================================================

test("GameScene.Setup() runs without error", function()
  local ok, err = pcall(function()
    TestGameNS.GameScene.Setup()
  end)
  assertTrue(ok, "Setup should run: " .. tostring(err))
end)

test("GameScene.Setup() creates parts in workspace", function()
  local redBox = workspace:FindFirstChild("RedBox")
  local blueBox = workspace:FindFirstChild("BlueBox")
  assertNotNil(redBox, "RedBox should exist in workspace")
  assertNotNil(blueBox, "BlueBox should exist in workspace")
end)

test("GameScene spawned boxes have correct properties", function()
  local redBox = workspace:FindFirstChild("RedBox")
  local blueBox = workspace:FindFirstChild("BlueBox")

  -- RedBox at (5, 3, 0) with red color
  assertEqual(redBox.Position.X, 5, "RedBox.Position.X")
  assertEqual(redBox.Position.Y, 3, "RedBox.Position.Y")
  assertTrue(redBox.Color.R > 0.9, "RedBox should be red")

  -- BlueBox at (-5, 3, 0) with blue color
  assertEqual(blueBox.Position.X, -5, "BlueBox.Position.X")
  assertTrue(blueBox.Color.B > 0.9, "BlueBox should be blue")
end)

-- ============================================================
-- PHASE 6: Test Manifest Structure
-- ============================================================

test("manifest.lua loads and returns a function", function()
  local manifest = luaLoader.loadWithCSharpEnv("TestGame/Output/manifest.lua")
  assertNotNil(manifest, "Manifest should load")
  assertEqual(type(manifest), "function", "Manifest should be a function")
end)

return tf.printSummary()
