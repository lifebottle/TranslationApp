using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationApp
{
    public class ExtendedTextBox : TextBox
    {
        public event EventHandler<ClipboardEventArgs> TextPasted;
        private const int WM_PASTE = 0x0302;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                var evt = TextPasted;
                if (evt != null)
                {
                    evt(this, new ClipboardEventArgs(Clipboard.GetText()));
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }

    public class ClipboardEventArgs : EventArgs
    {
        public string ClipboardText { get; set; }
        public ClipboardEventArgs(string clipboardText)
        {
            ClipboardText = clipboardText;
        }
    }
}
