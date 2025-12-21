--[[
  Lua Loader for Lune
  Loads .lua files using luau.compile() and luau.load()
  Provides custom environment support for loading CSharp.lua transpiled code.
--]]

local luau = require("@lune/luau")
local fs = require("@lune/fs")
local process = require("@lune/process")

local M = {}

-- Get the project root (C:\repos\CSharp.lua)
local function getProjectRoot()
  -- From test/LuneTests/lib, go up 3 levels
  local cwd = process.cwd
  -- cwd is test/LuneTests when running from there
  -- Go up 2 levels to get to project root
  return cwd .. "/../.."
end

M.projectRoot = getProjectRoot()

--[[
  Load a .lua file and execute it with a custom environment.

  @param path - Path to the .lua file (relative to project root or absolute)
  @param env - Optional custom environment table (defaults to _G with passthrough)
  @return The return value of the loaded script
--]]
function M.loadFile(path: string, env: { [string]: any }?)
  -- Resolve path relative to project root if not absolute
  local fullPath = path
  if not path:match("^/") and not path:match("^%a:") then
    fullPath = M.projectRoot .. "/" .. path
  end

  -- Read the source file
  local source = fs.readFile(fullPath)

  -- Compile to bytecode
  local bytecode = luau.compile(source, {
    debugLevel = 1,
    optimizationLevel = 1,
  })

  -- Create environment with _G passthrough if not provided
  local environment = env
  if not environment then
    environment = setmetatable({}, { __index = _G, __newindex = _G })
  end

  -- Load and execute
  local fn = luau.load(bytecode, {
    debugName = path,
    environment = environment,
  })

  return fn()
end

--[[
  Load a .lua file with the standard CSharp.lua environment.
  Sets up _G access and _VERSION = "Luau" for CoreSystem compatibility.

  @param path - Path to the .lua file
  @return The return value of the loaded script
--]]
function M.loadWithCSharpEnv(path: string)
  -- Use _G directly as the environment - simplest approach
  -- The transpiled code expects globals to be accessible
  return M.loadFile(path, _G)
end

--[[
  Create a loader function for a specific namespace.
  Useful for loading transpiled code that expects _G[namespace].

  @param namespace - The namespace (e.g., "TestGame")
  @return A loader function that loads files in that namespace context
--]]
function M.createNamespaceLoader(namespace: string)
  return function(path: string)
    -- Ensure namespace exists in _G
    if not _G[namespace] then
      error("Namespace '" .. namespace .. "' not initialized in _G")
    end

    return M.loadWithCSharpEnv(path)
  end
end

return M
