--[[
  CoreSystem Loader for Lune
  Loads the CSharp.lua CoreSystem runtime in Lune's environment.

  CoreSystem uses dot-notation require paths (e.g., "CoreSystem.String"),
  but Lune uses file-based paths. This loader bridges the gap.
--]]

local fs = require("@lune/fs")
local luau = require("@lune/luau")
local process = require("@lune/process")

-- Path to CoreSystem relative to project root
-- process.cwd is test/LuneTests, so we go up two levels
local PROJECT_ROOT = process.cwd .. "/../.."
local CORESYSTEM_PATH = PROJECT_ROOT .. "/CSharp.lua/CoreSystem.Lua/CoreSystem"

-- Cache for loaded modules
local moduleCache = {}

-- Track which namespaces have been initialized
local initializedNamespaces = {}
local cachedSystemRef = nil

-- Create a shared environment for all CoreSystem modules
-- This needs to include _VERSION = "Luau" so CoreSystem detects the Luau environment
-- Also need to expose the real _G so modules can write to it
local coreSystemEnv = setmetatable({
  _VERSION = "Luau",
  _G = _G,  -- Expose the real _G so rawset(_G, ...) works correctly
}, { __index = _G, __newindex = _G })

-- Custom require function for CoreSystem modules
local function loadModule(modulePath: string): any
  -- Check cache first
  if moduleCache[modulePath] then
    return moduleCache[modulePath]
  end

  -- Convert dot notation to file path
  local filePath = CORESYSTEM_PATH .. "/" .. modulePath:gsub("%.", "/") .. ".lua"

  -- Check if file exists
  if not fs.isFile(filePath) then
    error("CoreSystem module not found: " .. modulePath .. " (tried " .. filePath .. ")")
  end

  -- Read and compile the file
  local source = fs.readFile(filePath)
  local chunk, compileErr = luau.compile(source, {
    debugLevel = 1,
    optimizationLevel = 1,
  })

  if not chunk then
    error("Failed to compile " .. modulePath .. ": " .. tostring(compileErr))
  end

  -- Create function from bytecode with custom environment
  local fn, loadErr = luau.load(chunk, {
    debugName = modulePath,
    environment = coreSystemEnv,
  })

  if not fn then
    error("Failed to load " .. modulePath .. ": " .. tostring(loadErr))
  end

  -- Execute and cache
  local result = fn()
  moduleCache[modulePath] = result
  return result
end

-- List of modules to load in order (from init.lua)
local MODULE_ORDER = {
  "Core",
  "Interfaces",
  "Exception",
  "Math",
  "Number",
  "Char",
  "String",
  "Boolean",
  "Delegate",
  "Enum",
  "TimeSpan",
  "DateTime",
  "Collections.EqualityComparer",
  "Array",
  "Span",
  "MemoryExtensions",
  "ReadOnlySpan",
  "Type",
  "Collections.List",
  "Collections.Dictionary",
  "Collections.Queue",
  "Collections.Stack",
  "Collections.HashSet",
  "Collections.LinkedList",
  "Collections.SortedSet",
  "Collections.SortedList",
  "Collections.SortedDictionary",
  "Collections.PriorityQueue",
  "Collections.Linq",
  "Convert",
  "Random",
  "Text.StringBuilder",
  "Console",
  -- Skip IO.File - not supported in Roblox/Lune context
  "Reflection.Assembly",
  "Threading.Timer",
  "Threading.Thread",
  "Threading.Task",
  "Utilities",
  "Globalization.Globalization",
  "Numerics.HashCodeHelper",
  "Numerics.Complex",
  "Numerics.Matrix3x2",
  "Numerics.Matrix4x4",
  "Numerics.Plane",
  "Numerics.Quaternion",
  "Numerics.Vector",
  "Numerics.Vector2",
  "Numerics.Vector3",
  "Numerics.Vector4",
}

-- Load CoreSystem with the given configuration
local function loadCoreSystem(config: { systemNamespace: string }?): any
  config = config or { systemNamespace = "LuneTest" }

  if not config.systemNamespace or config.systemNamespace == "" then
    error("systemNamespace is required to initialize CoreSystem")
  end

  local namespace = config.systemNamespace

  -- If this namespace is already initialized, return the cached System
  if initializedNamespaces[namespace] then
    local current = _G
    for segment in namespace:gmatch("[^%.]+") do
      current = current[segment]
    end
    return current.System
  end

  -- If CoreSystem was already loaded for a different namespace, copy to this one
  if cachedSystemRef then
    -- Create the namespace hierarchy in _G
    local current = _G
    local segments = {}
    for segment in namespace:gmatch("[^%.]+") do
      table.insert(segments, segment)
    end

    for i, segment in ipairs(segments) do
      if not current[segment] then
        current[segment] = {}
      end
      current = current[segment]
    end

    -- Copy the System reference
    current.System = cachedSystemRef
    initializedNamespaces[namespace] = true

    return cachedSystemRef
  end

  -- First time loading - set up global config before loading Core.lua
  rawset(_G, "CSharpLuaSystemConfig", config)

  -- Load Core.lua first - it sets up the System namespace
  -- Core.lua returns a function that takes the config
  local coreModule = loadModule("Core")
  coreModule(config)

  -- Load remaining modules in order
  for _, moduleName in MODULE_ORDER do
    if moduleName ~= "Core" then
      local ok, err = pcall(function()
        loadModule(moduleName)
      end)
      if not ok then
        -- Some modules might fail if they have Roblox-specific issues
        -- Just warn and continue
        warn("Failed to load module " .. moduleName .. ": " .. tostring(err))
      end
    end
  end

  -- Get System from the configured namespace
  local current = _G
  for segment in namespace:gmatch("[^%.]+") do
    current = current[segment]
    if not current then
      error("Could not find namespace segment: " .. segment)
    end
  end
  local System = current.System

  -- Cache for future namespace requests
  cachedSystemRef = System
  initializedNamespaces[namespace] = true

  -- Clear internal reference
  rawset(_G, "__CoreSystemInternal", nil)

  return System
end

-- Clear the module cache and namespace tracking (for loading fresh)
local function clearCache()
  moduleCache = {}
  initializedNamespaces = {}
  cachedSystemRef = nil
end

return {
  load = loadCoreSystem,
  loadModule = loadModule,
  clearCache = clearCache,
  CORESYSTEM_PATH = CORESYSTEM_PATH,
}
