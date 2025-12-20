namespace Roblox;

/// <summary>
/// Roblox os library for time-related functions.
/// </summary>
public static class os {
    /// <summary>
    /// Returns the number of seconds since the Unix epoch (January 1, 1970).
    /// </summary>
    public static extern double time();

    /// <summary>
    /// Returns the amount of CPU time used by the program.
    /// </summary>
    public static extern double clock();

    /// <summary>
    /// Returns the difference in seconds between two times.
    /// </summary>
    public static extern double difftime(double t2, double t1);

    /// <summary>
    /// Returns a table or string containing the date and time.
    /// </summary>
    public static extern Dictionary<string, object> date(string? format = null, double? time = null);
}
