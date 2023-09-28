using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TranslationLib
{
    public class TranslationProject
    {
        public string ProjectPath { get; }
        public List<XMLFolder> XmlFolders { get; set; }
        public XMLFolder CurrentFolder { get; set; }

        public TranslationProject(string basePath, List<string> folderIncluded)
        {
            ProjectPath = basePath;
            XmlFolders = new List<XMLFolder>();

            foreach (var folder in folderIncluded)
                XmlFolders.Add(new XMLFolder(folder, Path.Combine(basePath, folder, "")));

            foreach (var xmlFolder in XmlFolders)
                xmlFolder.LoadXMLs();

            CurrentFolder = XmlFolders.First();
        }

        public XMLFolder GetFolderByName(string name)
        {
            return XmlFolders.First(x => x.Name == name);
        }

        public List<string> GetFolderNames()
        {
            return XmlFolders.Select(x => x.Name).ToList();
        }

        public void SetCurrentFolder(string name)
        {
            CurrentFolder = XmlFolders.First(x => x.Name == name);
        }
    }
}