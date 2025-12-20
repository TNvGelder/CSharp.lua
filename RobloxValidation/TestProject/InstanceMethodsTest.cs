using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests Instance methods: FindFirstChild, FindFirstChildOfClass, IsA, etc.
/// </summary>
public class InstanceMethodsTest {
    public static void TestFindFirstChild() {
        var part = Globals.workspace.FindFirstChild("Part");
        var partRecursive = Globals.workspace.FindFirstChild("Part", true);
    }

    public static void TestFindFirstChildOfClass() {
        var part = Globals.workspace.FindFirstChild("Model");
        if (part != null) {
            var humanoid = part.FindFirstChildOfClass<Humanoid>();
        }
    }

    public static void TestIsA() {
        var part = Globals.workspace.FindFirstChild("TestPart");
        if (part != null && part.IsA<BasePart>()) {
            System.Console.WriteLine("It's a BasePart!");
        }
    }

    public static void TestWaitForChild() {
        var part = Globals.workspace.WaitForChild("Part");
        var partWithTimeout = Globals.workspace.WaitForChild("Part", 5.0);
    }

    public static void TestGetChildren() {
        var children = Globals.workspace.GetChildren();
        var descendants = Globals.workspace.GetDescendants();
    }
}
