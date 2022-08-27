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
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public List<XMLSection> Sections = new List<XMLSection>();
        public XMLSection CurrentSection { get; set; }

        public XMLFile()
        {
            CurrentSection = new XMLSection("Empty");
        }
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
            return Sections.Select(s => s.Name).OrderBy(s => s).ToList();
        }

        public void SaveToDisk()
        {
            var sectionsElements = Sections.Select(GetXmlSectionElement);
            var document = new XDocument(
                new XElement(GetXMLTextTagName(), sectionsElements)
            );

            File.WriteAllText(FilePath, document.ToString().Replace(" />", "/>") + Environment.NewLine);
        }

        private string GetXMLTextTagName()
        {
            if (FileType == "Menu")
                return "MenuText";

            return "SceneText";
        }

        private XElement GetXmlSectionElement(XMLSection section)
        {
            var sectionEntry = new List<XElement>
            {
                new XElement("Section", section.Name),
            };

            sectionEntry.AddRange(section.Entries.Select(entry => GetXMLEntryElement(entry)).ToList());

            return new XElement("Strings", sectionEntry);
        }

        private static XElement GetXMLEntryElement(XMLEntry entry)
        {
            var elemenId = entry.Id == null ? null : new XElement("Id", entry.Id);
            return new XElement("Entry",
                new XElement("PointerOffset", entry.PointerOffset),
                new XElement("JapaneseText", entry.JapaneseText),
                new XElement("EnglishText", entry.EnglishText),
                new XElement("Notes", string.IsNullOrEmpty(entry.Notes) ? null : entry.Notes),
                elemenId,
                new XElement("Status", entry.Status)
            );
        }
    }
}