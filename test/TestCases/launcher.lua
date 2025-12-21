package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";test/TestCases/?.lua;"

if arg[1] == "nodebug" then
  debug = nil
end

-- Initialize CoreSystem with required namespace configuration
local init = require("init")
init("", { systemNamespace = "CSharpLua" })

require("out.manifest")("out")    

_G.CSharpLua.ILRuntimeTest.TestBase.StaticTestUnit.Run()    -- run main method

--local methodInfo = System.Reflection.Assembly.GetEntryAssembly().getEntryPoint()
--methodInfo:Invoke()


