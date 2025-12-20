namespace Roblox;

/// <summary>
/// The Players service contains Player objects for currently connected clients.
/// </summary>
public interface Players : Instance {
    /// <summary>The Player associated with the client, only available on the client.</summary>
    Player? LocalPlayer { get; }

    /// <summary>The maximum number of players that can be in this server.</summary>
    int MaxPlayers { get; }

    /// <summary>Fires when a player joins the game.</summary>
    ScriptSignal<Player> PlayerAdded { get; }

    /// <summary>Fires when a player is about to leave the game.</summary>
    ScriptSignal<Player> PlayerRemoving { get; }

    /// <summary>Returns a table of all currently connected Players.</summary>
    Player[] GetPlayers();

    /// <summary>Returns the Player with the given UserId, or nil if not found.</summary>
    Player? GetPlayerByUserId(long userId);

    /// <summary>Returns the Player with the given username, or nil if not found.</summary>
    Player? FindFirstChild(string name);
}

/// <summary>
/// Represents a user connected to the game.
/// </summary>
public interface Player : Instance {
    /// <summary>The unique identifier for the player's Roblox account.</summary>
    long UserId { get; }

    /// <summary>The player's display name.</summary>
    string DisplayName { get; }

    /// <summary>The player's account name.</summary>
    new string Name { get; }

    /// <summary>The player's Team, if any.</summary>
    Instance? Team { get; set; }

    /// <summary>The player's character model, if spawned.</summary>
    Model? Character { get; set; }

    /// <summary>Fires when the player's character is added to the Workspace.</summary>
    ScriptSignal<Model> CharacterAdded { get; }

    /// <summary>Fires when the player's character is about to be removed.</summary>
    ScriptSignal<Model> CharacterRemoving { get; }

    /// <summary>Fires when the player leaves the game.</summary>
    new ScriptSignal Destroying { get; }

    /// <summary>The Backpack that holds the player's Tools.</summary>
    Instance Backpack { get; }

    /// <summary>The PlayerGui that contains the player's GUI objects.</summary>
    Instance PlayerGui { get; }

    /// <summary>Forcibly disconnects the player from the game.</summary>
    void Kick(string message = "");

    /// <summary>Loads the player's character.</summary>
    void LoadCharacter();

    /// <summary>Returns the distance between the player's character and a point.</summary>
    double DistanceFromCharacter(Vector3 point);
}

/// <summary>
/// The Workspace is where all physical objects in the game reside.
/// </summary>
public interface Workspace : Instance {
    /// <summary>The camera used by the local player.</summary>
    Camera? CurrentCamera { get; set; }

    /// <summary>The Terrain object.</summary>
    Instance Terrain { get; }

    /// <summary>Time since the game started, scaled by physics.</summary>
    double DistributedGameTime { get; }

    /// <summary>The gravity applied to falling objects.</summary>
    double Gravity { get; set; }

    /// <summary>Returns parts that touch the given part.</summary>
    BasePart[] GetPartBoundsInBox(CFrame cframe, Vector3 size);

    /// <summary>Casts a ray and returns hit information.</summary>
    RaycastResult? Raycast(Vector3 origin, Vector3 direction, RaycastParams? raycastParams = null);
}

/// <summary>
/// The Camera represents the player's view of the world.
/// </summary>
public interface Camera : Instance {
    /// <summary>The CFrame of the camera.</summary>
    CFrame CFrame { get; set; }

    /// <summary>The focus point of the camera.</summary>
    CFrame Focus { get; set; }

    /// <summary>The field of view of the camera in degrees.</summary>
    double FieldOfView { get; set; }

    /// <summary>The viewport size in pixels.</summary>
    Vector2 ViewportSize { get; }

    /// <summary>Transforms a world position to screen coordinates.</summary>
    (Vector3 screenPoint, bool onScreen) WorldToScreenPoint(Vector3 worldPoint);

    /// <summary>Transforms a world position to viewport coordinates.</summary>
    (Vector3 viewportPoint, bool onScreen) WorldToViewportPoint(Vector3 worldPoint);

    /// <summary>Transforms a viewport position to a world ray.</summary>
    Ray ViewportPointToRay(double x, double y, double depth = 0);
}

/// <summary>
/// Contains objects that are replicated to all clients.
/// </summary>
public interface ReplicatedStorage : Instance { }

/// <summary>
/// Contains objects only accessible on the server.
/// </summary>
public interface ServerStorage : Instance { }

/// <summary>
/// Contains server-side scripts.
/// </summary>
public interface ServerScriptService : Instance { }

/// <summary>
/// Contains GUI objects that are copied to each player's PlayerGui.
/// </summary>
public interface StarterGui : Instance { }

/// <summary>
/// Contains objects that are copied to each player's Backpack and PlayerScripts.
/// </summary>
public interface StarterPlayer : Instance {
    /// <summary>Contains scripts that run on the client.</summary>
    Instance StarterPlayerScripts { get; }

    /// <summary>Contains character scripts.</summary>
    Instance StarterCharacterScripts { get; }
}

/// <summary>
/// Controls the lighting and atmosphere of the game.
/// </summary>
public interface Lighting : Instance {
    /// <summary>The ambient color of the world.</summary>
    Color3 Ambient { get; set; }

    /// <summary>The brightness of the lighting.</summary>
    double Brightness { get; set; }

    /// <summary>The time of day as a string (e.g., "14:30:00").</summary>
    string TimeOfDay { get; set; }

