--[[
  Roblox Compatibility Test

  Tests that the Roblox code paths in CoreSystem work correctly
  by mocking the `game` global before loading the runtime.

  NOTE ON __isRoblox FLAG:
  In actual Roblox, init.lua auto-detects the environment by checking for
  `game` and `typeof` globals, then sets __isRoblox automatically.

  In this test, we're running in standard Lua (not Roblox), so we manually
  set __isRoblox = true to test the Roblox code paths. This simulates what
  init.lua does automatically when running inside Roblox Studio/Player.

  We also mock `game`, `script`, and `warn` to prevent errors when code
  checks for these Roblox-specific globals.
--]]

local function printf(fmt, ...)
  print(string.format(fmt, ...))
end

local function test(name, fn)
  local ok, err = pcall(fn)
  if ok then
    printf("[PASS] %s", name)
  else
    printf("[FAIL] %s: %s", name, err)
    os.exit(1)
  end
end

local function testError(name, fn, expectedPattern)
  local ok, err = pcall(fn)
  if not ok and string.find(err, expectedPattern) then
    printf("[PASS] %s (expected error)", name)
  elseif ok then
    printf("[FAIL] %s: expected error but succeeded", name)
    os.exit(1)
  else
    printf("[FAIL] %s: wrong error: %s", name, err)
    os.exit(1)
  end
end

print("=== Roblox Compatibility Tests ===\n")

-- Mock Roblox environment BEFORE loading CoreSystem
-- We set __isRoblox manually because auto-detection only works in actual Roblox.
-- See init.lua documentation for details on how auto-detection works.
print("Setting up mock Roblox environment...")
_G.game = {}  -- Mock the Roblox 'game' global
_G.__isRoblox = true  -- Manually enable Roblox mode (normally auto-detected by init.lua)
_G.script = {
  Parent = {},
  CoreSystem = {}  -- Mock for init.lua's script.CoreSystem hierarchy
}
_G.warn = function(msg) print("[WARN] " .. tostring(msg)) end  -- Mock Roblox warn()

-- We need to test the individual modules, not load via All.lua
-- because All.lua expects actual ModuleScripts in Roblox
print("Loading CoreSystem modules with game mock...\n")

-- Load Core first (required by all other modules)
-- Core.lua reads CSharpLuaSystemConfig at load time, so we must set it BEFORE dofile
local corePath = "../../CSharp.lua/CoreSystem.Lua/CoreSystem/"
local conf = { systemNamespace = "CSharpLua" }
rawset(_G, "CSharpLuaSystemConfig", conf)
dofile(corePath .. "Core.lua")(conf)

-- Get System from the namespace (it's installed at _G.CSharpLua.System)
local System = _G.CSharpLua.System

print("--- Test 1: debug.setmetatable disabled ---")
test("debugsetmetatable is disabled in Roblox mode", function()
  -- In Roblox mode, debugsetmetatable should be falsy (nil or false)
  assert(not System.debugsetmetatable, "debugsetmetatable should be falsy")
end)

print("\n--- Test 2: Console fallbacks ---")
-- Load Console module
dofile(corePath .. "Interfaces.lua")
dofile(corePath .. "Exception.lua")
dofile(corePath .. "Math.lua")
dofile(corePath .. "Number.lua")
dofile(corePath .. "Char.lua")
dofile(corePath .. "String.lua")
dofile(corePath .. "Console.lua")

test("Console.WriteLine uses print", function()
  -- This should not error - it uses print() internally
  System.Console.WriteLine("Hello from Roblox mock!")
end)

test("Console.Write uses print", function()
  System.Console.Write("Test write... ")
  print("") -- newline
end)

testError("Console.Read throws error", function()
  System.Console.Read()
end, "not supported")

testError("Console.ReadLine throws error", function()
  System.Console.ReadLine()
end, "not supported")

print("\n--- Test 3: Environment.Exit throws error ---")
-- Load remaining dependencies for Utilities
dofile(corePath .. "Boolean.lua")
dofile(corePath .. "Delegate.lua")
dofile(corePath .. "Enum.lua")
dofile(corePath .. "TimeSpan.lua")
dofile(corePath .. "DateTime.lua")
dofile(corePath .. "Collections/EqualityComparer.lua")
dofile(corePath .. "Array.lua")
dofile(corePath .. "Span.lua")
dofile(corePath .. "MemoryExtensions.lua")
dofile(corePath .. "ReadOnlySpan.lua")
dofile(corePath .. "Type.lua")
dofile(corePath .. "Utilities.lua")

testError("Environment.Exit throws in Roblox", function()
  System.Environment.Exit(0)
end, "not supported")

print("\n--- Test 4: Exception traceback fallback ---")
test("traceback function exists", function()
  assert(System.traceback ~= nil, "traceback should exist")
  local trace = System.traceback()
  -- In mock mode, traceback returns empty string or limited info
  assert(type(trace) == "string", "traceback should return string")
end)

print("\n--- Test 5: Convert endianness detection ---")
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
dofile(corePath .. "Convert.lua")

test("BitConverter.IsLittleEndian exists", function()
  local bc = System.BitConverter
  assert(bc ~= nil, "BitConverter should exist")
  assert(type(bc.IsLittleEndian) == "boolean", "IsLittleEndian should be boolean")
  printf("  IsLittleEndian = %s", tostring(bc.IsLittleEndian))
end)

print("\n--- Test 6: Basic functionality still works ---")
dofile(corePath .. "Random.lua")
dofile(corePath .. "Text/StringBuilder.lua")

test("List<int> works", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  list:Add(3)
  assert(list:getCount() == 3, "List count should be 3")
  assert(list:get(0) == 1, "List[0] should be 1")
end)

test("Dictionary<string, int> works", function()
  local Dict = System.Collections.Generic.Dictionary
  local dict = Dict(System.String, System.Int32)()
  dict:Add("one", 1)
  dict:Add("two", 2)
  assert(dict:getCount() == 2, "Dict count should be 2")
  assert(dict:get("one") == 1, "Dict['one'] should be 1")
end)

test("StringBuilder works", function()
  local sb = System.Text.StringBuilder()
  sb:Append("Hello")
  sb:Append(" ")
  sb:Append("World")
  assert(sb:ToString() == "Hello World", "StringBuilder result mismatch")
end)

test("Math operations work", function()
  assert(System.Math.Max(5, 10) == 10, "Max failed")
  assert(System.Math.Min(5, 10) == 5, "Min failed")
  assert(System.Math.Abs(-5) == 5, "Abs failed")
end)

test("String operations work", function()
  assert(System.String.IsNullOrEmpty("") == true, "IsNullOrEmpty('') failed")
  assert(System.String.IsNullOrEmpty("hi") == false, "IsNullOrEmpty('hi') failed")
end)

print("\n=== All Roblox Compatibility Tests Passed! ===")
