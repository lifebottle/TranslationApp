using System;
using System.IO;
using Newtonsoft.Json;

namespace TranslationApp
{
    public class Config
    {
        [JsonIgnore] private string FilePath { get; set; }

        private string _lastProjectFolderPath;
        private string _TORFolderPath;

        public string LastProjectFolderPath
        {
            get => _lastProjectFolderPath;
            set
            {
                _lastProjectFolderPath = value;
                Save();
            }
        }

        public string TORFolderPath
        {
            get => _TORFolderPath;
            set
            {
                _TORFolderPath = value;
                Save();
            }
        }

        public Config()
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TranslationApp");
            Directory.CreateDirectory(appDataPath);
            FilePath = Path.Combine(appDataPath, "config.txt");
        }

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    var savedConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FilePath));
                    _lastProjectFolderPath = savedConfig.LastProjectFolderPath;
                    _TORFolderPath = savedConfig.TORFolderPath;
                }
                catch (Exception)
                {
                    File.Delete(FilePath);
                }
            }
        }

        private void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }
    }
}