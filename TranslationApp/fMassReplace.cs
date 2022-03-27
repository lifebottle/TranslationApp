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
    public partial class fMassReplace : Form
    {
        public fMassReplace()
        {
            InitializeComponent();
        }

        private void bSearchMass_Click(object sender, EventArgs e)
        {
            //Grab all entries from all the files where Japanese Text contains
        }

        private void fMassReplace_Load(object sender, EventArgs e)  
        {
            cbExactMatch.Text = "Yes";
        }
    }
}
