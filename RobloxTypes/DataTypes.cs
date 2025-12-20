namespace Roblox;

/// <summary>
/// Represents a 2D vector.
/// </summary>
public struct Vector2 {
    /// <summary>The X component.</summary>
    public double X { get; }

    /// <summary>The Y component.</summary>
    public double Y { get; }

    /// <summary>The length of the vector.</summary>
    public double Magnitude { get; }

    /// <summary>Returns the unit vector.</summary>
    public Vector2 GetUnit() => default;

    /// <summary>Creates a new Vector2.</summary>
    public static extern Vector2 @new(double x, double y);

    /// <summary>A Vector2 with both components set to 0.</summary>
    public static extern Vector2 zero { get; }

    /// <summary>A Vector2 with both components set to 1.</summary>
    public static extern Vector2 one { get; }

    /// <summary>A Vector2 pointing in the X direction.</summary>
    public static extern Vector2 xAxis { get; }

    /// <summary>A Vector2 pointing in the Y direction.</summary>
    public static extern Vector2 yAxis { get; }

    /// <summary>Returns the dot product of two vectors.</summary>
    public double Dot(Vector2 other) => default;

    /// <summary>Returns the cross product of two vectors.</summary>
    public double Cross(Vector2 other) => default;

    /// <summary>Linearly interpolates between two vectors.</summary>
    public Vector2 Lerp(Vector2 goal, double alpha) => default;
}

/// <summary>
/// Represents a 3D vector.
/// </summary>
public struct Vector3 {
    /// <summary>The X component.</summary>
    public double X { get; }

    /// <summary>The Y component.</summary>
    public double Y { get; }

    /// <summary>The Z component.</summary>
    public double Z { get; }

    /// <summary>The length of the vector.</summary>
    public double Magnitude { get; }

    /// <summary>Returns the unit vector.</summary>
    public Vector3 GetUnit() => default;

    /// <summary>Creates a new Vector3.</summary>
    public static extern Vector3 @new(double x, double y, double z);

    /// <summary>Creates a new Vector3 with all components equal.</summary>
    public static extern Vector3 @new(double value);

    /// <summary>A Vector3 with all components set to 0.</summary>
    public static extern Vector3 zero { get; }

    /// <summary>A Vector3 with all components set to 1.</summary>
    public static extern Vector3 one { get; }

    /// <summary>A Vector3 pointing in the X direction.</summary>
    public static extern Vector3 xAxis { get; }

    /// <summary>A Vector3 pointing in the Y direction.</summary>
    public static extern Vector3 yAxis { get; }

    /// <summary>A Vector3 pointing in the Z direction.</summary>
    public static extern Vector3 zAxis { get; }

    /// <summary>Returns the dot product of two vectors.</summary>
    public double Dot(Vector3 other) => default;

    /// <summary>Returns the cross product of two vectors.</summary>
    public Vector3 Cross(Vector3 other) => default;

    /// <summary>Linearly interpolates between two vectors.</summary>
    public Vector3 Lerp(Vector3 goal, double alpha) => default;

    /// <summary>Returns a vector with the largest components of both vectors.</summary>
    public static Vector3 Max(Vector3 a, Vector3 b) => default;

    /// <summary>Returns a vector with the smallest components of both vectors.</summary>
    public static Vector3 Min(Vector3 a, Vector3 b) => default;

    /// <summary>Returns the angle between two vectors in radians.</summary>
    public double Angle(Vector3 other, Vector3? axis = null) => default;

    /// <summary>Rounds each component to the nearest integer using IEEE 754 rounding rules.</summary>
    public Vector3 FuzzyEq(Vector3 other, double epsilon = 0.00001) => default;
}

/// <summary>
/// Represents a 3D position and rotation.
/// </summary>
public struct CFrame {
    /// <summary>The position component.</summary>
    public Vector3 Position { get; }

    /// <summary>Returns the rotation component as a matrix.</summary>
    public CFrame GetRotation() => default;

    /// <summary>The X component of the position.</summary>
    public double X { get; }

    /// <summary>The Y component of the position.</summary>
    public double Y { get; }

    /// <summary>The Z component of the position.</summary>
    public double Z { get; }

    /// <summary>The look vector (forward direction).</summary>
    public Vector3 LookVector { get; }

    /// <summary>The right vector.</summary>
    public Vector3 RightVector { get; }

    /// <summary>The up vector.</summary>
    public Vector3 UpVector { get; }

