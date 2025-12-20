using System.Text.Json.Serialization;

namespace RobloxTypeGenerator;

/// <summary>
/// Root structure of the Roblox API dump.
/// </summary>
public class ApiDump {
    [JsonPropertyName("Classes")]
    public List<ApiClass> Classes { get; set; } = new();

    [JsonPropertyName("Enums")]
    public List<ApiEnum> Enums { get; set; } = new();

    [JsonPropertyName("Version")]
    public int Version { get; set; }
}

/// <summary>
/// Represents a Roblox class/interface.
/// </summary>
public class ApiClass {
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Superclass")]
    public string Superclass { get; set; } = "";

    [JsonPropertyName("MemoryCategory")]
    public string MemoryCategory { get; set; } = "";

    [JsonPropertyName("Tags")]
    public List<object> RawTags { get; set; } = new();

    private List<string>? _parsedTags;
    [JsonIgnore]
    public List<string> Tags {
        get {
            if (_parsedTags == null) {
                _parsedTags = new List<string>();
                foreach (var tag in RawTags) {
                    if (tag is System.Text.Json.JsonElement elem) {
                        if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                            _parsedTags.Add(elem.GetString() ?? "");
                        } else if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                            foreach (var prop in elem.EnumerateObject()) {
                                _parsedTags.Add(prop.Name);
                            }
                        }
                    } else if (tag is string s) {
                        _parsedTags.Add(s);
                    }
                }
            }
            return _parsedTags;
        }
    }

    [JsonPropertyName("Members")]
    public List<ApiMember> Members { get; set; } = new();
}

/// <summary>
/// Represents a member (Property, Function, Event, Callback) of a class.
/// </summary>
public class ApiMember {
    [JsonPropertyName("MemberType")]
    public string MemberType { get; set; } = "";

    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("ValueType")]
    public object? RawValueType { get; set; }

    [JsonPropertyName("ReturnType")]
    public object? RawReturnType { get; set; }

    /// <summary>
    /// Parses the ValueType from the raw JSON representation.
    /// </summary>
    [JsonIgnore]
    public ApiValueType? ValueType => ParseValueType(RawValueType);

    /// <summary>
    /// Parses the ReturnType from the raw JSON representation.
    /// </summary>
    [JsonIgnore]
    public ApiValueType? ReturnType => ParseValueType(RawReturnType);

    private static ApiValueType? ParseValueType(object? raw) {
        if (raw is System.Text.Json.JsonElement elem) {
            if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                string category = "";
                string name = "";
                if (elem.TryGetProperty("Category", out var catProp)) {
                    category = catProp.GetString() ?? "";
                }
                if (elem.TryGetProperty("Name", out var nameProp)) {
                    name = nameProp.GetString() ?? "";
                }
                return new ApiValueType { Category = category, Name = name };
            } else if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                // Sometimes ReturnType is just a string
                return new ApiValueType { Category = "Primitive", Name = elem.GetString() ?? "" };
            }
        }
        return null;
    }

    [JsonPropertyName("Parameters")]
    public List<ApiParameter> Parameters { get; set; } = new();

    [JsonPropertyName("Security")]
    public object? Security { get; set; }

    [JsonPropertyName("Serialization")]
    public ApiSerialization? Serialization { get; set; }

    [JsonPropertyName("ThreadSafety")]
    public string ThreadSafety { get; set; } = "";

    [JsonPropertyName("Tags")]
    public List<object> RawTags { get; set; } = new();

    private List<string>? _parsedTags;
    [JsonIgnore]
    public List<string> Tags {
        get {
            if (_parsedTags == null) {
                _parsedTags = new List<string>();
                foreach (var tag in RawTags) {
                    if (tag is System.Text.Json.JsonElement elem) {
                        if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                            _parsedTags.Add(elem.GetString() ?? "");
                        } else if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                            // Some tags are objects like {"PreferredDescriptorName": "value"}
                            foreach (var prop in elem.EnumerateObject()) {
                                _parsedTags.Add(prop.Name);
                            }
                        }
                    } else if (tag is string s) {
                        _parsedTags.Add(s);
                    }
                }
            }
            return _parsedTags;
        }
    }

    /// <summary>
    /// Gets the security level for reading this member.
    /// </summary>
    public string GetReadSecurity() {
        if (Security is System.Text.Json.JsonElement elem) {
            if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                if (elem.TryGetProperty("Read", out var readProp)) {
                    return readProp.GetString() ?? "None";
                }
            } else if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                return elem.GetString() ?? "None";
            }
        }
        return "None";
    }

    /// <summary>
    /// Gets the security level for writing this member.
    /// </summary>
    public string GetWriteSecurity() {
        if (Security is System.Text.Json.JsonElement elem) {
            if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                if (elem.TryGetProperty("Write", out var writeProp)) {
                    return writeProp.GetString() ?? "None";
                }
            } else if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                return elem.GetString() ?? "None";
            }
        }
        return "None";
    }

    public bool IsReadOnly => Tags.Contains("ReadOnly");
    public bool IsWriteOnly => Tags.Contains("WriteOnly");
    public bool IsDeprecated => Tags.Contains("Deprecated");
    public bool IsNotScriptable => Tags.Contains("NotScriptable");
    public bool IsHidden => Tags.Contains("Hidden");
    public bool CanYield => Tags.Contains("CanYield") || Tags.Contains("Yields");
    public bool IsNoYield => Tags.Contains("NoYield");
}

