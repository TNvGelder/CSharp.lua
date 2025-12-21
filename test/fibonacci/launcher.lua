package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"

-- Initialize CoreSystem with required namespace configuration
local init = require("init")
init("", { systemNamespace = "CSharpLua" })

require("out.manifest")("out")    

Test.Program.Main()    -- run main method

--local methodInfo = System.Reflection.Assembly.GetEntryAssembly().getEntryPoint()
--methodInfo:Invoke()