    /// <summary>Creates a CFrame at the given position.</summary>
    public static extern CFrame @new(double x, double y, double z);

    /// <summary>Creates a CFrame at the given position.</summary>
    public static extern CFrame @new(Vector3 position);

    /// <summary>Creates a CFrame at the given position, looking at the target.</summary>
    public static extern CFrame @new(Vector3 position, Vector3 lookAt);

    /// <summary>Creates a CFrame from a position and quaternion.</summary>
    public static extern CFrame @new(double x, double y, double z, double qx, double qy, double qz, double qw);

    /// <summary>The identity CFrame (no position or rotation).</summary>
    public static extern CFrame identity { get; }

    /// <summary>Creates a CFrame from Euler angles in XYZ order.</summary>
    public static extern CFrame fromEulerAnglesXYZ(double rx, double ry, double rz);

    /// <summary>Creates a CFrame from Euler angles in YXZ order.</summary>
    public static extern CFrame fromEulerAnglesYXZ(double rx, double ry, double rz);

    /// <summary>Creates a CFrame from axis and angle.</summary>
    public static extern CFrame fromAxisAngle(Vector3 axis, double angle);

    /// <summary>Creates a CFrame from an orientation.</summary>
    public static extern CFrame fromOrientation(double rx, double ry, double rz);

    /// <summary>Creates a CFrame looking at a target from a position.</summary>
    public static extern CFrame lookAt(Vector3 at, Vector3 lookAt, Vector3? up = null);

    /// <summary>Returns the inverse of this CFrame.</summary>
    public CFrame Inverse() => default;

    /// <summary>Linearly interpolates between two CFrames.</summary>
    public CFrame Lerp(CFrame goal, double alpha) => default;

    /// <summary>Transforms a point from local to world space.</summary>
    public Vector3 PointToWorldSpace(Vector3 point) => default;

    /// <summary>Transforms a point from world to local space.</summary>
    public Vector3 PointToObjectSpace(Vector3 point) => default;

    /// <summary>Transforms a vector from local to world space.</summary>
    public Vector3 VectorToWorldSpace(Vector3 vector) => default;

    /// <summary>Transforms a vector from world to local space.</summary>
    public Vector3 VectorToObjectSpace(Vector3 vector) => default;

    /// <summary>Returns the Euler angles in XYZ order.</summary>
    public (double rx, double ry, double rz) ToEulerAnglesXYZ() => default;

    /// <summary>Returns the Euler angles in YXZ order.</summary>
    public (double rx, double ry, double rz) ToEulerAnglesYXZ() => default;

    /// <summary>Returns the orientation.</summary>
    public (double rx, double ry, double rz) ToOrientation() => default;

    /// <summary>Returns the axis and angle.</summary>
    public (Vector3 axis, double angle) ToAxisAngle() => default;

    /// <summary>Returns the matrix components.</summary>
    public (double, double, double, double, double, double, double, double, double, double, double, double) GetComponents() => default;
}

/// <summary>
/// Represents a color with red, green, and blue components.
/// </summary>
public struct Color3 {
    /// <summary>The red component (0-1).</summary>
    public double R { get; }

    /// <summary>The green component (0-1).</summary>
    public double G { get; }

    /// <summary>The blue component (0-1).</summary>
    public double B { get; }

    /// <summary>Creates a new Color3 from RGB values (0-1).</summary>
    public static extern Color3 @new(double r, double g, double b);

    /// <summary>Creates a new Color3 from RGB values (0-255).</summary>
    public static extern Color3 fromRGB(int r, int g, int b);

    /// <summary>Creates a new Color3 from HSV values.</summary>
    public static extern Color3 fromHSV(double h, double s, double v);

    /// <summary>Creates a new Color3 from a hex string.</summary>
    public static extern Color3 fromHex(string hex);

    /// <summary>Linearly interpolates between two colors.</summary>
    public Color3 Lerp(Color3 goal, double alpha) => default;

    /// <summary>Returns the HSV components.</summary>
    public (double h, double s, double v) ToHSV() => default;

    /// <summary>Returns the hex string representation.</summary>
    public string ToHex() => default;
}

/// <summary>
/// Represents a legacy brick color.
/// </summary>
public struct BrickColor {
    /// <summary>The name of the brick color.</summary>
    public string Name { get; }

    /// <summary>The number of the brick color.</summary>
    public int Number { get; }

    /// <summary>The Color3 equivalent.</summary>
    public Color3 Color { get; }

