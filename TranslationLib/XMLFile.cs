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
        public List<XMLEntry> Speakers = new List<XMLEntry>();
        public XMLSection CurrentSection { get; set; }

        public XMLFile()
        {
            CurrentSection = new XMLSection("Default");
        }
        public void SetSection(string name)
        {
            CurrentSection = Sections.First(c => c.Name == name);
        }

        public void UpdateAllEntryText()
        {
            var keys = Speakers.Select(x => x.Id).ToList();
            var values = Speakers.Select(x => !string.IsNullOrEmpty(x.EnglishText) ? x.EnglishText : x.JapaneseText).ToList();
            var speakerDict = keys.Zip(values, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);

            foreach (var XMLSection in Sections.Where(x=>x.Entries.Where(y => y.SpeakerId != null).Count() > 0))
            {
                foreach (var XMLEntry in XMLSection.Entries)
                {
                    List<string> ls = new List<string>();
           
                    if (XMLEntry.SpeakerId != null)
                    {
                        foreach (var id in XMLEntry.SpeakerId)
                        {
                            ls.Add(speakerDict[id]);
                        }

                        XMLEntry.SpeakerName = string.Join(" / ", ls);
                    }
                    else             
                        XMLEntry.SpeakerName = null;                  
                }
            }
        }

        public Dictionary<string, int> GetStatusData()
        {
            var dictionary = new Dictionary<string, int>()
            {
                { "To Do", 0 },
                { "Editing", 0 },
                { "Proofreading", 0 },
                { "Problematic", 0 },
                { "Done", 0 },
            };

            foreach (var section in Sections)
            {
                if (section.Name != "Other Strings")
                {
                    var sectionDictionary = section.GetStatusData();
                    foreach (var key in sectionDictionary.Keys)
                    {
                        dictionary[key] += sectionDictionary[key];
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, int> SpeakersGetStatusData()
        {
            Func<List<XMLEntry>, string, int> CountEntryByStatus = (entryList, status) => entryList.Count(e => e.Status == status);

            return new Dictionary<string, int>
            {
                { "To Do",          CountEntryByStatus(Speakers,"To Do") },
                { "Editing",        CountEntryByStatus(Speakers,"Editing") },
                { "Proofreading",   CountEntryByStatus(Speakers,"Proofreading") },
                { "Problematic",    CountEntryByStatus(Speakers,"Problematic") },
                { "Done",           CountEntryByStatus(Speakers,"Done") },
            };
        }

        public List<string> GetSectionNames()
        {
            return Sections.Select(s => s.Name).OrderBy(s => s).ToList();
        }

        public void SaveToDisk()
        {
            
            var sectionsElements = Sections.Select(GetXmlSectionElement);
            List<XElement> allSections = new List<XElement>();

            if (FileType != "Menu")
            {
                var speakerElements = GetXmlSpeakerElement(Speakers);
                allSections.Add(speakerElements);
            }
           
            
            allSections.AddRange(sectionsElements);
            var document = new XDocument(
                new XElement(GetXMLTextTagName(), allSections)
            );

            File.WriteAllText(FilePath, document.ToString().Replace(" />", "/>") + Environment.NewLine);
        }

        private string GetXMLTextTagName()
        {
            if (FileType == "Menu")
                return "MenuText";

            return "SceneText";
        }
        
        private XElement GetXmlSpeakerElement(List<XMLEntry> SpeakerList)
        {
            var speakerEntry = new List<XElement>
            {
                new XElement("Section", "Speaker"),
            };

            speakerEntry.AddRange( SpeakerList.Select(entry => GetXMLEntryElement(entry)).ToList());

            return new XElement("Speakers", speakerEntry);
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
            var structId = entry.StructId == null ? null : new XElement("StructId", entry.StructId);
            var speakerId = entry.SpeakerId == null ? null : new XElement("SpeakerId", entry.SpeakerId);
            var voiceId = entry.VoiceId == null ? null : new XElement("VoiceId", entry.VoiceId);
            var unknownPointer = entry.UnknownPointer == null ? null : new XElement("UnknownPointer", entry.UnknownPointer);
            return new XElement("Entry",
                new XElement("PointerOffset", entry.PointerOffset),
                voiceId,
                new XElement("JapaneseText", entry.JapaneseText),
                new XElement("EnglishText", string.IsNullOrEmpty(entry.EnglishText) ? null : entry.EnglishText),
                new XElement("Notes", string.IsNullOrEmpty(entry.Notes) ? null : entry.Notes),
                elemenId,
                structId,
                speakerId,
                unknownPointer,
                new XElement("Status", entry.Status)
                
            );
        }
    }
}