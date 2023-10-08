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
        public string FriendlyName { get; set; }
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
                { "Edited", 0 },
                { "Proofread", 0 },
                { "Problematic", 0 },
                { "Done", 0 },
            };

            foreach (var section in Sections)
            {
                if (section.Name != "Other Strings" && section.Name != "All strings")
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
                { "Edited",         CountEntryByStatus(Speakers,"Edited") },
                { "Proofread",      CountEntryByStatus(Speakers,"Proofread") },
                { "Problematic",    CountEntryByStatus(Speakers,"Problematic") },
                { "Done",           CountEntryByStatus(Speakers,"Done") },
            };
        }

        public List<string> GetSectionNames()
        {
            List<string> l = Sections.Select(s => s.Name).Where(s => s != "All strings").ToList();
            l.Insert(0, "All strings");
            return l;
        }

        public void SaveToDisk()
        {
            
            var sectionsElements = Sections.Where(s => s.Name != "All strings").Select(GetXmlSectionElement);
            List<XElement> allSections = new List<XElement>();

            if (FriendlyName != null)
            {
                allSections.Add(new XElement("FriendlyName", FriendlyName));
            }

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

        public void SaveAsCsv(string path)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write), new System.Text.UTF8Encoding(true)))
            {
                writer.WriteLine("File,Line Number,Section,Status,Speaker JP,Text JP,Speaker EN,Text EN,Comment");
                writer.WriteLine(
                            Name + ".xml" + "," +
                            "," +
                            "Friendly Name" + "," +
                            "," +
                            "," +
                            "\"" + (FriendlyName ?? "").Replace("\"", "\"\"") + "\"" + "," +
                            "," +
                            "\"" + (FriendlyName ?? "").Replace("\"", "\"\"") + "\"" + ","
                            );

                foreach (XMLEntry entry in Speakers)
                {
                    writer.WriteLine(
                            Name + ".xml" + "," +
                            entry.Id + "," +
                            "Speaker" + "," +
                            "," +
                            "," +
                            "\"" + (entry.JapaneseText ?? "").Replace("\"", "\"\"") + "\"" + "," +
                            "" + "," +
                            "\"" + (entry.EnglishText ?? "").Replace("\"", "\"\"") + "\"" + "," +
                            "\"" + entry.Notes + "\""
                            );
                }

                foreach (XMLSection section in Sections.Where(s => s.Name != "All strings"))
                {
                    foreach (XMLEntry entry in section.Entries)
                    {
                        List<string> en = new List<string>();
                        List<string> jp = new List<string>();

                        string en_name = "";
                        string jp_name = "";

                        if (entry.SpeakerId != null)
                        {
                            foreach (var id in entry.SpeakerId)
                            { 
                                if (!string.IsNullOrEmpty(Speakers[id].EnglishText))
                                    en.Add(Speakers[id].EnglishText);
                                if (!string.IsNullOrEmpty(Speakers[id].JapaneseText))
                                    jp.Add(Speakers[id].JapaneseText);
                            }
                            en_name = string.Join(",", entry.SpeakerId);
                            en_name += "[" + string.Join(" / ", en).Replace("\"", "\"\"") + "]";
                            jp_name = string.Join(",", entry.SpeakerId);
                            jp_name += "[" + string.Join(" / ", jp).Replace("\"", "\"\"") + "]";
                        }

                        string en_text = entry.EnglishText ?? "";
                        string jp_text = entry.JapaneseText ?? "";

                        writer.WriteLine(
                            Name + ".xml" + "," +
                            entry.Id + "," +
                            "\"" + section.Name.Replace("\"", "\"\"") + "\"" + "," +
                            entry.Status + "," +
                            "\"" + jp_name + "\"" + "," +
                            "\"" + jp_text.Replace("\"", "\"\"") + "\"" + "," +
                            "\"" + en_name + "\"" + "," +
                            "\"" + en_text.Replace("\"", "\"\"") + "\""  + "," +
                            "\"" + entry.Notes + "\""
                            );
                    }
                }
            }
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
            var speakerId = entry.SpeakerId == null ? null : new XElement("SpeakerId", string.Join(",",entry.SpeakerId));
            var voiceId = entry.VoiceId == null ? null : new XElement("VoiceId", entry.VoiceId);
            var unknownPointer = entry.UnknownPointer == null ? null : new XElement("UnknownPointer", entry.UnknownPointer);
            var maxLength = entry.MaxLength == null ? null : new XElement("MaxLength", entry.MaxLength);
            XElement embedOffset;

            if (entry.EmbedOffset)
            {
                var sectionEntry = new List<XElement>
                {
                    new XElement("hi", entry.hi),
                    new XElement("lo", entry.lo),
                };
                embedOffset = new XElement("EmbedOffset", sectionEntry);
            } else
            {
                embedOffset = null;
            }
            return new XElement("Entry",
                new XElement("PointerOffset", entry.PointerOffset),
                embedOffset,
                maxLength,
                voiceId,
                new XElement("JapaneseText", entry.JapaneseText),
                new XElement("EnglishText", entry.EnglishText),
                new XElement("Notes", string.IsNullOrEmpty(entry.Notes) ? null : entry.Notes),
                speakerId,
                elemenId,
                structId,
                unknownPointer,
                new XElement("Status", entry.Status)

            );
        }
    }
}