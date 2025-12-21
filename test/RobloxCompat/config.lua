-- Shared configuration for CSharp.lua Roblox tests
-- This file is read by both:
--   1. test.bat (for -namespace CLI flag)
--   2. TestRunner.server.lua (for CoreSystem initialization)

return {
    -- The namespace path where System will be initialized
    -- Generated Lua code will use: local System = _G.<namespace>.System
    systemNamespace = "MyGame",
}
