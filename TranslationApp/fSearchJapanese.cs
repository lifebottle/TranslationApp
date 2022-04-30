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
    public partial class fSearchJapanese : Form
    {
        public fSearchJapanese()
        {
            InitializeComponent();
        }

        private void fSearchJapanese_Load(object sender, EventArgs e)
        {
            cbWholeWordSearch.Text = "Yes";
        }
    }
}
