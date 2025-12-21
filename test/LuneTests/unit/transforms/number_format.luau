--[[
  Number Format Transform Tests
  Tests that number formatting in interpolated strings is correctly transformed.
  Validates: {value:F0} -> System.toString(value, "F0")

  The compiler transforms C# string interpolation with format specifiers
  into calls to System.toString().
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

print("\n--- Number Format Transform Tests ---")

-- These tests simulate what the transpiled code would look like
-- after format specifiers in interpolated strings are transformed

test("F0 format transform: System.toString(value, 'F0')", function()
  local value = 123.456
  -- Transpiled from: $"{value:F0}"
  local result = System.toString(value, "F0")
  assertEqual(result, "123", "F0 should round to integer")
end)

test("F1 format transform: System.toString(value, 'F1')", function()
  local value = 123.456
  -- Transpiled from: $"{value:F1}"
  local result = System.toString(value, "F1")
  assertEqual(result, "123.5", "F1 should have 1 decimal")
end)

test("F2 format transform: System.toString(value, 'F2')", function()
  local value = 123.456
  -- Transpiled from: $"{value:F2}"
  local result = System.toString(value, "F2")
  assertEqual(result, "123.46", "F2 should have 2 decimals")
end)

test("String concatenation with formatted numbers", function()
  local x = 10.5
  local y = 20.7
  -- Transpiled from: $"Position: ({x:F0}, {y:F0})"
  -- Note: 10.5 rounds to 10 using banker's rounding (round-half-to-even)
  local result = "Position: (" .. System.toString(x, "F0") .. ", " .. System.toString(y, "F0") .. ")"
  assertEqual(result, "Position: (10, 21)", "Should format both numbers")
end)

test("Format in loop scenario", function()
  local values = {1.1, 2.2, 3.3}
  local results = {}
  for _, v in values do
    -- Transpiled from: $"{v:F1}"
    table.insert(results, System.toString(v, "F1"))
  end
  assertEqual(results[1], "1.1", "first")
  assertEqual(results[2], "2.2", "second")
  assertEqual(results[3], "3.3", "third")
end)

test("Format with negative numbers", function()
  local value = -45.678
  -- Transpiled from: $"{value:F1}"
  local result = System.toString(value, "F1")
  assertEqual(result, "-45.7", "Should handle negative with F1")
end)

test("Format with zero", function()
  local value = 0
  -- Transpiled from: $"{value:F2}"
  local result = System.toString(value, "F2")
  assertEqual(result, "0.00", "Zero with F2 format")
end)

test("Mixed format and plain values in string", function()
  local name = "Player"
  local score = 1234.567
  -- Transpiled from: $"{name} scored {score:F0} points"
  local result = name .. " scored " .. System.toString(score, "F0") .. " points"
  assertEqual(result, "Player scored 1235 points", "Mixed format string")
end)

test("No format uses plain toString", function()
  local value = 42
  -- Transpiled from: $"{value}"
  local result = System.toString(value)
  assertEqual(result, "42", "No format = plain toString")
end)

test("Boolean formatting", function()
  -- Transpiled from: $"{isActive}"
  local result1 = System.toString(true)
  local result2 = System.toString(false)
  assertEqual(result1, "True", "true -> 'True'")
  assertEqual(result2, "False", "false -> 'False'")
end)

return tf.printSummary()
