using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_Example
{
    public static class LanguageExtensions
    {
        public static void ExpandColumns(this DataGridView sender)
        {
            foreach (DataGridViewColumn col in sender.Columns)
            {
                col.HeaderText = string.Join(" ", System.Text.RegularExpressions.Regex.Split(col.HeaderText, "(?=[A-Z])"));
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

    }
}
