--[[
  Roblox Globals Adapter for Lune
  Sets up Roblox datatypes as globals from @lune/roblox module
  to match the Roblox Studio environment.
--]]

local roblox = require("@lune/roblox") :: any

-- Export Roblox datatypes as globals
local exports = {}

-- Instance creation
exports.Instance = roblox.Instance

-- Vector types
exports.Vector2 = roblox.Vector2
exports.Vector2int16 = roblox.Vector2int16
exports.Vector3 = roblox.Vector3
exports.Vector3int16 = roblox.Vector3int16

-- Transform types
exports.CFrame = roblox.CFrame

-- Color types
exports.Color3 = roblox.Color3
exports.BrickColor = roblox.BrickColor

-- UI dimension types
exports.UDim = roblox.UDim
exports.UDim2 = roblox.UDim2
exports.Rect = roblox.Rect

-- Geometric types
exports.Ray = roblox.Ray
exports.Region3 = roblox.Region3
exports.Region3int16 = roblox.Region3int16

-- Sequence types
exports.ColorSequence = roblox.ColorSequence
exports.ColorSequenceKeypoint = roblox.ColorSequenceKeypoint
exports.NumberSequence = roblox.NumberSequence
exports.NumberSequenceKeypoint = roblox.NumberSequenceKeypoint

-- Other types
exports.NumberRange = roblox.NumberRange
exports.Axes = roblox.Axes
exports.Faces = roblox.Faces
exports.PhysicalProperties = roblox.PhysicalProperties
exports.Font = roblox.Font
exports.Enum = roblox.Enum


-- Set up globals in _G for compatibility with Roblox code
local function setupGlobals()
  for name, value in exports do
    rawset(_G, name, value)
  end

  -- Set _VERSION to "Luau" so CoreSystem detects Luau environment correctly
  -- Lune reports its own _VERSION which CoreSystem doesn't recognize
  rawset(_G, "_VERSION", "Luau")

  -- Create mock game (DataModel) and workspace
  local game = roblox.Instance.new("DataModel")
  rawset(_G, "game", game)

  -- Create workspace as a child of game
  local workspace = roblox.Instance.new("Workspace")
  workspace.Parent = game
  rawset(_G, "workspace", workspace)

  -- Set __isRoblox flag for CoreSystem detection
  rawset(_G, "__isRoblox", true)

  -- Mock typeof function (Lune may have it, but just in case)
  if not rawget(_G, "typeof") then
    rawset(_G, "typeof", function(value)
      local t = type(value)
      if t == "table" or t == "userdata" then
        -- Check for ClassName property (Instance)
        local ok, className = pcall(function()
          return value.ClassName
        end)
        if ok and className then
          return className
        end
        -- Check for __type metafield
        local mt = getmetatable(value)
        if mt and mt.__type then
          return mt.__type
        end
      end
      return t
    end)
  end

  -- Mock warn function
  if not rawget(_G, "warn") then
    rawset(_G, "warn", function(...)
      print("[WARN]", ...)
    end)
  end

  return game, workspace
end

exports.setupGlobals = setupGlobals

return exports
