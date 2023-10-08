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
    }
}