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
           
            try
            {
                foreach (DICurve.Form1.DaysPlusTax i in Form1.listToPlot)
                {

                    chart1.Series["TaxaDI"].Points.AddXY(i.Days, i.Tax);

                }
            }
            catch(NullReferenceException exc)
            {
                MessageBox.Show("Curva Inválida");
            }
        }
    }
}
