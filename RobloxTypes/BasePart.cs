namespace Roblox;

/// <summary>
/// Abstract base class for objects that have a physical presence.
/// </summary>
public interface PVInstance : Instance {
    /// <summary>Returns the CFrame that represents the center of mass.</summary>
    CFrame GetPivot();

    /// <summary>Moves the PVInstance to a new position by CFrame.</summary>
    void PivotTo(CFrame targetCFrame);
}

/// <summary>
/// Abstract base class for physical parts.
/// </summary>
public interface BasePart : PVInstance {
    /// <summary>Whether the part is anchored in place.</summary>
    bool Anchored { get; set; }

    /// <summary>Whether the part can be collided with.</summary>
    bool CanCollide { get; set; }

    /// <summary>Whether the part can be queried by spatial queries.</summary>
    bool CanQuery { get; set; }

    /// <summary>Whether the part can trigger touch events.</summary>
    bool CanTouch { get; set; }

    /// <summary>The position of the part in world coordinates.</summary>
    Vector3 Position { get; set; }

    /// <summary>The orientation of the part in degrees.</summary>
    Vector3 Orientation { get; set; }

    /// <summary>The size of the part.</summary>
    Vector3 Size { get; set; }

    /// <summary>The CFrame (position and rotation) of the part.</summary>
    CFrame CFrame { get; set; }

    /// <summary>The color of the part.</summary>
    Color3 Color { get; set; }

    /// <summary>The transparency of the part (0 = opaque, 1 = invisible).</summary>
    double Transparency { get; set; }

    /// <summary>The reflectance of the part.</summary>
    double Reflectance { get; set; }

    /// <summary>The material of the part.</summary>
    Enum Material { get; set; }

    /// <summary>The brick color of the part.</summary>
    BrickColor BrickColor { get; set; }

    /// <summary>Whether the part is massless.</summary>
    bool Massless { get; set; }

    /// <summary>The mass of the part.</summary>
    double Mass { get; }

    /// <summary>The linear velocity of the part.</summary>
    Vector3 AssemblyLinearVelocity { get; set; }

    /// <summary>The angular velocity of the part.</summary>
    Vector3 AssemblyAngularVelocity { get; set; }

    /// <summary>Fires when another part starts touching this part.</summary>
    ScriptSignal<BasePart> Touched { get; }

    /// <summary>Fires when another part stops touching this part.</summary>
    ScriptSignal<BasePart> TouchEnded { get; }

    /// <summary>Returns all parts currently touching this part.</summary>
    BasePart[] GetTouchingParts();

    /// <summary>Returns all parts within the part's bounding box.</summary>
    BasePart[] GetConnectedParts(bool recursive = false);

    /// <summary>Applies an impulse to the part.</summary>
    void ApplyImpulse(Vector3 impulse);

    /// <summary>Applies an impulse at a specific position.</summary>
    void ApplyImpulseAtPosition(Vector3 impulse, Vector3 position);

    /// <summary>Applies an angular impulse to the part.</summary>
    void ApplyAngularImpulse(Vector3 impulse);

    /// <summary>Sets the network owner of the part.</summary>
    void SetNetworkOwner(Player? player = null);

    /// <summary>Gets the network owner of the part.</summary>
    Player? GetNetworkOwner();
}

/// <summary>
/// A basic rectangular part.
/// </summary>
public interface Part : BasePart {
}

/// <summary>
/// A part shaped like a wedge.
/// </summary>
public interface WedgePart : BasePart {
}

/// <summary>
/// A part shaped like a corner wedge.
/// </summary>
public interface CornerWedgePart : BasePart {
}

/// <summary>
/// A part with a triangular mesh.
/// </summary>
public interface TrussPart : BasePart {
}

/// <summary>
/// A part that can display meshes.
/// </summary>
public interface MeshPart : BasePart {
    /// <summary>The mesh ID to display.</summary>
    string MeshId { get; set; }

    /// <summary>The texture ID to apply.</summary>
    string TextureID { get; set; }
}

/// <summary>
/// A part that forms a union of other parts.
/// </summary>
public interface UnionOperation : BasePart {
}

/// <summary>
/// A container for a set of parts that form a model.
/// </summary>
public interface Model : PVInstance {
    /// <summary>The primary part of the model.</summary>
    BasePart? PrimaryPart { get; set; }

    /// <summary>Returns the bounding box of the model.</summary>
    (CFrame cframe, Vector3 size) GetBoundingBox();

    /// <summary>Moves the model to a new position.</summary>
    void MoveTo(Vector3 position);

    /// <summary>Scales the model.</summary>
    void ScaleTo(double scale);

    /// <summary>Returns the scale of the model.</summary>
    double GetScale();
}

/// <summary>
/// Controls humanoid behavior for characters.
/// </summary>
public interface Humanoid : Instance {
    /// <summary>The current health of the humanoid.</summary>
    double Health { get; set; }

    /// <summary>The maximum health of the humanoid.</summary>
    double MaxHealth { get; set; }

    /// <summary>The walk speed of the humanoid.</summary>
    double WalkSpeed { get; set; }

    /// <summary>The jump power of the humanoid.</summary>
    double JumpPower { get; set; }

    /// <summary>The jump height of the humanoid.</summary>
    double JumpHeight { get; set; }

    /// <summary>Whether the humanoid is jumping.</summary>
    bool Jump { get; set; }

    /// <summary>The current state of the humanoid.</summary>
    Enum HumanoidStateType { get; }

    /// <summary>The root part of the humanoid's character.</summary>
    BasePart? RootPart { get; }

    /// <summary>Fires when the humanoid dies.</summary>
    ScriptSignal Died { get; }

    /// <summary>Fires when the humanoid's state changes.</summary>
    ScriptSignal<Enum, Enum> StateChanged { get; }

    /// <summary>Fires when the humanoid is running.</summary>
    ScriptSignal<double> Running { get; }

    /// <summary>Fires when the humanoid lands.</summary>
    ScriptSignal<bool> Jumping { get; }

    /// <summary>Damages the humanoid.</summary>
    void TakeDamage(double amount);

    /// <summary>Makes the humanoid move towards a point.</summary>
    void MoveTo(Vector3 location, BasePart? part = null);

    /// <summary>Changes the humanoid's state.</summary>
    void ChangeState(Enum state);

    /// <summary>Returns the humanoid's current state.</summary>
    Enum GetState();

    /// <summary>Equips a tool.</summary>
    void EquipTool(Instance tool);

    /// <summary>Unequips all tools.</summary>
    void UnequipTools();
}

/// <summary>
/// Represents a joint connecting two parts.
/// </summary>
public interface JointInstance : Instance {
    /// <summary>The first part of the joint.</summary>
    BasePart? Part0 { get; set; }

    /// <summary>The second part of the joint.</summary>
    BasePart? Part1 { get; set; }
}

/// <summary>
/// A weld constraint between two parts.
/// </summary>
public interface Weld : JointInstance {
}

/// <summary>
/// A weld constraint that maintains offset.
/// </summary>
public interface WeldConstraint : Instance {
    /// <summary>Whether the weld is enabled.</summary>
    bool Enabled { get; set; }

    /// <summary>The first part.</summary>
    BasePart? Part0 { get; set; }

    /// <summary>The second part.</summary>
    BasePart? Part1 { get; set; }
}

/// <summary>
/// A motor joint for animation.
/// </summary>
public interface Motor6D : JointInstance {
    /// <summary>The current angle of the motor.</summary>
    double CurrentAngle { get; }

    /// <summary>The transform to apply.</summary>
    CFrame Transform { get; set; }
}
