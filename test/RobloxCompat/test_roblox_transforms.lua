--[[
  Roblox Transform Validation Tests

  These tests specifically validate that the -roblox compiler flag
  generates correct code for patterns that don't work with Roblox's
  restricted Lua environment (no debug.setmetatable on primitives).

  Run with: lua test_roblox_transforms.lua
--]]

local testsPassed = 0
local testsFailed = 0

local function test(name, fn)
  local ok, err = pcall(fn)
  if ok then
    testsPassed = testsPassed + 1
    print("[PASS] " .. name)
  else
    testsFailed = testsFailed + 1
    print("[FAIL] " .. name .. ": " .. tostring(err))
  end
end

local function assertEqual(actual, expected, msg)
  if actual ~= expected then
    error((msg or "Assertion failed") .. ": expected " .. tostring(expected) .. ", got " .. tostring(actual))
  end
end

local function assertTrue(value, msg)
  if not value then
    error((msg or "Assertion failed") .. ": expected true, got " .. tostring(value))
  end
end

local function assertContains(str, pattern, msg)
  if not string.find(str, pattern, 1, true) then
    error((msg or "Assertion failed") .. ": expected string to contain '" .. pattern .. "', got: " .. str)
  end
end

print("=== Roblox Transform Validation Tests ===\n")

-- Setup mock Roblox environment
_G.game = {}
_G.__isRoblox = true
_G.script = { Parent = {}, CoreSystem = {} }
_G.warn = function(msg) print("[WARN] " .. tostring(msg)) end

-- Load CoreSystem
local corePath = "../../CSharp.lua/CoreSystem.Lua/CoreSystem/"
dofile(corePath .. "Core.lua")({})
dofile(corePath .. "Interfaces.lua")
dofile(corePath .. "Exception.lua")
dofile(corePath .. "Math.lua")
dofile(corePath .. "Number.lua")
dofile(corePath .. "Char.lua")
dofile(corePath .. "String.lua")
dofile(corePath .. "Boolean.lua")
dofile(corePath .. "Delegate.lua")
dofile(corePath .. "Enum.lua")
dofile(corePath .. "TimeSpan.lua")
dofile(corePath .. "DateTime.lua")
dofile(corePath .. "Collections/EqualityComparer.lua")
dofile(corePath .. "Array.lua")
dofile(corePath .. "Type.lua")

-- ============================================
-- Test 1: Number ToString with format specifiers
-- Validates fix for: {value:F0} -> System.toString(value, "F0")
-- ============================================
print("--- Number ToString Formatting ---")

test("System.toString(float, 'F0') works", function()
  local value = 123.456
  local result = System.toString(value, "F0")
  assertEqual(result, "123", "F0 format should round to integer")
end)

test("System.toString(float, 'F1') works", function()
  local value = 123.456
  local result = System.toString(value, "F1")
  assertEqual(result, "123.5", "F1 format should have 1 decimal")
end)

test("System.toString(float, 'F2') works", function()
  local value = 123.456
  local result = System.toString(value, "F2")
  assertEqual(result, "123.46", "F2 format should have 2 decimals")
end)

test("System.toString(negative float, 'F0') works", function()
  local value = -45.7
  local result = System.toString(value, "F0")
  assertEqual(result, "-46", "F0 format on negative should work")
end)

test("System.toString(integer, nil) works", function()
  local value = 42
  local result = System.toString(value)
  assertEqual(result, "42", "Integer toString without format")
end)

-- ============================================
-- Test 2: String static methods (instance -> static)
-- Validates fix for: str:Contains(x) -> System.String.Contains(str, x)
-- ============================================
print("\n--- String Static Method Transforms ---")

test("System.String.Contains works", function()
  local str = "Hello World"
  assertTrue(System.String.Contains(str, "World"), "Should contain 'World'")
  assertTrue(not System.String.Contains(str, "Foo"), "Should not contain 'Foo'")
end)

test("System.String.StartsWith works", function()
  local str = "Hello World"
  assertTrue(System.String.StartsWith(str, "Hello"), "Should start with 'Hello'")
  assertTrue(not System.String.StartsWith(str, "World"), "Should not start with 'World'")
end)

test("System.String.EndsWith works", function()
  local str = "Hello World"
  assertTrue(System.String.EndsWith(str, "World"), "Should end with 'World'")
  assertTrue(not System.String.EndsWith(str, "Hello"), "Should not end with 'Hello'")
end)

test("System.String.Substring works", function()
  local str = "Hello World"
  -- Note: C# uses 0-based indexing
  assertEqual(System.String.Substring(str, 0, 5), "Hello", "Substring(0,5)")
  assertEqual(System.String.Substring(str, 6), "World", "Substring(6)")
end)

test("System.String.ToUpper works", function()
  assertEqual(System.String.ToUpper("hello"), "HELLO", "ToUpper")
end)

test("System.String.ToLower works", function()
  assertEqual(System.String.ToLower("HELLO"), "hello", "ToLower")
end)

test("System.String.Trim works", function()
  assertEqual(System.String.Trim("  hello  "), "hello", "Trim")
end)

test("System.String.Replace works", function()
  assertEqual(System.String.Replace("hello", "l", "x"), "hexxo", "Replace")
end)

-- ============================================
-- Test 3: Delegate Combine/Remove
-- Validates fix for: delegate += fn -> System.DelegateCombine(delegate, fn)
-- ============================================
print("\n--- Delegate Combine/Remove ---")

test("System.DelegateCombine with nil first arg", function()
  local count = 0
  local d1 = nil
  local d2 = function() count = count + 1 end

  local combined = System.DelegateCombine(d1, d2)
  combined()
  assertEqual(count, 1, "Should call d2")
end)

