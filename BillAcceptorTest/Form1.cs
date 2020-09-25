using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using BillAcceptorTest.Properties;


namespace BillAcceptorTest
{
    public partial class Form1 : Form
    {

        string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        private bool isNetworkAvailable = true;
        public Form1()
        {
            InitializeComponent();
            using(var ping = new Ping())
            {
                try
                {
                    isNetworkAvailable = ping.Send("www.google.com").Status == IPStatus.Success;
                }
                catch
                {
                    isNetworkAvailable = false;
                }
            }
            
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Invoke(new Action(() =>
            {
                isNetworkAvailable = e.IsAvailable;
            }));
        }

        public static int AShopID;
        private void button1_Click(object sender, EventArgs e)
        {

            Hide();
            var frm2 = new Form2();
            AShopID = ShopLogin.AShopId;
            frm2.Show();

        }
        private Size oldSize;
        private void Form1_Load(object sender, EventArgs e)
        {
            //ShopLogin.AShopId 
            AShopID = Convert.ToInt32(ShopLogin.AShopId);
            //userid.Text = AShopID.ToString();
            userid.Text = Settings.Default["ShopId"].ToString();
            // MessageBox.Show(userid.Text);
            timer1.Start();

        }
        protected override void OnResize(EventArgs e)
        {


            foreach (Control cnt in Controls)
                ResizeAll(cnt, Size);

            oldSize = Size;
        }
        private void ResizeAll(Control control, Size newSize)
        {
            var width = newSize.Width - oldSize.Width;
            control.Left += (control.Left * width) / oldSize.Width;
            control.Width += (control.Width * width) / oldSize.Width;

            var height = newSize.Height - oldSize.Height;
            control.Top += (control.Top * height) / oldSize.Height;
            control.Height += (control.Height * height) / oldSize.Height;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            OnResize(e);
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!isNetworkAvailable)
            {
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    //Open connection
                    await conn.OpenAsync();
                    var cabinetId = Settings.Default["CabinetId"].ToString();
                    var ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + cabinetId + "'";

                    using (var dapt = new SqlDataAdapter(ss, conn))
                    {
                        var ds = new DataSet();
                        dapt.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var ise = Convert.ToInt32(ds.Tables[0].Rows[0]["IsOn"]);
                            if (ise == 1)
                            {
                                timer1.Stop();
                                Hide();
                                ShowInTaskbar = false;
                                using (var frm2 = new Form2())
                                {
                                    Close();
                                    Dispose();
                                    frm2.ShowDialog();
                                }
                            }
                        }
                    }

                    ss = "select TOP(1)* from CG_RedeemReceiptPrint where CabinetId='" + cabinetId + "' and IsPrinted=1";
                    using (var dapt1 = new SqlDataAdapter(ss, conn))
                    {
                        var ds1 = new DataSet();
                        dapt1.Fill(ds1);
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            var tkt = new Ticket(ds1.Tables[0].Rows[0]["CustomerId"].ToString(), DateTime.Now,
                                Convert.ToInt32(ds1.Tables[0].Rows[0]["CurrentBalance"]), default);

                            // tkt.RewardPoint = int.Parse(Rewardpoint);
                            tkt.printRedeem();
                            ss = "delete from CG_RedeemReceiptPrint where CabinetId='" + cabinetId + "' and IsPrinted=1 and CustomerId='" + tkt.TicketNo + "'";
                            using (var cmd = new SqlCommand(ss, conn))
                            {
                                await cmd.ExecuteNonQueryAsync();
                            }

                            ss = "insert into CG_CabinetsCustomerPerActionLogs(CabinetId,CustomerId,Timestamps,Redeem) values('" + cabinetId + "','" + tkt.TicketNo + "','" + DateTime.Now + "'," + tkt.Amount + ")";
                            using (var cmd = new SqlCommand(ss, conn))
                            {
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle Any exceptions
            }
        }
    }
}
