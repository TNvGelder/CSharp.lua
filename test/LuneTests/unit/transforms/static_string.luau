--[[
  Static String Transform Tests
  Tests that string instance methods are correctly transformed to static calls.
  Validates: str:Contains(x) -> System.String.Contains(str, x)

  In Roblox, we can't add metatables to primitive strings, so
  the compiler transforms instance method calls to static calls.
--]]

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertFalse = tf.assertFalse

-- Set up Roblox globals before loading CoreSystem
local robloxGlobals = require("../../lib/roblox_globals")
robloxGlobals.setupGlobals()

-- Load CoreSystem (use consistent namespace across all tests)
local csLoader = require("../../lib/coresystem_loader")
local System = csLoader.load({ systemNamespace = "LuneTest" })

print("\n--- Static String Transform Tests ---")

-- These tests simulate what the transpiled code would look like
-- after the -roblox flag transforms instance methods to static calls

test("String Contains transform: System.String.Contains(str, x)", function()
  local str = "Hello World"
  -- Transpiled from: str.Contains("World")
  local result = System.String.Contains(str, "World")
  assertTrue(result, "Should find 'World'")
end)

test("String StartsWith transform: System.String.StartsWith(str, x)", function()
  local str = "Hello World"
  -- Transpiled from: str.StartsWith("Hello")
  local result = System.String.StartsWith(str, "Hello")
  assertTrue(result, "Should start with 'Hello'")
end)

test("String EndsWith transform: System.String.EndsWith(str, x)", function()
  local str = "Hello World"
  -- Transpiled from: str.EndsWith("World")
  local result = System.String.EndsWith(str, "World")
  assertTrue(result, "Should end with 'World'")
end)

test("String Substring transform: System.String.Substring(str, start, len)", function()
  local str = "Hello World"
  -- Transpiled from: str.Substring(0, 5)
  local result = System.String.Substring(str, 0, 5)
  assertEqual(result, "Hello", "Should get 'Hello'")
end)

test("String ToUpper transform: System.String.ToUpper(str)", function()
  local str = "hello"
  -- Transpiled from: str.ToUpper()
  local result = System.String.ToUpper(str)
  assertEqual(result, "HELLO", "Should be uppercase")
end)

test("String ToLower transform: System.String.ToLower(str)", function()
  local str = "HELLO"
  -- Transpiled from: str.ToLower()
  local result = System.String.ToLower(str)
  assertEqual(result, "hello", "Should be lowercase")
end)

test("String Trim transform: System.String.Trim(str)", function()
  local str = "  hello  "
  -- Transpiled from: str.Trim()
  local result = System.String.Trim(str)
  assertEqual(result, "hello", "Should trim whitespace")
end)

test("String Replace transform: System.String.Replace(str, old, new)", function()
  local str = "hello"
  -- Transpiled from: str.Replace("l", "x")
  local result = System.String.Replace(str, "l", "x")
  assertEqual(result, "hexxo", "Should replace 'l' with 'x'")
end)

test("String IndexOf transform: System.String.IndexOf(str, x)", function()
  local str = "hello"
  -- Transpiled from: str.IndexOf("l")
  local result = System.String.IndexOf(str, "l")
  assertEqual(result, 2, "First 'l' at index 2")
end)

test("Chained string operations via static methods", function()
  local str = "  Hello World  "
  -- Transpiled from: str.Trim().ToUpper().Contains("HELLO")
  local trimmed = System.String.Trim(str)
  local upper = System.String.ToUpper(trimmed)
  local contains = System.String.Contains(upper, "HELLO")
  assertTrue(contains, "Chained operations should work")
end)

test("Static methods don't rely on string metatables", function()
  -- This test ensures our static approach works in Roblox's restricted environment
  -- In Roblox, strings don't have custom metatables, so instance methods would fail
  local str = "test"
  -- Verify these static calls work (they should in any Lua environment)
  assertTrue(System.String.Contains(str, "es"), "Static Contains works")
  assertTrue(System.String.StartsWith(str, "te"), "Static StartsWith works")
  assertTrue(System.String.EndsWith(str, "st"), "Static EndsWith works")
end)

return tf.printSummary()
