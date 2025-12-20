using System.Text.Json;
using RobloxTypeGenerator;
using RobloxTypeGenerator.Generators;

const string ApiDumpUrl = "https://raw.githubusercontent.com/MaximumADHD/Roblox-Client-Tracker/roblox/Mini-API-Dump.json";
const string ApiDocsUrl = "https://raw.githubusercontent.com/MaximumADHD/Roblox-Client-Tracker/roblox/api-docs/en-us.json";
const int CacheMaxAgeHours = 24;

// Parse command line args
bool forceRefresh = args.Contains("--force") || args.Contains("-f");

Console.WriteLine("Roblox Type Generator");
Console.WriteLine("=====================");
Console.WriteLine();

// Determine output paths
string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
string robloxTypesDir = Path.Combine(baseDir, "RobloxTypes", "Generated");
string robloxTypesPluginDir = Path.Combine(baseDir, "RobloxTypes.Plugin", "Generated");
string metadataDir = Path.Combine(baseDir, "RobloxMetadata");
string cacheDir = Path.Combine(baseDir, "RobloxTypeGenerator", ".cache");

// Create directories if needed
Directory.CreateDirectory(robloxTypesDir);
Directory.CreateDirectory(robloxTypesPluginDir);
Directory.CreateDirectory(metadataDir);
Directory.CreateDirectory(cacheDir);

// Helper function for cached fetching
var httpClient = new HttpClient();

async Task<string> FetchWithCacheAsync(string url, string cacheFileName) {
    string cachePath = Path.Combine(cacheDir, cacheFileName);

    // Check if cache exists and is fresh (unless force refresh)
    if (!forceRefresh && File.Exists(cachePath)) {
        var cacheAge = DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath);
        if (cacheAge.TotalHours < CacheMaxAgeHours) {
            string cachedContent = await File.ReadAllTextAsync(cachePath);
            Console.WriteLine($"  Using cached {cacheFileName} (age: {cacheAge.TotalHours:F1}h)");
            return cachedContent;
        }
        Console.WriteLine($"  Cache expired for {cacheFileName} (age: {cacheAge.TotalHours:F1}h)");
    }

    // Fetch fresh content
    string content = await httpClient.GetStringAsync(url);
    await File.WriteAllTextAsync(cachePath, content);
    Console.WriteLine($"  Downloaded {content.Length:N0} bytes -> cached to {cacheFileName}");
    return content;
}

// Fetch data (with caching)
Console.WriteLine("Fetching API dump...");
string apiDumpJson = await FetchWithCacheAsync(ApiDumpUrl, "Mini-API-Dump.json");

Console.WriteLine("Fetching API docs...");
string apiDocsJson = await FetchWithCacheAsync(ApiDocsUrl, "en-us.json");

// Parse data
Console.WriteLine("Parsing API dump...");
var apiDump = JsonSerializer.Deserialize<ApiDump>(apiDumpJson, new JsonSerializerOptions {
    PropertyNameCaseInsensitive = true
});
if (apiDump == null) {
    Console.Error.WriteLine("Failed to parse API dump");
    return 1;
}
Console.WriteLine($"  Found {apiDump.Classes.Count} classes, {apiDump.Enums.Count} enums");

Console.WriteLine("Parsing API docs...");
var apiDocs = JsonSerializer.Deserialize<Dictionary<string, ApiDocEntry>>(apiDocsJson, new JsonSerializerOptions {
    PropertyNameCaseInsensitive = true
}) ?? new();
Console.WriteLine($"  Found {apiDocs.Count} documentation entries");

// Generate enums (using Roslyn AST)
Console.WriteLine();
Console.WriteLine("Generating enums...");
var enumGenerator = new EnumGenerator(apiDump.Enums, apiDocs);
var enumsSyntax = enumGenerator.Generate();
string enumsCode = enumsSyntax.ToFullString();
string enumsPath = Path.Combine(robloxTypesDir, "Enums.cs");
await File.WriteAllTextAsync(enumsPath, enumsCode);
Console.WriteLine($"  Generated {enumsPath}");

// Generate classes (None security) - using Roslyn AST
Console.WriteLine("Generating classes (None security)...");
var classGenerator = new ClassGenerator(apiDump.Classes, apiDocs, SecurityLevel.None);
var classesSyntax = classGenerator.Generate();
string classesCode = classesSyntax.ToFullString();
string classesPath = Path.Combine(robloxTypesDir, "Classes.cs");
await File.WriteAllTextAsync(classesPath, classesCode);
Console.WriteLine($"  Generated {classesPath}");

// Generate classes (PluginSecurity) - skip classes already generated at None level
Console.WriteLine("Generating classes (PluginSecurity)...");
var pluginClassGenerator = new ClassGenerator(apiDump.Classes, apiDocs, SecurityLevel.PluginSecurity, classGenerator.GeneratedClasses);
var pluginClassesSyntax = pluginClassGenerator.Generate();
string pluginClassesCode = pluginClassesSyntax.ToFullString();
string pluginClassesPath = Path.Combine(robloxTypesPluginDir, "PluginClasses.cs");
await File.WriteAllTextAsync(pluginClassesPath, pluginClassesCode);
Console.WriteLine($"  Generated {pluginClassesPath}");

// Generate metadata (None security)
Console.WriteLine("Generating metadata (None security)...");
var metadataGenerator = new MetadataGenerator(apiDump.Classes, SecurityLevel.None);
string metadataXml = metadataGenerator.Generate();
string metadataPath = Path.Combine(metadataDir, "Roblox.Generated.xml");
await File.WriteAllTextAsync(metadataPath, metadataXml);
Console.WriteLine($"  Generated {metadataPath}");

// Generate metadata (PluginSecurity)
Console.WriteLine("Generating metadata (PluginSecurity)...");
var pluginMetadataGenerator = new MetadataGenerator(apiDump.Classes, SecurityLevel.PluginSecurity);
string pluginMetadataXml = pluginMetadataGenerator.Generate();
string pluginMetadataPath = Path.Combine(metadataDir, "Roblox.Plugin.Generated.xml");
await File.WriteAllTextAsync(pluginMetadataPath, pluginMetadataXml);
Console.WriteLine($"  Generated {pluginMetadataPath}");

Console.WriteLine();
Console.WriteLine("Generation complete!");
Console.WriteLine($"  Version: {apiDump.Version}");

return 0;