/// <summary>
/// Represents a type reference in the API.
/// </summary>
public class ApiValueType {
    [JsonPropertyName("Category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";
}

/// <summary>
/// Represents a parameter in a function/callback.
/// </summary>
public class ApiParameter {
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Type")]
    public object? RawType { get; set; }

    /// <summary>
    /// Parses the Type from the raw JSON representation.
    /// </summary>
    [JsonIgnore]
    public ApiValueType Type => ParseValueType(RawType) ?? new ApiValueType();

    [JsonPropertyName("Default")]
    public string? Default { get; set; }

    private static ApiValueType? ParseValueType(object? raw) {
        if (raw is System.Text.Json.JsonElement elem) {
            if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                string category = "";
                string name = "";
                if (elem.TryGetProperty("Category", out var catProp)) {
                    category = catProp.GetString() ?? "";
                }
                if (elem.TryGetProperty("Name", out var nameProp)) {
                    name = nameProp.GetString() ?? "";
                }
                return new ApiValueType { Category = category, Name = name };
            } else if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                return new ApiValueType { Category = "Primitive", Name = elem.GetString() ?? "" };
            }
        }
        return null;
    }
}

/// <summary>
/// Serialization information for a property.
/// </summary>
public class ApiSerialization {
    [JsonPropertyName("CanLoad")]
    public bool CanLoad { get; set; }

    [JsonPropertyName("CanSave")]
    public bool CanSave { get; set; }
}

/// <summary>
/// Represents a Roblox enum.
/// </summary>
public class ApiEnum {
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Items")]
    public List<ApiEnumItem> Items { get; set; } = new();
}

/// <summary>
/// Represents an item in a Roblox enum.
/// </summary>
public class ApiEnumItem {
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Value")]
    public int Value { get; set; }
}

/// <summary>
/// Documentation entry from api-docs.
/// </summary>
public class ApiDocEntry {
    [JsonPropertyName("documentation")]
    public string Documentation { get; set; } = "";

    [JsonPropertyName("learn_more_link")]
    public string? LearnMoreLink { get; set; }

    [JsonPropertyName("params")]
    public List<ApiDocParam>? Params { get; set; }

    [JsonPropertyName("returns")]
    public List<object>? RawReturns { get; set; }

    /// <summary>
    /// Parsed returns - handles both string references and object formats.
    /// </summary>
    [JsonIgnore]
    public List<ApiDocReturn> Returns {
        get {
            var result = new List<ApiDocReturn>();
            if (RawReturns == null) return result;

            foreach (var item in RawReturns) {
                if (item is System.Text.Json.JsonElement elem) {
                    if (elem.ValueKind == System.Text.Json.JsonValueKind.String) {
                        // String reference like "@roblox/globaltype/DateTime.ToUniversalTime/return/0"
                        result.Add(new ApiDocReturn { Documentation = elem.GetString() ?? "" });
                    } else if (elem.ValueKind == System.Text.Json.JsonValueKind.Object) {
                        // Object with documentation field
                        string doc = "";
                        string? name = null;
                        string? luaType = null;
                        if (elem.TryGetProperty("documentation", out var docProp)) {
                            doc = docProp.GetString() ?? "";
                        }
                        if (elem.TryGetProperty("name", out var nameProp)) {
                            name = nameProp.GetString();
                        }
                        if (elem.TryGetProperty("lua_type", out var typeProp)) {
                            luaType = typeProp.GetString();
                        }
                        result.Add(new ApiDocReturn { Documentation = doc, Name = name, LuaType = luaType });
                    }
                } else if (item is string s) {
                    result.Add(new ApiDocReturn { Documentation = s });
                }
            }
            return result;
        }
    }
}

public class ApiDocParam {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("documentation")]
    public string Documentation { get; set; } = "";

    [JsonPropertyName("lua_type")]
    public string? LuaType { get; set; }
}

public class ApiDocReturn {
    [JsonPropertyName("documentation")]
    public string Documentation { get; set; } = "";

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("lua_type")]
    public string? LuaType { get; set; }
}
