﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PackingLib
{
    public class PackingProject
    {
        public PackingProject()
        {
           
        }
        public List<string> GetPythonInstallation()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"cmd.exe"; // Specify exe name.
            start.Arguments = "/c where python";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = start;

            List<string> pythonInstallations = new List<string>();
            try
            {
                
                process.Start();
                process.WaitForExit();
                using (var reader = process.StandardOutput)
                {
                    while (reader.Peek() >= 0)
                    {
                        pythonInstallations.Add(reader.ReadLine());
                    }
                }
                process.Dispose();
            }
            catch(Exception ex)
            {
                string err = ex.Message;
            }
            return pythonInstallations;
        }

        public Task InstallRequirements(string pythonLocation, string pythonLib)
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine("\n.............................................");
                Console.WriteLine("Packages installation has started\n");
                ProcessStartInfo start = new ProcessStartInfo();
                string requirementsLocation = Path.Combine(pythonLib, "requirements.txt");
                start.WorkingDirectory = pythonLocation;
                start.FileName = "cmd.exe";
                start.Arguments = $@"/c python -m pip install -r {requirementsLocation}";
                start.UseShellExecute = false;
                try
                {
                    Process process = Process.Start(start);
                    process.WaitForExit();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Please, send what you see in the console to Stewie so\nwe can help you install the packages.");
                }
                finally
                {
                    Console.WriteLine("\nPackages has been correctly installed");
                }
                    
            });
        }
        public Task CallPython(string pythonLocation,string pythonLib, string game, string action, string args, string message)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Console.WriteLine("\n.............................................");
                    Console.WriteLine($"{message} has started\n");
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = "cmd.exe";
                    string pythonLoc = Path.Combine(pythonLocation, "python.exe");
                    string toolsExecutable = Path.Combine(pythonLib, "ToolsTales_Executable.py");
                    start.WorkingDirectory = pythonLib;
                    start.Arguments = $@"/c {pythonLoc} {toolsExecutable} --game {game} {action} {args}";
                    start.UseShellExecute = false;// Do not use OS shell
                    //start.CreateNoWindow = true; // We don't need new window
                    //start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
                    //start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
                    Process process = Process.Start(start);
                    process.WaitForExit();
                    Console.WriteLine($"\n{message} has completed");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
