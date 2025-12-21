--[[
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
--]]

--[[
  Roblox Environment Detection
  ============================

  Auto-detection:
    - This module automatically detects Roblox by checking for 'game' and 'typeof' globals
    - When detected, sets rawget(_G, "__isRoblox") = true for other modules to check
    - In Roblox, modules are loaded via ModuleScript hierarchy (script.CoreSystem.*)
    - In standard Lua, modules are loaded via dot-notation require paths

  Testing outside Roblox:
    - The __isRoblox flag is set automatically; you don't need to set it manually
    - For unit testing in standard Lua, the flag will be false (as expected)
    - If you need to simulate Roblox behavior in tests, you can set the flag before
      requiring this module: rawset(_G, "__isRoblox", true)

  Module loading order:
    - Core.lua is loaded first and sets up the System namespace
    - All other modules depend on System being available
    - In Roblox, init() is called automatically and returns System directly
    - In standard Lua, init function is returned for manual initialization with config
--]]

-- Detect Roblox environment (must be at top level before init)
-- In Roblox, 'game' and 'typeof' are implicit globals
-- Note: Can't use pcall alone as it only returns success boolean, not the value
-- Accessing undefined globals doesn't error in standard Lua, so we check types directly
local isRoblox = (type(game) ~= "nil") and (type(typeof) == "function")

-- Set a flag that other modules can check via rawget(_G, "__isRoblox")
-- This is needed because 'game' is not in _G in Roblox (it's an implicit global)
if isRoblox then
  rawset(_G, "__isRoblox", true)
end

local function init(dir, conf)
  local require = require
  local load

  if not (conf and conf.systemNamespace and conf.systemNamespace ~= "") then
    error("systemNamespace is required to initialize CoreSystem. Set __CoreSystemConfig.systemNamespace before requiring CoreSystem.")
  end

  -- Set config before loading Core.lua so it can initialize in the right namespace
  rawset(_G, "CSharpLuaSystemConfig", conf)

  if isRoblox then
    -- Roblox: use ModuleScript hierarchy
    local root = script.CoreSystem
    load = function(module)
      local target = root
      for segment in module:gmatch("[^%.]+") do
        target = target:FindFirstChild(segment)
        if not target then
          error("CoreSystem module not found: " .. module)
        end
      end
      return require(target)
    end
  else
    -- Standard Lua: use dot-notation require
    dir = (dir and #dir > 0) and (dir .. ".CoreSystem.") or "CoreSystem."
    load = function(module) return require(dir .. module) end
  end

  load("Core")(conf)
  load("Interfaces")
  load("Exception")
  load("Math")
  load("Number")
  load("Char")
  load("String")
  load("Boolean")
  load("Delegate")
  load("Enum")
  load("TimeSpan")
  load("DateTime")
  load("Collections.EqualityComparer")
  load("Array")
  load("Span")
  load("MemoryExtensions")
  load("ReadOnlySpan")
  load("Type")
  load("Collections.List")
  load("Collections.Dictionary")
  load("Collections.Queue")
  load("Collections.Stack")
  load("Collections.HashSet")
  load("Collections.LinkedList")
  load("Collections.SortedSet")
  load("Collections.SortedList")
  load("Collections.SortedDictionary")
  load("Collections.PriorityQueue")
  load("Collections.Linq")
  load("Convert")
  load("Random")
  load("Text.StringBuilder")
  load("Console")
  if not isRoblox then
    -- File I/O not supported in Roblox
    load("IO.File")
  end
  load("Reflection.Assembly")
  load("Threading.Timer")
  load("Threading.Thread")
  load("Threading.Task")
  load("Utilities")
  load("Globalization.Globalization")
  load("Numerics.HashCodeHelper")
  load("Numerics.Complex")
  load("Numerics.Matrix3x2")
  load("Numerics.Matrix4x4")
  load("Numerics.Plane")
  load("Numerics.Quaternion")
  load("Numerics.Vector")
  load("Numerics.Vector2")
  load("Numerics.Vector3")
  load("Numerics.Vector4")

  -- Get System from the configured namespace
  local current = _G
  for segment in conf.systemNamespace:gmatch("[^%.]+") do
    current = current[segment]
  end
  local System = current.System

  -- Clear the internal reference that was used during module loading.
  -- Modules have already captured their local references, so this is safe.
  -- System now lives only in _G.<namespace>.System as intended.
  rawset(_G, "__CoreSystemInternal", nil)

  return System
end

-- In Roblox, auto-initialize and return System directly
-- In standard Lua, return the init function for manual initialization
if isRoblox then
  -- Allow config via _G.__CoreSystemConfig for Roblox
  local conf = rawget(_G, "__CoreSystemConfig")
  return init(nil, conf)
else
  return init
end
