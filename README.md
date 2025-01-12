Having a Unity GameObject call RoslynTest.TestScriptProcessing() will start processing of the script by removing potentially unsafe operations that we don't want to have in AI-generated code.

Original Script:

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
            File.WriteAllText("data.txt", "Some data"); // Unsafe: File access
            var content = File.ReadAllText("data.txt"); // Unsafe: File access
        }

        public void StartProcess()
        {
            Process.Start("notepad.exe"); // Unsafe: External process call
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
            return "All executions performed!";
        }
    }

Cleaned Script:

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
        }

        public void FileOperations()
        {
        }

        public void StartProcess()
        {
        }

        public void BlockUserInput()
        {
        }

        public void ExitApplication()
        {
        }

        public void ForceGarbageCollection()
        {
        }

        public string Execute()
        {
            PauseExecution();
            FileOperations();
            StartProcess();
            BlockUserInput();
            ExitApplication();
            ForceGarbageCollection();
            return "All executions performed!";
        }
    }
