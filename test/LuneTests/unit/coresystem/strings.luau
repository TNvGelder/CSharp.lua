--[[
  String Static Method Tests
  Tests System.String static methods.
  Validates: str:Contains(x) -> System.String.Contains(str, x)
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

print("\n--- String Static Method Tests ---")

-- Contains
test("System.String.Contains finds substring", function()
  local str = "Hello World"
  assertTrue(System.String.Contains(str, "World"), "Should contain 'World'")
end)

test("System.String.Contains returns false for missing", function()
  local str = "Hello World"
  assertFalse(System.String.Contains(str, "Foo"), "Should not contain 'Foo'")
end)

test("System.String.Contains empty string", function()
  assertTrue(System.String.Contains("Hello", ""), "Should contain empty string")
end)

-- StartsWith
test("System.String.StartsWith works", function()
  local str = "Hello World"
  assertTrue(System.String.StartsWith(str, "Hello"), "Should start with 'Hello'")
  assertFalse(System.String.StartsWith(str, "World"), "Should not start with 'World'")
end)

test("System.String.StartsWith empty string", function()
  assertTrue(System.String.StartsWith("Hello", ""), "Should start with empty string")
end)

-- EndsWith
test("System.String.EndsWith works", function()
  local str = "Hello World"
  assertTrue(System.String.EndsWith(str, "World"), "Should end with 'World'")
  assertFalse(System.String.EndsWith(str, "Hello"), "Should not end with 'Hello'")
end)

-- Substring
test("System.String.Substring with start and length", function()
  local str = "Hello World"
  -- Note: C# uses 0-based indexing
  assertEqual(System.String.Substring(str, 0, 5), "Hello", "Substring(0,5)")
end)

test("System.String.Substring from index to end", function()
  local str = "Hello World"
  assertEqual(System.String.Substring(str, 6), "World", "Substring(6)")
end)

-- IndexOf
test("System.String.IndexOf finds character", function()
  local str = "Hello"
  -- Returns 0-based index, first 'l' is at index 2
  local idx = System.String.IndexOf(str, "l")
  assertEqual(idx, 2, "Index of 'l' should be 2")
end)

test("System.String.IndexOf returns -1 for missing", function()
  local str = "Hello"
  local idx = System.String.IndexOf(str, "x")
  assertEqual(idx, -1, "Index of 'x' should be -1")
end)

-- ToUpper / ToLower
test("System.String.ToUpper works", function()
  assertEqual(System.String.ToUpper("hello"), "HELLO", "ToUpper")
end)

test("System.String.ToLower works", function()
  assertEqual(System.String.ToLower("HELLO"), "hello", "ToLower")
end)

-- Trim
test("System.String.Trim works", function()
  assertEqual(System.String.Trim("  hello  "), "hello", "Trim")
end)

test("System.String.Trim no whitespace", function()
  assertEqual(System.String.Trim("hello"), "hello", "Trim no-op")
end)

-- Replace
test("System.String.Replace works", function()
  assertEqual(System.String.Replace("hello", "l", "x"), "hexxo", "Replace l with x")
end)

test("System.String.Replace no match", function()
  assertEqual(System.String.Replace("hello", "z", "x"), "hello", "Replace no match")
end)

-- IsNullOrEmpty
test("System.String.IsNullOrEmpty with empty", function()
  assertTrue(System.String.IsNullOrEmpty(""), "Empty string is null or empty")
end)

test("System.String.IsNullOrEmpty with nil", function()
  assertTrue(System.String.IsNullOrEmpty(nil), "nil is null or empty")
end)

test("System.String.IsNullOrEmpty with value", function()
  assertFalse(System.String.IsNullOrEmpty("hello"), "non-empty is not null or empty")
end)

-- Join
-- Note: System.String.Join requires Array module which has compatibility issues in Lune
-- This is tested in the Roblox environment instead
-- test("System.String.Join works", function()
--   local result = System.String.Join(", ", {"a", "b", "c"})
--   assertEqual(result, "a, b, c", "Join with comma")
-- end)

-- Split
test("System.String.Split works", function()
  local parts = System.String.Split("a,b,c", ",")
  assertEqual(parts[1], "a", "first part")
  assertEqual(parts[2], "b", "second part")
  assertEqual(parts[3], "c", "third part")
end)

return tf.printSummary()
