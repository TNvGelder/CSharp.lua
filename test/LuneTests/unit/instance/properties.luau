--[[
  Instance Properties Tests
  Tests setting and getting properties on instances like Part.
--]]

local roblox = require("@lune/roblox") :: any
local Instance = roblox.Instance
local Vector3 = roblox.Vector3
local Color3 = roblox.Color3
local CFrame = roblox.CFrame

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertApprox = tf.assertApprox

print("\n--- Instance Properties Tests ---")

-- Part properties
test("Part.Size can be set", function()
  local part = Instance.new("Part")
  local size = Vector3.new(2, 4, 6)
  part.Size = size
  assertEqual(part.Size.X, 2, "Size X")
  assertEqual(part.Size.Y, 4, "Size Y")
  assertEqual(part.Size.Z, 6, "Size Z")
end)

test("Part.Position can be set", function()
  local part = Instance.new("Part")
  local pos = Vector3.new(10, 20, 30)
  part.Position = pos
  assertEqual(part.Position.X, 10, "Position X")
  assertEqual(part.Position.Y, 20, "Position Y")
  assertEqual(part.Position.Z, 30, "Position Z")
end)

test("Part.CFrame can be set", function()
  local part = Instance.new("Part")
  local cf = CFrame.new(5, 10, 15)
  part.CFrame = cf
  assertEqual(part.CFrame.X, 5, "CFrame X")
  assertEqual(part.CFrame.Y, 10, "CFrame Y")
  assertEqual(part.CFrame.Z, 15, "CFrame Z")
end)

test("Part.Color can be set", function()
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
  assertTrue(part.Anchored, "Anchored should be true")
  part.Anchored = false
  assertTrue(not part.Anchored, "Anchored should be false")
end)

test("Part.CanCollide can be set", function()
  local part = Instance.new("Part")
  part.CanCollide = false
  assertTrue(not part.CanCollide, "CanCollide should be false")
  part.CanCollide = true
  assertTrue(part.CanCollide, "CanCollide should be true")
end)

test("Part.Transparency can be set", function()
  local part = Instance.new("Part")
  part.Transparency = 0.5
  assertApprox(part.Transparency, 0.5, 0.001, "Transparency")
end)

-- Model properties
test("Model.PrimaryPart can be set", function()
  local model = Instance.new("Model")
  local part = Instance.new("Part")
  part.Parent = model
  model.PrimaryPart = part
  assertEqual(model.PrimaryPart, part, "PrimaryPart")
end)

-- IsA
test("Part:IsA('Part') is true", function()
  local part = Instance.new("Part")
  assertTrue(part:IsA("Part"), "Part IsA Part")
end)

test("Part:IsA('BasePart') is true", function()
  local part = Instance.new("Part")
  assertTrue(part:IsA("BasePart"), "Part IsA BasePart")
end)

test("Part:IsA('Instance') is true", function()
  local part = Instance.new("Part")
  assertTrue(part:IsA("Instance"), "Part IsA Instance")
end)

test("Part:IsA('Model') is false", function()
  local part = Instance.new("Part")
  assertTrue(not part:IsA("Model"), "Part is not a Model")
end)

-- Clone
test("Instance:Clone creates copy", function()
  local original = Instance.new("Part")
  original.Name = "Original"
  original.Size = Vector3.new(1, 2, 3)
  original.Anchored = true

  local clone = original:Clone()
  assertEqual(clone.Name, "Original", "cloned Name")
  assertEqual(clone.Size.X, 1, "cloned Size X")
  assertEqual(clone.Anchored, true, "cloned Anchored")
  assertTrue(clone ~= original, "clone should be different instance")
end)

test("Clone copies children", function()
  local parent = Instance.new("Model")
  local child = Instance.new("Part")
  child.Name = "ChildPart"
  child.Parent = parent

  local clone = parent:Clone()
  local clonedChild = clone:FindFirstChild("ChildPart")
  assertTrue(clonedChild ~= nil, "cloned child should exist")
  assertTrue(clonedChild ~= child, "cloned child should be different instance")
end)

-- Destroy
test("Instance:Destroy sets Parent to nil", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Parent = parent

  child:Destroy()
  assertEqual(child.Parent, nil, "Parent should be nil after Destroy")
end)

test("Destroy removes from parent's children", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Name = "ToDestroy"
  child.Parent = parent

  child:Destroy()
  local found = parent:FindFirstChild("ToDestroy")
  assertEqual(found, nil, "child should not be found after Destroy")
end)

-- ClearAllChildren
test("ClearAllChildren removes all children", function()
  local parent = Instance.new("Folder")
  Instance.new("Part").Parent = parent
  Instance.new("Part").Parent = parent
  Instance.new("Part").Parent = parent

  assertEqual(#parent:GetChildren(), 3, "should have 3 children")
  parent:ClearAllChildren()
  assertEqual(#parent:GetChildren(), 0, "should have 0 children after clear")
end)

return tf.printSummary()
