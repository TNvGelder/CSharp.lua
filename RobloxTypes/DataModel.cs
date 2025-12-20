namespace Roblox;

/// <summary>
/// The DataModel (commonly known as "game") is the root of Roblox's parent-child hierarchy.
/// </summary>
public interface DataModel : Instance {
    /// <summary>
    /// Returns the service with the class name requested.
    /// Will yield if the service does not exist yet.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The requested service.</returns>
    T GetService<T>() where T : class, Instance;

    /// <summary>
    /// Returns the service with the class name requested, if it exists.
    /// </summary>
    /// <param name="className">The class name of the service.</param>
    /// <returns>The requested service, or nil if not found.</returns>
    Instance? FindService(string className);

    /// <summary>A unique identifier for the game instance.</summary>
    long GameId { get; }

    /// <summary>The PlaceId of the current place.</summary>
    long PlaceId { get; }

    /// <summary>The version of the current place.</summary>
    int PlaceVersion { get; }

    /// <summary>The JobId of this game server.</summary>
    string JobId { get; }

    /// <summary>Describes the user's membership type.</summary>
    int CreatorType { get; }

    /// <summary>The UserId of the place's creator, or the group ID if owned by a group.</summary>
    long CreatorId { get; }
}
