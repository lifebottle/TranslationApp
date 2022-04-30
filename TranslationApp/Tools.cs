using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TranslationApp
{
    public static class Tools
    {
        static string workingDirectory = "../../../../PythonLib_Playground/PythonLib";
        public static List<Entry> getEntries(TalesFile file, string section="")
        {
            List<Entry> entries = new List<Entry>();

            List<Strings> listStrings = file.Strings;
            if (section != "")
            {
                entries = file.Strings.Where(x => x.Section == section).FirstOrDefault().Entries;
            }
           

            return entries;
        }

        public static void insertFile()
        {

        }

        public static string callFunction(string gameName, string action, string args)
        {
            string result = "";
            ProcessStartInfo start = new ProcessStartInfo();
            start.WorkingDirectory = workingDirectory;
            start.FileName = "python.exe";
            string cmd = "ToolsTales_Executable.py";
            start.Arguments = string.Format("{0} --game {1} {2} {3}", cmd, gameName, action, args);
            Console.WriteLine(start.Arguments);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();

                    byte[] bytes = Encoding.Default.GetBytes(result);
                    result = Encoding.UTF8.GetString(bytes);

                }
            }

            return result;
        }
        public static string hexToJap(string gameName, string hex)
        {
            

            string res = callFunction(gameName, "utility", "hex2bytes " + $@"""{ hex}"" 0");
            string text = System.IO.File.ReadAllText("../../../../PythonLib_Playground/PythonLib/text_dump.txt");
            return text;
        }

        public static string massReplace()
        {
            return "";
        }

    }


}
