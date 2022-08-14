using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TranslationLib
{
    public class XMLFile
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public List<XMLSection> Sections = new List<XMLSection>();
        public XMLSection CurrentSection { get; set; }

        public void SetSection(string name)
        {
            CurrentSection = Sections.First(c => c.Name == name);
        }

        public Dictionary<string, int> GetStatusData()
        {
            var dictionary = new Dictionary<string, int>()
            {
                { "To Do", 0 },
                { "Proofreading", 0 },
                { "In Review", 0 },
                { "Problematic", 0 },
                { "Done", 0 },
            };

            foreach (var section in Sections)
            {
                var sectionDictionary = section.GetStatusData();
                foreach (var key in sectionDictionary.Keys)
                {
                    dictionary[key] += sectionDictionary[key];
                }
            }

            return dictionary;
        }

        public List<string> GetSectionNames()
        {
            return Sections.Select(s => s.Name).ToList();
        }

        public void SaveToDisk()
        {
            var document = new XDocument(
                new XElement("SceneText",
                    new XElement("Strings",
                        GetXmlSectionElement(Sections.First()))
                )
            );

            File.WriteAllText(FilePath, document.ToString().Replace(" />", "/>") + Environment.NewLine);
        }

        private List<XElement> GetXmlSectionElement(XMLSection section)
        {
            var sectionEntry = new List<XElement>
            {
                new XElement("Section", section.Name),
            };

            sectionEntry.AddRange(section.Entries.Select(entry => GetXMLEntryElement(entry)).ToList());

            return sectionEntry;
        }

        private static XElement GetXMLEntryElement(XMLEntry entry)
        {
            return new XElement("Entry",
                new XElement("PointerOffset", entry.PointerOffset),
                new XElement("JapaneseText", entry.JapaneseText),
                new XElement("EnglishText", entry.EnglishText),
                new XElement("Notes", string.IsNullOrEmpty(entry.Notes) ? null : entry.Notes),
                new XElement("Id", entry.Id),
                new XElement("Status", entry.Status)
            );
        }
    }
}