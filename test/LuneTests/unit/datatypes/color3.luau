--[[
  Color3 Datatype Tests
  Tests Lune's Color3 implementation.
--]]

local roblox = require("@lune/roblox") :: any
local Color3 = roblox.Color3

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertApprox = tf.assertApprox

print("\n--- Color3 Datatype Tests ---")

-- Constructor tests
test("Color3.new creates color with RGB components (0-1)", function()
  local c = Color3.new(1, 0, 0.5)
  assertApprox(c.R, 1, 0.001, "R component")
  assertApprox(c.G, 0, 0.001, "G component")
  assertApprox(c.B, 0.5, 0.001, "B component")
end)

test("Color3.new with no arguments creates black", function()
  local c = Color3.new()
  assertEqual(c.R, 0, "R should be 0")
  assertEqual(c.G, 0, "G should be 0")
  assertEqual(c.B, 0, "B should be 0")
end)

-- fromRGB tests
test("Color3.fromRGB creates color from 0-255 values", function()
  local c = Color3.fromRGB(255, 128, 0)
  assertApprox(c.R, 1, 0.01, "R should be 1")
  assertApprox(c.G, 0.502, 0.01, "G should be ~0.5")
  assertApprox(c.B, 0, 0.01, "B should be 0")
end)

test("Color3.fromRGB with 0,0,0 creates black", function()
  local c = Color3.fromRGB(0, 0, 0)
  assertEqual(c.R, 0, "R")
  assertEqual(c.G, 0, "G")
  assertEqual(c.B, 0, "B")
end)

test("Color3.fromRGB with 255,255,255 creates white", function()
  local c = Color3.fromRGB(255, 255, 255)
  assertApprox(c.R, 1, 0.01, "R")
  assertApprox(c.G, 1, 0.01, "G")
  assertApprox(c.B, 1, 0.01, "B")
end)

-- fromHSV tests
test("Color3.fromHSV creates color from HSV values", function()
  -- Red: H=0, S=1, V=1
  local red = Color3.fromHSV(0, 1, 1)
  assertApprox(red.R, 1, 0.01, "red R")
  assertApprox(red.G, 0, 0.01, "red G")
  assertApprox(red.B, 0, 0.01, "red B")
end)

-- Equality
test("Color3 equality", function()
  local a = Color3.new(1, 0, 0)
  local b = Color3.new(1, 0, 0)
  assertTrue(a == b, "should be equal")
end)

test("Color3 inequality", function()
  local a = Color3.new(1, 0, 0)
  local b = Color3.new(0, 1, 0)
  assertTrue(a ~= b, "should not be equal")
end)

-- Lerp
test("Color3:Lerp", function()
  local black = Color3.new(0, 0, 0)
  local white = Color3.new(1, 1, 1)
  local gray = black:Lerp(white, 0.5)
  assertApprox(gray.R, 0.5, 0.01, "lerp R")
  assertApprox(gray.G, 0.5, 0.01, "lerp G")
  assertApprox(gray.B, 0.5, 0.01, "lerp B")
end)

-- ToHSV
test("Color3:ToHSV", function()
  local red = Color3.new(1, 0, 0)
  local h, s, v = red:ToHSV()
  assertApprox(h, 0, 0.01, "hue")
  assertApprox(s, 1, 0.01, "saturation")
  assertApprox(v, 1, 0.01, "value")
end)

-- Common colors
test("Common colors via fromRGB", function()
  -- Red
  local red = Color3.fromRGB(255, 0, 0)
  assertApprox(red.R, 1, 0.01, "red R")
  assertApprox(red.G, 0, 0.01, "red G")
  assertApprox(red.B, 0, 0.01, "red B")

  -- Green
  local green = Color3.fromRGB(0, 255, 0)
  assertApprox(green.R, 0, 0.01, "green R")
  assertApprox(green.G, 1, 0.01, "green G")
  assertApprox(green.B, 0, 0.01, "green B")

  -- Blue
  local blue = Color3.fromRGB(0, 0, 255)
  assertApprox(blue.R, 0, 0.01, "blue R")
  assertApprox(blue.G, 0, 0.01, "blue G")
  assertApprox(blue.B, 1, 0.01, "blue B")
end)

return tf.printSummary()
