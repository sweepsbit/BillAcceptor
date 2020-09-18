using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
namespace BillAcceptorTest
{
    class Ticket
    {
        PrintDocument pdoc = null;
        string ticketNo;
        DateTime TicketDate;
        decimal Rewardpoints;
        int Amount;

        public string TicketNo
        {
            //set the person name
            set { this.ticketNo = value; }
            //get the person name 
            get { return this.ticketNo; }
        }
        public DateTime ticketDate
        {
            //set the person name
            set { this.TicketDate = value; }
            //get the person name 
            get { return this.TicketDate; }
        }


        public int amount
        {
            //set the person name
            set { this.Amount = value; }
            //get the person name 
            get { return this.Amount; }
        }

        public decimal RewardPoint
        {
            //set the person name
            set { this.Rewardpoints = value; }
            //get the person name 
            get { return this.Rewardpoints; }
        }

        public Ticket()
        {

        }
        public Ticket(string ticketNo, DateTime TicketDate, int Amount,decimal Rewardpoint)
        {
            this.ticketNo = ticketNo;
            this.TicketDate = TicketDate;
            this.Amount = Amount;
            this.Rewardpoints = Rewardpoint;

        }
        public void print()
        {
            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();
           // PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Courier New", 15);


            PaperSize psize = new PaperSize("Custom", 100, 200);
            //ps.DefaultPageSettings.PaperSize = psize;



            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            //pdoc.DefaultPageSettings.PaperSize.Height =320;
            pdoc.DefaultPageSettings.PaperSize.Height = 410;

            pdoc.DefaultPageSettings.PaperSize.Width = 520;

            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

           // DialogResult result = pd.ShowDialog();
           // if (result == DialogResult.OK)
            //{
                // PrintPreviewDialog pp = new PrintPreviewDialog();
                //  pp.Document = pdoc;
                //  result = pp.ShowDialog();
                // if (result == DialogResult.OK)
                // {
               
                pdoc.Print();
            
            
                // }
           // }

        }
        public void printRedeem()
        {
            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();
            // PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Courier New", 15);


            PaperSize psize = new PaperSize("Custom", 100, 200);
            //ps.DefaultPageSettings.PaperSize = psize;



            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            //pdoc.DefaultPageSettings.PaperSize.Height =320;
            pdoc.DefaultPageSettings.PaperSize.Height = 410;

            pdoc.DefaultPageSettings.PaperSize.Width = 520;

            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPageRedeem);

            // DialogResult result = pd.ShowDialog();
            // if (result == DialogResult.OK)
            //{
            // PrintPreviewDialog pp = new PrintPreviewDialog();
            //  pp.Document = pdoc;
            //  result = pp.ShowDialog();
            // if (result == DialogResult.OK)
            // {

            pdoc.Print();


            // }
            // }

        }
        void pdoc_PrintPageRedeem(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10);
            float fontHeight = font.GetHeight();
            int startX = 0; //50
            int startY = 0; //55
            int Offset = 0; //40
            graphics.DrawString("REDEEM", new Font("Courier New", 14), new SolidBrush(Color.Black), startX, startY + Offset);
            String underLine = "---------------------------";
            Offset = Offset + 20;
            
            graphics.DrawString("ID", new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + this.TicketNo, new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 30;
            graphics.DrawString("Credits  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + this.amount, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            //graphics.DrawString("Ticket Date :" + this.ticketDate, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

           // underLine = "---------------------------";
            //graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 30;
            //graphics.DrawString("Reward Points  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 30;

            //graphics.DrawString("" + this.RewardPoint, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 30;
            //graphics.DrawString("Ticket Date :" + this.ticketDate, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 30;
            // graphics.DrawString("Ticket Date ", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            graphics.DrawString("" + DateTime.Now, new Font("Courier New", 9), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            Image img = Image.FromFile(@"C:\Kiosk\GatorX.png");
            graphics.DrawImage(img, 0, 275, 125, 125);
            // graphics.DrawImage(img, 210, 0, 50, 50);
            //Offset = Offset + 20;
            //String Source = this.source;
            //graphics.DrawString("From " + Source + " To " + Destination, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            //Offset = Offset + 20;
            //String Grosstotal = "Total Amount to Pay = " + this.amount;

            //Offset = Offset + 20;
            //underLine = "------------------------------------------";
            //graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            //graphics.DrawString(Grosstotal, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            //String DrawnBy = this.drawnBy;
            //graphics.DrawString("Conductor - " + DrawnBy, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);





        }
        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10);
            float fontHeight = font.GetHeight();
            int startX = 0; //50
            int startY = 0; //55
            int Offset = 0; //40
            //graphics.DrawString("Welcome to Gator Games", new Font("Courier New", 14), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            graphics.DrawString("ID", new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + this.TicketNo, new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            String underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 30;
            graphics.DrawString("Credits  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + this.amount, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            //graphics.DrawString("Ticket Date :" + this.ticketDate, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            graphics.DrawString("Reward Points  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + this.RewardPoint, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            //graphics.DrawString("Ticket Date :" + this.ticketDate, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 30;
            // graphics.DrawString("Ticket Date ", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            graphics.DrawString("" + DateTime.Now, new Font("Courier New", 9), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            Image img = Image.FromFile(@"C:\Kiosk\GatorX.png");
            graphics.DrawImage(img, 0, 275, 125, 125);
           // graphics.DrawImage(img, 210, 0, 50, 50);
            //Offset = Offset + 20;
            //String Source = this.source;
            //graphics.DrawString("From " + Source + " To " + Destination, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            //Offset = Offset + 20;
            //String Grosstotal = "Total Amount to Pay = " + this.amount;

            //Offset = Offset + 20;
            //underLine = "------------------------------------------";
            //graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            //graphics.DrawString(Grosstotal, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            //String DrawnBy = this.drawnBy;
            //graphics.DrawString("Conductor - " + DrawnBy, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);





        }
    }
}
