using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests that game.GetService&lt;T&gt;() transpiles to game:GetService("T")
/// </summary>
public class GetServiceTest {
    public static void TestGetService() {
        // Use explicit Globals.game to trigger MemberAccessExpression path
        // which properly applies XML metadata templates
        var players = Globals.game.GetService<Players>();
        var workspace = Globals.game.GetService<Workspace>();
        var runService = Globals.game.GetService<RunService>();
        var lighting = Globals.game.GetService<Lighting>();
    }
}
