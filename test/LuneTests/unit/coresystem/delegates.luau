--[[
  Delegate Tests
  Tests System.DelegateCombine and System.DelegateRemove.
  Pattern from test/RobloxCompat/test_roblox_transforms.lua
--]]

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertNil = tf.assertNil

-- Set up Roblox globals before loading CoreSystem
local robloxGlobals = require("../../lib/roblox_globals")
robloxGlobals.setupGlobals()

-- Load CoreSystem (use consistent namespace across all tests)
local csLoader = require("../../lib/coresystem_loader")
local System = csLoader.load({ systemNamespace = "LuneTest" })

print("\n--- Delegate Tests ---")

test("System.DelegateCombine with nil first argument", function()
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

test("System.DelegateCombine with nil second argument", function()
  local count = 0
  local d1 = function() count = count + 1 end

  local combined = System.DelegateCombine(d1, nil)
  combined()
  assertEqual(count, 1, "Should call d1")
end)

test("System.DelegateCombine both nil returns nil", function()
  local combined = System.DelegateCombine(nil, nil)
  assertNil(combined, "Should return nil")
end)

test("System.DelegateRemove removes handler", function()
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
  assertNil(result, "Removing from nil should return nil")
end)

test("System.DelegateRemove non-existent handler", function()
  local count = 0
  local d1 = function() count = count + 1 end
  local d2 = function() count = count + 10 end
  local d3 = function() count = count + 100 end

  local combined = System.DelegateCombine(d1, d2)
  -- Try to remove d3 which was never added
  local afterRemove = System.DelegateRemove(combined, d3)
  afterRemove()
  -- Should still call both d1 and d2
  assertEqual(count, 11, "Should call d1 and d2 since d3 was not in list")
end)

test("Delegates work with arguments", function()
  local result = 0
  local d1 = function(x) result = result + x end
  local d2 = function(x) result = result + x * 2 end

  local combined = System.DelegateCombine(d1, d2)
  combined(5)
  assertEqual(result, 15, "d1(5)=5, d2(5)=10, total=15")
end)

test("Delegates preserve order", function()
  local order = {}
  local d1 = function() table.insert(order, "a") end
  local d2 = function() table.insert(order, "b") end
  local d3 = function() table.insert(order, "c") end

  local combined = System.DelegateCombine(System.DelegateCombine(d1, d2), d3)
  combined()

  assertEqual(order[1], "a", "first should be a")
  assertEqual(order[2], "b", "second should be b")
  assertEqual(order[3], "c", "third should be c")
end)

return tf.printSummary()
