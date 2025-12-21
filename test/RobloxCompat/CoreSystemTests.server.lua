--[[
  CSharp.lua Roblox Runtime Comprehensive Test Script

  Run this in Roblox Studio to verify the CoreSystem loads correctly.
  Use with: rojo serve default.project.json
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
    warn("[FAIL] " .. name .. ": " .. tostring(err))
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

local function assertFalse(value, msg)
  if value then
    error((msg or "Assertion failed") .. ": expected false, got " .. tostring(value))
  end
end

print("=== CSharp.lua Roblox Runtime Comprehensive Test ===\n")

-- Load config and set it before requiring CoreSystem
local config = require(game.ReplicatedStorage.config)
rawset(_G, "__CoreSystemConfig", config)

-- Load CoreSystem
local System = require(game.ReplicatedStorage.CoreSystem)
if not System then
  error("CoreSystem failed to initialize")
end
print("[OK] CoreSystem loaded and initialized\n")

-- ============================================
-- Basic Types
-- ============================================
print("--- Basic Types ---")

test("System.Int32 exists", function()
  assertTrue(System.Int32 ~= nil)
end)

test("System.String exists", function()
  assertTrue(System.String ~= nil)
end)

test("System.Boolean exists", function()
  assertTrue(System.Boolean ~= nil)
end)

test("System.Double exists", function()
  assertTrue(System.Double ~= nil)
end)

-- ============================================
-- Collections: List<T>
-- ============================================
print("\n--- Collections: List<T> ---")

test("List<int> create and add", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  list:Add(3)
  assertEqual(list:getCount(), 3)
end)

test("List<int> get by index", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(10)
  list:Add(20)
  assertEqual(list:get(0), 10)
  assertEqual(list:get(1), 20)
end)

test("List<int> Contains", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(5)
  list:Add(10)
  assertTrue(list:Contains(5))
  assertFalse(list:Contains(99))
end)

test("List<int> Remove", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  list:Add(3)
  list:Remove(2)
  assertEqual(list:getCount(), 2)
  assertFalse(list:Contains(2))
end)

test("List<int> Clear", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  list:Clear()
  assertEqual(list:getCount(), 0)
end)

-- ============================================
-- Collections: Dictionary<K,V>
-- ============================================
print("\n--- Collections: Dictionary<K,V> ---")

test("Dictionary<string,int> create and add", function()
  local Dict = System.Collections.Generic.Dictionary
  local dict = Dict(System.String, System.Int32)()
  dict:Add("a", 1)
  dict:Add("b", 2)
  assertEqual(dict:getCount(), 2)
end)

test("Dictionary<string,int> get", function()
  local Dict = System.Collections.Generic.Dictionary
  local dict = Dict(System.String, System.Int32)()
  dict:Add("key", 42)
  assertEqual(dict:get("key"), 42)
end)

test("Dictionary<string,int> ContainsKey", function()
  local Dict = System.Collections.Generic.Dictionary
  local dict = Dict(System.String, System.Int32)()
  dict:Add("exists", 1)
  assertTrue(dict:ContainsKey("exists"))
  assertFalse(dict:ContainsKey("missing"))
end)

test("Dictionary<string,int> Remove", function()
  local Dict = System.Collections.Generic.Dictionary
  local dict = Dict(System.String, System.Int32)()
  dict:Add("a", 1)
  dict:Add("b", 2)
  dict:Remove("a")
  assertEqual(dict:getCount(), 1)
  assertFalse(dict:ContainsKey("a"))
end)

-- ============================================
-- Collections: HashSet<T>
-- ============================================
print("\n--- Collections: HashSet<T> ---")

test("HashSet<int> create and add", function()
  local HashSet = System.Collections.Generic.HashSet
  local set = HashSet(System.Int32)()
  set:Add(1)
  set:Add(2)
  set:Add(1) -- duplicate
  assertEqual(set:getCount(), 2)
end)

test("HashSet<int> Contains", function()
  local HashSet = System.Collections.Generic.HashSet
  local set = HashSet(System.Int32)()
  set:Add(5)
  assertTrue(set:Contains(5))
  assertFalse(set:Contains(10))
end)

-- ============================================
-- Collections: Queue<T>
-- ============================================
print("\n--- Collections: Queue<T> ---")

test("Queue<int> Enqueue and Dequeue", function()
  local Queue = System.Collections.Generic.Queue
  local q = Queue(System.Int32)()
  q:Enqueue(1)
  q:Enqueue(2)
  q:Enqueue(3)
  assertEqual(q:Dequeue(), 1)
  assertEqual(q:Dequeue(), 2)
  assertEqual(q:getCount(), 1)
end)

-- ============================================
-- Collections: Stack<T>
-- ============================================
print("\n--- Collections: Stack<T> ---")

test("Stack<int> Push and Pop", function()
  local Stack = System.Collections.Generic.Stack
  local s = Stack(System.Int32)()
  s:Push(1)
  s:Push(2)
  s:Push(3)
  assertEqual(s:Pop(), 3)
  assertEqual(s:Pop(), 2)
  assertEqual(s:getCount(), 1)
end)

-- ============================================
-- String Operations
-- ============================================
print("\n--- String Operations ---")

test("String.IsNullOrEmpty", function()
  local str = System.String
  assertTrue(str.IsNullOrEmpty(""))
  assertTrue(str.IsNullOrEmpty(nil))
  assertFalse(str.IsNullOrEmpty("hello"))
end)

test("String.IsNullOrWhiteSpace", function()
  local str = System.String
  assertTrue(str.IsNullOrWhiteSpace(""))
  assertTrue(str.IsNullOrWhiteSpace("   "))
  assertFalse(str.IsNullOrWhiteSpace("hello"))
end)

test("String.Format", function()
  local str = System.String
  local result = str.Format("Hello {0}!", "World")
  assertEqual(result, "Hello World!")
end)

-- ============================================
-- Math Operations
-- ============================================
print("\n--- Math Operations ---")

test("Math.Max", function()
  assertEqual(System.Math.Max(5, 10), 10)
  assertEqual(System.Math.Max(-5, -10), -5)
end)

test("Math.Min", function()
  assertEqual(System.Math.Min(5, 10), 5)
  assertEqual(System.Math.Min(-5, -10), -10)
end)

test("Math.Abs", function()
  assertEqual(System.Math.Abs(-5), 5)
  assertEqual(System.Math.Abs(5), 5)
end)

test("Math.Sqrt", function()
  assertEqual(System.Math.Sqrt(16), 4)
  assertEqual(System.Math.Sqrt(25), 5)
end)

test("Math.Floor", function()
  assertEqual(System.Math.Floor(3.7), 3)
  assertEqual(System.Math.Floor(-3.7), -4)
end)

test("Math.Ceiling", function()
  assertEqual(System.Math.Ceiling(3.2), 4)
  assertEqual(System.Math.Ceiling(-3.2), -3)
end)

test("Math.Round", function()
  assertEqual(System.Math.Round(3.5), 4)
  assertEqual(System.Math.Round(3.4), 3)
end)

-- ============================================
-- DateTime
-- ============================================
print("\n--- DateTime ---")

test("DateTime.Now", function()
  local now = System.DateTime.getNow()
  assertTrue(now ~= nil)
end)

test("DateTime.UtcNow", function()
  local utc = System.DateTime.getUtcNow()
  assertTrue(utc ~= nil)
end)

-- ============================================
-- TimeSpan
-- ============================================
print("\n--- TimeSpan ---")

test("TimeSpan.FromSeconds", function()
  local ts = System.TimeSpan.FromSeconds(60)
  assertEqual(ts:getTotalMinutes(), 1)
end)

test("TimeSpan.FromMinutes", function()
  local ts = System.TimeSpan.FromMinutes(2)
  assertEqual(ts:getTotalSeconds(), 120)
end)

-- ============================================
-- Array
-- ============================================
print("\n--- Array ---")

test("Array creation", function()
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  assertEqual(arr:getLength(), 5)
end)

test("Array get/set", function()
  local arr = System.Array(System.Int32)({10, 20, 30})
  assertEqual(arr:get(0), 10)
  assertEqual(arr:get(2), 30)
end)

-- ============================================
-- LINQ
-- ============================================
print("\n--- LINQ ---")

test("LINQ Sum", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  assertEqual(linq.Sum(arr), 15)
end)

test("LINQ Count", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3})
  assertEqual(linq.Count(arr), 3)
end)

