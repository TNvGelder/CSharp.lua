namespace Roblox;

/// <summary>
/// Represents a connection to a Roblox signal that can be disconnected.
/// </summary>
public interface RBXScriptConnection {
    /// <summary>Whether the connection is still active.</summary>
    bool Connected { get; }

    /// <summary>Disconnects the callback from the signal.</summary>
    void Disconnect();
}

/// <summary>
/// Represents a Roblox event/signal with a single parameter.
/// </summary>
/// <typeparam name="T">The type of the event parameter.</typeparam>
public interface ScriptSignal<T> {
    /// <summary>Connects a callback to the signal.</summary>
    /// <param name="callback">The function to call when the signal fires.</param>
    /// <returns>A connection that can be used to disconnect the callback.</returns>
    RBXScriptConnection Connect(Action<T> callback);

    /// <summary>Yields until the signal fires and returns the arguments.</summary>
    T Wait();
}

/// <summary>
/// Represents a Roblox event/signal with two parameters.
/// </summary>
public interface ScriptSignal<T1, T2> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with three parameters.
/// </summary>
public interface ScriptSignal<T1, T2, T3> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2, T3> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with four parameters.
/// </summary>
public interface ScriptSignal<T1, T2, T3, T4> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2, T3, T4> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with five parameters.
/// </summary>
public interface ScriptSignal<T1, T2, T3, T4, T5> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2, T3, T4, T5> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with six parameters.
/// </summary>
public interface ScriptSignal<T1, T2, T3, T4, T5, T6> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2, T3, T4, T5, T6> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with seven parameters.
/// </summary>
public interface ScriptSignal<T1, T2, T3, T4, T5, T6, T7> {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action<T1, T2, T3, T4, T5, T6, T7> callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}

/// <summary>
/// Represents a Roblox event/signal with no parameters.
/// </summary>
public interface ScriptSignal {
    /// <summary>Connects a callback to the signal.</summary>
    RBXScriptConnection Connect(Action callback);

    /// <summary>Yields until the signal fires.</summary>
    void Wait();
}