    /// <summary>The red component (0-1).</summary>
    public double r { get; }

    /// <summary>The green component (0-1).</summary>
    public double g { get; }

    /// <summary>The blue component (0-1).</summary>
    public double b { get; }

    /// <summary>Creates a new BrickColor from a number.</summary>
    public static extern BrickColor @new(int number);

    /// <summary>Creates a new BrickColor from a name.</summary>
    public static extern BrickColor @new(string name);

    /// <summary>Creates a new BrickColor from RGB values.</summary>
    public static extern BrickColor @new(double r, double g, double b);

    /// <summary>Returns a random BrickColor.</summary>
    public static extern BrickColor Random();

    /// <summary>Returns the white BrickColor.</summary>
    public static extern BrickColor White();

    /// <summary>Returns the black BrickColor.</summary>
    public static extern BrickColor Black();

    /// <summary>Returns the red BrickColor.</summary>
    public static extern BrickColor Red();

    /// <summary>Returns the green BrickColor.</summary>
    public static extern BrickColor Green();

    /// <summary>Returns the blue BrickColor.</summary>
    public static extern BrickColor Blue();

    /// <summary>Returns the yellow BrickColor.</summary>
    public static extern BrickColor Yellow();
}

/// <summary>
/// Represents a dimension with scale and offset.
/// </summary>
public struct UDim {
    /// <summary>The scale component.</summary>
    public double Scale { get; }

    /// <summary>The offset component.</summary>
    public int Offset { get; }

    /// <summary>Creates a new UDim.</summary>
    public static extern UDim @new(double scale, int offset);
}

/// <summary>
/// Represents 2D dimensions with scale and offset.
/// </summary>
public struct UDim2 {
    /// <summary>The X dimension.</summary>
    public UDim X { get; }

    /// <summary>The Y dimension.</summary>
    public UDim Y { get; }

    /// <summary>The width.</summary>
    public UDim Width { get; }

    /// <summary>The height.</summary>
    public UDim Height { get; }

    /// <summary>Creates a new UDim2.</summary>
    public static extern UDim2 @new(double scaleX, int offsetX, double scaleY, int offsetY);

    /// <summary>Creates a new UDim2.</summary>
    public static extern UDim2 @new(UDim x, UDim y);

    /// <summary>Creates a UDim2 from scale values.</summary>
    public static extern UDim2 fromScale(double x, double y);

    /// <summary>Creates a UDim2 from offset values.</summary>
    public static extern UDim2 fromOffset(int x, int y);

    /// <summary>Linearly interpolates between two UDim2s.</summary>
    public UDim2 Lerp(UDim2 goal, double alpha) => default;
}

/// <summary>
/// Represents information for creating a tween.
/// </summary>
public struct TweenInfo {
    /// <summary>The duration of the tween.</summary>
    public double Time { get; }

    /// <summary>The easing style.</summary>
    public Enum EasingStyle { get; }

    /// <summary>The easing direction.</summary>
    public Enum EasingDirection { get; }

    /// <summary>The number of times to repeat.</summary>
    public int RepeatCount { get; }

    /// <summary>Whether to reverse on repeat.</summary>
    public bool Reverses { get; }

    /// <summary>The delay before starting.</summary>
    public double DelayTime { get; }

    /// <summary>Creates a new TweenInfo.</summary>
    public static extern TweenInfo @new(
        double time = 1,
        Enum? easingStyle = null,
        Enum? easingDirection = null,
        int repeatCount = 0,
        bool reverses = false,
        double delayTime = 0
    );
}

/// <summary>
/// Represents a ray in 3D space.
/// </summary>
public struct Ray {
    /// <summary>The origin of the ray.</summary>
    public Vector3 Origin { get; }

    /// <summary>The direction of the ray.</summary>
    public Vector3 Direction { get; }

    /// <summary>Returns the unit ray.</summary>
    public Ray GetUnit() => default;

    /// <summary>Creates a new Ray.</summary>
    public static extern Ray @new(Vector3 origin, Vector3 direction);

    /// <summary>Returns the point at the given distance along the ray.</summary>
    public Vector3 ClosestPoint(Vector3 point) => default;

    /// <summary>Returns the distance from the ray to a point.</summary>
    public double Distance(Vector3 point) => default;
}

/// <summary>
/// Represents a rectangular region.
/// </summary>
public struct Rect {
    /// <summary>The minimum point.</summary>
    public Vector2 Min { get; }

