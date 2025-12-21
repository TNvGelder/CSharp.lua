using Roblox;

namespace TestGame;

/// <summary>
/// Base class for anything that can be spawned at a location.
/// </summary>
public abstract class Spawnable
{
    public Vector3 Position { get; set; }
    public string Name { get; set; }

    protected Spawnable(string name, Vector3 position)
    {
        Name = name;
        Position = position;
    }

    /// <summary>
    /// Spawns the entity into the workspace and returns the created Instance.
    /// </summary>
    public abstract Instance Spawn();
}
