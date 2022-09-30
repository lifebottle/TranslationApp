using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace TranslationApp
{
    public class GameConfig
    {
        private string _game;
        private string _folderPath;
        private string _isoPath;
        private string _lastFolderPath;
        private DateTime _lastTimeLoaded;

        public string Game
        {
            get => _game;
            set => _game = value;
        }
        public string FolderPath
        {
            get => _folderPath;
            set => _folderPath = value;
        }
        public string IsoPath
        {
            get => _isoPath;
            set => _isoPath = value;
        }
        public string LastFolderPath
        {
            get => _lastFolderPath;
            set => _lastFolderPath = value;
        }
        public DateTime LastTimeLoaded
        {
            get => _lastTimeLoaded;
            set => _lastTimeLoaded = value;
        }

    }
    public class Config
    {
        [JsonIgnore] private string FilePath { get; set; }

        private List<GameConfig> _gamesConfigList;
        private string _pythonLocation;
        private string _pythonLib;
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
        public List<GameConfig> GamesConfigList
        {
            get => _gamesConfigList;
            set => _gamesConfigList = value;
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
                    _gamesConfigList = savedConfig.GamesConfigList;
                    _pythonLocation = savedConfig.PythonLocation;
                    _pythonLib = savedConfig.PythonLib;
                }
                catch (Exception)
                {
                    File.Delete(FilePath);
                }
            }
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public GameConfig GetGameConfig(string gameName)
        {
            return _gamesConfigList.Where(x => x.Game == gameName).FirstOrDefault();
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
        public bool IsPackingVisibility(string gameName)
        {
            var gameConfig = GetGameConfig(gameName);
            if (gameConfig != null)
            {
                bool gameFolder = !string.IsNullOrEmpty(gameConfig.FolderPath);
                bool gameIso = !string.IsNullOrEmpty(gameConfig.IsoPath);
                bool pythonFolder = !string.IsNullOrEmpty(_pythonLocation);
                bool pythonLib = !string.IsNullOrEmpty(_pythonLib);
                return gameFolder && gameIso && pythonFolder && pythonLib;
            }
            else
                return false;
        }
    }
}