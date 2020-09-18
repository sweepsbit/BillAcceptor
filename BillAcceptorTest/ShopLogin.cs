using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using BillAcceptorTest.Properties;
using System.Net;
using System.Configuration;
using System.Diagnostics;

namespace BillAcceptorTest
{
    public partial class ShopLogin : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        SqlConnection conn = new SqlConnection(connectionString);
        public ShopLogin()
        {
            InitializeComponent();
            int St = 0;

            string username = Settings.Default["ShopUserName"].ToString();
            string password = Settings.Default["ShopPassword"].ToString();
            string CabinetId = Settings.Default["CabinetId"].ToString();
            if (username.Length != 0 && password.Length != 0 && CabinetId.Length != 0)
            {

                St = ShopLoginCheck(username, password, CabinetId);
                if (St > 0)
                {
                    AShopId = St;

                    label3.Text = St.ToString();
                    //string filename = "shutdown.exe";
                    //string arguments = "-r";
                    //ProcessStartInfo startinfo = new ProcessStartInfo(filename, arguments);
                    //Process.Start(startinfo);
                    this.Hide();
                    Form1 frm1 = new BillAcceptorTest.Form1();
                    frm1.Show();
                }
            }
        }

        private void ShopLogin_Load(object sender, EventArgs e)
        {
            //panel1.Location = new Point(ClientSize.Width / 2 - panel1.Size.Width / 2, ClientSize.Height / 2 - panel1.Size.Height / 2);
            //panel1.Anchor = AnchorStyles.None;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            

        }
        public void bindcmb(int ShopId)
        {
            string query = "select * from CG_Cabinets where IsUsed=0 and ShopId="+ ShopId +"";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            da.Fill(ds);
            comboBox1.DisplayMember = "CabinetId";

            comboBox1.DataSource = ds.Tables[0];
        }
        public int ShopLoginCheck(string username, string password, string CabinetId)
        {

            password = EncryptPass(password);
            int ShopId = 0;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            string sql = "select CGS.Id as 'ShopId' FROM CG_Admin CGA,CG_Shop CGS where  CGA.Id=CGS.AdminId  and AdminName='" + username + "' and Password='" + password + "'";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter dapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            dapt.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ShopId = Convert.ToInt32(ds.Tables[0].Rows[0]["ShopId"]);

                sql = "select * from CG_Cabinets where ShopId=" + ShopId + " and CabinetId='" + CabinetId + "' and IsUsed=0";// and PrivateIP='"+ myIP +"'";
                dapt = new SqlDataAdapter(sql, conn);
                ds = new DataSet();
                dapt.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Settings.Default["CabinetId"] = CabinetId;
                    Settings.Default["MyIP"] = myIP;
                    Settings.Default.Save();
                    sql = "update CG_Cabinets set IsUsed=1 where ShopId=" + ShopId + " and CabinetId='" + CabinetId + "'"; //  and PrivateIP='" + myIP + "'";
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    return ShopId;
                }
                else
                {

                    sql = "select IsUsed from CG_Cabinets where ShopId=" + ShopId + " and CabinetId='" + CabinetId + "'";
                    cmd = new SqlCommand(sql, conn);
                    int IsUsed = Convert.ToInt32(cmd.ExecuteScalar());
                    if (IsUsed == 1)
                    {
                        MessageBox.Show("Device/Cabinet is already in Used/Assigned");
                    }
                    else
                    {
                        // Settings.Default["CabinetId"] = "";
                        // Settings.Default.Save();
                        MessageBox.Show("Invalid Details. Try Again");
                    }
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }
        public static string encryptionKey = "DISTRIBUTORADMIN_04_03_2018";
        public static string EncryptPass(string Password)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(Password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    Password = Convert.ToBase64String(ms.ToArray());
                }
            }

            return Password;
        }
        public static int AShopId;


        private void button1_Click_1(object sender, EventArgs e)
        {
            string username = tuname.Text;
            string password = tpassword.Text;
            string CabinetId = comboBox1.Text;
            int Status = ShopLoginCheck(username, password, CabinetId);
            if (Status > 0)
            {
                Properties.Settings.Default["ShopUserName"] = username;
                Properties.Settings.Default["ShopPassword"] = password;
                Properties.Settings.Default["CabinetId"] = CabinetId;
                Properties.Settings.Default["ShopId"] = Status;
                Properties.Settings.Default.Save();
                Form1 frm1 = new BillAcceptorTest.Form1();

                AShopId = Status;
                label3.Text = Status.ToString();
                this.Hide();
                frm1.Show();
            }
        }
        private Size oldSize;
        protected override void OnResize(System.EventArgs e)
        {


            foreach (Control cnt in this.Controls)
                ResizeAll(cnt, base.Size);

            oldSize = base.Size;
        }
        private void ResizeAll(Control control, Size newSize)
        {
            int width = newSize.Width - oldSize.Width;
            control.Left += (control.Left * width) / oldSize.Width;
            control.Width += (control.Width * width) / oldSize.Width;

            int height = newSize.Height - oldSize.Height;
            control.Top += (control.Top * height) / oldSize.Height;
            control.Height += (control.Height * height) / oldSize.Height;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            OnResize(e);
        }

        private void chkShopId(object sender, EventArgs e)
        {
            int ShopId = 0;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            string sql = "select CGS.Id as 'ShopId' FROM CG_Admin CGA,CG_Shop CGS where  CGA.Id=CGS.AdminId  and AdminName='" + tuname.Text + "' ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter dapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            
            dapt.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ShopId = Convert.ToInt32(ds.Tables[0].Rows[0]["ShopId"]);
                bindcmb(ShopId);
            }
        }
    
    }
}
