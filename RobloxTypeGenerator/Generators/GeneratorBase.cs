using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RobloxTypeGenerator.Generators;

public enum SecurityLevel {
    None,
    PluginSecurity
}

public enum PluginGenerationMode {
    /// <summary>Normal None-security pass - generates full interfaces.</summary>
    None,
    /// <summary>Plugin-only classes - generates XPlugin : Roblox.Instance.</summary>
    PluginOnly,
    /// <summary>Extension interfaces for mixed classes - generates XPlugin : Roblox.X with plugin members.</summary>
    PluginExtension
}

/// <summary>
/// Base class providing Roslyn SyntaxFactory helpers for code generation.
/// </summary>
public abstract class GeneratorBase {
    /// <summary>
    /// C# reserved keywords that need escaping with @.
    /// </summary>
    protected static readonly HashSet<string> CSharpKeywords = new() {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
        "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit",
        "extern", "false", "finally", "fixed", "float", "for", "foreach",
        "goto", "if", "implicit", "in", "int", "interface", "internal",
        "is", "lock", "long", "namespace", "new", "null", "object",
        "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "virtual", "void",
        "volatile", "while"
    };

    #region Type Mapping

    /// <summary>
    /// Maps a Roblox type to a C# type string.
    /// </summary>
    public static string MapType(ApiValueType? type) {
        if (type == null) return "void";

        string typeName = type.Name;
        bool isNullable = typeName.EndsWith("?");
        string baseName = isNullable ? typeName[..^1] : typeName;

        // First check for special types that need mapping regardless of category
        string? special = MapSpecialTypeBase(baseName);
        if (special != null) {
            return isNullable ? special + "?" : special;
        }

        string result = type.Category switch {
            "Primitive" => MapPrimitiveType(baseName),
            "DataType" => MapDataType(baseName),
            "Enum" => MapEnumType(baseName),
            "Class" => MapClassName(baseName),
            "Group" => MapGroupType(baseName),
            _ => "object"
        };

        // Reapply nullable suffix if the result is a valid type (not object)
        if (isNullable && result != "object" && !result.EndsWith("?")) {
            result += "?";
        }
        return result;
    }

    private static string? MapSpecialTypeBase(string baseName) {
        return baseName switch {
            "Function" => "object",
            "function" => "object",
            "SharedTable" => "object",
            _ => null
        };
    }

    private static string MapClassName(string name) => name switch {
        "RBXScriptSignal" => "ScriptSignal",
        "RBXScriptConnection" => "RBXScriptConnection",
        "Function" => "object",
        "function" => "object",
        _ => name
    };

    private static string MapPrimitiveType(string name) => name switch {
        "bool" => "bool",
        "double" => "double",
        "float" => "float",
        "int" => "int",
        "int64" => "long",
        "string" => "string",
        "void" => "void",
        _ => "object"
    };

    private static string MapDataType(string name) => name switch {
        "Vector2" => "Vector2",
        "Vector3" => "Vector3",
        "Vector3int16" => "Vector3int16",
        "CFrame" => "CFrame",
        "Color3" => "Color3",
        "Color3uint8" => "Color3",
        "BrickColor" => "BrickColor",
        "ColorSequence" => "ColorSequence",
        "ColorSequenceKeypoint" => "ColorSequenceKeypoint",
        "NumberSequence" => "NumberSequence",
        "NumberSequenceKeypoint" => "NumberSequenceKeypoint",
        "DateTime" => "DateTime",
        "TweenInfo" => "TweenInfo",
        "NumberRange" => "NumberRange",
        "UDim" => "UDim",
        "UDim2" => "UDim2",
        "Ray" => "Ray",
        "Rect" => "Rect",
        "Region3" => "Region3",
        "Region3int16" => "Region3int16",
        "PhysicalProperties" => "PhysicalProperties",
        "Axes" => "Axes",
        "Faces" => "Faces",
        "Font" => "Font",
        "Content" => "string",
        "ContentId" => "string",
        "SharedString" => "string",
        "BinaryString" => "string",
        "OptionalCoordinateFrame" => "CFrame?",
        "Path2DControlPoint" => "Path2DControlPoint",
        "RaycastParams" => "RaycastParams",
        "OverlapParams" => "OverlapParams",
        "RaycastResult" => "RaycastResult",
        "Objects" => "Instance[]",
        "Instances" => "Instance[]",
        "Function" => "object",
        "function" => "object",
        "CatalogSearchParams" => "object",
        "ProtectedString" => "string",
        "QDir" => "string",
        "QFont" => "object",
        "SystemAddress" => "string",
        "UniqueId" => "string",
        "OpenCloudModel" => "object",
        "buffer" => "byte[]",
        "RBXScriptSignal" => "ScriptSignal",
        "ScriptSignal" => "ScriptSignal",
        "RotationCurveKey" => "object",
        "SharedTable" => "object",
        "AdReward" => "object",
        "ClipEvaluator" => "object",
        "FloatCurveKey" => "object",
        _ => name
    };

