-- Roblox Types Shim for CSharp.lua
-- Binds Roblox global types to work with the CSharp.lua type system.
-- This module sets up _G.<namespace>.Roblox with type definitions that
-- are compatible with System.default() and other CSharp.lua conventions.

-- Capture Roblox globals before module scope
-- (In Roblox, these are globals but not in _G)
local RobloxGlobals = {
  Vector2 = Vector2,
  Vector3 = Vector3,
  Vector3int16 = Vector3int16,
  CFrame = CFrame,
  Color3 = Color3,
  BrickColor = BrickColor,
  UDim = UDim,
  UDim2 = UDim2,
  Rect = Rect,
  Ray = Ray,
  Region3 = Region3,
  Region3int16 = Region3int16,
  NumberRange = NumberRange,
  TweenInfo = TweenInfo,
  PhysicalProperties = PhysicalProperties,
  Font = Font,
  Axes = Axes,
  Faces = Faces,
  ColorSequenceKeypoint = ColorSequenceKeypoint,
  ColorSequence = ColorSequence,
  NumberSequenceKeypoint = NumberSequenceKeypoint,
  NumberSequence = NumberSequence,
}

return function(namespace)
  local System = _G[namespace].System

  -- Helper to create a struct type that wraps a Roblox global
  local function wrapRobloxType(globalName, defaultFn)
    local global = RobloxGlobals[globalName]
    if not global then
      error("Roblox global '" .. globalName .. "' not found")
    end

    -- Create a type object compatible with CSharp.lua
    local typeObj = {
      __name__ = globalName,
      default = defaultFn,
      -- Forward constructor calls to the Roblox global
      __call = function(_, ...)
        return global.new(...)
      end,
      -- Clone method for value types
      __clone__ = function(v)
        return v  -- Roblox types are immutable, no clone needed
      end
    }

    -- Copy static members from the Roblox global
    setmetatable(typeObj, {
      __index = global,
      __call = typeObj.__call
    })

    return typeObj
  end

  -- Create Roblox namespace
  local Roblox = {}

  -- Vector types
  Roblox.Vector2 = wrapRobloxType("Vector2", function() return Vector2.zero end)
  Roblox.Vector3 = wrapRobloxType("Vector3", function() return Vector3.zero end)
  Roblox.Vector3int16 = wrapRobloxType("Vector3int16", function() return Vector3int16.new(0, 0, 0) end)

  -- Transform types
  Roblox.CFrame = wrapRobloxType("CFrame", function() return CFrame.identity end)

  -- Color types
  Roblox.Color3 = wrapRobloxType("Color3", function() return Color3.new(0, 0, 0) end)
  Roblox.BrickColor = wrapRobloxType("BrickColor", function() return BrickColor.new("Medium stone grey") end)

  -- UI dimension types
  Roblox.UDim = wrapRobloxType("UDim", function() return UDim.new(0, 0) end)
  Roblox.UDim2 = wrapRobloxType("UDim2", function() return UDim2.new(0, 0, 0, 0) end)
  Roblox.Rect = wrapRobloxType("Rect", function() return Rect.new(0, 0, 0, 0) end)

  -- Geometric types
  Roblox.Ray = wrapRobloxType("Ray", function() return Ray.new(Vector3.zero, Vector3.zAxis) end)
  Roblox.Region3 = wrapRobloxType("Region3", function() return Region3.new(Vector3.zero, Vector3.zero) end)
  Roblox.Region3int16 = wrapRobloxType("Region3int16", function() return Region3int16.new(Vector3int16.new(0,0,0), Vector3int16.new(0,0,0)) end)

  -- Other types
  Roblox.NumberRange = wrapRobloxType("NumberRange", function() return NumberRange.new(0) end)
  Roblox.TweenInfo = wrapRobloxType("TweenInfo", function() return TweenInfo.new() end)
  Roblox.PhysicalProperties = wrapRobloxType("PhysicalProperties", function() return PhysicalProperties.new(1, 0.3, 0.5) end)
  Roblox.Font = wrapRobloxType("Font", function() return Font.fromEnum(Enum.Font.SourceSans) end)
  Roblox.Axes = wrapRobloxType("Axes", function() return Axes.new() end)
  Roblox.Faces = wrapRobloxType("Faces", function() return Faces.new() end)

  -- Sequence types
  Roblox.ColorSequenceKeypoint = wrapRobloxType("ColorSequenceKeypoint", function() return ColorSequenceKeypoint.new(0, Color3.new(0,0,0)) end)
  Roblox.ColorSequence = wrapRobloxType("ColorSequence", function() return ColorSequence.new(Color3.new(0,0,0)) end)
  Roblox.NumberSequenceKeypoint = wrapRobloxType("NumberSequenceKeypoint", function() return NumberSequenceKeypoint.new(0, 0) end)
  Roblox.NumberSequence = wrapRobloxType("NumberSequence", function() return NumberSequence.new(0) end)

  -- Install into namespace
  _G[namespace].Roblox = Roblox

  return Roblox
end
