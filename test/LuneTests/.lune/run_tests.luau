--[[
  Lune Test Runner for CSharp.lua Roblox Typing System

  Runs all tests in the unit/ and integration/ directories.
  Returns exit code 1 if any tests fail, 0 if all pass.

  Usage: lune run run_tests
--]]

local fs = require("@lune/fs")
local process = require("@lune/process")
local stdio = require("@lune/stdio")

print(stdio.color("cyan") .. "\n" .. string.rep("=", 60))
print("  CSharp.lua Lune Test Runner")
print("  Testing Roblox Typing System Integration")
print(string.rep("=", 60) .. stdio.color("reset") .. "\n")

-- Get test directory path (process.cwd is already test/LuneTests when run from there)
local scriptPath = process.cwd

-- Collect all test files recursively
local function collectTests(dir: string, tests: {string}): {string}
  tests = tests or {}

  if not fs.isDir(dir) then
    return tests
  end

  for _, entry in fs.readDir(dir) do
    local path = dir .. "/" .. entry

    if fs.isDir(path) then
      -- Recurse into subdirectories
      collectTests(path, tests)
    elseif entry:match("%.luau$") and not entry:match("^_") then
      -- Add .luau files (excluding those starting with _)
      table.insert(tests, path)
    end
  end

  return tests
end

-- Run all tests
local testDirs = {
  scriptPath .. "/unit/datatypes",
  scriptPath .. "/unit/instance",
  scriptPath .. "/unit/coresystem",
  scriptPath .. "/unit/transforms",
  scriptPath .. "/integration/testgame",
  scriptPath .. "/integration/transpiled",
}

local allPassed = true
local totalFilesRun = 0
local totalFilesPassed = 0
local totalFilesFailed = 0
local failedFiles = {}

for _, testDir in testDirs do
  if fs.isDir(testDir) then
    local tests = collectTests(testDir, {})

    for _, testFile in tests do
      totalFilesRun += 1

      -- Get relative path for display
      local displayPath = testFile:gsub(scriptPath .. "/", "")

      print(stdio.color("blue") .. "\n>>> Running: " .. displayPath .. stdio.color("reset"))

      local ok, result = pcall(function()
        -- Convert absolute path to relative path
        -- Since this script is in .lune/, we need to go up one level with ../
        local relativePath = testFile:gsub(scriptPath .. "/", ""):gsub("%.luau$", "")
        relativePath = "../" .. relativePath
        return require(relativePath)
      end)

      if not ok then
        print(stdio.color("red") .. "[ERROR] Failed to run test file:" .. stdio.color("reset"))
        print("  " .. tostring(result))
        totalFilesFailed += 1
        table.insert(failedFiles, displayPath)
        allPassed = false
      else
        -- result is the return value of tf.printSummary() (boolean)
        if result == true then
          totalFilesPassed += 1
        else
          totalFilesFailed += 1
          table.insert(failedFiles, displayPath)
          allPassed = false
        end
      end
    end
  else
    print(stdio.color("yellow") .. "Warning: Test directory not found: " .. testDir .. stdio.color("reset"))
  end
end

-- Final summary
print(stdio.color("cyan") .. "\n" .. string.rep("=", 60))
print("  FINAL SUMMARY")
print(string.rep("=", 60) .. stdio.color("reset"))
print(string.format("Test Files Run:    %d", totalFilesRun))
print(string.format("Test Files Passed: %d", totalFilesPassed))
print(string.format("Test Files Failed: %d", totalFilesFailed))

if #failedFiles > 0 then
  print(stdio.color("red") .. "\nFailed test files:" .. stdio.color("reset"))
  for _, file in failedFiles do
    print("  - " .. file)
  end
end

print("")

if allPassed and totalFilesRun > 0 then
  print(stdio.color("green") .. "All tests passed!" .. stdio.color("reset"))
  process.exit(0)
elseif totalFilesRun == 0 then
  print(stdio.color("yellow") .. "No tests were run!" .. stdio.color("reset"))
  process.exit(1)
else
  print(stdio.color("red") .. "Some tests failed!" .. stdio.color("reset"))
  process.exit(1)
end
