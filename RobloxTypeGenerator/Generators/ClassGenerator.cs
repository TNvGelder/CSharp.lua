using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RobloxTypeGenerator.Generators;

public class ClassGenerator : GeneratorBase {
    private readonly List<ApiClass> _classes;
    private readonly Dictionary<string, ApiDocEntry> _docs;
    private readonly SecurityLevel _securityLevel;
    private readonly PluginGenerationMode _pluginMode;
    private readonly string _namespaceName;
    private readonly HashSet<string> _classNames;
    private readonly HashSet<string> _nonePassClasses;
    private readonly HashSet<string> _generatedClasses = new();

    // Reference shared skip list from Constants.cs
    private static HashSet<string> SkipClasses => GeneratorConstants.HandWrittenClasses;

    public ClassGenerator(
        List<ApiClass> classes,
        Dictionary<string, ApiDocEntry> docs,
        SecurityLevel securityLevel,
        PluginGenerationMode pluginMode = PluginGenerationMode.None,
        string namespaceName = "Roblox",
        HashSet<string>? nonePassClasses = null) {

        _classes = classes;
        _docs = docs;
        _securityLevel = securityLevel;
        _pluginMode = pluginMode;
        _namespaceName = namespaceName;
        _classNames = classes.Select(c => c.Name).ToHashSet();
        _nonePassClasses = nonePassClasses ?? new();
    }

    /// <summary>
    /// Gets the set of class names that were generated.
    /// </summary>
    public HashSet<string> GeneratedClasses => _generatedClasses;