    private static string MapEnumType(string name) {
        // Strip "Enum." prefix if present - enums are at top level in C#
        // Note: Roblox uses Enum.Material in Lua, but C# cannot use "Enum" as a class name
        // (conflicts with System.Enum). The transpiler maps Material.Plastic -> Enum.Material.Plastic
        // in LuaSyntaxNodeTransform.cs (see IsRobloxEnumItemType).
        if (name.StartsWith("Enum.")) {
            name = name[5..];
        }
        return name;
    }

    private static string MapGroupType(string name) => name switch {
        "Variant" => "object",
        "Array" => "object[]",
        "Dictionary" => "Dictionary<string, object>",
        "Map" => "Dictionary<string, object>",
        "Tuple" => "object[]",
        _ => "object"
    };

    #endregion

    #region Syntax Helpers

    /// <summary>
    /// Creates a safe C# identifier from a name.
    /// </summary>
    public static SyntaxToken SafeIdentifier(string name) {
        string safeName = MakeSafeIdentifierString(name);
        return Identifier(safeName);
    }

    /// <summary>
    /// Makes a valid C# identifier string from a name.
    /// </summary>
    public static string MakeSafeIdentifierString(string name) {
        if (string.IsNullOrEmpty(name)) {
            return "_";
        }

        var sb = new System.Text.StringBuilder();
        foreach (char c in name) {
            if (char.IsLetterOrDigit(c) || c == '_') {
                sb.Append(c);
            } else if (c != ' ') {
                sb.Append('_');
            }
        }

        string result = sb.ToString();

        if (result.Length > 0 && char.IsDigit(result[0])) {
            result = "_" + result;
        }

        if (CSharpKeywords.Contains(result)) {
            return "@" + result;
        }

        return result;
    }

    /// <summary>
    /// Parses a type name string into a TypeSyntax.
    /// </summary>
    public static TypeSyntax ParseTypeString(string typeName) {
        if (typeName == "void") {
            return PredefinedType(Token(SyntaxKind.VoidKeyword));
        }
        return ParseTypeName(typeName);
    }

    /// <summary>
    /// Creates an XML documentation trivia with summary text.
    /// Uses simple summary-only format for reliability.
    /// </summary>
    public static SyntaxTriviaList CreateDocComment(string summaryText, string? remarks = null) {
        // Build full documentation text - simpler approach that avoids complex Roslyn XML structures
        var lines = new List<string>();
        lines.Add($"/// <summary>{EscapeXmlDoc(summaryText)}</summary>");

        if (!string.IsNullOrEmpty(remarks)) {
            lines.Add($"/// <remarks>{EscapeXmlDoc(remarks)}</remarks>");
        }

        var trivias = new List<SyntaxTrivia>();
        foreach (var line in lines) {
            trivias.Add(Comment(line));
            trivias.Add(CarriageReturnLineFeed);
        }

        return TriviaList(trivias);
    }

    /// <summary>
    /// Creates an XML documentation trivia with summary, params, returns, and remarks.
    /// Uses simple string-based format for reliability.
    /// </summary>
    public static SyntaxTriviaList CreateMethodDocComment(
        string summaryText,
        IEnumerable<(string name, string doc)>? parameters = null,
        string? returns = null,
        string? remarks = null) {

        var lines = new List<string>();
        lines.Add($"/// <summary>{EscapeXmlDoc(summaryText)}</summary>");

        // Parameters
        if (parameters != null) {
            foreach (var (name, doc) in parameters) {
                lines.Add($"/// <param name=\"{name}\">{EscapeXmlDoc(doc)}</param>");
            }
        }

        // Returns
        if (!string.IsNullOrEmpty(returns)) {
            lines.Add($"/// <returns>{EscapeXmlDoc(returns)}</returns>");
        }

        // Remarks
        if (!string.IsNullOrEmpty(remarks)) {
            lines.Add($"/// <remarks>{EscapeXmlDoc(remarks)}</remarks>");
        }

        var trivias = new List<SyntaxTrivia>();
        foreach (var line in lines) {
            trivias.Add(Comment(line));
            trivias.Add(CarriageReturnLineFeed);
        }

        return TriviaList(trivias);
    }

