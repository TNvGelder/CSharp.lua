namespace Roblox;

/// <summary>
/// Roblox task library for scheduling and managing coroutines.
/// </summary>
public static class task {
    /// <summary>
    /// Runs a function in a new thread immediately.
    /// </summary>
    public static extern object spawn(Action callback);

    /// <summary>
    /// Runs a function in a new thread immediately with an argument.
    /// </summary>
    public static extern object spawn<T>(Action<T> callback, T arg);

    /// <summary>
    /// Schedules a function to run after a delay.
    /// </summary>
    public static extern object delay(double duration, Action callback);

    /// <summary>
    /// Yields the current thread for the specified duration.
    /// </summary>
    public static extern double wait(double duration = 0);

    /// <summary>
    /// Cancels a thread created by task.spawn or task.delay.
    /// </summary>
    public static extern void cancel(object thread);

    /// <summary>
    /// Defers a function to run at the end of the current resumption cycle.
    /// </summary>
    public static extern object defer(Action callback);

    /// <summary>
    /// Defers a function with an argument to run at the end of the current resumption cycle.
    /// </summary>
    public static extern object defer<T>(Action<T> callback, T arg);

    /// <summary>
    /// Yields the current thread until it is resumed by the engine.
    /// </summary>
    public static extern object synchronize();

    /// <summary>
    /// Causes the current thread to become desynchronized.
    /// </summary>
    public static extern object desynchronize();
}