    public CompilationUnitSyntax Generate() {
        var members = new List<MemberDeclarationSyntax>();

        // Filter classes based on mode:
        // - None mode: skip hand-written classes (they're in RobloxTypes/*.cs)
        // - Plugin mode: include hand-written classes so we can generate extension interfaces for plugin members
        var filteredClasses = _pluginMode == PluginGenerationMode.None
            ? _classes.Where(c => !SkipClasses.Contains(c.Name)).OrderBy(c => c.Name).ToList()
            : _classes.Where(c => c.Name != "<<<ROOT>>>").OrderBy(c => c.Name).ToList();

        // For None mode: first identify required types that need stub interfaces
        if (_pluginMode == PluginGenerationMode.None) {
            // First pass: identify all classes that will be generated
            var classesToGenerate = new HashSet<string>();
            foreach (var cls in filteredClasses) {
                bool hasNoneMembers = cls.Members.Any(m => ShouldGenerateMember(m, SecurityLevel.None));
                if (hasNoneMembers) {
                    classesToGenerate.Add(cls.Name);
                }
            }

            // Second pass: identify required types (base types + referenced types)
            var requiredTypes = new HashSet<string>();

            void AddRequiredType(string? typeName) {
                if (string.IsNullOrEmpty(typeName) ||
                    typeName == "<<<ROOT>>>" ||
                    !_classNames.Contains(typeName) ||
                    classesToGenerate.Contains(typeName) ||
                    requiredTypes.Contains(typeName) ||
                    SkipClasses.Contains(typeName)) {
                    return;
                }
                requiredTypes.Add(typeName);
            }

            // Check classes that will be generated for referenced types
            foreach (var cls in filteredClasses) {
                if (classesToGenerate.Contains(cls.Name)) {
                    // Add superclass
                    AddRequiredType(cls.Superclass);

                    // Check members for referenced types
                    foreach (var member in cls.Members) {
                        if (!ShouldGenerateMember(member, SecurityLevel.None)) continue;

                        // Property/return types
                        if (member.ValueType?.Category == "Class") {
                            AddRequiredType(member.ValueType.Name?.TrimEnd('?'));
                        }
                        if (member.ReturnType?.Category == "Class") {
                            AddRequiredType(member.ReturnType.Name?.TrimEnd('?'));
                        }

                        // Parameter types
                        foreach (var param in member.Parameters) {
                            if (param.Type?.Category == "Class") {
                                AddRequiredType(param.Type.Name?.TrimEnd('?'));
                            }
                        }
                    }
                }
            }

            // Recursively add base types of required types
            bool addedNew;
            do {
                addedNew = false;
                foreach (var typeName in requiredTypes.ToList()) {
                    var cls = _classes.FirstOrDefault(c => c.Name == typeName);
                    if (cls != null) {
                        string? superclass = cls.Superclass;
                        if (!string.IsNullOrEmpty(superclass) &&
                            superclass != "<<<ROOT>>>" &&
                            _classNames.Contains(superclass) &&
                            !classesToGenerate.Contains(superclass) &&
                            !requiredTypes.Contains(superclass) &&
                            !SkipClasses.Contains(superclass)) {
                            requiredTypes.Add(superclass);
                            addedNew = true;
                        }
                    }
                }
            } while (addedNew);

            // Generate stub interfaces for required types
            foreach (var typeName in requiredTypes.OrderBy(n => n)) {
                var cls = _classes.First(c => c.Name == typeName);
                var stub = GenerateStubInterface(cls);
                if (stub != null) {
                    members.Add(stub);
                    _generatedClasses.Add(cls.Name);
                }
            }
        }

        foreach (var cls in filteredClasses) {
            bool hasNoneMembers = cls.Members.Any(m => ShouldGenerateMember(m, SecurityLevel.None));
            bool hasPluginMembers = cls.Members.Any(m => ShouldGenerateMember(m, SecurityLevel.PluginSecurity));

            if (_pluginMode == PluginGenerationMode.None) {
                // Normal None-security pass: generate full interfaces for classes with None-level members
                if (hasNoneMembers) {
                    var interfaceDecl = GenerateClass(cls);
                    if (interfaceDecl != null) {
                        members.Add(interfaceDecl);
                        _generatedClasses.Add(cls.Name);
                    }
                }
            } else {
                // Plugin pass: generate for classes with plugin members
                if (hasPluginMembers) {
                    // Check if base class exists (generated in None pass, or hand-written)
                    bool baseClassExists = hasNoneMembers || _nonePassClasses.Contains(cls.Name) || SkipClasses.Contains(cls.Name);

                    if (baseClassExists) {
                        // Mixed class or hand-written class: generate extension interface (XPlugin : Roblox.X)
                        var extensionDecl = GeneratePluginExtension(cls);
                        if (extensionDecl != null) {
                            members.Add(extensionDecl);
                            _generatedClasses.Add(cls.Name + "Plugin");
                        }
                    } else {
                        // Plugin-only class: generate full interface (X : Roblox.Instance, no Plugin suffix)
                        var interfaceDecl = GeneratePluginOnlyClass(cls);
                        if (interfaceDecl != null) {
                            members.Add(interfaceDecl);
                            _generatedClasses.Add(cls.Name);  // No suffix for plugin-only classes
                        }
                    }
                }
            }
        }

        // Create namespace
        var namespaceDecl = FileScopedNamespaceDeclaration(IdentifierName(_namespaceName))
            .WithMembers(List(members));

        // Create compilation unit
        var compilationUnit = CompilationUnit();

        // For plugin namespace, add using directive for base Roblox types
        if (_namespaceName != "Roblox") {
            compilationUnit = compilationUnit.WithUsings(SingletonList(
                UsingDirective(IdentifierName("Roblox"))));
        }

        compilationUnit = compilationUnit
            .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDecl))
            .NormalizeWhitespace();

        // Apply header AFTER NormalizeWhitespace (which would strip it otherwise)
        compilationUnit = compilationUnit.WithLeadingTrivia(CreateFileHeader());

        return compilationUnit;
    }

    private static SyntaxTriviaList CreateFileHeader() {
        // Use pre-formatted strings for preprocessor directives to ensure proper spacing
        // Roslyn's structured trivia doesn't preserve whitespace correctly after NormalizeWhitespace
        return TriviaList(
            Comment("// <auto-generated>"),
            CarriageReturnLineFeed,
            Comment("// This file was generated by RobloxTypeGenerator."),
            CarriageReturnLineFeed,
            Comment("// Do not edit this file manually."),
            CarriageReturnLineFeed,
            Comment("// </auto-generated>"),
            CarriageReturnLineFeed,
            CarriageReturnLineFeed,
            Trivia(ParseLeadingTrivia("#nullable enable\n").First().GetStructure() as NullableDirectiveTriviaSyntax
                ?? NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)),
            CarriageReturnLineFeed,
            Trivia(ParseLeadingTrivia("#pragma warning disable CS0108\n").First().GetStructure() as PragmaWarningDirectiveTriviaSyntax
                ?? PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), SingletonSeparatedList<ExpressionSyntax>(IdentifierName("CS0108")), true)),
            CarriageReturnLineFeed,
            CarriageReturnLineFeed
        );
    }

    private InterfaceDeclarationSyntax? GenerateClass(ApiClass cls) {
        // Filter members based on security level
        var filteredMembers = cls.Members
            .Where(m => ShouldGenerateMember(m, _securityLevel))
            .OrderBy(m => m.MemberType)
            .ThenBy(m => m.Name)
            .ToList();

        // Generate interface members
        var members = new List<MemberDeclarationSyntax>();
        foreach (var member in filteredMembers) {
            var memberSyntax = GenerateMember(cls.Name, member);
            if (memberSyntax != null) {
                members.Add(memberSyntax);
            }
        }

        // Create interface declaration
        string safeName = MakeSafeIdentifierString(cls.Name);
        var interfaceDecl = InterfaceDeclaration(safeName)
            .WithModifiers(TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.PartialKeyword)));

        // Add base type
        var baseType = GetBaseType(cls);
        if (baseType != null) {
            interfaceDecl = interfaceDecl.WithBaseList(
                BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(IdentifierName(baseType)))));
        }

        // Add members
        interfaceDecl = interfaceDecl.WithMembers(List(members));

        // Add documentation and attributes
        var leadingTrivia = GetClassDocComment(cls);

        // Add Obsolete attribute if deprecated
        if (cls.Tags.Contains("Deprecated")) {
            interfaceDecl = interfaceDecl.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This class is deprecated.")));
        }

        interfaceDecl = interfaceDecl.WithLeadingTrivia(leadingTrivia);

        return interfaceDecl;
    }

    /// <summary>
    /// Generates a plugin extension interface for a mixed class (has both None and Plugin members).
    /// The extension inherits from the base Roblox class and adds plugin-only members.
    /// </summary>
    private InterfaceDeclarationSyntax? GeneratePluginExtension(ApiClass cls) {
        // Only include plugin-security members
        var pluginMembers = cls.Members
            .Where(m => ShouldGenerateMember(m, SecurityLevel.PluginSecurity))
            .OrderBy(m => m.MemberType)
            .ThenBy(m => m.Name)
            .ToList();

        if (pluginMembers.Count == 0) return null;

        // Generate interface members
        var members = new List<MemberDeclarationSyntax>();
        foreach (var member in pluginMembers) {
            var memberSyntax = GenerateMember(cls.Name, member);
            if (memberSyntax != null) {
                members.Add(memberSyntax);
            }
        }

        if (members.Count == 0) return null;

        // Name: {ClassName}Plugin, inherits from base class in Roblox namespace
        string safeName = MakeSafeIdentifierString(cls.Name) + "Plugin";
        string baseTypeName = "global::Roblox." + MakeSafeIdentifierString(cls.Name);

        var interfaceDecl = InterfaceDeclaration(safeName)
            .WithModifiers(TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.PartialKeyword)))
            .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                SimpleBaseType(ParseTypeName(baseTypeName)))))
            .WithMembers(List(members))
            .WithLeadingTrivia(CreateDocComment($"Plugin extension for {cls.Name}. Provides plugin-only members."));

        // Add Obsolete attribute if deprecated
        if (cls.Tags.Contains("Deprecated")) {
            interfaceDecl = interfaceDecl.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This class is deprecated.")));
        }

        return interfaceDecl;
    }

    /// <summary>
    /// Generates a full plugin-only class (class has only Plugin security members, no None members).
    /// </summary>
    private InterfaceDeclarationSyntax? GeneratePluginOnlyClass(ApiClass cls) {
        // Only include plugin-security members
        var pluginMembers = cls.Members
            .Where(m => ShouldGenerateMember(m, SecurityLevel.PluginSecurity))
            .OrderBy(m => m.MemberType)
            .ThenBy(m => m.Name)
            .ToList();

        if (pluginMembers.Count == 0) return null;

        // Generate interface members
        var members = new List<MemberDeclarationSyntax>();
        foreach (var member in pluginMembers) {
            var memberSyntax = GenerateMember(cls.Name, member);
            if (memberSyntax != null) {
                members.Add(memberSyntax);
            }
        }

        if (members.Count == 0) return null;

        // Name: {ClassName} (no Plugin suffix - these are new types, not extensions)
        // Inherits from Roblox.Instance
        string safeName = MakeSafeIdentifierString(cls.Name);

        var interfaceDecl = InterfaceDeclaration(safeName)
            .WithModifiers(TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.PartialKeyword)))
            .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                SimpleBaseType(ParseTypeName("global::Roblox.Instance")))))
            .WithMembers(List(members))
            .WithLeadingTrivia(GetClassDocComment(cls));

        // Add Obsolete attribute if deprecated
        if (cls.Tags.Contains("Deprecated")) {
            interfaceDecl = interfaceDecl.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This class is deprecated.")));
        }

        return interfaceDecl;
    }

    /// <summary>
    /// Generates a stub interface for a base type that has no None-security members
    /// but is required by subclasses in the inheritance chain.
    /// </summary>
    private InterfaceDeclarationSyntax GenerateStubInterface(ApiClass cls) {
        string safeName = MakeSafeIdentifierString(cls.Name);

        var interfaceDecl = InterfaceDeclaration(safeName)
            .WithModifiers(TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.PartialKeyword)));

        // Add base type
        var baseType = GetBaseType(cls);
        if (baseType != null) {
            interfaceDecl = interfaceDecl.WithBaseList(
                BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(IdentifierName(baseType)))));
        }

        // Add documentation
        interfaceDecl = interfaceDecl.WithLeadingTrivia(
            CreateDocComment($"Base type for {cls.Name} hierarchy."));

        // Add Obsolete attribute if deprecated
        if (cls.Tags.Contains("Deprecated")) {
            interfaceDecl = interfaceDecl.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This class is deprecated.")));
        }

        return interfaceDecl;
    }

    private string? GetBaseType(ApiClass cls) {
        if (!string.IsNullOrEmpty(cls.Superclass) &&
            cls.Superclass != "<<<ROOT>>>" &&
            _classNames.Contains(cls.Superclass)) {
            return MakeSafeIdentifierString(cls.Superclass);
        } else if (cls.Name != "Instance") {
            return "Instance";
        }
        return null;
    }

    private SyntaxTriviaList GetClassDocComment(ApiClass cls) {
        string docKey = $"@roblox/globaltype/{cls.Name}";
        _docs.TryGetValue(docKey, out var doc);

        string summary = doc != null && !string.IsNullOrEmpty(doc.Documentation)
            ? doc.Documentation
            : $"Represents the {cls.Name} class.";

        return CreateDocComment(summary);
    }

    private MemberDeclarationSyntax? GenerateMember(string className, ApiMember member) {
        return member.MemberType switch {
            "Property" => GenerateProperty(className, member),
            "Function" => GenerateFunction(className, member),
            "Event" => GenerateEvent(className, member),
            "Callback" => GenerateCallback(className, member),
            _ => null
        };
    }

    private PropertyDeclarationSyntax? GenerateProperty(string className, ApiMember member) {
        string typeName = MapType(member.ValueType);

        // Make class-typed properties nullable (like RobloxTS does)
        // This prevents null reference issues at runtime since these can return nil
        if (member.ValueType?.Category == "Class" && !typeName.EndsWith("?")) {
            typeName += "?";
        }

        var type = ParseTypeString(typeName);
        string safeName = MakeSafeIdentifierString(member.Name);

        // Check accessor security - setter may require higher security than getter
        string readSecurity = member.GetReadSecurity();
        string writeSecurity = member.GetWriteSecurity();

        bool canRead = !member.IsWriteOnly && CanAccessAtSecurityLevel(readSecurity, _securityLevel);
        bool canWrite = !member.IsReadOnly && CanAccessAtSecurityLevel(writeSecurity, _securityLevel);

        // Skip if neither accessor is available at this security level
        if (!canRead && !canWrite) {
            return null;
        }

        // Create accessors based on read/write permissions and security
        var accessors = new List<AccessorDeclarationSyntax>();
        if (canRead) {
            accessors.Add(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }
        if (canWrite) {
            accessors.Add(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }

        var property = PropertyDeclaration(type, Identifier(safeName))
            .WithAccessorList(AccessorList(List(accessors)));

        // Add documentation
        var docComment = GetPropertyDocComment(className, member);
        property = property.WithLeadingTrivia(docComment);

        // Add Obsolete attribute if deprecated
        if (member.IsDeprecated) {
            property = property.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This property is deprecated.")));
        }

        return property;
    }

    /// <summary>
    /// Checks if a member is accessible at the given security level.
    /// </summary>
    private static bool CanAccessAtSecurityLevel(string memberSecurity, SecurityLevel currentLevel) {
        return currentLevel switch {
            SecurityLevel.None => memberSecurity == "None",
            SecurityLevel.PluginSecurity => memberSecurity == "None" || memberSecurity == "PluginSecurity",
            _ => false
        };
    }

    private MethodDeclarationSyntax GenerateFunction(string className, ApiMember member) {
        string returnTypeName = MapType(member.ReturnType);
        var returnType = ParseTypeString(returnTypeName);
        string safeName = MakeSafeIdentifierString(member.Name);

        // Create parameters
        var parameters = member.Parameters
            .Select(p => Parameter(SafeIdentifier(p.Name))
                .WithType(ParseTypeString(MapType(p.Type))))
            .ToList();

        var method = MethodDeclaration(returnType, Identifier(safeName))
            .WithParameterList(ParameterList(SeparatedList(parameters)))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        // Add documentation
        var docComment = GetFunctionDocComment(className, member);
        method = method.WithLeadingTrivia(docComment);

        // Add Obsolete attribute if deprecated
        if (member.IsDeprecated) {
            method = method.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This method is deprecated.")));
        }

        return method;
    }

    private PropertyDeclarationSyntax GenerateEvent(string className, ApiMember member) {
        string signalType = BuildSignalType(member.Parameters);
        var type = ParseTypeString(signalType);
        string safeName = MakeSafeIdentifierString(member.Name);

        var property = PropertyDeclaration(type, Identifier(safeName))
            .WithAccessorList(AccessorList(SingletonList(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))));

        // Add documentation
        var docComment = GetEventDocComment(className, member);
        property = property.WithLeadingTrivia(docComment);

        // Add Obsolete attribute if deprecated
        if (member.IsDeprecated) {
            property = property.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This event is deprecated.")));
        }

        return property;
    }

    private PropertyDeclarationSyntax GenerateCallback(string className, ApiMember member) {
        string delegateType = BuildDelegateType(member.Parameters, member.ReturnType);
        var type = ParseTypeString(delegateType);
        string safeName = MakeSafeIdentifierString(member.Name);

        var property = PropertyDeclaration(type, Identifier(safeName))
            .WithAccessorList(AccessorList(List(new[] {
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            })));

        // Add documentation
        var docComment = GetCallbackDocComment(className, member);
        property = property.WithLeadingTrivia(docComment);

        // Add Obsolete attribute if deprecated
        if (member.IsDeprecated) {
            property = property.WithAttributeLists(
                SingletonList(ObsoleteAttribute("This callback is deprecated.")));
        }

        return property;
    }

    private SyntaxTriviaList GetPropertyDocComment(string className, ApiMember member) {
        string docKey = $"@roblox/globaltype/{className}.{member.Name}";
        _docs.TryGetValue(docKey, out var doc);

        string summary = doc != null && !string.IsNullOrEmpty(doc.Documentation)
            ? doc.Documentation
            : $"Gets or sets the {member.Name} property.";

        string? remarks = null;
        if (!string.IsNullOrEmpty(member.ThreadSafety)) {
            remarks = GetThreadSafetyDoc(member.ThreadSafety);
        }

        return CreateDocComment(summary, remarks);
    }

    private SyntaxTriviaList GetFunctionDocComment(string className, ApiMember member) {
        string docKey = $"@roblox/globaltype/{className}.{member.Name}";
        _docs.TryGetValue(docKey, out var doc);

        string summary = doc != null && !string.IsNullOrEmpty(doc.Documentation)
            ? doc.Documentation
            : $"{member.Name} method.";

        // Build parameters documentation
        var paramDocs = new List<(string name, string doc)>();
        if (doc?.Params != null) {
            foreach (var param in doc.Params) {
                paramDocs.Add((param.Name, param.Documentation));
            }
        }

        // Returns documentation
        string? returns = null;
        if (doc?.Returns != null && doc.Returns.Count > 0) {
            returns = doc.Returns[0].Documentation;
        }

        // Remarks (thread safety + yield)
        string? remarks = null;
        var remarksList = new List<string>();
        if (!string.IsNullOrEmpty(member.ThreadSafety)) {
            var threadDoc = GetThreadSafetyDoc(member.ThreadSafety);
            if (!string.IsNullOrEmpty(threadDoc)) {
                remarksList.Add(threadDoc);
            }
        }
        if (member.CanYield) {
            remarksList.Add("This method can yield.");
        }
        if (remarksList.Count > 0) {
            remarks = string.Join(" ", remarksList);
        }

        return CreateMethodDocComment(summary, paramDocs.Count > 0 ? paramDocs : null, returns, remarks);
    }

    private SyntaxTriviaList GetEventDocComment(string className, ApiMember member) {
        string docKey = $"@roblox/globaltype/{className}.{member.Name}";
        _docs.TryGetValue(docKey, out var doc);

        string summary = doc != null && !string.IsNullOrEmpty(doc.Documentation)
            ? doc.Documentation
            : $"The {member.Name} event.";

        return CreateDocComment(summary);
    }

    private SyntaxTriviaList GetCallbackDocComment(string className, ApiMember member) {
        string docKey = $"@roblox/globaltype/{className}.{member.Name}";
        _docs.TryGetValue(docKey, out var doc);

        string summary = doc != null && !string.IsNullOrEmpty(doc.Documentation)
            ? doc.Documentation
            : $"The {member.Name} callback.";

        return CreateDocComment(summary);
    }

    private static string BuildSignalType(List<ApiParameter> parameters) {
        if (parameters.Count == 0) {
            return "ScriptSignal";
        }

        var types = parameters.Select(p => MapType(p.Type)).ToList();
        return $"ScriptSignal<{string.Join(", ", types)}>";
    }

    private static string BuildDelegateType(List<ApiParameter> parameters, ApiValueType? returnType) {
        string ret = MapType(returnType);
        if (parameters.Count == 0) {
            return ret == "void" ? "Action" : $"Func<{ret}>";
        }

        var types = parameters.Select(p => MapType(p.Type)).ToList();
        if (ret == "void") {
            return $"Action<{string.Join(", ", types)}>";
        }
        types.Add(ret);
        return $"Func<{string.Join(", ", types)}>";
    }
}
