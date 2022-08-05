using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TranslationLib
{
    public class TranslationProject
    {
        public string ProjectFolder;
        public XMLFolder CurrentFolder;
        public List<XMLFolder> XmlFolders;

        public TranslationProject(string basePath, List<string> folderIncluded)
        {
            XmlFolders = new List<XMLFolder>();

            foreach (var folder in folderIncluded)
                XmlFolders.Add(new XMLFolder(Path.Combine(basePath, folder, "XML")));

            foreach (var xmlFolder in XmlFolders)
                xmlFolder.LoadXMLs();

            CurrentFolder = XmlFolders.First();
        }
    }

    public class XMLFolder
    {
        public string FolderPath { get; set; }
        public Dictionary<string, XDocument> XMLs { get; set; }
        public Dictionary<string, TranslationEntry> Translations { get; set; }

        public XMLFolder(string path)
        {
            FolderPath = path;
        }

        public void LoadXMLs()
        {
            var fileList = Directory.GetFiles(FolderPath);
            Translations = new Dictionary<string, TranslationEntry>();
            XMLs = new Dictionary<string, XDocument>();

            foreach (var file in fileList)
            {
                var document = XDocument.Load(file);
                XMLs.Add(Path.GetFileName(file), document);

                var elements = document.Root.Element("Strings").Elements("Entry");
                foreach (var element in elements)
                {
                    var key = element.Element("JapaneseText").Value;
                    var value = element.Element("EnglishText").Value;

                    if (key != string.Empty)
                    {
                        if (!Translations.ContainsKey(key))
                        {
                            Translations[key] = new TranslationEntry { EnglishTranslation = value, Count = 1 };
                        }
                        else
                        {
                            if (Translations[key].EnglishTranslation == string.Empty && value != string.Empty)
                                Translations[key].EnglishTranslation = value;
                            
                            Translations[key].Count++;
                        }
                    }
                }
            }
        }
    }

    public class TranslationEntry
    {
        public string EnglishTranslation { get; set; }
        public int Count { get; set; }
    }
}