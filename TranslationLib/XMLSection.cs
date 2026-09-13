using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace TranslationLib
{
    public class XMLSection
    {
        public string Name { get; set; }
        public List<XMLEntry> Entries { get; set; }

        public XMLSection(string name)
        {

            if (name.StartsWith("1"))
                Name = "2" + name.Substring(1);
            else
                Name = name;
            Entries = new List<XMLEntry>();
        }

        public Dictionary<string, int> GetStatusData(string chapter)
        {
            return new Dictionary<string, int>
            {
                { "To Do", GetEntryCountByStatus("To Do", chapter) },
                { "Translated", GetEntryCountByStatus("Translated", chapter) },
                { "Edited", GetEntryCountByStatus("Edited", chapter) },
                { "Spaced", GetEntryCountByStatus("Spaced", chapter) },
                { "Finalized", GetEntryCountByStatus("Finalized", chapter) },
                { "Problematic", GetEntryCountByStatus("Problematic", chapter) },
                { "Done", GetEntryCountByStatus("Done", chapter) },
            };
        }

        private int GetEntryCountByStatus(string status, string chapter)
        {
            if (chapter == "All chapters")
                return Entries.Count(e => e.Status == status);
            else
                return Entries.Count(e => e.Status == status && e.Chapter == chapter);
        }

        public List<string> GetChapterNames()
        {
            List<string> l = Entries.Select(s => s.Chapter).Where(s => s != "All strings").Distinct().ToList();
            l.Insert(0, "All chapters");
            return l;
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
                    entry.Id = Entries[index].Id ?? default(int);
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