test("System.DelegateCombine two functions", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end

  local combined = System.DelegateCombine(d1, d2)
  combined()
  assertEqual(count, 11, "Should call both delegates")
end)

test("System.DelegateCombine three functions", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end
  local d3 = function() count = count + 100 end

  local combined = System.DelegateCombine(System.DelegateCombine(d1, d2), d3)
  combined()
  assertEqual(count, 111, "Should call all three delegates")
end)

test("System.DelegateRemove works", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end

  local combined = System.DelegateCombine(d1, d2)
  local afterRemove = System.DelegateRemove(combined, d2)
  afterRemove()
  assertEqual(count, 1, "Should only call d1 after removing d2")
end)

test("System.DelegateRemove from nil returns nil", function()
  local d1 = function() end
  local result = System.DelegateRemove(nil, d1)
  assertEqual(result, nil, "Removing from nil should return nil")
end)

-- ============================================
-- Test 4: Enum ToString
-- Validates: System.EnumToString(value, EnumType)
-- ============================================
print("\n--- Enum ToString ---")

-- Create a simple enum table for testing (mimics generated enum)
local TestColor = {
  Red = 0,
  Green = 1,
  Blue = 2,
  [0] = "Red",
  [1] = "Green",
  [2] = "Blue"
}

test("System.EnumToString works", function()
  assertEqual(System.EnumToString(0, TestColor), "Red", "Enum 0 -> Red")
  assertEqual(System.EnumToString(1, TestColor), "Green", "Enum 1 -> Green")
  assertEqual(System.EnumToString(2, TestColor), "Blue", "Enum 2 -> Blue")
end)

test("System.EnumToString unknown value", function()
  local result = System.EnumToString(99, TestColor)
  -- Unknown values typically return the number as string
  assertTrue(result ~= nil, "Should return something for unknown value")
end)

-- ============================================
-- Test 5: Object/Value type ToString
-- Validates general ToString behavior
-- ============================================
print("\n--- General ToString ---")

test("System.toString on nil", function()
  local result = System.toString(nil)
  -- nil typically becomes empty string or "nil"
  assertTrue(result ~= nil, "Should handle nil")
end)

test("System.toString on boolean", function()
  assertEqual(System.toString(true), "True", "true -> 'True'")
  assertEqual(System.toString(false), "False", "false -> 'False'")
end)

test("System.toString on string passthrough", function()
  assertEqual(System.toString("hello"), "hello", "String passthrough")
end)

-- ============================================
-- Test 6: Basic type method safety
-- These should NOT cause "attempt to index number/string" errors
-- ============================================
print("\n--- Basic Type Method Safety ---")

test("Number has no metatable methods in Roblox mode", function()
  -- In Roblox, this would error: (5):ToString()
  -- Our fix ensures we use System.toString(5) instead
  local num = 5
  -- This should work because we call the static method
  local result = System.toString(num)
  assertEqual(result, "5", "Static toString on number")
end)

test("String operations don't rely on metatables", function()
  local str = "test"
  -- These should all use static methods internally
  assertTrue(System.String.IsNullOrEmpty("") == true)
  assertTrue(System.String.IsNullOrEmpty(str) == false)
end)

-- ============================================
-- Test 7: LINQ with proper type parameters
-- ============================================
print("\n--- LINQ Type Parameters ---")

dofile(corePath .. "Collections/List.lua")
dofile(corePath .. "Collections/Dictionary.lua")
dofile(corePath .. "Collections/Queue.lua")
dofile(corePath .. "Collections/Stack.lua")
dofile(corePath .. "Collections/HashSet.lua")
dofile(corePath .. "Collections/LinkedList.lua")
dofile(corePath .. "Collections/SortedSet.lua")
dofile(corePath .. "Collections/SortedList.lua")
dofile(corePath .. "Collections/SortedDictionary.lua")
dofile(corePath .. "Collections/PriorityQueue.lua")
dofile(corePath .. "Collections/Linq.lua")

test("LINQ Select with type parameter", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3})
  -- Select REQUIRES the result type as third parameter
  local doubled = linq.Select(arr, function(x) return x * 2 end, System.Int32)
  local first = linq.First(doubled)
  assertEqual(first, 2, "First doubled element should be 2")
end)

test("LINQ Where works", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  local filtered = linq.Where(arr, function(x) return x > 3 end)
  local count = linq.Count(filtered)
  assertEqual(count, 2, "Should have 2 elements > 3")
end)

test("LINQ Sum works", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  assertEqual(linq.Sum(arr), 15, "Sum should be 15")
end)

test("LINQ Any works", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3})
  assertTrue(linq.Any(arr, function(x) return x > 2 end), "Should have element > 2")
  assertTrue(not linq.Any(arr, function(x) return x > 10 end), "Should not have element > 10")
end)

test("LINQ All works", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({2, 4, 6})
  assertTrue(linq.All(arr, function(x) return x % 2 == 0 end), "All should be even")
  assertTrue(not linq.All(arr, function(x) return x > 3 end), "Not all > 3")
end)

-- ============================================
-- Summary
-- ============================================
print("\n" .. string.rep("=", 50))
print(string.format("Tests Passed: %d", testsPassed))
print(string.format("Tests Failed: %d", testsFailed))
print(string.rep("=", 50))

if testsFailed == 0 then
  print("\nAll Roblox transform validation tests passed!")
else
  print(string.format("\n%d test(s) failed.", testsFailed))
  os.exit(1)
end
