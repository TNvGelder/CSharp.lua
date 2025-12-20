using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests null-conditional operator (?.) transpilation
/// </summary>
public class NullConditionalTest {
    public static void TestNullConditionalMethod() {
        // Use ?. to safely call methods on potentially null instances
        var part = Globals.workspace.FindFirstChild("MaybeExists");
        var humanoid = part?.FindFirstChildOfClass<Humanoid>();
        humanoid?.TakeDamage(10);
    }

    public static void TestNullConditionalProperty() {
        // Use ?. to safely access properties
        var player = Globals.game.GetService<Players>().LocalPlayer;
        var character = player?.Character;
        var name = character?.Name;
        System.Console.WriteLine("Character: " + (name ?? "none"));
    }

    public static void TestNullConditionalChain() {
        // Chain multiple null-conditional operators
        var humanoid = Globals.workspace
            .FindFirstChild("Model")?
            .FindFirstChildOfClass<Humanoid>();

        var health = humanoid?.Health;
        if (health != null) {
            System.Console.WriteLine("Health: " + health);
        }
    }
}
