using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests generated types including enums
/// Note: Roblox uses Enum.Material in Lua, but C# uses Material directly
/// The transpiler handles mapping C# enum access to Lua's Enum.{EnumName} pattern
/// </summary>
public class GeneratedTypesTest {
    public static void TestEnumAccess() {
        // Access enum items directly in C# (transpiler maps to Enum.Material.Plastic in Lua)
        var material = Material.Plastic;
        var material2 = Material.Brick;

        System.Console.WriteLine("Material: " + material.Name);
        System.Console.WriteLine("Material value: " + material.Value);
    }

    public static void TestEnumGetItems() {
        // Test GetEnumItems method
        var items = Material.GetEnumItems();
        System.Console.WriteLine("Enum items count: " + items.Length);
    }
}
