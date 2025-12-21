using Roblox;

namespace TestGame;

/// <summary>
/// A spawnable box (Part) with configurable size and color.
/// </summary>
public class SpawnableBox : Spawnable
{
    public Vector3 Size { get; set; }
    public Color3 Color { get; set; }

    public SpawnableBox(string name, Vector3 position, Vector3 size, Color3 color)
        : base(name, position)
    {
        Size = size;
        Color = color;
    }

    public override Instance Spawn()
    {
        var part = InstanceFactory.Create<Part>();
        part.Name = Name;
        part.Size = Size;
        part.Position = Position;
        part.Color = Color;
        part.Anchored = true;  // So it doesn't fall
        part.Parent = Globals.workspace;

        System.Console.WriteLine($"Spawned {Name} at {Position}");

        return part;
    }
}