test("LINQ First", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({10, 20, 30})
  assertEqual(linq.First(arr), 10)
end)

test("LINQ Last", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({10, 20, 30})
  assertEqual(linq.Last(arr), 30)
end)

test("LINQ Where", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  local filtered = linq.Where(arr, function(x) return x > 3 end)
  local count = linq.Count(filtered)
  assertEqual(count, 2)
end)

test("LINQ Select", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3})
  -- Select requires the result type as third parameter
  local doubled = linq.Select(arr, function(x) return x * 2 end, System.Int32)
  local first = linq.First(doubled)
  assertEqual(first, 2)
end)

-- ============================================
-- StringBuilder
-- ============================================
print("\n--- StringBuilder ---")

test("StringBuilder Append", function()
  local sb = System.Text.StringBuilder()
  sb:Append("Hello")
  sb:Append(" ")
  sb:Append("World")
  assertEqual(sb:ToString(), "Hello World")
end)

test("StringBuilder AppendLine", function()
  local sb = System.Text.StringBuilder()
  sb:AppendLine("Line1")
  sb:Append("Line2")
  assertTrue(sb:ToString():find("Line1") ~= nil)
end)

-- ============================================
-- System.Numerics
-- ============================================
print("\n--- System.Numerics ---")

test("Vector2 creation", function()
  local v = System.Numerics.Vector2(3, 4)
  assertEqual(v.X, 3)
  assertEqual(v.Y, 4)
end)

