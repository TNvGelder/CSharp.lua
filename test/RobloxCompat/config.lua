-- Shared configuration for CSharp.lua Roblox tests
-- This file is read by both:
--   1. test.bat (for -namespace CLI flag)
--   2. TestRunner.server.lua (for System.relocateTo)

return {
    -- The namespace path where System will be located
    -- Generated Lua code will use: local System = _G.<namespace>.System
    systemNamespace = "MyGame",

    -- Whether to clear _G.System after relocation (recommended for Roblox)
    clearGlobalSystem = true,
}
