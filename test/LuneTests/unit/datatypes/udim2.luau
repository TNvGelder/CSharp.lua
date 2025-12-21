--[[
  UDim2 Datatype Tests
  Tests Lune's UDim2 implementation.
--]]

local roblox = require("@lune/roblox") :: any
local UDim = roblox.UDim
local UDim2 = roblox.UDim2

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertApprox = tf.assertApprox

print("\n--- UDim2 Datatype Tests ---")

-- UDim tests first
test("UDim.new creates dimension", function()
  local dim = UDim.new(0.5, 100)
  assertApprox(dim.Scale, 0.5, 0.001, "scale")
  assertEqual(dim.Offset, 100, "offset")
end)

test("UDim with no arguments", function()
  local dim = UDim.new()
  assertEqual(dim.Scale, 0, "scale")
  assertEqual(dim.Offset, 0, "offset")
end)

test("UDim addition", function()
  local a = UDim.new(0.5, 10)
  local b = UDim.new(0.3, 5)
  local sum = a + b
  assertApprox(sum.Scale, 0.8, 0.001, "sum scale")
  assertEqual(sum.Offset, 15, "sum offset")
end)

test("UDim subtraction", function()
  local a = UDim.new(0.5, 10)
  local b = UDim.new(0.3, 5)
  local diff = a - b
  assertApprox(diff.Scale, 0.2, 0.001, "diff scale")
  assertEqual(diff.Offset, 5, "diff offset")
end)

-- UDim2 constructor tests
test("UDim2.new with 4 numbers", function()
  local dim = UDim2.new(0.5, 10, 0.3, 20)
  assertApprox(dim.X.Scale, 0.5, 0.001, "X scale")
  assertEqual(dim.X.Offset, 10, "X offset")
  assertApprox(dim.Y.Scale, 0.3, 0.001, "Y scale")
  assertEqual(dim.Y.Offset, 20, "Y offset")
end)

test("UDim2.new with 2 UDims", function()
  local x = UDim.new(0.5, 10)
  local y = UDim.new(0.3, 20)
  local dim = UDim2.new(x, y)
  assertEqual(dim.X, x, "X UDim")
  assertEqual(dim.Y, y, "Y UDim")
end)

test("UDim2.new with no arguments", function()
  local dim = UDim2.new()
  assertEqual(dim.X.Scale, 0, "X scale")
  assertEqual(dim.X.Offset, 0, "X offset")
  assertEqual(dim.Y.Scale, 0, "Y scale")
  assertEqual(dim.Y.Offset, 0, "Y offset")
end)

-- fromScale and fromOffset
test("UDim2.fromScale", function()
  local dim = UDim2.fromScale(0.5, 0.75)
  assertApprox(dim.X.Scale, 0.5, 0.001, "X scale")
  assertEqual(dim.X.Offset, 0, "X offset")
  assertApprox(dim.Y.Scale, 0.75, 0.001, "Y scale")
  assertEqual(dim.Y.Offset, 0, "Y offset")
end)

test("UDim2.fromOffset", function()
  local dim = UDim2.fromOffset(100, 200)
  assertEqual(dim.X.Scale, 0, "X scale")
  assertEqual(dim.X.Offset, 100, "X offset")
  assertEqual(dim.Y.Scale, 0, "Y scale")
  assertEqual(dim.Y.Offset, 200, "Y offset")
end)

-- Width and Height properties
test("UDim2 Width and Height properties", function()
  local dim = UDim2.new(0.5, 10, 0.3, 20)
  assertEqual(dim.Width, dim.X, "Width equals X")
  assertEqual(dim.Height, dim.Y, "Height equals Y")
end)

-- Arithmetic
test("UDim2 addition", function()
  local a = UDim2.new(0.5, 10, 0.3, 20)
  local b = UDim2.new(0.2, 5, 0.1, 10)
  local sum = a + b
  assertApprox(sum.X.Scale, 0.7, 0.001, "sum X scale")
  assertEqual(sum.X.Offset, 15, "sum X offset")
  assertApprox(sum.Y.Scale, 0.4, 0.001, "sum Y scale")
  assertEqual(sum.Y.Offset, 30, "sum Y offset")
end)

test("UDim2 subtraction", function()
  local a = UDim2.new(0.5, 10, 0.3, 20)
  local b = UDim2.new(0.2, 5, 0.1, 10)
  local diff = a - b
  assertApprox(diff.X.Scale, 0.3, 0.001, "diff X scale")
  assertEqual(diff.X.Offset, 5, "diff X offset")
  assertApprox(diff.Y.Scale, 0.2, 0.001, "diff Y scale")
  assertEqual(diff.Y.Offset, 10, "diff Y offset")
end)

-- Equality
test("UDim2 equality", function()
  local a = UDim2.new(0.5, 10, 0.3, 20)
  local b = UDim2.new(0.5, 10, 0.3, 20)
  assertTrue(a == b, "should be equal")
end)

test("UDim2 inequality", function()
  local a = UDim2.new(0.5, 10, 0.3, 20)
  local b = UDim2.new(0.5, 10, 0.3, 21)
  assertTrue(a ~= b, "should not be equal")
end)

-- Lerp
test("UDim2:Lerp", function()
  local a = UDim2.new(0, 0, 0, 0)
  local b = UDim2.new(1, 100, 1, 200)
  local mid = a:Lerp(b, 0.5)
  assertApprox(mid.X.Scale, 0.5, 0.001, "lerp X scale")
  assertEqual(mid.X.Offset, 50, "lerp X offset")
  assertApprox(mid.Y.Scale, 0.5, 0.001, "lerp Y scale")
  assertEqual(mid.Y.Offset, 100, "lerp Y offset")
end)

return tf.printSummary()
