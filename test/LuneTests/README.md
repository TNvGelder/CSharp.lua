# CSharp.lua Lune Tests

Lune-based tests for the CSharp.lua Roblox typing system. These tests verify that the RobloxTypes typing system transpiles correctly and works in Luau outside of Roblox Studio.

## Prerequisites

1. Install [Rokit](https://github.com/roblox/rokit)
2. Run `rokit install` in this directory

## Running Tests

```bash
# From test/LuneTests directory
lune run run_tests

# Or from project root
lune run test/LuneTests/.lune/run_tests
```

## Test Structure

- `lib/` - Shared test utilities
  - `test_framework.luau` - Simple assertion-based test framework
  - `roblox_globals.luau` - Sets up Roblox globals from @lune/roblox
  - `coresystem_loader.luau` - Loads CoreSystem.Lua for Lune
  - `roblox_types_shim.luau` - Adapts roblox-types.lua for Lune

- `unit/` - Unit tests for individual components
  - `datatypes/` - Roblox datatype tests (Vector3, Color3, CFrame, UDim2)
  - `instance/` - Instance system tests (creation, hierarchy, properties)
  - `coresystem/` - CoreSystem runtime tests (delegates, strings, numbers)
  - `transforms/` - Compiler transform tests (static methods, number formatting)

- `integration/` - Integration tests with transpiled code
  - `testgame/` - Tests using TestGame transpiled output

## Adding New Tests

1. Create a `.luau` file in the appropriate directory
2. Import the test framework:
   ```lua
   local tf = require("../../lib/test_framework")
   local test = tf.test
   ```
3. Use `tf.test(name, fn)` for each test case
4. Return `tf.printSummary()` at the end

## What Gets Tested

1. **Roblox Datatypes**: Vector3, Color3, CFrame, UDim2 work correctly
2. **Instance System**: Instance.new, hierarchy, FindFirstChild, properties
3. **CoreSystem Integration**: Delegates, string methods, number formatting
4. **Compiler Transforms**: Static method transforms, enum toString
5. **End-to-End**: TestGame's transpiled code runs correctly
