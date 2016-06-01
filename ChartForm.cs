using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DICurve
{

    public partial class ChartForm : Form
    {
        // private System.ComponentModel.IContainer components = null;
        //System.Windows.Forms.DataVisualization.Charting.Chart chart1 = new Chart();
        public ChartForm()
        {
            InitializeComponent();
        }
      
        private void ChartForm_Load(object sender, EventArgs e)
        {
            Random rdn = new Random();
            //for (int i = 0; i < 50; i++)
            //{
            //    chart1.Series["Series2"].Points.AddXY
            //                    (rdn.Next(0, 10), rdn.Next(0, 10));
            //    //chart1.Series["test2"].Points.AddXY
            //      //              (rdn.Next(0, 10), rdn.Next(0, 10));
            //}

            foreach (DICurve.Form1.DaysPlusTax i in Form1.listToPlot)
            {
                chart1.Series["Series2"].Points.AddXY(i.Days,i.Tax);
                
            }
        }
    }
}
