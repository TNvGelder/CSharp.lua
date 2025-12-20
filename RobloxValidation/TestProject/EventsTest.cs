using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests ScriptSignal.Connect with lambdas
/// </summary>
public class EventsTest {
    public static void TestPlayerAdded() {
        var players = Globals.game.GetService<Players>();
        players.PlayerAdded.Connect(player => {
            System.Console.WriteLine("Player joined: " + player.Name);
        });
    }

    public static void TestCharacterAdded() {
        var players = Globals.game.GetService<Players>();
        players.PlayerAdded.Connect(player => {
            player.CharacterAdded.Connect(character => {
                var humanoid = character.FindFirstChildOfClass<Humanoid>();
                if (humanoid != null) {
                    humanoid.TakeDamage(10);
                }
            });
        });
    }

    public static void TestTouched() {
        var part = Globals.workspace.FindFirstChild("Part");
        if (part != null && part.IsA<BasePart>()) {
            ((BasePart)part).Touched.Connect(otherPart => {
                System.Console.WriteLine("Touched: " + otherPart.Name);
            });
        }
    }
}
