-- Load config and set it before requiring CoreSystem
local config = require(game.ReplicatedStorage.config)
rawset(_G, "__CoreSystemConfig", config)

-- Initialize the runtime (System is placed directly in _G.<namespace>.System)
local System = require(game.ReplicatedStorage.CoreSystem)

-- Load Roblox types shim (binds Roblox globals to work with CSharp.lua type system)
local robloxTypes = require(game.ReplicatedStorage.RobloxTypes)
robloxTypes(config.systemNamespace)

-- Load compiled game code (manifest returns init function, must call it)
-- Pass empty string for path - in Roblox mode, System.init uses script.Parent as root
local GameManifest = require(game.ReplicatedStorage.Game.manifest)
GameManifest("")  -- Initialize all types (registers them in namespace)

-- Run the scene setup (types are at _G.<namespace>.<namespace>.<ClassName>)
local TestGame = _G.TestGame.TestGame
TestGame.GameScene.Setup()
