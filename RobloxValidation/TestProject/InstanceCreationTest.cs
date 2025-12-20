using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests Instance creation, Clone, and Destroy
/// </summary>
public class InstanceCreationTest {
    public static void TestInstanceNew() {
        // Create a new Part instance
        var part = InstanceFactory.Create<Part>();
        part.Name = "MyPart";
        part.Position = Vector3.@new(0, 10, 0);
        part.Parent = Globals.workspace;
    }

    public static void TestInstanceNewWithParent() {
        // Create a new Model with parent
        var model = InstanceFactory.Create<Model>(Globals.workspace);
        model.Name = "MyModel";
    }

    public static void TestClone() {
        var original = Globals.workspace.FindFirstChild("Template");
        if (original != null) {
            var clone = original.Clone();
            clone.Name = "ClonedTemplate";
            clone.Parent = Globals.workspace;
        }
    }

    public static void TestDestroy() {
        var part = Globals.workspace.FindFirstChild("PartToDestroy");
        if (part != null) {
            part.Destroy();
        }
    }
}
