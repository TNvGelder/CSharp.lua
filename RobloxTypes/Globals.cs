namespace Roblox;

/// <summary>
/// Provides access to global Roblox variables.
/// </summary>
public static class Globals {
    /// <summary>The root DataModel instance for the game.</summary>
    public static extern DataModel game { get; }

    /// <summary>The Workspace service, where all physical objects reside.</summary>
    public static extern Workspace workspace { get; }

    /// <summary>The script that is currently running this code.</summary>
    public static extern LuaSourceContainer script { get; }
}
