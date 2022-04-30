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
        public static List<EntryElement> getEntryElements(TOPXSceneText storyText)
        {
            List<EntryElement> res = new List<EntryElement>();
            List<Entry> entries = new List<Entry>();
            List<Struct> listStruct = storyText.Struct;
            string pointerOffset = "";

            foreach (Struct ele in listStruct)
            {

                pointerOffset = ele.PointerOffset;
                entries = ele.Entries;
                foreach (Entry entry in entries)
                {
                    res.Add(new EntryElement(entry));
                }
            }
            return res;

        }

        public static List<EntryElement> getEntryElements(TalesFile storyText, string section="")
        {

            List<EntryElement> res = new List<EntryElement>();
            List<Entry> entries = new List<Entry>();

            List<Strings> listStrings = storyText.Strings;
            if (section != "")
            {
                listStrings = storyText.Strings.Where(x => x.Section == section).ToList();
            }
           

            
            foreach (Strings str in listStrings)
            {
                foreach (Entry ele in str.Entries)
                {
                    res.Add(new EntryElement(ele));

                }
            }
            return res;

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

    public class EntryElement
    {
        public EntryElement(Entry Entry)
        {
            this.Entry = Entry;

        }
        public EntryElement() {}
        public Entry Entry{ get; set;}


        public string DisplayText
        {

            get { return $"{this.Entry.JapaneseText}"; }
        }

    }


}