    /// <summary>
    /// Escapes text for use in XML doc comments.
    /// </summary>
    public static string EscapeXmlDoc(string text) {
        if (string.IsNullOrEmpty(text)) return "";
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }

    /// <summary>
    /// Creates an Obsolete attribute.
    /// </summary>
    public static AttributeListSyntax ObsoleteAttribute(string message = "This member is deprecated.") {
        return AttributeList(SingletonSeparatedList(
            Attribute(
                QualifiedName(
                    IdentifierName("System"),
                    IdentifierName("Obsolete")))
            .WithArgumentList(AttributeArgumentList(SingletonSeparatedList(
                AttributeArgument(LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(message))))))));
    }

    /// <summary>
    /// Creates a property with get accessor only.
    /// </summary>
    public static PropertyDeclarationSyntax CreateGetOnlyProperty(TypeSyntax type, string name) {
        return PropertyDeclaration(type, SafeIdentifier(name))
            .WithAccessorList(AccessorList(SingletonList(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))));
    }

    /// <summary>
    /// Creates a property with get and set accessors.
    /// </summary>
    public static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string name) {
        return PropertyDeclaration(type, SafeIdentifier(name))
            .WithAccessorList(AccessorList(List(new[] {
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            })));
    }

    /// <summary>
    /// Creates a method signature (for interfaces).
    /// </summary>
    public static MethodDeclarationSyntax CreateMethodSignature(
        TypeSyntax returnType,
        string name,
        IEnumerable<ParameterSyntax>? parameters = null) {

        var paramList = parameters != null
            ? ParameterList(SeparatedList(parameters))
            : ParameterList();

        return MethodDeclaration(returnType, SafeIdentifier(name))
            .WithParameterList(paramList)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    /// <summary>
    /// Creates a parameter syntax.
    /// </summary>
    public static ParameterSyntax CreateParameter(string type, string name) {
        return Parameter(SafeIdentifier(name))
            .WithType(ParseTypeString(type));
    }

    #endregion

    #region Security Helpers

    /// <summary>
    /// Checks if a member should be generated based on security level.
    /// </summary>
    public static bool ShouldGenerateMember(ApiMember member, SecurityLevel level) {
        if (member.IsNotScriptable || member.IsHidden) {
            return false;
        }

        string readSecurity = member.GetReadSecurity();
        string writeSecurity = member.GetWriteSecurity();

        return level switch {
            SecurityLevel.None => readSecurity == "None" || writeSecurity == "None",
            SecurityLevel.PluginSecurity =>
                (readSecurity == "PluginSecurity" || writeSecurity == "PluginSecurity") &&
                readSecurity != "RobloxSecurity" && writeSecurity != "RobloxSecurity" &&
                readSecurity != "NotAccessibleSecurity" && writeSecurity != "NotAccessibleSecurity",
            _ => false
        };
    }

    /// <summary>
    /// Checks if a class should be generated based on security level.
    /// </summary>
    public static bool ShouldGenerateClass(ApiClass cls, SecurityLevel level) {
        return cls.Members.Any(m => ShouldGenerateMember(m, level));
    }

    /// <summary>
    /// Gets the thread safety documentation string.
    /// </summary>
    public static string GetThreadSafetyDoc(string threadSafety) => threadSafety switch {
        "Safe" => "Thread-safe for read and write.",
        "ReadSafe" => "Thread-safe for reading only.",
        "Unsafe" => "Main thread only.",
        _ => ""
    };

    #endregion

    #region XML Helpers (for MetadataGenerator)

    /// <summary>
    /// Escapes a string for use in XML attributes.
    /// </summary>
    public static string EscapeXml(string text) {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    #endregion
}
