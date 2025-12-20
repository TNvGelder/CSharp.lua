namespace Roblox;

/// <summary>
/// Roblox debug library for debugging and profiling.
/// </summary>
public static class debug {
    /// <summary>
    /// Returns a traceback of the current function call stack.
    /// </summary>
    public static extern string traceback(string? message = null, int? level = null);

    /// <summary>
    /// Returns information about the given level of the call stack.
    /// </summary>
    public static extern object info(int level, string options);

    /// <summary>
    /// Returns information about a function.
    /// </summary>
    public static extern object info(object function, string options);

    /// <summary>
    /// Starts a profiler label.
    /// </summary>
    public static extern void profilebegin(string label);

    /// <summary>
    /// Ends the current profiler label.
    /// </summary>
    public static extern void profileend();

    /// <summary>
    /// Sets the memory category for subsequent allocations.
    /// </summary>
    public static extern void setmemorycategory(string tag);

    /// <summary>
    /// Resets the memory category to the default.
    /// </summary>
    public static extern void resetmemorycategory();
}
