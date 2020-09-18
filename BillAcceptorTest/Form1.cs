using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BillAcceptorTest.Properties;


namespace BillAcceptorTest
{
    public partial class Form1 : Form
    {
         
        string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        public Form1()
        {
            InitializeComponent();
            
        }
        public static int AShopID;
        private void button1_Click(object sender, EventArgs e)
        {
           
            this.Hide();
            Form2 frm2 = new BillAcceptorTest.Form2();
            AShopID = ShopLogin.AShopId;
            frm2.Show();

        }
        private Size oldSize;
        private void Form1_Load(object sender, EventArgs e)
        {
            //ShopLogin.AShopId 
            //button1.Left = (this.ClientSize.Width - button1.Width) / 2;
           // button1.Top = (this.ClientSize.Height - button1.Height) / 2;
            AShopID = Convert.ToInt32(ShopLogin.AShopId);
            //userid.Text = AShopID.ToString();
            userid.Text = Properties.Settings.Default["ShopId"].ToString();
            // MessageBox.Show(userid.Text);
            timer1.Start();

        }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string CabinetId = Settings.Default["CabinetId"].ToString();
            string ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + CabinetId + "'";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlDataAdapter dapt = new SqlDataAdapter(ss, conn);
            DataSet ds = new DataSet();
            dapt.Fill(ds);
            int Ise = 1;
            if (ds.Tables[0].Rows.Count > 0)
            {
                Ise = Convert.ToInt32(ds.Tables[0].Rows[0]["IsOn"]);
                if (Ise == 1)
                {
                    timer1.Stop();
                     this.Hide();
                    this.ShowInTaskbar = false;
                    Form2 frm2 = new Form2();
                    this.Close();
                    this.Dispose();
                    //AShopID = ShopLogin.AShopId;
                    frm2.ShowDialog();
                    // this.Dispose();

                }


            }
            ss = "select TOP(1)* from CG_RedeemReceiptPrint where CabinetId='" + CabinetId + "' and IsPrinted=1";
            SqlDataAdapter dapt1 = new SqlDataAdapter(ss, conn);
            DataSet ds1 = new DataSet();
            dapt1.Fill(ds1);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                Ticket tkt = new Ticket();
                tkt.TicketNo = ds1.Tables[0].Rows[0]["CustomerId"].ToString();
                tkt.ticketDate = DateTime.Now;

                tkt.amount = Convert.ToInt32(ds1.Tables[0].Rows[0]["CurrentBalance"]);
                // tkt.RewardPoint = int.Parse(Rewardpoint);
                tkt.printRedeem();
                ss = "delete from CG_RedeemReceiptPrint where CabinetId='" + CabinetId + "' and IsPrinted=1 and CustomerId='" + tkt.TicketNo +"'";
                SqlCommand cmd = new SqlCommand(ss, conn);
                cmd.ExecuteNonQuery();

                ss = "insert into CG_CabinetsCustomerPerActionLogs(CabinetId,CustomerId,Timestamps,Redeem) values('" + CabinetId + "','" + tkt.TicketNo + "','" + DateTime.Now + "'," + tkt.amount + ")";
                cmd = new SqlCommand(ss, conn);
                cmd.ExecuteNonQuery();

            }
        }
    }
}
