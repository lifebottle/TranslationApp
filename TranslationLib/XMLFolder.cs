using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public bool isLegacy { get; set; }

        public XMLFolder(string name, string path, bool legacy)
        {
            Name = name;
            FolderPath = path;
            Translations = new Dictionary<string, TranslationEntry>();
            XMLFiles = new List<XMLFile>();
            CurrentFile = new XMLFile();
            isLegacy = legacy;
        }

        public void LoadXMLs()
        {
            var fileList = Directory.GetFiles(FolderPath);

            if (fileList.Count() > 0)
            {
                foreach (var file in fileList)
                {
                    if (file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) {
                        XMLFiles.Add(LoadXML(file));
                    }
                }
                CurrentFile = XMLFiles.First();
            }
        }

        public XMLFile LoadXML(string xmlpath)
        {

            var XMLFile = new XMLFile { Name = Path.GetFileNameWithoutExtension(xmlpath), FilePath = xmlpath };
            var document = XDocument.Load(xmlpath, LoadOptions.PreserveWhitespace);
            XMLFile.FileType = document.Root.Name.ToString();
            XMLFile.FriendlyName = document.Root.Element("FriendlyName")?.Value;
            XMLFile.isLegacy = isLegacy;
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
                XMLFile.Speakers = new List<XMLEntry>();
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
            return XMLFile;
        }

        public int SaveChanged()
        {
            int count = 0;
            Parallel.For(0, XMLFiles.Count(), () => 0, (i, loop, subtotal) =>
            {
                if (XMLFiles[i].needsSave)
                {
                    XMLFiles[i].SaveToDisk();
                    XMLFiles[i].needsSave = false;
                    subtotal++;
                }
                return subtotal;
            },
            (x) => Interlocked.Add(ref count, x));
            return count;
        }

        public void InvalidateTranslations()
        {
            Translations = new Dictionary<string, TranslationEntry>();
            foreach (XMLFile file in XMLFiles)
            {

                foreach (var section in file.Sections.Where(x => x.Name != "All strings"))
                {
                    foreach (var entry in section.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.JapaneseText))
                        {
                            if (!Translations.ContainsKey(entry.JapaneseText))
                                AddNewDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                            else
                                AddExistingDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                        }
                    }
                }

                if (file.Speakers != null)
                {
                    foreach (var entry in file.Speakers)
                    {
                        if (!string.IsNullOrEmpty(entry.JapaneseText))
                        {
                            if (!Translations.ContainsKey(entry.JapaneseText))
                                AddNewDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                            else
                                AddExistingDictionnaryEntry(entry.JapaneseText, entry.EnglishText);
                        }
                    }
                }
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
                BubbleId = ExtractNullableInt(element.Element("BubbleId")),
                SubId = ExtractNullableInt(element.Element("SubId")),
                StructId = ExtractNullableInt(element.Element("StructId")),
                UnknownPointer = ExtractNullableInt(element.Element("UnknownPointer")),
                MaxLength = ExtractNullableInt(element.Element("MaxLength"))
            };

            if (element.Element("EmbedOffset") != null)
            {
                e.EmbedOffset = true;
                e.hi = ExtractNullableString(element.Element("EmbedOffset").Element("hi"));
                e.lo = ExtractNullableString(element.Element("EmbedOffset").Element("lo"));
            }
            else
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

        public void SetCurrentFile(int index)
        {
            CurrentFile = XMLFiles[index];
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

        public List<EntryFound> SearchJapanese(string japText, bool matchWholeEntry, bool matchCase, bool matchWholeWord, string language)
        {
            List<EntryFound> dict = new List<EntryFound>();

            for(int i=0; i<XMLFiles.Count; i++)
            {
                var res = XMLFiles[i].SearchJapanese(Name, i, japText, matchWholeEntry, matchCase, matchWholeWord, language);
                if(res.Count() > 0)
                    dict.AddRange(res);
            }
            return dict;
        }
    }
}