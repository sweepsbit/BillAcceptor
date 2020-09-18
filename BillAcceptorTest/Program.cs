using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BillAcceptorTest.Properties;
 

namespace BillAcceptorTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Settings.Default["ShopUserName"] = "";
            //Settings.Default["ShopPassword"] = "";
            //Settings.Default["CabinetId"] = "";
            //Settings.Default["MyIP"] = "";
            //Settings.Default.Save();
            string username = Settings.Default["ShopUserName"].ToString();
            string password = Settings.Default["ShopPassword"].ToString();
            string CabinetId = Settings.Default["CabinetId"].ToString();
            string myIP = Settings.Default["MyIP"].ToString();
            if (username.Length == 0 && password.Length == 0 && CabinetId.Length ==0 && myIP.Length ==0)
            {
                Application.Run(new ShopLogin());
            }else
            {
                Form1 frm = new Form1();
                frm.ShowDialog();
                // Application.Run(new Form1());
            }
        }
    }
}
