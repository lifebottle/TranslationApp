using System.Collections.Generic;
using System.Linq;

namespace TranslationLib
{
    public class XMLSection
    {
        public string Name { get; set; }
        public List<XMLEntry> Entries { get; set; }

        public XMLSection(string name)
        {
            Name = name;
            Entries = new List<XMLEntry>();
        }

        public Dictionary<string, int> GetStatusData()
        {
            return new Dictionary<string, int>
            {
                { "To Do", GetEntryCountByStatus("To Do") },
                { "Edited", GetEntryCountByStatus("Editing") },
                { "Proofread", GetEntryCountByStatus("Proofreading") },
                { "Problematic", GetEntryCountByStatus("Problematic") },
                { "Done", GetEntryCountByStatus("Done") },
            };
        }

        private int GetEntryCountByStatus(string status)
        {
            return Entries.Count(e => e.Status == status);
        }

        public List<Dictionary<string, string>> SearchJapanese(string folder, int fileId, string sectionName, string text, bool exactMatch, string language)
        {
            List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
            List<int> foundIndexes;
            foundIndexes = Enumerable.Range(0, Entries.Count)
                    .Where(e => Entries[e].IsFound(text, exactMatch, language))
                    .ToList();
    
            if (foundIndexes.Count > 0)
            {
              
                foreach (int index in foundIndexes)
                {
                    Dictionary<string, string> foundEntries = new Dictionary<string, string>();
                    foundEntries["Folder"] = folder;
                    foundEntries["FileId"] = fileId.ToString();
                    foundEntries["Section"] = sectionName;
                    foundEntries["Id"] = index.ToString();
                    foundEntries["JapaneseText"] = Entries[index].JapaneseText;
                    foundEntries["EnglishText"] = Entries[index].EnglishText;
                    foundEntries["Status"] = Entries[index].Status;
                    res.Add(foundEntries);
                }
                
            }
            return res;
        }
    }
}