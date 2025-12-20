namespace Roblox;

/// <summary>
/// An abstract class for objects that contain Lua code.
/// </summary>
public interface LuaSourceContainer : Instance {
}

/// <summary>
/// An abstract class for scripts that can be enabled/disabled.
/// </summary>
public interface BaseScript : LuaSourceContainer {
    /// <summary>Whether the script is currently enabled/running.</summary>
    bool Enabled { get; set; }
}

/// <summary>
/// A script that runs on the server.
/// </summary>
public interface Script : BaseScript {
}

/// <summary>
/// A script that runs on the client.
/// </summary>
public interface LocalScript : BaseScript {
}

/// <summary>
/// A script that can be required by other scripts.
/// </summary>
public interface ModuleScript : LuaSourceContainer {
}