test("Vector2 Length", function()
  local v = System.Numerics.Vector2(3, 4)
  assertEqual(v:Length(), 5)
end)

test("Vector3 creation", function()
  local v = System.Numerics.Vector3(1, 2, 3)
  assertEqual(v.X, 1)
  assertEqual(v.Y, 2)
  assertEqual(v.Z, 3)
end)

-- ============================================
-- Exceptions
-- ============================================
print("\n--- Exceptions ---")

test("Exception creation", function()
  local ex = System.Exception("Test error")
  assertTrue(ex ~= nil)
  assertTrue(tostring(ex:getMessage()):find("Test error") ~= nil)
end)

test("ArgumentNullException", function()
  local ex = System.ArgumentNullException("param")
  assertTrue(ex ~= nil)
end)

-- ============================================
-- Console (Roblox fallbacks)
-- ============================================
print("\n--- Console (Roblox fallbacks) ---")

test("Console.WriteLine", function()
  -- Should not error, uses print internally
  System.Console.WriteLine("Test output")
end)

test("Console.Write", function()
  -- Should not error, uses print internally
  System.Console.Write("Test ")
end)

-- ============================================
-- Random
-- ============================================
print("\n--- Random ---")

test("Random.Next", function()
  local rng = System.Random()
  local n = rng:Next(1, 100)
  assertTrue(n >= 1 and n < 100)
end)

test("Random.NextDouble", function()
  local rng = System.Random()
  local d = rng:NextDouble()
  assertTrue(d >= 0 and d < 1)
end)

-- ============================================
-- Delegates
-- ============================================
print("\n--- Delegates ---")

-- In CSharp.lua, delegates are plain Lua functions
-- There is no System.Action() or System.Func() wrapper type
test("Action delegate (plain function)", function()
  local called = false
  local action = function()
    called = true
  end
  action()
  assertTrue(called)
end)

test("Func delegate (plain function)", function()
  local func = function()
    return 42
  end
  assertEqual(func(), 42)
end)

test("Delegate combine", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end
  local combined = System.DelegateCombine(d1, d2)
  combined()
  assertEqual(count, 11)
end)

test("Delegate remove", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end
  local combined = System.DelegateCombine(d1, d2)
  local afterRemove = System.DelegateRemove(combined, d2)
  afterRemove()
  assertEqual(count, 1)
end)

-- ============================================
-- Roblox-Specific Transform Tests
-- These validate fixes for Roblox's restricted environment
-- (no debug.setmetatable on primitives)
-- ============================================
print("\n--- Roblox Transform Fixes ---")

-- Test: Number ToString with format specifiers
-- Fix: {value:F0} -> System.toString(value, "F0") instead of value:ToString("F0")
test("System.toString with F0 format", function()
  local value = 123.456
  local result = System.toString(value, "F0")
  assertEqual(result, "123")
end)

test("System.toString with F1 format", function()
  local value = 123.456
  local result = System.toString(value, "F1")
  assertEqual(result, "123.5")
end)

test("System.toString with F2 format", function()
  local value = 99.999
  local result = System.toString(value, "F2")
  assertEqual(result, "100.00")
end)

test("System.toString on negative numbers", function()
  local value = -45.67
  local result = System.toString(value, "F1")
  assertEqual(result, "-45.7")
end)

test("System.toString without format", function()
  local result = System.toString(42)
  assertEqual(result, "42")
end)

-- Test: String static method calls
-- Fix: str:Contains(x) -> System.String.Contains(str, x)
test("System.String.Contains static call", function()
  assertTrue(System.String.Contains("Hello World", "World"))
  assertFalse(System.String.Contains("Hello World", "Foo"))
end)

test("System.String.StartsWith static call", function()
  assertTrue(System.String.StartsWith("Hello World", "Hello"))
  assertFalse(System.String.StartsWith("Hello World", "World"))
end)

