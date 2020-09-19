using System;
using System.Collections.Generic;
using PyramidNETRS232;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using BillAcceptorTest.Properties;
using System.Configuration;

namespace BillAcceptorTest
{
    public partial class Form2 : Form
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

        private PyramidAcceptor validator;
        private RS232Config config;

        private readonly FixedObservableLinkedList<DebugBufferEntry> debugQueueMaster =
            new FixedObservableLinkedList<DebugBufferEntry>(20);

        private readonly FixedObservableLinkedList<DebugBufferEntry> debugQueueSlave =
            new FixedObservableLinkedList<DebugBufferEntry>(20);

        private int bill1 = 0;
        private int bill2 = 0;
        private int bill3 = 0;
        private int bill4 = 0;
        private int bill5 = 0;
        private int bill6 = 0;
        private int bill7 = 0;
        private int total = 0;

        private const string PortName = "COM100";

        private static readonly Dictionary<int, int> CurrencyMap = new Dictionary<int, int>
        {
            {1, 1},
            {2, 2},
            {3, 5},
            {4, 10},
            {5, 20},
            {6, 50},
            {7, 100}
        };

        private static int TotalDenomination = 0;

        private readonly DateTime sessionStartTime;
        private int customerId;
        private int shopId;
        private decimal amount;

        public Form2()
        {
            InitializeComponent();
            sessionStartTime = DateTime.Now;
        }

        private static async Task<SqlConnection> GetConnection()
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            await sqlConnection.OpenAsync();
            return sqlConnection;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            userid.Text = Settings.Default["ShopId"].ToString(); //Form1.AShopID.ToString();
            shopId = Convert.ToInt32(Settings.Default["ShopId"]);
            //MessageBox.Show(userid.Text);
            timer2.Enabled = true;
            timer1.Enabled = true;

