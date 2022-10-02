using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationApp
{
    public partial class fSetup : Form
    {
        private Config config;
        private fMain fMainInstance;
        private PackingLib.PackingProject PackingAssistant;
        public fSetup(fMain Instance, Config MyConfig, PackingLib.PackingProject MyPackingAssitant)
        {
            InitializeComponent();
            fMainInstance = Instance;
            lbPythonInstallations.DataSource = MyPackingAssitant.GetPythonInstallation();
            this.config = MyConfig;
            this.PackingAssistant = MyPackingAssitant;
            ShowPythonLibOptions();
            ShowIsoOptions();
            
        }
        private void ShowPythonLibOptions()
        {
            var lastFolderPath = this.config.GamesConfigList.Where(x => x.LastFolderPath != null).FirstOrDefault().LastFolderPath;
            DirectoryInfo infos = Directory.GetParent(lastFolderPath);
            string option = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(lastFolderPath).FullName).FullName).FullName, "PythonLib");
            if (Directory.Exists(option))
                lbPythonLib.DataSource = new List<string> { option };
  
        }

        private void ShowIsoOptions()
        {
            var basePath = config.GamesConfigList.Where(x => x.FolderPath != null).FirstOrDefault().FolderPath;
            if (!string.IsNullOrEmpty(basePath))
            {
                var fileList = Directory.GetFiles(Directory.GetParent(Directory.GetParent(Directory.GetParent(basePath).FullName).FullName).FullName);
                lbNDXIso.DataSource = fileList.Where(x => x.EndsWith(".iso")).ToList();
                lbNDXIso.SelectedIndex = -1;
                lbTORIso.DataSource = fileList.Where(x => x.EndsWith(".iso")).ToList();
                lbTORIso.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("You need to first load one of the Game's repos");
            }
        }

        private void SavePython()
        {
            if (lbPythonInstallations.Items.Count > 0)
            {
                string pythonPath = Path.GetDirectoryName((string)lbPythonInstallations.Items[lbPythonInstallations.SelectedIndex]);
                if (Directory.Exists(pythonPath))
                {
                    config.PythonLocation = pythonPath;
                }
                Console.WriteLine("Python's location has been updated");

            }
            else
            {
                MessageBox.Show("You need to install Python 3.9.12 on your machine");
            }
            
        }
        private void SaveIsos()
        {
            //TOR
            var TORConfig = config.GetGameConfig("TOR");
            if (TORConfig != null)
            {
                if (lbTORIso.SelectedIndex > -1)
                {
                    TORConfig.IsoPath = lbTORIso.SelectedItem.ToString();
                    Console.WriteLine("Tales of Rebirth's iso location has been updated");
                }

            }


            //NDX
            var NDXConfig = config.GetGameConfig("NDX");
            if (NDXConfig != null)
            {
                if (lbNDXIso.SelectedIndex > -1)
                {
                    NDXConfig.IsoPath = lbNDXIso.SelectedItem.ToString();
                    Console.WriteLine("Narikiri Dungeon X's iso location has been updated");
                }
            }
        }

        private void fSetup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    Clipboard.SetText((string)lbRepos.Items[lbRepos.SelectedIndex]);
                }
            }
        }

        private void bInstallPackages(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(config.PythonLib) && !string.IsNullOrEmpty(config.PythonLocation))
                PackingAssistant.InstallRequirements(config.PythonLocation, config.PythonLib);
            else
                MessageBox.Show("You need to first save the configuration of Python's location and PythonLib's location");
        }

        private void SavePythonLib()
        {
            if (lbPythonLib.Items.Count > 0)
            {
                string pythonLib = (string)lbPythonLib.Items[lbPythonLib.SelectedIndex];
                if (Directory.Exists(pythonLib))
                {
                    config.PythonLib = pythonLib;
                }
                Console.WriteLine("Python Library's location has been updated");

            }
            else
            {
                MessageBox.Show("You need to clone the PythonLib's repository inside your main folder.\nFollow the structure on the screenshot.");
            }
        }

        private void bSaveConfiguration_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\n.............................................");
            Console.WriteLine("Saving your configuration\n");
            SavePython();
            SavePythonLib();
            SaveIsos();
            config.ReadConfigText();
            fMainInstance.UpdateOptionsVisibility();
        }

        private void bShowConfig_Click(object sender, EventArgs e)
        {
            config.ReadConfigText();
        }
    }
}
