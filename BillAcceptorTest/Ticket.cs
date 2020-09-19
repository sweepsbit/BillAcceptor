using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
namespace BillAcceptorTest
{
    public class Ticket
    {
        public string TicketNo { get; }
        private PrintDocument printDocument;
        private DateTime ticketDate;
        private readonly decimal rewardPoints;
        public int Amount { get; }

        /// <summary>
        /// Initializes <see cref="Ticket"/>
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <param name="ticketDate"></param>
        /// <param name="amount"></param>
        /// <param name="rewardPoint"></param>
        public Ticket(string ticketNo, DateTime ticketDate, int amount, decimal rewardPoint)
        {
            TicketNo = ticketNo;
            this.ticketDate = ticketDate;
            Amount = amount;
            rewardPoints = rewardPoint;

        }
        public void Print()
        {
            var pd = new PrintDialog();
            printDocument = new PrintDocument();
            // PrinterSettings ps = new PrinterSettings();
            var font = new Font("Courier New", 15);

            var paperSize = new PaperSize("Custom", 100, 200);
            //ps.DefaultPageSettings.PaperSize = psize;

            pd.Document = printDocument;
            pd.Document.DefaultPageSettings.PaperSize = paperSize;
            //pdoc.DefaultPageSettings.PaperSize.Height =320;
            printDocument.DefaultPageSettings.PaperSize.Height = 410;

            printDocument.DefaultPageSettings.PaperSize.Width = 520;

            printDocument.PrintPage += pdoc_PrintPage;

            // DialogResult result = pd.ShowDialog();
            // if (result == DialogResult.OK)
            //{
            // PrintPreviewDialog pp = new PrintPreviewDialog();
            //  pp.Document = pdoc;
            //  result = pp.ShowDialog();
            // if (result == DialogResult.OK)
            // {

            printDocument.Print();


            // }
            // }

        }
        public void printRedeem()
        {
            var pd = new PrintDialog();
            // PrinterSettings ps = new PrinterSettings();
            var font = new Font("Courier New", 15);
            var psize = new PaperSize("Custom", 100, 200);
            //ps.DefaultPageSettings.PaperSize = psize;

            printDocument = new PrintDocument();
            printDocument.DefaultPageSettings.PaperSize.Height = 410;
            printDocument.DefaultPageSettings.PaperSize.Width = 520;

            pd.Document = printDocument;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            //pdoc.DefaultPageSettings.PaperSize.Height =320;

            printDocument.PrintPage += pdoc_PrintPageRedeem;

            printDocument.Print();

        }
        void pdoc_PrintPageRedeem(object sender, PrintPageEventArgs e)
        {
            var graphics = e.Graphics;
            var font = new Font("Courier New", 10);
            var fontHeight = font.GetHeight();
            var startX = 0; //50
            var startY = 0; //55
            var offset = 0; //40
            graphics.DrawString("REDEEM", new Font("Courier New", 14), new SolidBrush(Color.Black), startX, startY + offset);
            var underLine = "---------------------------";
            offset += 20;

            graphics.DrawString("ID", new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;

            graphics.DrawString("" + TicketNo, new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;
            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);

            offset += 30;
            graphics.DrawString("Credits  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;

            graphics.DrawString("" + Amount, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;
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
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);

            offset += 30;
            // graphics.DrawString("Ticket Date ", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            graphics.DrawString("" + DateTime.Now, new Font("Courier New", 9), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);
            offset += 30;

            var img = Image.FromFile(@"C:\Kiosk\GatorX.png");
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
            var graphics = e.Graphics;
            var font = new Font("Courier New", 10);
            var fontHeight = font.GetHeight();
            var startX = 0; //50
            var startY = 0; //55
            var Offset = 0; //40
            //graphics.DrawString("Welcome to Gator Games", new Font("Courier New", 14), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            graphics.DrawString("ID", new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + TicketNo, new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            var underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 30;
            graphics.DrawString("Credits  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + Amount, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            //graphics.DrawString("Ticket Date :" + this.ticketDate, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;

            underLine = "---------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;
            graphics.DrawString("Reward Points  ", new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            graphics.DrawString("" + rewardPoints, new Font("Courier New", 14, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
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

            var img = Image.FromFile(@"C:\Kiosk\GatorX.png");
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
