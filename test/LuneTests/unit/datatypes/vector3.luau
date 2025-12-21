--[[
  Vector3 Datatype Tests
  Tests Lune's Vector3 implementation and integration with CSharp.lua type system.
--]]

local roblox = require("@lune/roblox") :: any
local Vector3 = roblox.Vector3

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertApprox = tf.assertApprox

print("\n--- Vector3 Datatype Tests ---")

-- Basic constructor tests
test("Vector3.new creates vector with components", function()
  local v = Vector3.new(1, 2, 3)
  assertEqual(v.X, 1, "X component")
  assertEqual(v.Y, 2, "Y component")
  assertEqual(v.Z, 3, "Z component")
end)

test("Vector3.new with single argument", function()
  local v = Vector3.new(5)
  assertEqual(v.X, 5, "X should be 5")
  assertEqual(v.Y, 0, "Y should be 0")
  assertEqual(v.Z, 0, "Z should be 0")
end)

test("Vector3.new with no arguments", function()
  local v = Vector3.new()
  assertEqual(v.X, 0, "X should be 0")
  assertEqual(v.Y, 0, "Y should be 0")
  assertEqual(v.Z, 0, "Z should be 0")
end)

-- Constants
test("Vector3 constants exist", function()
  assertTrue(Vector3.zero ~= nil, "zero exists")
  assertTrue(Vector3.one ~= nil, "one exists")
  assertTrue(Vector3.xAxis ~= nil, "xAxis exists")
  assertTrue(Vector3.yAxis ~= nil, "yAxis exists")
  assertTrue(Vector3.zAxis ~= nil, "zAxis exists")
end)

test("Vector3.zero is (0, 0, 0)", function()
  local v = Vector3.zero
  assertEqual(v.X, 0, "X")
  assertEqual(v.Y, 0, "Y")
  assertEqual(v.Z, 0, "Z")
end)

test("Vector3.one is (1, 1, 1)", function()
  local v = Vector3.one
  assertEqual(v.X, 1, "X")
  assertEqual(v.Y, 1, "Y")
  assertEqual(v.Z, 1, "Z")
end)

test("Vector3.xAxis is (1, 0, 0)", function()
  assertEqual(Vector3.xAxis, Vector3.new(1, 0, 0), "xAxis")
end)

test("Vector3.yAxis is (0, 1, 0)", function()
  assertEqual(Vector3.yAxis, Vector3.new(0, 1, 0), "yAxis")
end)

test("Vector3.zAxis is (0, 0, 1)", function()
  assertEqual(Vector3.zAxis, Vector3.new(0, 0, 1), "zAxis")
end)

-- Arithmetic operations
test("Vector3 addition", function()
  local a = Vector3.new(1, 2, 3)
  local b = Vector3.new(4, 5, 6)
  local sum = a + b
  assertEqual(sum.X, 5, "sum X")
  assertEqual(sum.Y, 7, "sum Y")
  assertEqual(sum.Z, 9, "sum Z")
end)

test("Vector3 subtraction", function()
  local a = Vector3.new(5, 7, 9)
  local b = Vector3.new(1, 2, 3)
  local diff = a - b
  assertEqual(diff.X, 4, "diff X")
  assertEqual(diff.Y, 5, "diff Y")
  assertEqual(diff.Z, 6, "diff Z")
end)

test("Vector3 scalar multiplication", function()
  local v = Vector3.new(1, 2, 3)
  local scaled = v * 2
  assertEqual(scaled.X, 2, "scaled X")
  assertEqual(scaled.Y, 4, "scaled Y")
  assertEqual(scaled.Z, 6, "scaled Z")
end)

test("Vector3 scalar division", function()
  local v = Vector3.new(2, 4, 6)
  local divided = v / 2
  assertEqual(divided.X, 1, "divided X")
  assertEqual(divided.Y, 2, "divided Y")
  assertEqual(divided.Z, 3, "divided Z")
end)

test("Vector3 component-wise multiplication", function()
  local a = Vector3.new(2, 3, 4)
  local b = Vector3.new(5, 6, 7)
  local result = a * b
  assertEqual(result.X, 10, "result X")
  assertEqual(result.Y, 18, "result Y")
  assertEqual(result.Z, 28, "result Z")
end)

-- Magnitude and normalization
test("Vector3 Magnitude", function()
  local v = Vector3.new(3, 4, 0)
  assertApprox(v.Magnitude, 5, 0.001, "3-4-5 triangle magnitude")
end)

test("Vector3 Unit", function()
  local v = Vector3.new(3, 0, 0)
  local unit = v.Unit
  assertApprox(unit.X, 1, 0.001, "unit X")
  assertApprox(unit.Y, 0, 0.001, "unit Y")
  assertApprox(unit.Z, 0, 0.001, "unit Z")
end)

-- Equality
test("Vector3 equality", function()
  local a = Vector3.new(1, 2, 3)
  local b = Vector3.new(1, 2, 3)
  assertTrue(a == b, "should be equal")
end)

test("Vector3 inequality", function()
  local a = Vector3.new(1, 2, 3)
  local b = Vector3.new(1, 2, 4)
  assertTrue(a ~= b, "should not be equal")
end)

-- Methods
test("Vector3:Dot", function()
  local a = Vector3.new(1, 0, 0)
  local b = Vector3.new(1, 0, 0)
  assertEqual(a:Dot(b), 1, "parallel dot product")

  local c = Vector3.new(0, 1, 0)
  assertEqual(a:Dot(c), 0, "perpendicular dot product")
end)

test("Vector3:Cross", function()
  local x = Vector3.xAxis
  local y = Vector3.yAxis
  local cross = x:Cross(y)
  assertEqual(cross, Vector3.zAxis, "x cross y = z")
end)

test("Vector3:Lerp", function()
  local a = Vector3.new(0, 0, 0)
  local b = Vector3.new(10, 10, 10)
  local mid = a:Lerp(b, 0.5)
  assertEqual(mid.X, 5, "lerp X")
  assertEqual(mid.Y, 5, "lerp Y")
  assertEqual(mid.Z, 5, "lerp Z")
end)

return tf.printSummary()
