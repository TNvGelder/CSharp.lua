--[[
  Instance Hierarchy Tests
  Tests Parent/Child relationships, FindFirstChild, GetChildren, etc.
--]]

local roblox = require("@lune/roblox") :: any
local Instance = roblox.Instance

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertFalse = tf.assertFalse
local assertNotNil = tf.assertNotNil
local assertNil = tf.assertNil

print("\n--- Instance Hierarchy Tests ---")

-- Parent/Child basics
test("Setting Parent establishes relationship", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Parent = parent
  assertEqual(child.Parent, parent, "child's Parent should be parent")
end)

test("Parent can be set to nil", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Parent = parent
  child.Parent = nil
  assertNil(child.Parent, "Parent should be nil")
end)

-- GetChildren
test("GetChildren returns children", function()
  local parent = Instance.new("Folder")
  local child1 = Instance.new("Part")
  local child2 = Instance.new("Part")
  child1.Name = "Child1"
  child2.Name = "Child2"
  child1.Parent = parent
  child2.Parent = parent

  local children = parent:GetChildren()
  assertEqual(#children, 2, "should have 2 children")
end)

test("GetChildren returns empty table for no children", function()
  local folder = Instance.new("Folder")
  local children = folder:GetChildren()
  assertEqual(#children, 0, "should have 0 children")
end)

-- FindFirstChild
test("FindFirstChild finds child by name", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Name = "MyPart"
  child.Parent = parent

  local found = parent:FindFirstChild("MyPart")
  assertEqual(found, child, "should find the child")
end)

test("FindFirstChild returns nil for non-existent child", function()
  local parent = Instance.new("Folder")
  local found = parent:FindFirstChild("NonExistent")
  assertNil(found, "should return nil")
end)

test("FindFirstChild with recursive=true searches descendants", function()
  local root = Instance.new("Folder")
  root.Name = "Root"
  local mid = Instance.new("Folder")
  mid.Name = "Middle"
  mid.Parent = root
  local deep = Instance.new("Part")
  deep.Name = "DeepPart"
  deep.Parent = mid

  local found = root:FindFirstChild("DeepPart", true)
  assertEqual(found, deep, "should find deep child with recursive")
end)

test("FindFirstChild without recursive doesn't search descendants", function()
  local root = Instance.new("Folder")
  local mid = Instance.new("Folder")
  mid.Parent = root
  local deep = Instance.new("Part")
  deep.Name = "DeepPart"
  deep.Parent = mid

  local found = root:FindFirstChild("DeepPart", false)
  assertNil(found, "should not find deep child without recursive")
end)

-- FindFirstChildOfClass
test("FindFirstChildOfClass finds child by class", function()
  local parent = Instance.new("Folder")
  local part = Instance.new("Part")
  part.Parent = parent

  local found = parent:FindFirstChildOfClass("Part")
  assertEqual(found, part, "should find Part")
end)

test("FindFirstChildOfClass returns nil for wrong class", function()
  local parent = Instance.new("Folder")
  local part = Instance.new("Part")
  part.Parent = parent

  local found = parent:FindFirstChildOfClass("Model")
  assertNil(found, "should return nil for Model")
end)

-- FindFirstChildWhichIsA
test("FindFirstChildWhichIsA finds child that is a class", function()
  local parent = Instance.new("Folder")
  local part = Instance.new("Part")
  part.Parent = parent

  local found = parent:FindFirstChildWhichIsA("BasePart")
  assertEqual(found, part, "Part IsA BasePart")
end)

-- GetDescendants
test("GetDescendants returns all descendants", function()
  local root = Instance.new("Folder")
  local child1 = Instance.new("Part")
  child1.Parent = root
  local child2 = Instance.new("Folder")
  child2.Parent = root
  local grandchild = Instance.new("Part")
  grandchild.Parent = child2

  local descendants = root:GetDescendants()
  assertEqual(#descendants, 3, "should have 3 descendants")
end)

-- IsDescendantOf
test("IsDescendantOf returns true for ancestor", function()
  local root = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Parent = root

  assertTrue(child:IsDescendantOf(root), "child should be descendant of root")
end)

test("IsDescendantOf returns false for non-ancestor", function()
  local folder1 = Instance.new("Folder")
  local folder2 = Instance.new("Folder")
  local part = Instance.new("Part")
  part.Parent = folder1

  assertFalse(part:IsDescendantOf(folder2), "part should not be descendant of folder2")
end)

test("IsDescendantOf works with deep hierarchy", function()
  local root = Instance.new("Folder")
  local mid = Instance.new("Folder")
  mid.Parent = root
  local deep = Instance.new("Part")
  deep.Parent = mid

  assertTrue(deep:IsDescendantOf(root), "deep should be descendant of root")
  assertTrue(deep:IsDescendantOf(mid), "deep should be descendant of mid")
end)

-- IsAncestorOf
test("IsAncestorOf returns true for descendant", function()
  local root = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Parent = root

  assertTrue(root:IsAncestorOf(child), "root should be ancestor of child")
end)

test("IsAncestorOf returns false for non-descendant", function()
  local folder1 = Instance.new("Folder")
  local folder2 = Instance.new("Folder")
  local part = Instance.new("Part")
  part.Parent = folder1

  assertFalse(folder2:IsAncestorOf(part), "folder2 should not be ancestor of part")
end)

-- Index access for children
test("Children accessible via indexing", function()
  local parent = Instance.new("Folder")
  local child = Instance.new("Part")
  child.Name = "MyPart"
  child.Parent = parent

  local found = (parent :: any).MyPart
  assertEqual(found, child, "should access child via index")
end)

-- GetFullName
test("GetFullName returns path", function()
  local root = Instance.new("Folder")
  root.Name = "Root"
  local child = Instance.new("Part")
  child.Name = "MyPart"
  child.Parent = root

  local fullName = child:GetFullName()
  assertNotNil(fullName, "should return full name")
  assertTrue(string.find(fullName, "Root") ~= nil, "should contain Root")
  assertTrue(string.find(fullName, "MyPart") ~= nil, "should contain MyPart")
end)

return tf.printSummary()