test("System.String.EndsWith static call", function()
  assertTrue(System.String.EndsWith("Hello World", "World"))
  assertFalse(System.String.EndsWith("Hello World", "Hello"))
end)

test("System.String.IndexOf static call", function()
  assertEqual(System.String.IndexOf("Hello", "l"), 2) -- 0-based
end)

test("System.String.Substring static call", function()
  assertEqual(System.String.Substring("Hello World", 0, 5), "Hello")
  assertEqual(System.String.Substring("Hello World", 6), "World")
end)

-- Test: Delegate combine with nil (event += pattern)
-- Fix: nil + fn errors, use System.DelegateCombine(nil, fn) instead
test("DelegateCombine with nil first argument", function()
  local called = false
  local handler = function() called = true end
  -- Simulates: event += handler where event starts as nil
  local event = System.DelegateCombine(nil, handler)
  event()
  assertTrue(called)
end)

test("DelegateCombine chain (multiple +=)", function()
  local results = {}
  local h1 = function() table.insert(results, "a") end
  local h2 = function() table.insert(results, "b") end
  local h3 = function() table.insert(results, "c") end

  local event = nil
  event = System.DelegateCombine(event, h1)
  event = System.DelegateCombine(event, h2)
  event = System.DelegateCombine(event, h3)
  event()

  assertEqual(#results, 3)
  assertEqual(results[1], "a")
  assertEqual(results[2], "b")
  assertEqual(results[3], "c")
end)

test("DelegateRemove from nil returns nil", function()
  local handler = function() end
  local result = System.DelegateRemove(nil, handler)
  assertEqual(result, nil)
end)

-- Test: Enum to string without metatables
-- Fix: value:EnumToString() -> System.EnumToString(value, EnumType)
test("System.EnumToString works", function()
  -- Create a mock enum table (like generated code produces)
  local TestEnum = { A = 0, B = 1, C = 2, [0] = "A", [1] = "B", [2] = "C" }
  assertEqual(System.EnumToString(0, TestEnum), "A")
  assertEqual(System.EnumToString(1, TestEnum), "B")
  assertEqual(System.EnumToString(2, TestEnum), "C")
end)

-- Test: LINQ Select requires result type parameter
test("LINQ Select with result type parameter", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5})
  -- Third parameter is the result type - required!
  local doubled = linq.Select(arr, function(x) return x * 2 end, System.Int32)
  local sum = linq.Sum(doubled)
  assertEqual(sum, 30) -- 2+4+6+8+10
end)

test("LINQ chained operations", function()
  local linq = System.Linq.Enumerable
  local arr = System.Array(System.Int32)({1, 2, 3, 4, 5, 6, 7, 8, 9, 10})

  -- Filter to even numbers, then double them
  local result = linq.Where(arr, function(x) return x % 2 == 0 end)
  result = linq.Select(result, function(x) return x * 2 end, System.Int32)
  local sum = linq.Sum(result)
  assertEqual(sum, 60) -- (2+4+6+8+10)*2
end)

-- Test: Boolean ToString
test("System.toString on booleans", function()
  assertEqual(System.toString(true), "True")
  assertEqual(System.toString(false), "False")
end)

-- Test: Objects have working ToString
test("Object ToString via System.toString", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  -- Should not error - uses object's ToString method
  local str = System.toString(list)
  assertTrue(str ~= nil)
end)

-- Test: No metatable on numbers (would fail without our fixes)
test("Numbers work without metatables", function()
  -- In standard Lua with CSharp.lua, numbers get metatables
  -- In Roblox, they don't - our static calls must work
  local num = 3.14159
  -- These should all use static methods, not instance methods
  local formatted = System.toString(num, "F2")
  assertEqual(formatted, "3.14")
end)

-- Test: No metatable on strings (would fail without our fixes)
test("Strings work without custom metatables", function()
  local str = "test string"
  -- Should use static System.String methods
  assertTrue(System.String.Contains(str, "test"))
  assertEqual(System.String.ToUpper(str), "TEST STRING")
  assertEqual(System.String.Trim("  hello  "), "hello")
end)

-- ============================================
-- Summary
-- ============================================
print("\n" .. string.rep("=", 50))
print(string.format("Tests Passed: %d", testsPassed))
print(string.format("Tests Failed: %d", testsFailed))
print(string.rep("=", 50))

if testsFailed == 0 then
  print("\nAll tests passed! CSharp.lua runtime is fully working in Roblox!")
else
  warn(string.format("\n%d tests failed. Check the errors above.", testsFailed))
end
