using Roblox;
using static Roblox.Globals;

namespace RobloxTest;

/// <summary>
/// Test script demonstrating Roblox API usage from C#.
/// This should transpile to valid Luau code.
/// </summary>
public class TestScript {
    public static void Main() {
        // Test GetService with generic type argument -> should become game:GetService("Players")
        var players = game.GetService<Players>();

        // Test event connection with lambda
        players.PlayerAdded.Connect(player => {
            System.Console.WriteLine($"Player joined: {player.Name}");
            System.Console.WriteLine($"UserId: {player.UserId}");
            System.Console.WriteLine($"DisplayName: {player.DisplayName}");

            // Test nested event connection
            player.CharacterAdded.Connect(character => {
                // Test FindFirstChildOfClass with generic -> should become :FindFirstChildOfClass("Humanoid")
                var humanoid = character.FindFirstChildOfClass<Humanoid>();
                if (humanoid != null) {
                    humanoid.TakeDamage(10);
                    System.Console.WriteLine($"Humanoid health: {humanoid.Health}");
                }

                // Test FindFirstChild with string
                var head = character.FindFirstChild("Head");
                if (head != null) {
                    System.Console.WriteLine("Found head!");
                }
            });
        });

        // Test workspace global
        var part = workspace.FindFirstChild("TestPart");
        if (part != null) {
            // Test IsA with generic -> should become :IsA("BasePart")
            if (part.IsA<BasePart>()) {
                System.Console.WriteLine("TestPart is a BasePart!");
            }
        }

        // Test Vector3 creation
        var position = Vector3.@new(10, 5, 20);
        System.Console.WriteLine($"Position: {position.X}, {position.Y}, {position.Z}");

        // Test Color3 creation
        var color = Color3.fromRGB(255, 128, 64);
        System.Console.WriteLine($"Color R: {color.R}");

        // Test CFrame creation
        var cframe = CFrame.@new(0, 10, 0);
        System.Console.WriteLine($"CFrame position: {cframe.Position}");
    }
}

/// <summary>
/// Another test class for server-side logic.
/// </summary>
public class ServerScript {
    public static void Initialize() {
        var runService = game.GetService<RunService>();

        // Test RunService events
        runService.Heartbeat.Connect(deltaTime => {
            // Game loop logic here
        });

        // Test if running on server
        if (runService.IsServer()) {
            System.Console.WriteLine("Running on server!");
        }
    }
}

/// <summary>
/// Test class for physics interactions.
/// </summary>
public class PhysicsTest {
    public static void TestRaycast() {
        var origin = Vector3.@new(0, 10, 0);
        var direction = Vector3.@new(0, -100, 0);

        // Create raycast params
        var params_ = RaycastParams.@new();

        // Perform raycast
        var result = workspace.Raycast(origin, direction, params_);
        if (result != null) {
            System.Console.WriteLine($"Hit: {result.Instance.Name}");
            System.Console.WriteLine($"Position: {result.Position}");
            System.Console.WriteLine($"Normal: {result.Normal}");
            System.Console.WriteLine($"Distance: {result.Distance}");
        }
    }
}
