--[[
  CFrame Datatype Tests
  Tests Lune's CFrame implementation.
--]]

local roblox = require("@lune/roblox") :: any
local CFrame = roblox.CFrame
local Vector3 = roblox.Vector3

local tf = require("../../lib/test_framework")
local test = tf.test
local assertEqual = tf.assertEqual
local assertTrue = tf.assertTrue
local assertApprox = tf.assertApprox

print("\n--- CFrame Datatype Tests ---")

-- Constructor tests
test("CFrame.new creates identity at origin", function()
  local cf = CFrame.new()
  assertEqual(cf.Position, Vector3.zero, "position at origin")
end)

test("CFrame.new with position", function()
  local cf = CFrame.new(1, 2, 3)
  assertEqual(cf.X, 1, "X")
  assertEqual(cf.Y, 2, "Y")
  assertEqual(cf.Z, 3, "Z")
end)

test("CFrame.new from Vector3", function()
  local pos = Vector3.new(5, 10, 15)
  local cf = CFrame.new(pos)
  assertEqual(cf.Position, pos, "position from vector")
end)

-- Constants
test("CFrame.identity exists", function()
  assertTrue(CFrame.identity ~= nil, "identity exists")
  assertEqual(CFrame.identity.Position, Vector3.zero, "identity at origin")
end)

-- Position property
test("CFrame.Position property", function()
  local cf = CFrame.new(1, 2, 3)
  local pos = cf.Position
  assertEqual(pos.X, 1, "X")
  assertEqual(pos.Y, 2, "Y")
  assertEqual(pos.Z, 3, "Z")
end)

-- LookVector, RightVector, UpVector
test("CFrame LookVector for identity", function()
  local cf = CFrame.identity
  local look = cf.LookVector
  -- Identity CFrame looks down -Z axis
  assertApprox(look.X, 0, 0.001, "look X")
  assertApprox(look.Y, 0, 0.001, "look Y")
  assertApprox(look.Z, -1, 0.001, "look Z")
end)

test("CFrame RightVector for identity", function()
  local cf = CFrame.identity
  local right = cf.RightVector
  assertApprox(right.X, 1, 0.001, "right X")
  assertApprox(right.Y, 0, 0.001, "right Y")
  assertApprox(right.Z, 0, 0.001, "right Z")
end)

test("CFrame UpVector for identity", function()
  local cf = CFrame.identity
  local up = cf.UpVector
  assertApprox(up.X, 0, 0.001, "up X")
  assertApprox(up.Y, 1, 0.001, "up Y")
  assertApprox(up.Z, 0, 0.001, "up Z")
end)

-- Multiplication
test("CFrame multiplication (composition)", function()
  local cf1 = CFrame.new(1, 0, 0)
  local cf2 = CFrame.new(0, 2, 0)
  local result = cf1 * cf2
  assertEqual(result.X, 1, "composed X")
  assertEqual(result.Y, 2, "composed Y")
  assertEqual(result.Z, 0, "composed Z")
end)

test("CFrame * Vector3 transforms point", function()
  local cf = CFrame.new(10, 0, 0)
  local point = Vector3.new(1, 0, 0)
  local transformed = cf * point
  assertEqual(transformed.X, 11, "transformed X")
  assertEqual(transformed.Y, 0, "transformed Y")
  assertEqual(transformed.Z, 0, "transformed Z")
end)

-- Inverse
test("CFrame:Inverse", function()
  local cf = CFrame.new(5, 0, 0)
  local inv = cf:Inverse()
  assertEqual(inv.X, -5, "inverse X")
end)

-- Lerp
test("CFrame:Lerp", function()
  local a = CFrame.new(0, 0, 0)
  local b = CFrame.new(10, 0, 0)
  local mid = a:Lerp(b, 0.5)
  assertApprox(mid.X, 5, 0.01, "lerp X")
end)

-- ToWorldSpace / ToObjectSpace
test("CFrame:ToWorldSpace", function()
  local cf = CFrame.new(10, 0, 0)
  local local_cf = CFrame.new(5, 0, 0)
  local world = cf:ToWorldSpace(local_cf)
  assertEqual(world.X, 15, "world X")
end)

test("CFrame:ToObjectSpace", function()
  local cf = CFrame.new(10, 0, 0)
  local world_cf = CFrame.new(15, 0, 0)
  local obj = cf:ToObjectSpace(world_cf)
  assertEqual(obj.X, 5, "object X")
end)

-- Rotation constructors
test("CFrame.Angles creates rotation", function()
  local cf = CFrame.Angles(0, math.pi / 2, 0)
  assertTrue(cf ~= nil, "angles creates CFrame")
  -- After 90 degree Y rotation (counterclockwise from above), LookVector points along -X
  -- Default LookVector is (0, 0, -1), rotating 90 degrees CCW around Y gives (-1, 0, 0)
  local look = cf.LookVector
  assertApprox(look.X, -1, 0.01, "rotated look X")
  assertApprox(look.Z, 0, 0.01, "rotated look Z")
end)

test("CFrame.fromEulerAnglesXYZ", function()
  local cf = CFrame.fromEulerAnglesXYZ(0, math.pi, 0)
  assertTrue(cf ~= nil, "fromEulerAnglesXYZ creates CFrame")
end)

return tf.printSummary()
