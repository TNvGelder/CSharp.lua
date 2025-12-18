--[[
  CSharp.lua Roblox Runtime Test Script (Stub)

  The comprehensive test suite has been moved to:
    test/RobloxCompat/CoreSystemTests.server.lua

  This file exists for quick development testing in Roblox Studio.
  For the full test suite, use the tests in test/RobloxCompat/
--]]

-- Quick smoke test to verify CoreSystem loads
local System = require(game.ReplicatedStorage.CoreSystem)

print("=== Quick Roblox CoreSystem Smoke Test ===")
print("")

-- Basic sanity checks
local function test(name, fn)
  local ok, err = pcall(fn)
  if ok then
    print("[PASS] " .. name)
  else
    warn("[FAIL] " .. name .. ": " .. tostring(err))
  end
end

test("System namespace exists", function()
  assert(System ~= nil, "System should not be nil")
end)

test("System.String exists", function()
  assert(System.String ~= nil, "System.String should not be nil")
end)

test("System.Collections.Generic.List exists", function()
  assert(System.Collections.Generic.List ~= nil, "List should not be nil")
end)

test("List basic operations", function()
  local List = System.Collections.Generic.List
  local list = List(System.Int32)()
  list:Add(1)
  list:Add(2)
  list:Add(3)
  assert(list:getCount() == 3, "Count should be 3")
end)

print("")
print("=== Quick Smoke Test Complete ===")
print("For comprehensive tests, run test/RobloxCompat/CoreSystemTests.server.lua")