            if (IsConnected)
            {
                validator.Close();
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

        private bool isConnected = false;

        private bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                NotifyPropertyChanged("IsConnected");
                NotifyPropertyChanged("IsNotConnected");
            }
        }

        private async void validator_OnCredit(object sender, CreditArgs e)
        {
            _counter = 0;
            var denomination = e.Index;
            if (!CurrencyMap.TryGetValue(denomination, out _)) return;

            var creditAdded = await AddCredit(denomination);
            Console.WriteLine($"Credited {creditAdded}");
        }

        private async Task<int> AddCredit(int denomination)
        {
            switch (denomination)
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
            var val = CurrencyMap[denomination];
            TotalDenomination += val;
            amount = val;
            Total += val;

            using (var conn = await GetConnection())
            {
                var str = "insert into CG_KioskLogs(ShopId,SessionStartTime,BillAmount,TotalAmount) values(" + shopId +
                          ",'" + sessionStartTime + "'," + amount + "," + TotalDenomination + ")";

                using (var cmd = new SqlCommand(str, conn))
                    await cmd.ExecuteNonQueryAsync();

                var cabinetId = Settings.Default["CabinetId"].ToString();

                str = "update CG_BillAcceptorOnOff set CurrentDenom=" + val + ",TotalDenom=TotalDenom+" + val +
                      " where BillAcceptorIp='" + cabinetId + "'";

                using (var cmd = new SqlCommand(str, conn))
                    await cmd.ExecuteNonQueryAsync();

                str = "update CG_CabinetsLogsMapping set TotalIn=TotalIn+" + val + " where CabinetId='" + cabinetId +
                      "'";
                using (var cmd = new SqlCommand(str, conn))
                    await cmd.ExecuteNonQueryAsync();

                // CG_TempCabinetsLogsMapping
                str = "update CG_TempCabinetsLogsMapping set TotalIn=TotalIn+" + val + " where CabinetId='" +
                      cabinetId +
                      "'";
                using (var cmd = new SqlCommand(str, conn))
                    await cmd.ExecuteNonQueryAsync();
            }

            return val;
        }

        #region Properties

        private int Bill1
        {
            get => bill1;
            set
            {
                bill1 = value;
                NotifyPropertyChanged("Bill1");
            }
        }

        private int Bill2
        {
            get => bill2;
            set
            {
                bill2 = value;
                NotifyPropertyChanged("Bill2");
            }
        }

        private int Bill3
        {
            get => bill3;
            set
            {
                bill3 = value;
                NotifyPropertyChanged("Bill3");
            }
        }

        private int Bill4
        {
            get => bill4;
            set
            {
                bill4 = value;
                NotifyPropertyChanged("Bill4");
            }
        }

        private int Bill5
        {
            get => bill5;
            set
            {
                bill5 = value;
                NotifyPropertyChanged("Bill5");
            }
        }

        private int Bill6
        {
            get => bill6;
            set
            {
                bill6 = value;
                NotifyPropertyChanged("Bill6");
            }
        }

        private int Bill7
        {
            get => bill7;
            set
            {
                bill7 = value;
                NotifyPropertyChanged("Bill7");
            }
        }

        public int Total
        {
            get => total;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Clear()
        {
            TotalDenomination = 0;
            label1.Text = "0";
        }

        private static string RandomDigits(int length)
        {
            var random = new Random();
            var s = string.Empty;
            for (var i = 0; i < length; i++)
                s = string.Concat(s, random.Next(10).ToString());
            return s;
        }


        /// <summary>
        /// Creates Customer
        /// </summary>
        /// <param name="initialCredits"></param>
        /// <param name="customerShopId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private async Task<(bool success, int customerId, int initialCredits, DateTime time, string email, decimal rewardPoint)>
            CreateNewCustomerTest(int initialCredits, int customerShopId, SqlConnection conn)
        {
            _counter = 0;
            try
            {
                var randomNos = RandomDigits(10);
                var email = randomNos;
                decimal rewardPoint = 0;
                var createdByShopId = customerShopId;
                var rawSqlQuery = "select * from CG_ApplyPromotions where Status=1";
                using (var dataAdapter = new SqlDataAdapter(rawSqlQuery, conn))
                {
                    var ds = new DataSet();
                    dataAdapter.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var customerTo = Convert.ToDecimal(ds.Tables[0].Rows[i]["CustomerBuy"]);
                            var customerBuyTo = Convert.ToDecimal(ds.Tables[0].Rows[i]["CutomerBuyTo"]);
                            if (initialCredits >= customerTo && initialCredits <= customerBuyTo)
                            {
                                rewardPoint = Convert.ToDecimal(ds.Tables[0].Rows[i]["RewardsPoints"]);
                            }
                        }
                    }
                }

                var sql = "insert into CG_Customer(Email,CreatedByShopId,Enabled,Locked,InitialCredits,IsCompleted,DashboardStatus,RewardsPoints,Gender,LoginStatus,CreatedDateTime) values"; // after Locked CreatedDateTime
                sql += " ('" + email + "'," + createdByShopId + ",1,0," + initialCredits + ",1,0," + rewardPoint +
                       ",1,'IN','" + DateTime.Now + "')"; // after 0,'" + DateTime.Now + "'


                int custId;
                using (var cmd = new SqlCommand(sql, conn))
                    custId = await cmd.ExecuteNonQueryAsync();

                if (custId > 0)
                {
                    rawSqlQuery = "update CG_ShopInOutReport set KioskIn=KioskIn +" + initialCredits +
                                  " where ShopId=" +
                                  customerShopId + "";
                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                        await cmdcc.ExecuteNonQueryAsync();

                    rawSqlQuery = "update CG_ShopCurrentCredit set Credits=Credits +" + initialCredits +
                                  " where ShopId=" +
                                  customerShopId + "";

                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                        await cmdcc.ExecuteNonQueryAsync();

                    rawSqlQuery = "select DistriutorId from CG_Shop where Id=" + customerShopId + "";
                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                    {
                        var distId = Convert.ToInt32(await cmdcc.ExecuteScalarAsync());
                        rawSqlQuery = "update CG_Shop set UserLimit=UserLimit+1 where Id=" + customerShopId + "";
                        using (var cmd = new SqlCommand(rawSqlQuery, conn))
                        {
                            await cmd.ExecuteNonQueryAsync();
                            rawSqlQuery = "update CG_DistributorCurrentCredit set Credits=Credits +" + initialCredits +
                                          " where DistributorId=" + distId + "";
                        }

                        using (var updateDistributorCurrentCredit = new SqlCommand(rawSqlQuery, conn))
                            await updateDistributorCurrentCredit.ExecuteNonQueryAsync();
                    }


                    rawSqlQuery = "update CG_EmployeeInOutReport set KioskIn=KioskIn +" + initialCredits +
                                  " where ShopId=" +
                                  customerShopId + "";

                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                        await cmdcc.ExecuteNonQueryAsync();

                    rawSqlQuery = "update CG_ShopStatus set Kiosk=Kiosk +" + initialCredits + " where ShopId=" +
                                  customerShopId + "";
                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                        await cmdcc.ExecuteNonQueryAsync();

                    rawSqlQuery = "update CG_EmployeeStatus set Kiosk=Kiosk +" + initialCredits + " where ShopId=" +
                                  customerShopId +
                                  "";
                    using (var cmdcc = new SqlCommand(rawSqlQuery, conn))
                        await cmdcc.ExecuteNonQueryAsync();

                    sql = "select max(Id) from CG_Customer";
                    using (var cmd = new SqlCommand(sql, conn))
                        custId = (int)await cmd.ExecuteScalarAsync();

                    sql =
                        "insert into CG_CustomerDashboard(CustomerId,CurrentBalance,CreditsEarned,CreditsPayout,Profit,TerminalId,WinAmount,CreatedByShopId)";
                    sql += " values(" + custId + "," + initialCredits + "," + initialCredits + ",0,0,0,0," +
                           createdByShopId + ")";

                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();

                    sql =
                        "insert into CG_CustomerCreditHistory(CustomerId,TerminalId,CurrentCredits,Payouts,Profit,Loss,GameId,CreatedDate)"; //CreatedDate,
                    sql += " values(" + custId + ",0," + initialCredits + ",0,0,0,0,'" + DateTime.Now +
                           "')"; //'" + DateTime.Now + "'

                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();

                    sql =
                        "insert into CG_CustomerCurrentCredit(CustomerId,TerminalId,CreditsEarned,CurrentBalance,CreditsPayout,Profit,WinAmount,CurrentDateTime)"; //,CurrentDateTime
                    sql += " values(" + custId + ",0," + initialCredits + "," + initialCredits + ",0,0,0,'" +
                           DateTime.Now + "')"; //,'" + DateTime.Now + "'
                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();

                    sql =
                        "insert into CG_ShopCreditHistory(ShopId,CreditsEarned,CreditsEarnedFromDistributorId,CreditsLoaned,CreditsLoanedToCustomerId,Currency,CurrentDateTime)"; //,CurrentDateTime
                    sql += " values(" + createdByShopId + ",0,0," + initialCredits + "," + custId + ",'USD','" +
                           DateTime.Now + "')"; //,'" + DateTime.Now + "'
                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();

                    var cabinetId = Settings.Default["CabinetId"].ToString();
                    sql = "insert into CG_CabinetCustomerMapping(CabinetId,CustomerId,CreateDatetime) values('" +
                          cabinetId + "','" + custId + "','" + DateTime.Now + "')";
                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();

                    sql =
                        "update CG_BillAcceptorOnOff set IsOn = 0,CurrentDenom = 0,IsCreateCustomer = 0 where BillAcceptorIp = '" +
                        cabinetId + "'";

                    using (var cmd = new SqlCommand(sql, conn))
                        await cmd.ExecuteNonQueryAsync();
                }

                validator.Close();
                return (true, custId, initialCredits, DateTime.Now, email, rewardPoint);
            }
            catch (Exception)
            {
                //var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return (false, default, default, default, default, default);
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "$" + TotalDenomination.ToString();

            var cabinetId = Settings.Default["CabinetId"].ToString();

            var ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + cabinetId +
                     "' and IsCreateCustomer=1";

            using (var conn = await GetConnection())
            {
                using (var dataAdapter = new SqlDataAdapter(ss, conn))
                {
                    var ds = new DataSet();
                    dataAdapter.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var ise = Convert.ToInt32(ds.Tables[0].Rows[0]["IsOn"]);
                        if (ise == 0)
                        {
                            _counter = 0;
                            var creditsIn = TotalDenomination;
                            if (creditsIn > 0)
                            {
                                var customerShopId = Convert.ToInt32(userid.Text);

                                var (success, custId, initialCredits, dateTime, _, rewardPoint) =
                                    await CreateNewCustomerTest(creditsIn, customerShopId, conn);

                                if (!success)
                                {
                                    //Todo: Display some error message
                                    return;
                                }

                                var tkt = new Ticket(custId.ToString(), dateTime, initialCredits, rewardPoint);

                                var sql =
                                    "insert into CG_KioskLogs(ShopId,SessionStartTime,BillAmount,TotalAmount,SessionEndTime,CustomerID,SessionResetTime,CabinetId,CabinetIn)";
                                sql += " values(" + customerShopId + ",'" + sessionStartTime + "',0," + creditsIn +
                                       ",'" +
                                       DateTime.Now + "','" + customerId + "','" + DateTime.Now + "','" + cabinetId +
                                       "'," +
                                       creditsIn + ")";

                                using (var cmd = new SqlCommand(sql, conn))
                                    await cmd.ExecuteNonQueryAsync();

                                sql = "update CG_ShopInOutReport set KioskIn = KioskIn+" + creditsIn +
                                      ",CabinetIn=CabinetIn+" + creditsIn + " where ShopId=" + customerShopId + "";
                                using (var cmd = new SqlCommand(sql, conn))
                                    await cmd.ExecuteNonQueryAsync();

                                //CG_EmployeeInOutReport
                                sql = "update CG_EmployeeInOutReport set KioskIn = KioskIn+" + creditsIn +
                                      ",CabinetIn=CabinetIn+" + creditsIn + " where ShopId=" + customerShopId + "";
                                using (var cmd = new SqlCommand(sql, conn))
                                    await cmd.ExecuteNonQueryAsync();

                                sql =
                                    "insert into CG_CabinetsCustomerPerActionLogs(CabinetId,CustomerId,Timestamps,InsertCredit) values('" +
                                    cabinetId + "','" + customerId + "','" + DateTime.Now + "'," + creditsIn + ")";
                                using (var cmd = new SqlCommand(sql, conn))
                                    await cmd.ExecuteNonQueryAsync();

                                tkt.Print();

                                validator.Close();
                                timer1.Enabled = false;
                                timer2.Enabled = false;
                                Clear();
                                Hide();
                                ShowInTaskbar = false;
                                using (var frm1 = new enjoy())
                                {
                                    Close();
                                    Dispose();
                                    frm1.ShowDialog();
                                }
                            }

                            TotalDenomination = 0;
                        }
                    }
                }

                ss = "select * from CG_BillAcceptorOnOff where BillAcceptorIp='" + cabinetId +
                     "' and IsOn=0 and  IsCreateCustomer=0";

                using (var dataAdapter = new SqlDataAdapter(ss, conn))
                {
                    var ds = new DataSet();
                    dataAdapter.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        validator.Close();
                        Clear();
                        Hide();
                        Close();
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        ShowInTaskbar = false;
                        using (var frm1 = new Form1())
                        {
                            frm1.ShowDialog();
                            Dispose();
                        }
                    }
                }
            }
        }

        private static int _counter = 0;

        private void timer2_Tick(object sender, EventArgs e)
        {
            _counter++;
            if (_counter != 120 || TotalDenomination != 0) return;

            validator.Close();

            Hide();
            Close();
            _counter = 0;
            timer2.Enabled = false;
            ShowInTaskbar = false;
            var frm1 = new Form1();
            Dispose();
            frm1.ShowDialog();

            // this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Used before
            //Now customer gets created from Timer1_Tick
            throw new NotImplementedException();
        }
    }
}