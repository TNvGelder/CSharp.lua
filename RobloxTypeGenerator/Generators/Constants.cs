namespace RobloxTypeGenerator.Generators;

/// <summary>
/// Shared constants for the Roblox type generators.
/// </summary>
public static class GeneratorConstants {
    /// <summary>
    /// Classes that should be skipped during generation because they are
    /// either hand-written, internal, or problematic.
    /// These classes are defined in RobloxTypes/*.cs files and have
    /// corresponding metadata in RobloxMetadata/Roblox.xml.
    /// </summary>
    public static readonly HashSet<string> HandWrittenClasses = new() {
        "<<<ROOT>>>",

        // Hand-written core types (RobloxTypes/Instance.cs, DataModel.cs)
        "Instance", "DataModel",

        // Hand-written services (RobloxTypes/Services.cs)
        "Players", "Player", "Workspace", "ReplicatedStorage", "ServerStorage",
        "ServerScriptService", "StarterGui", "StarterPlayer", "Lighting",
        "SoundService", "TweenService", "RunService", "UserInputService",
        "HttpService", "Camera", "Tween", "InputObject",

        // Hand-written parts/physics (RobloxTypes/BasePart.cs)
        "PVInstance", "BasePart", "Part", "WedgePart", "CornerWedgePart",
        "TrussPart", "MeshPart", "UnionOperation", "Model", "Humanoid",
        "JointInstance", "Weld", "WeldConstraint", "Motor6D",

        // Hand-written script types (RobloxTypes/LuaSourceContainer.cs)
        "LuaSourceContainer", "BaseScript", "Script", "LocalScript", "ModuleScript"
    };
}
