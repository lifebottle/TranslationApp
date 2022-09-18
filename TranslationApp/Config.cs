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
        private string _TORIsoPath;
        private string _NDXFolderPath;
        private string _NDXIsoPath;
        private string _pythonLocation;
        private string _pythonLib;

        public string LastProjectFolderPath
        {
            get => _lastProjectFolderPath;
            set
            {
                _lastProjectFolderPath = value;
                Save();
            }
        }
        public string PythonLib
        {
            get => _pythonLib;
            set
            {
                _pythonLib = value;
                Save();
            }
        }
        public string PythonLocation
        {
            get => _pythonLocation;
            set
            {
                _pythonLocation = value;
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
        public string TORIsoPath
        {
            get => _TORIsoPath;
            set
            {
                _TORIsoPath = value;
                Save();
            }
        }
        public string NDXFolderPath
        {
            get => _NDXFolderPath;
            set
            {
                _NDXFolderPath = value;
                Save();
            }
        }
        public string NDXIsoPath
        {
            get => _NDXIsoPath;
            set
            {
                _NDXIsoPath = value;
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
                    _TORIsoPath = savedConfig.TORIsoPath;
                    _NDXFolderPath = savedConfig.NDXFolderPath;
                    _NDXIsoPath = savedConfig.NDXIsoPath;
                    _pythonLocation = savedConfig.PythonLocation;
                    _pythonLib = savedConfig.PythonLib;
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

        public void ReadConfigText()
        {
            if (File.Exists(FilePath))
            {
                dynamic dynJson = JsonConvert.DeserializeObject(File.ReadAllText(FilePath));
                Console.WriteLine("\n.............................................");
                Console.WriteLine("Config file\n");
                foreach (var item in dynJson)
                    Console.WriteLine($"{item}");
            }
        }
    }
}