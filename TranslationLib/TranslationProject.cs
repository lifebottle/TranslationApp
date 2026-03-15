using System;
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
            {
                string fullPath = Path.Combine(basePath, folder, "");
                var files = Directory.GetFiles(fullPath);
                if (files.Count() != 0 && files.Any(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
                {
                    XmlFolders.Add(new XMLFolder(folder, fullPath));
                }
            }
        }

        public void LoadXMLs(Action<int, int> progressCallback = null)
        {
            if (XmlFolders.Count == 0)
            {
                CurrentFolder = null;
                return;
            }

            int totalFiles = 0;
            foreach (var folder in XmlFolders)
            {
                if (Directory.Exists(folder.FolderPath))
                {
                    totalFiles += Directory.GetFiles(folder.FolderPath, "*.xml").Length;
                }
            }

            int currentFile = 0;
            foreach (var xmlFolder in XmlFolders)
            {
                xmlFolder.LoadXMLs(() => 
                {
                    currentFile++;
                    progressCallback?.Invoke(currentFile, totalFiles);
                });
            }

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

        public int GetFolderId(string name)
        {
            return XmlFolders.FindIndex(x => x.Name == name);
        }

    }
}