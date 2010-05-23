using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
/*Copyright: Barry O'Gorman ( bogorman@gmail.com ) under the 
 * Appache License http://www.apache.org/licenses/LICENSE-2.0

Modified By LordGregGreg to work with x64 and to have a icon.

Description: Send's Notifications from Skype to Growl for Windows.
 * */
namespace SkypeToGrowl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }
    }
}
