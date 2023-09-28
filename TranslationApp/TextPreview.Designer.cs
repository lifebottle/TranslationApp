using System.Windows.Forms;
using System.Drawing;

namespace TranslationApp
{
    partial class TextPreview
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Drawing surface where graphics should be drawn.
        /// Use this member in the OnDraw method.
        /// </summary>
        protected Graphics graphics;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TextPreview
            // 
            this.Name = "TextPreview";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TextPreview_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
