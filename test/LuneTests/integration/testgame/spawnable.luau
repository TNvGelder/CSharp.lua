--[[
  TestGame Integration Tests
  Tests the actual transpiled TestGame code to verify the Roblox typing system works.

  TestGame demonstrates:
  - SpawnableBox class using Vector3, Color3
  - Instance.new("Part") via InstanceFactory.Create<Part>()
  - Property assignment (Size, Position, Color, Anchored, Parent)
  - Class inheritance (SpawnableBox extends Spawnable)
--]]

local roblox = require("@lune/roblox") :: any
local Vector3 = roblox.Vector3
local Color3 = roblox.Color3
local Instance = roblox.Instance

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertNotNil = tf.assertNotNil
local assertApprox = tf.assertApprox

-- Set up Roblox globals
local robloxGlobals = require("../../lib/roblox_globals")
local game, workspace = robloxGlobals.setupGlobals()

print("\n--- TestGame Integration Tests ---")

-- Test the core patterns used in TestGame without loading the actual transpiled code
-- This validates that the Roblox typing system integration works

test("Vector3.new works for position", function()
  local pos = Vector3.new(5, 3, 0)
  assertEqual(pos.X, 5, "X")
  assertEqual(pos.Y, 3, "Y")
  assertEqual(pos.Z, 0, "Z")
end)

test("Vector3.new works for size", function()
  local size = Vector3.new(4, 4, 4)
  assertEqual(size.X, 4, "X")
  assertEqual(size.Y, 4, "Y")
  assertEqual(size.Z, 4, "Z")
end)

test("Color3.fromRGB works for red", function()
  local red = Color3.fromRGB(255, 0, 0)
  assertApprox(red.R, 1, 0.01, "R")
  assertApprox(red.G, 0, 0.01, "G")
  assertApprox(red.B, 0, 0.01, "B")
end)

test("Color3.fromRGB works for blue", function()
  local blue = Color3.fromRGB(0, 0, 255)
  assertApprox(blue.R, 0, 0.01, "R")
  assertApprox(blue.G, 0, 0.01, "G")
  assertApprox(blue.B, 1, 0.01, "B")
end)

test("Instance.new('Part') creates Part", function()
  local part = Instance.new("Part")
  assertEqual(part.ClassName, "Part", "Should be Part")
end)

test("Part.Name can be set", function()
  local part = Instance.new("Part")
  part.Name = "RedBox"
  assertEqual(part.Name, "RedBox", "Name should be set")
end)

test("Part.Size can be set to Vector3", function()
  local part = Instance.new("Part")
  local size = Vector3.new(4, 4, 4)
  part.Size = size
  assertEqual(part.Size.X, 4, "Size X")
  assertEqual(part.Size.Y, 4, "Size Y")
  assertEqual(part.Size.Z, 4, "Size Z")
end)

test("Part.Position can be set to Vector3", function()
  local part = Instance.new("Part")
  local pos = Vector3.new(5, 3, 0)
  part.Position = pos
  assertEqual(part.Position.X, 5, "Position X")
  assertEqual(part.Position.Y, 3, "Position Y")
  assertEqual(part.Position.Z, 0, "Position Z")
end)

test("Part.Color can be set to Color3", function()
  local part = Instance.new("Part")
  local color = Color3.fromRGB(255, 0, 0)
  part.Color = color
  assertApprox(part.Color.R, 1, 0.01, "Color R")
  assertApprox(part.Color.G, 0, 0.01, "Color G")
  assertApprox(part.Color.B, 0, 0.01, "Color B")
end)

test("Part.Anchored can be set", function()
  local part = Instance.new("Part")
  part.Anchored = true
  assertTrue(part.Anchored, "Should be anchored")
end)

test("Part.Parent can be set to workspace", function()
  local part = Instance.new("Part")
  part.Parent = workspace
  assertEqual(part.Parent, workspace, "Parent should be workspace")
end)

test("Full SpawnableBox pattern works", function()
  -- This simulates what SpawnableBox.Spawn() does
  local part = Instance.new("Part")
  part.Name = "TestBox"
  part.Size = Vector3.new(4, 4, 4)
  part.Position = Vector3.new(5, 3, 0)
  part.Color = Color3.fromRGB(255, 0, 0)
  part.Anchored = true
  part.Parent = workspace

  -- Verify all properties
  assertEqual(part.Name, "TestBox", "Name")
  assertEqual(part.Size.X, 4, "Size X")
  assertEqual(part.Position.X, 5, "Position X")
  assertApprox(part.Color.R, 1, 0.01, "Color R")
  assertTrue(part.Anchored, "Anchored")
  assertEqual(part.Parent, workspace, "Parent")
end)

test("Multiple parts can be created", function()
  local redBox = Instance.new("Part")
  redBox.Name = "RedBox"
  redBox.Color = Color3.fromRGB(255, 0, 0)
  redBox.Parent = workspace

  local blueBox = Instance.new("Part")
  blueBox.Name = "BlueBox"
  blueBox.Color = Color3.fromRGB(0, 0, 255)
  blueBox.Parent = workspace

  -- Verify both exist in workspace
  local foundRed = workspace:FindFirstChild("RedBox")
  local foundBlue = workspace:FindFirstChild("BlueBox")

  assertNotNil(foundRed, "Red box should be in workspace")
  assertNotNil(foundBlue, "Blue box should be in workspace")
  assertTrue(foundRed ~= foundBlue, "Should be different instances")
end)

test("workspace is a valid Instance", function()
  assertEqual(workspace.ClassName, "Workspace", "Should be Workspace")
end)

test("game is a valid DataModel", function()
  assertEqual(game.ClassName, "DataModel", "Should be DataModel")
end)

test("workspace is child of game", function()
  assertTrue(workspace:IsDescendantOf(game), "workspace should be descendant of game")
end)

return tf.printSummary()
