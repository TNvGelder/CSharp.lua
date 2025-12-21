--[[
  Simple Test Framework for Lune Tests
  Matches the pattern from test/RobloxCompat/test_roblox_transforms.lua
--]]

local stdio = require("@lune/stdio")

local TestFramework = {}
TestFramework.passed = 0
TestFramework.failed = 0
TestFramework.errors = {}

function TestFramework.test(name: string, fn: () -> ())
  local ok, err = pcall(fn)
  if ok then
    TestFramework.passed += 1
    print(stdio.color("green") .. "[PASS]" .. stdio.color("reset") .. " " .. name)
  else
    TestFramework.failed += 1
    table.insert(TestFramework.errors, { name = name, error = tostring(err) })
    print(stdio.color("red") .. "[FAIL]" .. stdio.color("reset") .. " " .. name .. ": " .. tostring(err))
  end
end

function TestFramework.assertEqual(actual: any, expected: any, msg: string?)
  if actual ~= expected then
    error((msg or "Assertion failed") .. ": expected " .. tostring(expected) .. ", got " .. tostring(actual))
  end
end

function TestFramework.assertTrue(value: any, msg: string?)
  if not value then
    error((msg or "Assertion failed") .. ": expected true, got " .. tostring(value))
  end
end

function TestFramework.assertFalse(value: any, msg: string?)
  if value then
    error((msg or "Assertion failed") .. ": expected false, got " .. tostring(value))
  end
end

function TestFramework.assertNotNil(value: any, msg: string?)
  if value == nil then
    error((msg or "Assertion failed") .. ": expected non-nil value")
  end
end

function TestFramework.assertNil(value: any, msg: string?)
  if value ~= nil then
    error((msg or "Assertion failed") .. ": expected nil, got " .. tostring(value))
  end
end

function TestFramework.assertApprox(actual: number, expected: number, tolerance: number?, msg: string?)
  tolerance = tolerance or 0.001
  if math.abs(actual - expected) > tolerance then
    error((msg or "Assertion failed") .. ": expected ~" .. tostring(expected) .. ", got " .. tostring(actual) .. " (tolerance: " .. tostring(tolerance) .. ")")
  end
end

function TestFramework.assertContains(str: string, pattern: string, msg: string?)
  if not string.find(str, pattern, 1, true) then
    error((msg or "Assertion failed") .. ": expected string to contain '" .. pattern .. "', got: " .. str)
  end
end

function TestFramework.assertError(fn: () -> (), msg: string?)
  local ok, _ = pcall(fn)
  if ok then
    error((msg or "Assertion failed") .. ": expected function to throw an error")
  end
end

function TestFramework.printSummary(): boolean
  print("")
  print(string.rep("=", 50))
  print(string.format("Tests Passed: %d", TestFramework.passed))
  print(string.format("Tests Failed: %d", TestFramework.failed))
  print(string.rep("=", 50))

  if TestFramework.failed > 0 then
    print("\nFailed tests:")
    for _, err in TestFramework.errors do
      print("  - " .. err.name)
    end
  end

  return TestFramework.failed == 0
end

function TestFramework.reset()
  TestFramework.passed = 0
  TestFramework.failed = 0
  TestFramework.errors = {}
end

return TestFramework
