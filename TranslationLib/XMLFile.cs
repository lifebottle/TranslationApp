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
        public List<XMLEntry> Speakers = null;
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

            List<XMLEntry> t = Speakers ?? new List<XMLEntry>();

            return new Dictionary<string, int>
            {
                { "To Do",          CountEntryByStatus(t,"To Do") },
                { "Edited",         CountEntryByStatus(t,"Edited") },
                { "Proofread",      CountEntryByStatus(t,"Proofread") },
                { "Problematic",    CountEntryByStatus(t,"Problematic") },
                { "Done",           CountEntryByStatus(t,"Done") },
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

            if (Speakers != null)
            {
                var speakerElements = GetXmlSpeakerElement(Speakers);
                allSections.Add(speakerElements);
            }

            allSections.AddRange(sectionsElements);
            var document = new XDocument(
                new XElement(FileType, allSections)
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

                Dictionary<int, string> en_names = new Dictionary<int, string>();
                Dictionary<int, string> jp_names = new Dictionary<int, string>();
                foreach (XMLEntry entry in Speakers)
                {
                    string en_name = (entry.EnglishText ?? "").Replace("\"", "\"\"");
                    string jp_name = (entry.JapaneseText ?? "").Replace("\"", "\"\"");
                    writer.WriteLine(
                            Name + ".xml" + "," +
                            entry.Id + "," +
                            "Speaker" + "," +
                            "," +
                            "," +
                            "\"" + jp_name + "\"" + "," +
                            "" + "," +
                            "\"" + en_name + "\"" + "," +
                            "\"" + entry.Notes + "\""
                            );

                    en_names.Add(entry.Id.Value, en_name);
                    jp_names.Add(entry.Id.Value, jp_name);
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
                                if (!string.IsNullOrEmpty(en_names[id]))
                                    en.Add(en_names[id]);
                                if (!string.IsNullOrEmpty(jp_names[id]))
                                    jp.Add(jp_names[id]);
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
            var bubbleId = entry.BubbleId == null ? null : new XElement("BubbleId", entry.BubbleId);
            var subId = entry.BubbleId == null ? null : new XElement("SubId", entry.SubId);
            var speakerId = entry.SpeakerId == null ? null : new XElement("SpeakerId", string.Join(",",entry.SpeakerId));
            var voiceId = entry.VoiceId == null ? null : new XElement("VoiceId", entry.VoiceId);
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
                bubbleId,
                subId,
                new XElement("Status", entry.Status)

            );
        }


        public List<EntryFound> SearchJapanese(string folder, int fileId, string text, bool matchWholeentry, bool matchCase, bool matchWholeWord, string language)
        {
            List<EntryFound> res = new List<EntryFound>();
            foreach( XMLSection section in Sections)
            {
                if (section.Name != "All strings")
                {
                    var temp = section.SearchJapanese(folder, fileId, section.Name, text, matchWholeentry, matchCase, matchWholeWord, language);

                    if (temp.Count > 0)
                        res.AddRange(temp);
                }
            }

            if (Speakers != null)
            {
                var speakerFound = SearchSpeaker(folder, fileId, text, matchWholeentry, matchCase, matchWholeWord, language);
                if (speakerFound.Count > 0)
                    res.AddRange(speakerFound);
            }

            return res;
        }

        private List<EntryFound> SearchSpeaker(string folder, int fileId, string text, bool matchWholeEntry, bool matchCase, bool matchWholeWord, string language)
        {
            List<EntryFound> res = new List<EntryFound>();
            List<int> foundIndexes;
                foundIndexes = Enumerable.Range(0, Speakers.Count)
                     .Where(e => Speakers[e].IsFound(text, matchWholeEntry, matchCase, matchWholeWord, language))
                     .ToList();

            if (foundIndexes.Count > 0)
            {

                foreach (int index in foundIndexes)
                {
                    EntryFound entry = new EntryFound();
                    entry.Folder = folder;
                    entry.FileId = fileId;
                    entry.Section = "Speaker";
                    entry.Id = index;
                    entry.Entry = Speakers[index];
                    res.Add(entry);
                }
            }
            return res;
        }

    }
}