using System.Collections.Generic;

public static class UnsafePatterns
{
    // List of unsafe methods and their corresponding replacements
    public static readonly Dictionary<string, string> Patterns = new Dictionary<string, string>
    {
        { "Thread.Sleep", "// Removed unsafe sleep call" },
        { "Console.ReadLine", "// Removed unsafe blocking input" },
        { "File.", "// Removed unsafe file access" },
        { "Process.Start", "// Removed unsafe external process call" },
        { "Environment.Exit", "// Removed unsafe application exit" },
        { "GC.Collect", "// Removed unsafe garbage collection" },
        { "HttpClient", "// Removed unsafe network call" },
        { "GetStringAsync", "// Removed unsafe async network operation" }
    };

    // Helper method to check if a statement matches an unsafe pattern
    public static bool IsUnsafe(string expression, out string replacement)
    {
        foreach (var pattern in Patterns)
        {
            if (expression.Contains(pattern.Key))
            {
                replacement = pattern.Value;
                return true;
            }
        }

        replacement = null;
        return false;
    }
}
