--[[
  Enum ToString Transform Tests
  Tests System.EnumToString for converting enum values to their names.

  In the transpiled code, enums are tables with both name->value
  and value->name mappings.
--]]

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertNotNil = tf.assertNotNil

-- Set up Roblox globals before loading CoreSystem
local robloxGlobals = require("../../lib/roblox_globals")
robloxGlobals.setupGlobals()

-- Load CoreSystem (use consistent namespace across all tests)
local csLoader = require("../../lib/coresystem_loader")
local System = csLoader.load({ systemNamespace = "LuneTest" })

print("\n--- Enum ToString Transform Tests ---")

-- Create a mock enum table (mimics transpiled enum)
local TestColor = {
  Red = 0,
  Green = 1,
  Blue = 2,
  [0] = "Red",
  [1] = "Green",
  [2] = "Blue"
}

local TestStatus = {
  Pending = 0,
  Active = 1,
  Completed = 2,
  Failed = 3,
  [0] = "Pending",
  [1] = "Active",
  [2] = "Completed",
  [3] = "Failed"
}

test("System.EnumToString converts 0 to 'Red'", function()
  local result = System.EnumToString(0, TestColor)
  assertEqual(result, "Red", "0 -> Red")
end)

test("System.EnumToString converts 1 to 'Green'", function()
  local result = System.EnumToString(1, TestColor)
  assertEqual(result, "Green", "1 -> Green")
end)

test("System.EnumToString converts 2 to 'Blue'", function()
  local result = System.EnumToString(2, TestColor)
  assertEqual(result, "Blue", "2 -> Blue")
end)

test("System.EnumToString with different enum", function()
  assertEqual(System.EnumToString(0, TestStatus), "Pending", "0 -> Pending")
  assertEqual(System.EnumToString(1, TestStatus), "Active", "1 -> Active")
  assertEqual(System.EnumToString(2, TestStatus), "Completed", "2 -> Completed")
  assertEqual(System.EnumToString(3, TestStatus), "Failed", "3 -> Failed")
end)

test("System.EnumToString unknown value returns something", function()
  local result = System.EnumToString(99, TestColor)
  -- Unknown values typically return the number as string
  assertNotNil(result, "Should return something for unknown value")
end)

test("Enum value used in string concatenation", function()
  local color = TestColor.Green
  -- Transpiled from: $"Selected color: {color}"
  local result = "Selected color: " .. System.EnumToString(color, TestColor)
  assertEqual(result, "Selected color: Green", "String with enum")
end)

test("Enum comparison works", function()
  local status = TestStatus.Active
  assertTrue(status == TestStatus.Active, "Should equal Active")
  assertTrue(status ~= TestStatus.Pending, "Should not equal Pending")
end)

test("Enum values are numeric", function()
  assertEqual(TestColor.Red, 0, "Red is 0")
  assertEqual(TestColor.Green, 1, "Green is 1")
  assertEqual(TestColor.Blue, 2, "Blue is 2")
end)

test("Enum lookup by name", function()
  local colorValue = TestColor["Green"]
  assertEqual(colorValue, 1, "Green lookup = 1")
end)

test("Enum lookup by value", function()
  local colorName = TestColor[1]
  assertEqual(colorName, "Green", "1 lookup = Green")
end)

return tf.printSummary()
