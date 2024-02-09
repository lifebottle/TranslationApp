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

        public List<EntryFound> SearchJapanese(string folder, int fileId, string sectionName, string text, bool matchWholeEntry, bool matchCase, bool matchWholeWord, string language)
        {
            List<EntryFound> res = new List<EntryFound>();
            List<int> foundIndexes;
            foundIndexes = Enumerable.Range(0, Entries.Count)
                    .Where(e => Entries[e].IsFound(text, matchWholeEntry, matchCase, matchWholeWord, language))
                    .ToList();
    
            if (foundIndexes.Count > 0)
            {
              
                foreach (int index in foundIndexes)
                {
                    EntryFound entry = new EntryFound();
                    entry.Folder = folder;
                    entry.FileId = fileId;
                    entry.Section = sectionName;
                    entry.Id = index;
                    entry.Entry = new XMLEntry();
                    entry.Entry.JapaneseText = Entries[index].JapaneseText;
                    entry.Entry.EnglishText = Entries[index].EnglishText;
                    entry.Entry.SpeakerId = Entries[index].SpeakerId;
                    entry.Entry.SpeakerName = Entries[index].SpeakerName;
                    entry.Entry.Status = "To Do";
                    res.Add(entry);
                }
                
            }
            return res;
        }
    }
}