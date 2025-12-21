--[[
  Instance Creation Tests
  Tests Instance.new and basic instance creation.
--]]

local roblox = require("@lune/roblox") :: any
local Instance = roblox.Instance

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertNotNil = tf.assertNotNil
local assertError = tf.assertError

print("\n--- Instance Creation Tests ---")

-- Basic creation
test("Instance.new('Part') creates Part", function()
  local part = Instance.new("Part")
  assertNotNil(part, "part should not be nil")
  assertEqual(part.ClassName, "Part", "ClassName should be Part")
end)

test("Instance.new('Model') creates Model", function()
  local model = Instance.new("Model")
  assertNotNil(model, "model should not be nil")
  assertEqual(model.ClassName, "Model", "ClassName should be Model")
end)

test("Instance.new('Folder') creates Folder", function()
  local folder = Instance.new("Folder")
  assertNotNil(folder, "folder should not be nil")
  assertEqual(folder.ClassName, "Folder", "ClassName should be Folder")
end)

test("Instance.new sets Name to ClassName by default", function()
  local part = Instance.new("Part")
  assertEqual(part.Name, "Part", "Name should default to ClassName")

  local folder = Instance.new("Folder")
  assertEqual(folder.Name, "Folder", "Name should default to ClassName")
end)

-- Invalid creation
test("Instance.new with invalid class throws error", function()
  assertError(function()
    Instance.new("InvalidClassName12345")
  end, "should error on invalid class")
end)

test("Instance.new is case sensitive", function()
  assertError(function()
    Instance.new("part") -- lowercase
  end, "should error on wrong case")
end)

-- Parent initially nil
test("New instance has nil Parent", function()
  local part = Instance.new("Part")
  assertEqual(part.Parent, nil, "Parent should be nil initially")
end)

-- Name property
test("Instance Name can be set", function()
  local part = Instance.new("Part")
  part.Name = "MyPart"
  assertEqual(part.Name, "MyPart", "Name should be set")
end)

test("Instance Name can be changed", function()
  local part = Instance.new("Part")
  part.Name = "FirstName"
  assertEqual(part.Name, "FirstName", "first name")
  part.Name = "SecondName"
  assertEqual(part.Name, "SecondName", "second name")
end)

-- Various instance types
test("Instance.new('BasePart') creates BasePart", function()
  local bp = Instance.new("BasePart")
  assertEqual(bp.ClassName, "BasePart", "ClassName")
end)

test("Instance.new('Script') creates Script", function()
  local script = Instance.new("Script")
  assertEqual(script.ClassName, "Script", "ClassName")
end)

test("Instance.new('LocalScript') creates LocalScript", function()
  local ls = Instance.new("LocalScript")
  assertEqual(ls.ClassName, "LocalScript", "ClassName")
end)

test("Instance.new('ModuleScript') creates ModuleScript", function()
  local ms = Instance.new("ModuleScript")
  assertEqual(ms.ClassName, "ModuleScript", "ClassName")
end)

test("Instance.new('Frame') creates Frame", function()
  local frame = Instance.new("Frame")
  assertEqual(frame.ClassName, "Frame", "ClassName")
end)

test("Instance.new('TextLabel') creates TextLabel", function()
  local label = Instance.new("TextLabel")
  assertEqual(label.ClassName, "TextLabel", "ClassName")
end)

-- Services
test("Instance.new('Workspace') creates Workspace", function()
  local ws = Instance.new("Workspace")
  assertEqual(ws.ClassName, "Workspace", "ClassName")
end)

test("Instance.new('ReplicatedStorage') creates ReplicatedStorage", function()
  local rs = Instance.new("ReplicatedStorage")
  assertEqual(rs.ClassName, "ReplicatedStorage", "ClassName")
end)

return tf.printSummary()
