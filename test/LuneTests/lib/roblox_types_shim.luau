--[[
  Roblox Types Shim for Lune
  Provides CSharp.lua compatibility for Roblox types.

  This shim provides:
  - default() method for System.default(Roblox.Vector3) calls
  - __call metatable for Roblox.Vector3(x, y, z) syntax

  Note: __clone__ is NOT needed because the transpiler now skips
  __clone__ calls for types in the Roblox namespace (immutable types).
--]]

local roblox = require("@lune/roblox") :: any

return function(namespace: string)
  local namespaceRoot = _G[namespace]
  if not namespaceRoot then
    error("Namespace not found: " .. namespace .. ". Make sure CoreSystem is loaded first.")
  end

  local System = namespaceRoot.System
  if not System then
    error("System not found in namespace: " .. namespace)
  end

  -- Helper to create a type wrapper for CSharp.lua compatibility
  local function wrapRobloxType(luneType: any, defaultFn: () -> any)
    if not luneType then
      return nil
    end

    local typeObj = {
      __name__ = tostring(luneType),
      default = defaultFn,
    }

    -- Metatable for static member access and constructor calls
    setmetatable(typeObj, {
      __index = luneType,
      __call = function(_, ...)
        return luneType.new(...)
      end
    })

    return typeObj
  end

  -- Create Roblox namespace
  local Roblox = {}

  -- Vector types
  Roblox.Vector2 = wrapRobloxType(roblox.Vector2, function()
    return roblox.Vector2.zero
  end)

  Roblox.Vector3 = wrapRobloxType(roblox.Vector3, function()
    return roblox.Vector3.zero
  end)

  Roblox.Vector3int16 = wrapRobloxType(roblox.Vector3int16, function()
    return roblox.Vector3int16.new(0, 0, 0)
  end)

  -- Transform types
  Roblox.CFrame = wrapRobloxType(roblox.CFrame, function()
    return roblox.CFrame.identity
  end)

  -- Color types
  Roblox.Color3 = wrapRobloxType(roblox.Color3, function()
    return roblox.Color3.new(0, 0, 0)
  end)

  Roblox.BrickColor = wrapRobloxType(roblox.BrickColor, function()
    return roblox.BrickColor.new("Medium stone grey")
  end)

  -- UI dimension types
  Roblox.UDim = wrapRobloxType(roblox.UDim, function()
    return roblox.UDim.new(0, 0)
  end)

  Roblox.UDim2 = wrapRobloxType(roblox.UDim2, function()
    return roblox.UDim2.new(0, 0, 0, 0)
  end)

  Roblox.Rect = wrapRobloxType(roblox.Rect, function()
    return roblox.Rect.new(0, 0, 0, 0)
  end)

  -- Geometric types
  Roblox.Ray = wrapRobloxType(roblox.Ray, function()
    return roblox.Ray.new(roblox.Vector3.zero, roblox.Vector3.zAxis)
  end)

  Roblox.Region3 = wrapRobloxType(roblox.Region3, function()
    return roblox.Region3.new(roblox.Vector3.zero, roblox.Vector3.zero)
  end)

  Roblox.Region3int16 = wrapRobloxType(roblox.Region3int16, function()
    return roblox.Region3int16.new(
      roblox.Vector3int16.new(0, 0, 0),
      roblox.Vector3int16.new(0, 0, 0)
    )
  end)

  -- Other types
  Roblox.NumberRange = wrapRobloxType(roblox.NumberRange, function()
    return roblox.NumberRange.new(0)
  end)

  Roblox.PhysicalProperties = wrapRobloxType(roblox.PhysicalProperties, function()
    return roblox.PhysicalProperties.new(1, 0.3, 0.5)
  end)

  Roblox.Font = wrapRobloxType(roblox.Font, function()
    return roblox.Font.fromEnum(roblox.Enum.Font.SourceSans)
  end)

  Roblox.Axes = wrapRobloxType(roblox.Axes, function()
    return roblox.Axes.new()
  end)

  Roblox.Faces = wrapRobloxType(roblox.Faces, function()
    return roblox.Faces.new()
  end)

  -- Sequence types
  Roblox.ColorSequenceKeypoint = wrapRobloxType(roblox.ColorSequenceKeypoint, function()
    return roblox.ColorSequenceKeypoint.new(0, roblox.Color3.new(0, 0, 0))
  end)

  Roblox.ColorSequence = wrapRobloxType(roblox.ColorSequence, function()
    return roblox.ColorSequence.new(roblox.Color3.new(0, 0, 0))
  end)

  Roblox.NumberSequenceKeypoint = wrapRobloxType(roblox.NumberSequenceKeypoint, function()
    return roblox.NumberSequenceKeypoint.new(0, 0)
  end)

  Roblox.NumberSequence = wrapRobloxType(roblox.NumberSequence, function()
    return roblox.NumberSequence.new(0)
  end)

  -- Install into namespace
  namespaceRoot.Roblox = Roblox

  return Roblox
end
