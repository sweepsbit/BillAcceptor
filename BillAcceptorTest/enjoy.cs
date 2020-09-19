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

        private static int _ctr = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            _ctr++;
            if (_ctr >= 5)
            {
                _ctr = 0;
                timer1.Enabled = false;
                timer1.Stop();

                Hide();
                ShowInTaskbar = false;
                using (var frm1 = new Form1())
                {
                    Close();
                    Dispose();
                    frm1.ShowDialog();
                }
            }
        }
    }
}