    /// <summary>The time of day as hours since midnight.</summary>
    double ClockTime { get; set; }

    /// <summary>The color of the fog.</summary>
    Color3 FogColor { get; set; }

    /// <summary>How far the fog extends.</summary>
    double FogEnd { get; set; }

    /// <summary>Where the fog starts.</summary>
    double FogStart { get; set; }
}

/// <summary>
/// Provides functions for playing sounds.
/// </summary>
public interface SoundService : Instance {
    /// <summary>Plays a sound at a specific position.</summary>
    void PlayLocalSound(Instance sound);
}

/// <summary>
/// Provides methods for creating tweens.
/// </summary>
public interface TweenService : Instance {
    /// <summary>Creates a new Tween.</summary>
    Tween Create(Instance instance, TweenInfo tweenInfo, Dictionary<string, object> propertyTable);
}

/// <summary>
/// Provides methods for getting time and running callbacks.
/// </summary>
public interface RunService : Instance {
    /// <summary>Fires every frame before physics.</summary>
    ScriptSignal<double> Heartbeat { get; }

    /// <summary>Fires every frame before rendering.</summary>
    ScriptSignal<double> RenderStepped { get; }

    /// <summary>Fires every frame after physics.</summary>
    ScriptSignal<double> Stepped { get; }

    /// <summary>Returns true if the script is running on the client.</summary>
    bool IsClient();

    /// <summary>Returns true if the script is running on the server.</summary>
    bool IsServer();

    /// <summary>Returns true if the script is running in Studio.</summary>
    bool IsStudio();
}

/// <summary>
/// Provides methods for detecting user input.
/// </summary>
public interface UserInputService : Instance {
    /// <summary>Fires when a key is pressed.</summary>
    ScriptSignal<InputObject, bool> InputBegan { get; }

    /// <summary>Fires when a key is released.</summary>
    ScriptSignal<InputObject, bool> InputEnded { get; }

    /// <summary>Fires when input changes (e.g., mouse movement).</summary>
    ScriptSignal<InputObject, bool> InputChanged { get; }

    /// <summary>Returns true if the given key is currently pressed.</summary>
    bool IsKeyDown(Enum keyCode);

    /// <summary>Returns true if the given mouse button is currently pressed.</summary>
    bool IsMouseButtonPressed(Enum userInputType);

    /// <summary>Returns the current position of the mouse.</summary>
    Vector2 GetMouseLocation();
}

/// <summary>
/// Provides HTTP request functionality.
/// </summary>
public interface HttpService : Instance {
    /// <summary>Encodes a table to JSON.</summary>
    string JSONEncode(object input);

    /// <summary>Decodes a JSON string to a table.</summary>
    object JSONDecode(string input);

    /// <summary>URL-encodes a string.</summary>
    string UrlEncode(string input);

    /// <summary>Generates a random UUID.</summary>
    string GenerateGUID(bool wrapInCurlyBraces = true);

    /// <summary>Performs an HTTP GET request.</summary>
    string GetAsync(string url, bool nocache = false, Dictionary<string, string>? headers = null);

    /// <summary>Performs an HTTP POST request.</summary>
    string PostAsync(string url, string data, string? contentType = null, bool compress = false, Dictionary<string, string>? headers = null);

    /// <summary>Performs an HTTP request.</summary>
    Dictionary<string, object> RequestAsync(Dictionary<string, object> requestOptions);
}

/// <summary>
/// Represents user input.
/// </summary>
public interface InputObject : Instance {
    /// <summary>The type of input.</summary>
    Enum UserInputType { get; }

    /// <summary>The state of the input.</summary>
    Enum UserInputState { get; }

    /// <summary>The key code if this is keyboard input.</summary>
    Enum KeyCode { get; }

    /// <summary>The position of the input.</summary>
    Vector3 Position { get; }

    /// <summary>The change in position.</summary>
    Vector3 Delta { get; }
}

/// <summary>
/// Represents an animated transition between property values.
/// </summary>
public interface Tween : Instance {
    /// <summary>Plays the tween.</summary>
    void Play();

    /// <summary>Pauses the tween.</summary>
    void Pause();

    /// <summary>Cancels the tween.</summary>
    void Cancel();

    /// <summary>Fires when the tween completes.</summary>
    ScriptSignal<Enum> Completed { get; }
}

/// <summary>
/// Represents parameters for raycasting.
/// </summary>
public class RaycastParams {
    /// <summary>Instances to filter from the raycast.</summary>
    public Instance[]? FilterDescendantsInstances { get; set; }

    /// <summary>The filter type (Blacklist or Whitelist).</summary>
    public Enum? FilterType { get; set; }

    /// <summary>Whether to ignore water.</summary>
    public bool IgnoreWater { get; set; }

    /// <summary>The collision group to use.</summary>
    public string? CollisionGroup { get; set; }

    /// <summary>Creates a new RaycastParams.</summary>
    public static extern RaycastParams @new();
}

/// <summary>
/// Represents the result of a raycast.
/// </summary>
public interface RaycastResult {
    /// <summary>The Instance that was hit.</summary>
    Instance Instance { get; }

    /// <summary>The position where the ray hit.</summary>
    Vector3 Position { get; }

    /// <summary>The surface normal at the hit position.</summary>
    Vector3 Normal { get; }

    /// <summary>The material at the hit position.</summary>
    Enum Material { get; }

    /// <summary>The distance from the ray origin to the hit position.</summary>
    double Distance { get; }
}
