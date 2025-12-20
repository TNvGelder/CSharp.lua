namespace Roblox;

/// <summary>
/// Static factory class for creating new Roblox instances.
/// Usage: InstanceFactory.Create&lt;Part&gt;() transpiles to Instance.new("Part")
/// </summary>
public static class InstanceFactory {
    /// <summary>Creates a new instance of the specified Roblox class.</summary>
    /// <typeparam name="T">The type of Instance to create.</typeparam>
    /// <returns>A new instance of type T.</returns>
    public static extern T Create<T>() where T : class, Instance;

    /// <summary>Creates a new instance of the specified Roblox class with a parent.</summary>
    /// <typeparam name="T">The type of Instance to create.</typeparam>
    /// <param name="parent">The parent to assign to the new instance.</param>
    /// <returns>A new instance of type T.</returns>
    public static extern T Create<T>(Instance parent) where T : class, Instance;
}
