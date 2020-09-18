using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

using PyramidNETRS232;
using System.ComponentModel;
//using Casino.Controllers;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using BillAcceptorTest.Properties;
using System.Configuration;
 
namespace BillAcceptorTest
{

    public partial class Form2 : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        SqlConnection conn = new SqlConnection(connectionString);
        private PyramidAcceptor validator;
        private RS232Config config;
        private FixedObservableLinkedList<DebugBufferEntry> debugQueueMaster = new FixedObservableLinkedList<DebugBufferEntry>(20);
        private FixedObservableLinkedList<DebugBufferEntry> debugQueueSlave = new FixedObservableLinkedList<DebugBufferEntry>(20);
        private int bill1 = 0;
        private int bill2 = 0;
        private int bill3 = 0;
        private int bill4 = 0;
        private int bill5 = 0;
        private int bill6 = 0;
        private int bill7 = 0;
        private int total = 0;

        private static Dictionary<int, int> currencyMap = new Dictionary<int, int>();
        private Dictionary<int, long> cashbox = new Dictionary<int, long>();
        static int totalDenomination = 0;

        DateTime sessionstarttime;
        DateTime sessionendtime;
        DateTime sessionresettime;
        int CustomerID;
        int ShopId;
        decimal BillAmount;
        decimal TotalDenomination;
        public Form2()
        {
            InitializeComponent();
            sessionstarttime = DateTime.Now;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            userid.Text = Properties.Settings.Default["ShopId"].ToString();//Form1.AShopID.ToString();
            ShopId = Convert.ToInt32(Properties.Settings.Default["ShopId"]);
            //MessageBox.Show(userid.Text);
            timer2.Enabled = true;
            timer1.Enabled = true;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (currencyMap.Count == 0)
            {
                currencyMap.Add(1, 1);
                currencyMap.Add(2, 2);
                currencyMap.Add(3, 5);
                currencyMap.Add(4, 10);
                currencyMap.Add(5, 20);
                currencyMap.Add(6, 50);
                currencyMap.Add(7, 100);
            }
          
            if (IsConnected)
            {
                validator.Close();

                return;
            }


            string PortName = "COM100";
            if (string.IsNullOrEmpty(PortName) || PortName.Equals("Select Port"))
            {
                Console.WriteLine("Please select a port");
                return;
            }


            IsConnected = true;

            config = new RS232Config(PortName, false);
            validator = new PyramidAcceptor(config);
             
            validator.OnCredit += validator_OnCredit;
            
            // This starts the acceptor - REQUIRED!!
            validator.Connect();
        }
        void config_OnSerialData(object sender, DebugEntryArgs e)
        {
            var entry = e.Entry;
            DoOnUIThread(() =>
            {
                if (entry.Flow == Flows.Master)
                {
                    debugQueueMaster.Add(entry);
 
                }
                else
                {
                    debugQueueSlave.Add(entry);

                     
                }
            });
        }
        private void DoOnUIThread(Action action)
        {

            action.Invoke();

        }
        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyPropertyChanged("IsConnected");
                NotifyPropertyChanged("IsNotConnected");
            }
        }

        private void validator_OnCredit(object sender, CreditArgs e)
        {
            counter = 0;
            var denomination = e.Index;
            if (currencyMap.ContainsKey(denomination))
            {
                if (denomination > 0)
                {
                    Console.WriteLine("Credited ${0}", AddCredit(denomination));
                   
                }
                else
                    Console.WriteLine("Failed to credit: {0}", denomination);
            }

        }
      
        internal int AddCredit(int denom)
        {
            switch (denom)
            {
                case 1:
                    Bill1++;
                    break;
                case 2:
                    Bill2++;
                    break;
                case 3:
                    Bill3++;
                    break;
                case 4:
                    Bill4++;
                    break;
                case 5:
                    Bill5++;
                    break;
                case 6:
                    Bill6++;
                    break;
                case 7:
                    Bill7++;
                    break;

                default:
                    // Illegal value
                    return 0;
            }

            // Return translated value and increment bill bank total
            var val = currencyMap[denom];
            totalDenomination += val;
            BillAmount = val;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            Total += val;
            string str = "insert into CG_KioskLogs(ShopId,SessionStartTime,BillAmount,TotalAmount) values(" + ShopId + ",'" + sessionstarttime + "'," + BillAmount + "," + totalDenomination + ")";
            SqlCommand cmd = new SqlCommand(str, conn);
            cmd.ExecuteNonQuery();
            string CabinetId = Settings.Default["CabinetId"].ToString();
            str = "update CG_BillAcceptorOnOff set CurrentDenom=" + val + ",TotalDenom=TotalDenom+" + val + " where BillAcceptorIp='"+ CabinetId + "'";
            cmd = new SqlCommand(str, conn);
            cmd.ExecuteNonQuery();
            str = "update CG_CabinetsLogsMapping set TotalIn=TotalIn+" + val + " where CabinetId='" + CabinetId + "'";
            cmd = new SqlCommand(str, conn);
            cmd.ExecuteNonQuery();
            
            str = "update CG_TempCabinetsLogsMapping set TotalIn=TotalIn+" + val + " where CabinetId='" + CabinetId + "'";
            cmd = new SqlCommand(str, conn);
            cmd.ExecuteNonQuery();


            return val;
        }
        #region Properties
        public int Bill1
        {
            get { return bill1; }
            set
            {
                bill1 = value;
                NotifyPropertyChanged("Bill1");
            }
        }

        public int Bill2
        {
            get { return bill2; }
            set
            {
                bill2 = value;
                NotifyPropertyChanged("Bill2");
            }
        }

        public int Bill3
        {
            get { return bill3; }
            set
            {
                bill3 = value;
                NotifyPropertyChanged("Bill3");
            }
        }

        public int Bill4
        {
            get { return bill4; }
            set
            {
                bill4 = value;
                NotifyPropertyChanged("Bill4");
            }
        }

        public int Bill5
        {
            get { return bill5; }
            set
            {
                bill5 = value;
                NotifyPropertyChanged("Bill5");
            }
        }

        public int Bill6
        {
            get { return bill6; }
            set
            {
                bill6 = value;
                NotifyPropertyChanged("Bill6");
            }
        }

        public int Bill7
        {
            get { return bill7; }
            set
            {
                bill7 = value;
                NotifyPropertyChanged("Bill7");
            }
        }

        public int Total
        {
            get { return total; }
            set
            {
                total = value;
                NotifyPropertyChanged("Total");
            }
        }
        #endregion
        #region Private Helpers
        #endregion
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        #endregion

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        internal class FixedObservableLinkedList<T> : LinkedList<T>, INotifyCollectionChanged
        {
            private readonly object syncObject = new object();

            public int Size { get; private set; }

            public FixedObservableLinkedList(int size)
            {
                Size = size;
            }


            public void Add(T obj)
            {
                AddFirst(obj);
                lock (syncObject)
                {
                    while (Count > Size)
                    {
                        RemoveLast();
                    }
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
            private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
            {

            }
        }

        
        private void clear()
        {
            totalDenomination = 0;
            label1.Text = "0";
        }
        public string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
        public string CreateNewCustomerTest(int InitialCredits, int ShopId)
        {
            counter = 0;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string message = string.Empty;
            int CustId = 0;
            try
            {
                string randomNos = RandomDigits(10);
                string Email = randomNos;
                decimal CustomerTo = 0;
                decimal CustomerBuyTo = 0;
                decimal RewardPoint = 0;
                int CreatedByShopId = ShopId;
                string rsql = "select * from CG_ApplyPromotions where Status=1";
                SqlDataAdapter dapt = new SqlDataAdapter(rsql, conn);
                DataSet ds = new DataSet();
                dapt.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CustomerTo = Convert.ToDecimal(ds.Tables[0].Rows[i]["CustomerBuy"]);
                        CustomerBuyTo = Convert.ToDecimal(ds.Tables[0].Rows[i]["CutomerBuyTo"]);
                        if (InitialCredits >= CustomerTo && InitialCredits <= CustomerBuyTo)
                        {
                            RewardPoint = Convert.ToDecimal(ds.Tables[0].Rows[i]["RewardsPoints"]);
                        }
                    }
                }
               
                string sql = "insert into CG_Customer(Email,CreatedByShopId,Enabled,Locked,InitialCredits,IsCompleted,DashboardStatus,RewardsPoints,Gender,LoginStatus,CreatedDateTime) values";  // after Locked CreatedDateTime
                sql += " ('" + Email + "'," + CreatedByShopId + ",1,0," + InitialCredits + ",1,0," + RewardPoint + ",1,'IN','" + DateTime.Now + "')"; // after 0,'" + DateTime.Now + "'


                SqlCommand cmd = new SqlCommand(sql, conn);
                
                CustId = (int)cmd.ExecuteNonQuery();

                if (CustId > 0)
                {
                    rsql = "update CG_ShopInOutReport set KioskIn=KioskIn +" + InitialCredits + " where ShopId=" + ShopId + "";
                    SqlCommand cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();
                    rsql = "update CG_ShopCurrentCredit set Credits=Credits +" + InitialCredits + " where ShopId=" + ShopId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();
                    rsql = "select DistriutorId from CG_Shop where Id=" + ShopId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    int DistId = Convert.ToInt32(cmdcc.ExecuteScalarAsync());
                    rsql = "update CG_Shop set UserLimit=UserLimit+1 where Id=" + ShopId + "";
                    cmd = new SqlCommand(rsql, conn);
                    cmd.ExecuteNonQueryAsync();
                    rsql = "update CG_DistributorCurrentCredit set Credits=Credits +" + InitialCredits + " where DistributorId=" + DistId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();

                    rsql = "update CG_EmployeeInOutReport set KioskIn=KioskIn +" + InitialCredits + " where ShopId=" + ShopId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();
                    rsql = "update CG_ShopStatus set Kiosk=Kiosk +" + InitialCredits + " where ShopId=" + ShopId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();
                    rsql = "update CG_EmployeeStatus set Kiosk=Kiosk +" + InitialCredits + " where ShopId=" + ShopId + "";
                    cmdcc = new SqlCommand(rsql, conn);
                    cmdcc.ExecuteNonQueryAsync();

                    sql = "select max(Id) from CG_Customer";
                    cmd = new SqlCommand(sql, conn);
                    CustId = (int)cmd.ExecuteScalar();
                    sql = "insert into CG_CustomerDashboard(CustomerId,CurrentBalance,CreditsEarned,CreditsPayout,Profit,TerminalId,WinAmount,CreatedByShopId)";
                    sql += " values(" + CustId + "," + InitialCredits + "," + InitialCredits + ",0,0,0,0," + CreatedByShopId + ")";
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();

                    sql = "insert into CG_CustomerCreditHistory(CustomerId,TerminalId,CurrentCredits,Payouts,Profit,Loss,GameId,CreatedDate)";//CreatedDate,
                    sql += " values(" + CustId + ",0," + InitialCredits + ",0,0,0,0,'" + DateTime.Now + "')"; //'" + DateTime.Now + "'
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();

                    sql = "insert into CG_CustomerCurrentCredit(CustomerId,TerminalId,CreditsEarned,CurrentBalance,CreditsPayout,Profit,WinAmount,CurrentDateTime)"; //,CurrentDateTime
                    sql += " values(" + CustId + ",0," + InitialCredits + "," + InitialCredits + ",0,0,0,'" + DateTime.Now + "')"; //,'" + DateTime.Now + "'
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();

                    sql = "insert into CG_ShopCreditHistory(ShopId,CreditsEarned,CreditsEarnedFromDistributorId,CreditsLoaned,CreditsLoanedToCustomerId,Currency,CurrentDateTime)"; //,CurrentDateTime
                    sql += " values(" + CreatedByShopId + ",0,0," + InitialCredits + "," + CustId + ",'USD','" + DateTime.Now + "')"; //,'" + DateTime.Now + "'
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();
                    string CabinetId = Settings.Default["CabinetId"].ToString();
                    sql = "insert into CG_CabinetCustomerMapping(CabinetId,CustomerId,CreateDatetime) values('"+ CabinetId + "','"+ CustId + "','" + DateTime.Now + "')";
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();
                    sql = "update CG_BillAcceptorOnOff set IsOn = 0,CurrentDenom = 0,IsCreateCustomer = 0 where BillAcceptorIp = '"+ CabinetId +"'";

                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
                //_shopServices.UpdateShopCurrentBalnce(model.CreditsIN, 2);

                //      _shopServices.UpdateShopUserLimit(1, 2);
                validator.Close();
                message = "Ok," + CustId + "," + InitialCredits + "," + DateTime.Now + "," + Email + "," + RewardPoint;
                return message;

            }
            catch (Exception e)
            {
                //var allErrors = ModelState.Values.SelectMany(v => v.Errors);

            }
            return "Success,";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "$" + totalDenomination.ToString();
            if (totalDenomination == 0)
            {
                //button1.Visible = false;
            }
            else
            {
               // button1.Visible = true;
            }
            string CabinetId = Settings.Default["CabinetId"].ToString();
            string myIP = Settings.Default["MyIP"].ToString();

            string ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + CabinetId + "' and IsCreateCustomer=1";
            SqlDataAdapter dapt = new SqlDataAdapter(ss, conn);
            DataSet ds = new DataSet();
            dapt.Fill(ds);
            int Ise = 1;
            if (ds.Tables[0].Rows.Count > 0)
            {
                Ise = Convert.ToInt32(ds.Tables[0].Rows[0]["IsOn"]);
                if (Ise == 0)
                {


                    counter = 0;
                    string denomi = label1.Text.Substring(1);
                    int CreditsIn = Convert.ToInt32(denomi);
                    if (CreditsIn > 0)
                    {

                        int ShopId = Convert.ToInt32(userid.Text);
                        string Result = CreateNewCustomerTest(CreditsIn, ShopId);
                        string[] res = Result.Split(',');
                        string FinalCredits = res[2];
                        string CustomerId = res[4];
                        string CreatedDateTime = res[3];
                        string Rewardpoint = res[5];

                        Ticket tkt = new Ticket();
                        tkt.TicketNo = CustomerId;
                        tkt.ticketDate = DateTime.Now;

                        tkt.amount = int.Parse(FinalCredits);
                        tkt.RewardPoint = int.Parse(Rewardpoint);
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        TotalDenomination = CreditsIn;
                        string sql = "insert into CG_KioskLogs(ShopId,SessionStartTime,BillAmount,TotalAmount,SessionEndTime,CustomerID,SessionResetTime,CabinetId,CabinetIn)";
                        sql += " values(" + ShopId + ",'" + sessionstarttime + "',0," + CreditsIn + ",'" + DateTime.Now + "','" + CustomerId + "','" + DateTime.Now + "','"+ CabinetId +"',"+ CreditsIn +")";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQueryAsync();
                        sql = "update CG_ShopInOutReport set KioskIn = KioskIn+" + CreditsIn + ",CabinetIn=CabinetIn+"+  CreditsIn +" where ShopId=" + ShopId + "";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQueryAsync();
                        //CG_EmployeeInOutReport
                        sql = "update CG_EmployeeInOutReport set KioskIn = KioskIn+" + CreditsIn + ",CabinetIn=CabinetIn+" + CreditsIn + " where ShopId=" + ShopId + "";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQueryAsync();
                        sql = "insert into CG_CabinetsCustomerPerActionLogs(CabinetId,CustomerId,Timestamps,InsertCredit) values('" + CabinetId + "','" + CustomerId + "','" + DateTime.Now + "'," + CreditsIn + ")";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQueryAsync();
                        tkt.print();
                        // MessageBox.Show("Ticket Printed Successfully");


                        validator.Close();
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        clear();
                        this.Hide();
                        this.ShowInTaskbar = false;
                        enjoy frm1 = new enjoy();
                        this.Close();
                        this.Dispose();
                        frm1.ShowDialog();
                       // this.Dispose();

                    }

                    totalDenomination = 0;

                }
            }
            ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + CabinetId + "' and IsOn=0 and  IsCreateCustomer=0";
            dapt = new SqlDataAdapter(ss, conn);
            ds = new DataSet();
            dapt.Fill(ds);
             
            if (ds.Tables[0].Rows.Count > 0)
            {

                validator.Close();
                clear();
                this.Hide();
                this.Close();
                timer1.Enabled = false;
                timer2.Enabled = false;
                this.ShowInTaskbar = false;
                Form1 frm1 = new Form1();
                this.Dispose();
                frm1.ShowDialog();
                //this.Dispose();



            }
        }
        public string LabelText
        {
            get
            {
                return this.userid.Text;
            }
            set
            {
                this.userid.Text = value;
            }
        }
        public static int counter = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter == 120 && totalDenomination == 0)
            {
                //validator.Close();
                //Form1 frm1 = new Form1();
                //this.Hide(); counter = 0;
                //timer2.Enabled = false;
                //frm1.Show();

               
                validator.Close();

                this.Hide();
                this.Close();
                counter = 0;
                timer2.Enabled = false;
                this.ShowInTaskbar = false;
                Form1 frm1 = new Form1();
                this.Dispose();
                frm1.ShowDialog();
               
               // this.Dispose();
            }
        }
    }
}
