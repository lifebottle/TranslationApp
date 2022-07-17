using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationApp
{
    public partial class fHexToJapanese : Form
    {
        public fHexToJapanese()
        {
            InitializeComponent();
        }

        private void HexToJapanese_Load(object sender, EventArgs e)
        {
            cbGameList.SelectedIndex = 0;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbHextoConvert_TextChanged(object sender, EventArgs e)
        {
            //Call my python function
        }

        private void bConvert_Click(object sender, EventArgs e)
        {
            //string res = Tools.hexToJap(cbGameList.Text, tbHextoConvert.Text);
            //tbConvertedJapanese.Text = res;
        }

        private void bDumpText_Click(object sender, EventArgs e)
        {
            int startOffset = int.Parse(tbStartOffset.Text, System.Globalization.NumberStyles.HexNumber);
            string filename = tbFileOrigin.Text;
            //string res = Tools.callFunction(cbGameList.Text, "utility", string.Format("dumptext {0} {1}", filename, startOffset));

        }
    }
}
