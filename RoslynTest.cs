using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Linq;
using UnityEngine;

public class RoslynTest
{
    public static void TestSyntaxTree()
    {
        // Test syntax tree parsing
        var tree = CSharpSyntaxTree.ParseText(@"
            using System;
            public class Test
            {
                public void HelloWorld()
                {
                    Console.WriteLine(""Hello, World!"");
                }
            }
        ");

        // Parse the syntax tree and get the root
        var root = tree.GetRoot() as CompilationUnitSyntax;

        // Log the parsed syntax tree
        Debug.Log("Parsed class:\n" + root?.ToFullString());
    }

    public static void TestScriptProcessing()
    {
        // Complex script with potentially unsafe operations
        string scriptCode = @"
    using System;
    using System.Net.Http;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;
    using UnityEngine;

    public class UnsafeExample
    {            
        public void PauseExecution()
        {
            Thread.Sleep(1000); // Unsafe: Freezes the thread for 1 second
        }

        public void FileOperations()
        {
            File.WriteAllText(""data.txt"", ""Some data""); // Unsafe: File access
            var content = File.ReadAllText(""data.txt""); // Unsafe: File access
        }

        public void StartProcess()
        {
            Process.Start(""notepad.exe""); // Unsafe: External process call
        }

        public void BlockUserInput()
        {
            Console.ReadLine(); // Unsafe: Blocking call for user input
        }

        public void ExitApplication()
        {
            Environment.Exit(0); // Unsafe: Abruptly terminates the application
        }

        public void ForceGarbageCollection()
        {
            GC.Collect(); // Unsafe: Forcing garbage collection
        }

        public string Execute()
        {
            PauseExecution();
            FileOperations();
            StartProcess();
            BlockUserInput();
            ExitApplication();
            ForceGarbageCollection();
            return ""All executions performed!"";
        }
    }
    ";

        Debug.Log("Original Script:\n" + scriptCode);

        try
        {
            // Process the script using RuntimeScriptProcessor
            var compiledDelegate = RuntimeScriptProcessor.ProcessAndCompileScript(scriptCode);

            if (compiledDelegate != null)
            {
                // Execute the processed script
                var result = compiledDelegate.Invoke();
                Debug.Log("Processed Script Result: " + result);
            }
            else
            {
                Debug.LogError("Script processing failed.");
            }
        }
        catch (CompilationErrorException ex)
        {
            Debug.LogError($"Compilation failed:\n{string.Join("\n", ex.Diagnostics)}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error during script processing: {ex.Message}\n{ex.StackTrace}");
        }
    }


}
