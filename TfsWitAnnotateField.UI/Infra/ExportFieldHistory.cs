using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TfsWitAnnotateField.UI.Infra
{
    internal class ExportFieldHistory
    {
        public static void ExportHistory(IReadOnlyCollection<string> fieldHistory)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Export Field History";
            fd.Filter = "Text Document|*.txt";
            fd.ShowDialog();

            if (fd.FileName != string.Empty)
            {
                using (StreamWriter outfile = new StreamWriter(fd.FileName))
                {
                    foreach (string rev in fieldHistory)
                    {
                        outfile.WriteLine(rev);
                    }
                }
            }
        }
    }
}
