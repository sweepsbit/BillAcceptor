using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BillAcceptorTest
{
    public partial class enjoy : Form
    {
        public enjoy()
        {
            InitializeComponent();
        }

        private void enjoy_Load(object sender, EventArgs e)
        {
            
        }
        public static int ctr=0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            ctr++;
            if (ctr >= 5)
            {
                ctr = 0;
                timer1.Enabled = false;
                timer1.Stop();
               
                this.Hide();
                this.ShowInTaskbar = false;
                Form1 frm1 = new Form1();
                this.Close();
                this.Dispose();
                frm1.ShowDialog();
                //this.Dispose();
            }
        }
    }
}
