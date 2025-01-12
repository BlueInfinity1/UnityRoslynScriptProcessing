using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using UnityEngine;

public static class RuntimeScriptProcessor
{
    public static Func<object> ProcessAndCompileScript(string scriptCode)
    {
        try
        {
            Debug.Log("[Step 1] Running Regex Pre-Filter...");
            if (ContainsUnsafePatterns(scriptCode))
            {
                Debug.Log("[Step 1] Unsafe patterns detected via regex.");

                Debug.Log("[Step 2] Analyzing and Cleaning with Roslyn...");
                string cleanedCode = AnalyzeAndCleanCode(scriptCode);

                // Log the cleaned script here
                Debug.Log("Cleaned Script:\n" + cleanedCode);

                Debug.Log("[Step 3] Compiling Script...");
                return CompileScript(cleanedCode);
            }
            else
            {
                Debug.Log("[Step 1] No unsafe patterns detected via regex.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Script processing failed: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }



    private static bool ContainsUnsafePatterns(string code)
    {
        foreach (var pattern in UnsafePatterns.Patterns.Keys)
        {
            if (code.Contains(pattern))
            {
                Debug.Log($"[Regex Pre-Filter] Unsafe pattern detected: {pattern}");
                return true;
            }
        }
        return false;
    }


    private static string AnalyzeAndCleanCode(string code)
    {
        try
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var rewriter = new UnsafeCodeRewriter();
            var newRoot = rewriter.Visit(root);

            return newRoot.ToFullString();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during Roslyn analysis: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }


    private static Func<object> CompileScript(string cleanedCode)
    {
        try
        {
            Debug.Log("[Compilation] Starting compilation...");

            var scriptOptions = ScriptOptions.Default.AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),            // mscorlib
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),          // System.Console
                MetadataReference.CreateFromFile(typeof(UnityEngine.Debug).Assembly.Location), // UnityEngine
                MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location) // System.Net.Http
            ).WithImports("System", "System.Net.Http", "System.Threading");

            // Create and run the script
            var script = CSharpScript.Create(cleanedCode, scriptOptions);
            var scriptState = script.RunAsync().Result;

            Debug.Log("[Compilation] Compilation successful.");

            // Return the result of the script
            return () => scriptState.ReturnValue; // Top-level scripts return a result
        }
        catch (CompilationErrorException ex)
        {
            Debug.LogError("Compilation errors detected:");
            foreach (var diag in ex.Diagnostics)
            {
                Debug.LogError(diag.ToString());
            }
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Compilation failed: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }
}