    /// <summary>The maximum point.</summary>
    public Vector2 Max { get; }

    /// <summary>The width of the rectangle.</summary>
    public double Width { get; }

    /// <summary>The height of the rectangle.</summary>
    public double Height { get; }

    /// <summary>Creates a new Rect from min and max points.</summary>
    public static extern Rect @new(Vector2 min, Vector2 max);

    /// <summary>Creates a new Rect from coordinates.</summary>
    public static extern Rect @new(double minX, double minY, double maxX, double maxY);
}

/// <summary>
/// Represents a range of numbers.
/// </summary>
public struct NumberRange {
    /// <summary>The minimum value.</summary>
    public double Min { get; }

    /// <summary>The maximum value.</summary>
    public double Max { get; }

    /// <summary>Creates a new NumberRange.</summary>
    public static extern NumberRange @new(double value);

    /// <summary>Creates a new NumberRange.</summary>
    public static extern NumberRange @new(double min, double max);
}

/// <summary>
/// Represents a 3D region.
/// </summary>
public struct Region3 {
    /// <summary>The center of the region.</summary>
    public CFrame CFrame { get; }

    /// <summary>The size of the region.</summary>
    public Vector3 Size { get; }

    /// <summary>Creates a new Region3.</summary>
    public static extern Region3 @new(Vector3 min, Vector3 max);

    /// <summary>Returns an expanded copy of the Region3.</summary>
    public Region3 ExpandToGrid(double resolution) => default;
}

/// <summary>
/// Represents physical properties of a part.
/// </summary>
public struct PhysicalProperties {
    /// <summary>The density of the material.</summary>
    public double Density { get; }

    /// <summary>The friction coefficient.</summary>
    public double Friction { get; }

    /// <summary>The elasticity (bounciness).</summary>
    public double Elasticity { get; }

    /// <summary>The weight of friction.</summary>
    public double FrictionWeight { get; }

    /// <summary>The weight of elasticity.</summary>
    public double ElasticityWeight { get; }

    /// <summary>Creates custom physical properties.</summary>
    public static extern PhysicalProperties @new(double density, double friction, double elasticity);

    /// <summary>Creates custom physical properties with weights.</summary>
    public static extern PhysicalProperties @new(double density, double friction, double elasticity, double frictionWeight, double elasticityWeight);

    /// <summary>Creates physical properties from a material.</summary>
    public static extern PhysicalProperties @new(Enum material);
}

/// <summary>
/// Represents a font.
/// </summary>
public struct Font {
    /// <summary>The font family.</summary>
    public string Family { get; }

    /// <summary>The font weight.</summary>
    public Enum Weight { get; }

    /// <summary>The font style.</summary>
    public Enum Style { get; }

    /// <summary>Creates a new Font.</summary>
    public static extern Font @new(string family, Enum? weight = null, Enum? style = null);

    /// <summary>Creates a Font from a name.</summary>
    public static extern Font fromName(string name, Enum? weight = null, Enum? style = null);

    /// <summary>Creates a Font from an enum.</summary>
    public static extern Font fromEnum(Enum font);

    /// <summary>Creates a Font from an ID.</summary>
    public static extern Font fromId(long id, Enum? weight = null, Enum? style = null);
}

/// <summary>
/// Represents a color sequence keypoint.
/// </summary>
public struct ColorSequenceKeypoint {
    public double Time { get; }
    public Color3 Value { get; }

    public static extern ColorSequenceKeypoint @new(double time, Color3 value);
}

/// <summary>
/// Represents a sequence of colors over time.
/// </summary>
public struct ColorSequence {
    public ColorSequenceKeypoint[] Keypoints { get; }

    public static extern ColorSequence @new(Color3 color);
    public static extern ColorSequence @new(Color3 startColor, Color3 endColor);
    public static extern ColorSequence @new(ColorSequenceKeypoint[] keypoints);
}

/// <summary>
/// Represents a number sequence keypoint.
/// </summary>
public struct NumberSequenceKeypoint {
    public double Time { get; }
    public double Value { get; }
    public double Envelope { get; }

    public static extern NumberSequenceKeypoint @new(double time, double value, double envelope = 0);
}

/// <summary>
/// Represents a sequence of numbers over time.
/// </summary>
public struct NumberSequence {
    public NumberSequenceKeypoint[] Keypoints { get; }

    public static extern NumberSequence @new(double value);
    public static extern NumberSequence @new(double startValue, double endValue);
    public static extern NumberSequence @new(NumberSequenceKeypoint[] keypoints);
}

