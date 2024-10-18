using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jim
{
    internal static class Program
    {
      
        [STAThread]
        static async Task Main() 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var dbHelper = new SQLiteDBHelper("database.db");
            await dbHelper.ConnectAsync();

            System.Windows.Forms.Application.Run(new Application());
        }
    }
}
