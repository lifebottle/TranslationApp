using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationLib
{
    public class EntryFound
    {
        public string Folder { get; set; }
        public int FileId { get; set; }
        public string Section { get; set; }
        public int Id { get; set; }
        public XMLEntry Entry { get; set; }

    }
}
