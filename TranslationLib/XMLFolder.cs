using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TranslationLib
{
    public class XMLFolder
    {
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public List<XMLFile> XMLFiles { get; set; }
        public Dictionary<string, TranslationEntry> Translations { get; set; }
        public XMLFile CurrentFile { get; set; }

        public XMLFolder(string name, string path)
        {
            Name = name;
            FolderPath = path;
            Translations = new Dictionary<string, TranslationEntry>();
            XMLFiles = new List<XMLFile>();
            CurrentFile = new XMLFile();
        }

        public void LoadXMLs()
        {
            var fileList = Directory.GetFiles(FolderPath);

            if (fileList.Count() > 0)
            {
                foreach (var file in fileList)
                {
                    var XMLFile = new XMLFile { Name = Path.GetFileName(file), FilePath = file, FileType = Name };
                    XMLFiles.Add(XMLFile);
                    var document = XDocument.Load(file, LoadOptions.PreserveWhitespace);

                    var XMLSections = document.Root.Elements("Strings");

                    // Add a dummy "Everyting" section
                    var everything_section = new XMLSection("All strings");
                    XMLFile.Sections.Add(everything_section);

                    foreach (var XMLSection in XMLSections)
                    {
                        var section = new XMLSection(XMLSection.Element("Section").Value);
                        XMLFile.Sections.Add(section);

                        foreach (var XMLEntry in XMLSection.Elements("Entry"))
                        {
                            var entry = ExtractXMLEntry(XMLEntry);
                            section.Entries.Add(entry);
                            everything_section.Entries.Add(entry);

                            if (!string.IsNullOrEmpty(entry.JapaneseText))
                            {
                                if (!Translations.ContainsKey(entry.JapaneseText))
                                    AddNewDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                                else
                                    AddExistingDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                            }
                        }
                    }

                    var XMLSpeaker = document.Root.Element("Speakers");

                    if (XMLSpeaker != null)
                    {
                        foreach (var XMLEntry in XMLSpeaker.Elements("Entry"))
                        {
                            var entry = ExtractXMLEntry(XMLEntry);
                            XMLFile.Speakers.Add(entry);

                            if (!string.IsNullOrEmpty(entry.JapaneseText))
                            {
                                if (!Translations.ContainsKey(entry.JapaneseText))
                                    AddNewDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                                else
                                    AddExistingDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                            }
                        }
                        XMLFile.UpdateAllEntryText();
                    }

                    XMLFile.CurrentSection = XMLFile.Sections.First();
                }
                CurrentFile = XMLFiles.First();
            }
        }

        private XMLEntry ExtractXMLEntry(XElement element)
        {
            var e = new XMLEntry
            {
                Id = ExtractNullableInt(element.Element("Id")),
                PointerOffset = ExtractNullableString(element.Element("PointerOffset")),
                VoiceId = ExtractNullableString(element.Element("VoiceId")),
                EnglishText = ExtractNullableString(element.Element("EnglishText")),
                JapaneseText = ExtractNullableString(element.Element("JapaneseText")),
                Notes = ExtractNullableString(element.Element("Notes")),
                Status = ExtractNullableString(element.Element("Status")),
                SpeakerId = ExtractNullableIntArray(element.Element("SpeakerId")),
                StructId = ExtractNullableInt(element.Element("StructId")),
                UnknownPointer = ExtractNullableInt(element.Element("UnknownPointer")),
                MaxLength = ExtractNullableInt(element.Element("MaxLength"))
            };

            if (element.Element("EmbedOffset") != null)
            {
                e.EmbedOffset = true;
                e.hi = ExtractNullableString(element.Element("EmbedOffset").Element("hi"));
                e.lo = ExtractNullableString(element.Element("EmbedOffset").Element("lo"));
            } else
            {
                e.EmbedOffset = false;
            }

            return e;
        }

        private int? ExtractNullableInt(XElement element)
        {
            if (element == null)
                return null;

            return int.Parse(element.Value);
        }

        private int[] ExtractNullableIntArray(XElement element)
        {
            if (element == null)
                return null;        

            return element.Value.Split(',').Select(x => int.Parse(x)).ToArray();
        }

        private string ExtractNullableString(XElement element)
        {
            if (element == null)
                return null;
            else
                return element.IsEmpty ? null : element.Value;
        }

        public List<string> FileList()
        {
            return XMLFiles.Select(x => x.Name).ToList();
        }
        
        public void SetCurrentFile(string name)
        {
            CurrentFile = XMLFiles.First(x => x.Name == name);
            CurrentFile.CurrentSection = CurrentFile.Sections.First();
        }

        private void AddNewDictionnaryEntry(string entryJapaneseText, string entryEnglishText)
        {
            Translations[entryJapaneseText] = new TranslationEntry { EnglishTranslation = entryEnglishText, Count = 1 };
        }

        private void AddExistingDictionnaryEntry(string entryJapaneseText, string entryEnglishText)
        {
            if (Translations[entryJapaneseText].EnglishTranslation == string.Empty && entryEnglishText != string.Empty)
                Translations[entryJapaneseText].EnglishTranslation = entryEnglishText;

            Translations[entryJapaneseText].Count++;
        }
    }
}