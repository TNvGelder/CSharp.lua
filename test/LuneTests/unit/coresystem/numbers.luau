--[[
  Number Formatting Tests
  Tests System.toString with format specifiers.
  Validates: {value:F0} -> System.toString(value, "F0")
--]]

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue

-- Set up Roblox globals before loading CoreSystem
local robloxGlobals = require("../../lib/roblox_globals")
robloxGlobals.setupGlobals()

-- Load CoreSystem (use consistent namespace across all tests)
local csLoader = require("../../lib/coresystem_loader")
local System = csLoader.load({ systemNamespace = "LuneTest" })

print("\n--- Number Formatting Tests ---")

-- F format (fixed-point)
test("System.toString with F0 format", function()
  local value = 123.456
  local result = System.toString(value, "F0")
  assertEqual(result, "123", "F0 should round to integer")
end)

test("System.toString with F1 format", function()
  local value = 123.456
  local result = System.toString(value, "F1")
  assertEqual(result, "123.5", "F1 should have 1 decimal")
end)

test("System.toString with F2 format", function()
  local value = 123.456
  local result = System.toString(value, "F2")
  assertEqual(result, "123.46", "F2 should have 2 decimals")
end)

test("System.toString with F0 on rounding boundary", function()
  local value = 99.5
  local result = System.toString(value, "F0")
  assertEqual(result, "100", "99.5 rounds to 100")
end)

test("System.toString with F0 on negative", function()
  local value = -45.7
  local result = System.toString(value, "F0")
  assertEqual(result, "-46", "F0 format on negative")
end)

test("System.toString with F1 on negative", function()
  local value = -45.67
  local result = System.toString(value, "F1")
  assertEqual(result, "-45.7", "F1 on negative")
end)

-- Plain toString
test("System.toString on integer without format", function()
  local value = 42
  local result = System.toString(value)
  assertEqual(result, "42", "Integer toString")
end)

test("System.toString on float without format", function()
  local value = 3.14
  local result = System.toString(value)
  -- May have trailing decimals
  assertTrue(string.find(result, "3.14") ~= nil, "Should contain 3.14")
end)

-- Boolean toString
test("System.toString on true", function()
  local result = System.toString(true)
  assertEqual(result, "True", "true -> 'True'")
end)

test("System.toString on false", function()
  local result = System.toString(false)
  assertEqual(result, "False", "false -> 'False'")
end)

-- nil toString
test("System.toString on nil", function()
  local result = System.toString(nil)
  -- nil typically becomes empty string
  assertTrue(result ~= nil, "Should handle nil")
end)

-- String passthrough
test("System.toString on string passthrough", function()
  local result = System.toString("hello")
  assertEqual(result, "hello", "String passthrough")
end)

-- Zero handling
test("System.toString F2 on zero", function()
  local result = System.toString(0, "F2")
  assertEqual(result, "0.00", "0 with F2")
end)

test("System.toString F0 on zero", function()
  local result = System.toString(0, "F0")
  assertEqual(result, "0", "0 with F0")
end)

-- Large numbers
test("System.toString on large number", function()
  local value = 1234567
  local result = System.toString(value)
  assertEqual(result, "1234567", "Large number")
end)

-- Small decimals
test("System.toString F2 on small decimal", function()
  local value = 0.005
  local result = System.toString(value, "F2")
  assertEqual(result, "0.01", "0.005 rounds to 0.01")
end)

return tf.printSummary()
