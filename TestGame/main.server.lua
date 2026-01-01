-- Load config and set it before requiring CoreSystem
local config = require(game.ReplicatedStorage.config)
rawset(_G, "__CoreSystemConfig", config)

-- Initialize the runtime (System is placed directly in _G.<namespace>.System)
local System = require(game.ReplicatedStorage.CoreSystem)

-- Load Roblox types shim (binds Roblox globals to work with CSharp.lua type system)
local robloxTypes = require(game.ReplicatedStorage.RobloxTypes)
robloxTypes(config.systemNamespace)

-- Load auto-generated instance type stubs (enables Instance types in generic parameters)
local instanceTypes = require(game.ReplicatedStorage.RobloxInstanceTypes)
instanceTypes(config.systemNamespace)

-- Load compiled game code (manifest returns init function, must call it)
-- Pass empty string for path - in Roblox mode, System.init uses script.Parent as root
local GameManifest = require(game.ReplicatedStorage.Game.manifest)
GameManifest("")  -- Initialize all types (registers them in namespace)

-- Run the scene setup (types are at _G.<namespace>.<namespace>.<ClassName>)
local TestGame = _G.TestGame.TestGame
TestGame.GameScene.Setup()

-- Initialize shop UI for players
local Players = game:GetService("Players")

local function setupPlayer(player)
	-- Wait for character to load before setting up shop
	if not player.Character then
		player.CharacterAdded:Wait()
	end

	-- Initialize the shop UI for this player
	TestGame.ShopUI.Initialize(player)
end

-- Handle players that join
Players.PlayerAdded:Connect(setupPlayer)

-- Handle already-connected players (for Studio testing)
for _, player in ipairs(Players:GetPlayers()) do
	task.spawn(setupPlayer, player)
end