/// <summary>
/// Represents a control point on a Path2D.
/// </summary>
public struct Path2DControlPoint {
    public UDim2 Position { get; }
    public UDim2 LeftTangent { get; }
    public UDim2 RightTangent { get; }

    public static extern Path2DControlPoint @new(UDim2 position, UDim2? leftTangent = null, UDim2? rightTangent = null);
}

/// <summary>
/// Represents a set of axis flags for constraints.
/// </summary>
public struct Axes {
    public bool X { get; }
    public bool Y { get; }
    public bool Z { get; }

    public static extern Axes @new(params object[] axes);
}

/// <summary>
/// Represents a set of face flags.
/// </summary>
public struct Faces {
    public bool Top { get; }
    public bool Bottom { get; }
    public bool Left { get; }
    public bool Right { get; }
    public bool Front { get; }
    public bool Back { get; }

    public static extern Faces @new(params object[] faces);
}

/// <summary>
/// Represents a 3D vector with 16-bit integer components.
/// </summary>
public struct Vector3int16 {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public static extern Vector3int16 @new(int x, int y, int z);
}

/// <summary>
/// Represents a 3D region with 16-bit integer coordinates.
/// </summary>
public struct Region3int16 {
    public Vector3int16 Min { get; }
    public Vector3int16 Max { get; }

    public static extern Region3int16 @new(Vector3int16 min, Vector3int16 max);
}

/// <summary>
/// Represents a keyframe for value curves.
/// </summary>
public struct ValueCurveKey {
    public double Time { get; }
    public double Value { get; }
    public double LeftTangent { get; }
    public double RightTangent { get; }
}

/// <summary>
/// Parameters for overlap queries.
/// </summary>
public struct OverlapParams {
    public Instance[] FilterDescendantsInstances { get; set; }
    public Enum FilterType { get; set; }
    public int MaxParts { get; set; }
    public string CollisionGroup { get; set; }

    public static extern OverlapParams @new();
}

/// <summary>
/// Alias for CFrame.
/// </summary>
public struct CoordinateFrame {
    public Vector3 Position { get; }
    public static extern CoordinateFrame @new(double x, double y, double z);
}

// Stub enums for types that have item name conflicts with EnumItem base class

/// <summary>
/// Represents sort order options.
/// </summary>
public abstract class SortOrder : EnumItem {
    public override abstract string Name { get; }
    public override abstract int Value { get; }
    public static extern SortOrder LayoutOrder { get; }
    public static extern SortOrder Custom { get; }
}

/// <summary>
/// Represents completion item kinds (for autocomplete).
/// </summary>
public abstract class CompletionItemKind : EnumItem {
    public override abstract string Name { get; }
    public override abstract int Value { get; }
    public static extern CompletionItemKind Text { get; }
    public static extern CompletionItemKind Method { get; }
    public static extern CompletionItemKind Function { get; }
    public static extern CompletionItemKind Property { get; }
    public static extern CompletionItemKind Field { get; }
    public static extern CompletionItemKind Variable { get; }
    public static extern CompletionItemKind Class { get; }
    public static extern CompletionItemKind Module { get; }
    public static extern CompletionItemKind Keyword { get; }
    public static extern CompletionItemKind Snippet { get; }
    public static extern CompletionItemKind Enum { get; }
    public static extern CompletionItemKind EnumMember { get; }
    public static extern CompletionItemKind Constant { get; }
}

/// <summary>
/// Represents lexeme types for code tokenization.
/// </summary>
public abstract class LexemeType : EnumItem {
    public override abstract string Name { get; }
    public override abstract int Value { get; }
    public static extern LexemeType Eof { get; }
    public static extern LexemeType Error { get; }
    public static extern LexemeType Keyword { get; }
    public static extern LexemeType String { get; }
    public static extern LexemeType Comment { get; }
    public static extern LexemeType Number { get; }
}

/// <summary>
/// Info for creating a DockWidgetPluginGui.
/// </summary>
public struct DockWidgetPluginGuiInfo {
    /// <summary>
    /// Creates a new DockWidgetPluginGuiInfo.
    /// </summary>
    public static extern DockWidgetPluginGuiInfo @new(
        InitialDockState initDockState,
        bool initEnabled = false,
        bool overrideEnabledRestore = false,
        double floatXSize = 0,
        double floatYSize = 0,
        double minWidth = 0,
        double minHeight = 0
    );
}
