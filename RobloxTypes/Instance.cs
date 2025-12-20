namespace Roblox;

/// <summary>
/// Base class for all Roblox instances. Instance is the base class for all classes in the Roblox class hierarchy.
/// </summary>
public interface Instance {
    /// <summary>A non-unique identifier of the Instance.</summary>
    string Name { get; set; }

    /// <summary>The hierarchical parent of the Instance.</summary>
    Instance? Parent { get; set; }

    /// <summary>A read-only string representing the class this Instance belongs to.</summary>
    string ClassName { get; }

    /// <summary>Returns the first child of this Instance with the given name.</summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The found Instance, or nil if not found.</returns>
    Instance? FindFirstChild(string name);

    /// <summary>Returns the first child of this Instance with the given name, optionally searching descendants.</summary>
    /// <param name="name">The name to search for.</param>
    /// <param name="recursive">Whether to search all descendants.</param>
    /// <returns>The found Instance, or nil if not found.</returns>
    Instance? FindFirstChild(string name, bool recursive);

    /// <summary>Returns the first child of this Instance with the given name, cast to the specified type.</summary>
    /// <typeparam name="T">The expected type of the child.</typeparam>
    /// <param name="name">The name to search for.</param>
    /// <returns>The found Instance cast to T, or nil if not found.</returns>
    T? FindFirstChild<T>(string name) where T : class, Instance;

    /// <summary>Returns the first child of this Instance with the given name, optionally searching descendants.</summary>
    /// <typeparam name="T">The expected type of the child.</typeparam>
    /// <param name="name">The name to search for.</param>
    /// <param name="recursive">Whether to search all descendants.</param>
    /// <returns>The found Instance cast to T, or nil if not found.</returns>
    T? FindFirstChild<T>(string name, bool recursive) where T : class, Instance;

    /// <summary>Returns the first child of this Instance whose ClassName equals the given class name.</summary>
    /// <param name="className">The class name to search for.</param>
    /// <returns>The found Instance, or nil if not found.</returns>
    Instance? FindFirstChildOfClass(string className);

    /// <summary>Returns the first child of this Instance whose class matches the type parameter.</summary>
    /// <typeparam name="T">The class type to search for.</typeparam>
    /// <returns>The found Instance, or nil if not found.</returns>
    T? FindFirstChildOfClass<T>() where T : class, Instance;

    /// <summary>Returns the first child which is a subclass of the given class name.</summary>
    /// <param name="className">The class name to check inheritance against.</param>
    /// <returns>The found Instance, or nil if not found.</returns>
    Instance? FindFirstChildWhichIsA(string className);

    /// <summary>Returns the first child which is a subclass of the type parameter.</summary>
    /// <typeparam name="T">The class type to check inheritance against.</typeparam>
    /// <returns>The found Instance, or nil if not found.</returns>
    T? FindFirstChildWhichIsA<T>() where T : class, Instance;

    /// <summary>Returns the first ancestor of this Instance with the given name.</summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The found ancestor, or nil if not found.</returns>
    Instance? FindFirstAncestor(string name);

    /// <summary>Returns the first ancestor of this Instance whose ClassName equals the given class name.</summary>
    /// <param name="className">The class name to search for.</param>
    /// <returns>The found ancestor, or nil if not found.</returns>
    Instance? FindFirstAncestorOfClass(string className);

    /// <summary>Returns the first ancestor which is a subclass of the given class name.</summary>
    /// <param name="className">The class name to check inheritance against.</param>
    /// <returns>The found ancestor, or nil if not found.</returns>
    Instance? FindFirstAncestorWhichIsA(string className);

    /// <summary>Returns a list of all children of this Instance.</summary>
    /// <returns>An array containing all children.</returns>
    Instance[] GetChildren();

    /// <summary>Returns a list of all descendants of this Instance.</summary>
    /// <returns>An array containing all descendants.</returns>
    Instance[] GetDescendants();

    /// <summary>Sets the Parent property to nil, locks the Parent property, and disconnects all connections.</summary>
    void Destroy();

    /// <summary>Create a copy of this Instance and all its descendants, with all Ref properties pointing to the clones.</summary>
    /// <returns>A clone of the Instance.</returns>
    Instance Clone();

    /// <summary>Returns true if this Instance is of the given class or a subclass of it.</summary>
    /// <param name="className">The class name to check.</param>
    /// <returns>True if this Instance is or inherits from the class.</returns>
    bool IsA(string className);

    /// <summary>Returns true if this Instance is of the type parameter or a subclass of it.</summary>
    /// <typeparam name="T">The class type to check.</typeparam>
    /// <returns>True if this Instance is or inherits from T.</returns>
    bool IsA<T>() where T : Instance;

    /// <summary>Returns true if this Instance is an ancestor of the given descendant.</summary>
    /// <param name="descendant">The potential descendant to check.</param>
    /// <returns>True if this Instance is an ancestor of the descendant.</returns>
    bool IsAncestorOf(Instance descendant);

    /// <summary>Returns true if this Instance is a descendant of the given ancestor.</summary>
    /// <param name="ancestor">The potential ancestor to check.</param>
    /// <returns>True if this Instance is a descendant of the ancestor.</returns>
    bool IsDescendantOf(Instance ancestor);

    /// <summary>Yields until a child with the given name exists.</summary>
    /// <param name="childName">The name of the child to wait for.</param>
    /// <returns>The child Instance.</returns>
    Instance WaitForChild(string childName);

    /// <summary>Yields until a child with the given name exists, with a timeout.</summary>
    /// <param name="childName">The name of the child to wait for.</param>
    /// <param name="timeout">The maximum time to wait in seconds.</param>
    /// <returns>The child Instance, or nil if timed out.</returns>
    Instance? WaitForChild(string childName, double timeout);

    /// <summary>Gets the value of an attribute.</summary>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>The attribute value, or nil if not set.</returns>
    object? GetAttribute(string attribute);

    /// <summary>Sets the value of an attribute.</summary>
    /// <param name="attribute">The name of the attribute.</param>
    /// <param name="value">The value to set.</param>
    void SetAttribute(string attribute, object? value);

    /// <summary>Gets all attributes as a dictionary.</summary>
    /// <returns>A dictionary of attribute names to values.</returns>
    Dictionary<string, object> GetAttributes();

    /// <summary>Fires when a child is added to this Instance.</summary>
    ScriptSignal<Instance> ChildAdded { get; }

    /// <summary>Fires when a child is removed from this Instance.</summary>
    ScriptSignal<Instance> ChildRemoved { get; }

    /// <summary>Fires when a descendant is added.</summary>
    ScriptSignal<Instance> DescendantAdded { get; }

    /// <summary>Fires when a descendant is removed.</summary>
    ScriptSignal<Instance> DescendantRemoving { get; }

    /// <summary>Fires when an attribute changes.</summary>
    ScriptSignal<string> AttributeChanged { get; }

    /// <summary>Fires when the Instance is being destroyed.</summary>
    ScriptSignal Destroying { get; }
}
