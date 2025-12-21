using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RobloxValidation;

/// <summary>
/// Validation runner for Roblox type transpilation tests.
/// Compiles C# test files to Lua and compares against expected output.
/// </summary>
public class Program {
    private static readonly string ProjectRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    public static int Main(string[] args) {
        Console.WriteLine("=== Roblox Validation Runner ===");
        Console.WriteLine($"Project root: {ProjectRoot}");
        Console.WriteLine();

        var testProjectDir = Path.Combine(ProjectRoot, "RobloxValidation", "TestProject");
        var outputDir = Path.Combine(ProjectRoot, "RobloxValidation", "Output");
        var expectedDir = Path.Combine(ProjectRoot, "RobloxValidation", "Expected");

        // Clean output directory
        if (Directory.Exists(outputDir)) {
            Directory.Delete(outputDir, true);
        }
        Directory.CreateDirectory(outputDir);

        // Run transpiler
        Console.WriteLine("Running transpiler...");
        var exitCode = RunTranspiler(testProjectDir, outputDir);
        if (exitCode != 0) {
            Console.WriteLine($"ERROR: Transpiler failed with exit code {exitCode}");
            return 1;
        }
        Console.WriteLine("Transpiler completed successfully.");
        Console.WriteLine();

        // Compare outputs
        Console.WriteLine("Comparing outputs...");
        var failures = new List<string>();
        var successes = 0;

        var expectedFiles = Directory.GetFiles(expectedDir, "*.lua")
            .Where(f => !f.EndsWith("manifest.lua"))
            .ToArray();

        foreach (var expectedFile in expectedFiles) {
            var fileName = Path.GetFileName(expectedFile);
            var actualFile = Path.Combine(outputDir, fileName);

            if (!File.Exists(actualFile)) {
                Console.WriteLine($"  FAIL: {fileName} - Output file not found");
                failures.Add(fileName);
                continue;
            }

            var expected = NormalizeContent(File.ReadAllText(expectedFile));
            var actual = NormalizeContent(File.ReadAllText(actualFile));

            if (expected == actual) {
                Console.WriteLine($"  PASS: {fileName}");
                successes++;
            } else {
                Console.WriteLine($"  FAIL: {fileName} - Content mismatch");
                failures.Add(fileName);

                // Show diff for debugging
                Console.WriteLine("  --- Expected (first 500 chars) ---");
                Console.WriteLine("  " + expected.Substring(0, Math.Min(500, expected.Length)).Replace("\n", "\n  "));
                Console.WriteLine("  --- Actual (first 500 chars) ---");
                Console.WriteLine("  " + actual.Substring(0, Math.Min(500, actual.Length)).Replace("\n", "\n  "));
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Results: {successes} passed, {failures.Count} failed");

        if (failures.Count > 0) {
            Console.WriteLine();
            Console.WriteLine("Failed tests:");
            foreach (var failure in failures) {
                Console.WriteLine($"  - {failure}");
            }
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine("All validation tests passed!");
        return 0;
    }

    private static int RunTranspiler(string sourceDir, string outputDir) {
        var launcherProject = Path.Combine(ProjectRoot, "CSharp.lua.Launcher", "CSharp.lua.Launcher.csproj");
        var robloxTypesLib = Path.Combine(ProjectRoot, "RobloxTypes", "bin", "Debug", "net10.0", "RobloxTypes.dll");
        var metadataFile = Path.Combine(ProjectRoot, "RobloxMetadata", "Roblox.xml");
        var generatedMetadata = Path.Combine(ProjectRoot, "RobloxMetadata", "Roblox.Generated.xml");

        // Multiple metadata files are separated with semicolons
        var metadataPaths = $"\"{metadataFile}\";\"{generatedMetadata}\"";
        var arguments = $"run --project \"{launcherProject}\" -- " +
            $"-s \"{sourceDir}\" " +
            $"-d \"{outputDir}\" " +
            $"-l \"{robloxTypesLib}\" " +
            $"-m {metadataPaths} " +
            $"-roblox -namespace RobloxValidation.Runtime";

        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = ProjectRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(output)) {
            Console.WriteLine(output);
        }
        if (!string.IsNullOrEmpty(error)) {
            Console.WriteLine(error);
        }

        return process.ExitCode;
    }

    private static string NormalizeContent(string content) {
        // Normalize line endings
        content = content.Replace("\r\n", "\n").Replace("\r", "\n");

        // Remove trailing whitespace from each line
        content = Regex.Replace(content, @"[ \t]+$", "", RegexOptions.Multiline);

        // Normalize multiple blank lines to single blank line
        content = Regex.Replace(content, @"\n{3,}", "\n\n");

        return content.Trim();
    }
}
