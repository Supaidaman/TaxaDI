using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DICurve
{
    public partial class Form1 : Form
    {
      
        public Form1()
        {
           
            InitializeComponent();
        }
        private static Chart chart1;

        public static Chart Chart1
        {
            get { return Form1.chart1; }
            private set { Form1.chart1 = value; }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void SetMyCustomFormat()
        {
            // Set the Format type and the CustomFormat string.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/yyyy";
        }
        List<CurrentDateAndTax> datesList;
        //public static List<CurrentDateAndTax> datesListRef;
       
        //HELPER STRUCT
        public  struct DaysPlusTax{
            int days;

            public int Days
            {
                get { return days; }
                set { days = value; }
            }
            double tax;

            public double Tax
            {
                get { return tax; }
                set { tax = value; }
            }

            public DaysPlusTax(int days, double tax)
            {
                this.days = days;
                this.tax = tax;
            }
        }


        public static  List<DaysPlusTax> listToPlot;
        private void button1_Click(object sender, EventArgs e)
        {
            datesList = new List<CurrentDateAndTax>();
            listToPlot = new List<DaysPlusTax>();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            //method that will download data from current time from the time picker.
            //LOOK INTO:
            //HOW TO DOWNLOAD DATA
            //HOW TO GET DATA FROM OTHER COMPONENTS FROM THE FORM. 
            //- OK, just call them, like unity
            //Inserting objects into the data grid.
            //example of simple one below...
            //the tax is from http://www.bmf.com.br/ftp/ContratosPregaoFinal/BFAAMMDD.ex_
            //rename it to .exe, then execute it
            // open an BD_Final.txt then execute the weird calc on line with DI1
            downloadArchive();
            String time = dateTimePicker1.Value.ToString("yyMMdd") ;
            MessageBox.Show("Arquivo Carregado.");
        }
        BindingSource bs = new BindingSource(); 
       
        private void Form1_Load(object sender, EventArgs e)
        {
            bs.DataSource = typeof(DateAndTaxString);
           // downloadArchive();
          //  bs.Add(new CurrentDateAndTax("22/04/2016", 8));
           // bs.Add(new CurrentDateAndTax("22/05/2016", 8));
            //bs.Add(new CurrentDateAndTax("22/06/2016", 8));
           //dataGridView1.DataSource = bs;
          //  dataGridView1.AutoGenerateColumns = true;

        }

        private void downloadArchive()
        {
            String time;

        
            using (WebClient wc = new WebClient())
            {
                time = dateTimePicker1.Value.ToString("yyMMdd");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                wc.DownloadFile(new System.Uri("http://www.bmf.com.br/ftp/ContratosPregaoFinal/BF" + time + ".ex_"),
                "BF" + time + ".exe");
               //  MessageBox.Show("Foi");
            }
            //Executa .exe
            Process proc = new Process();
            Process.Start("BF" + time + ".exe");
            //Abre arquivo texto
            openTextFile();
        }

        private void openTextFile()
        {
            int milliseconds = 2000;
            Thread.Sleep(milliseconds);
           string[] lines = System.IO.File.ReadAllLines("BD_Final.txt");
             foreach (string line in lines)
            {
                //divide into columns
                string code = line.Substring(21, 3);
                if (code=="DI1")
                { 
                    calculateTax(line); 
                }
                   
              
                    
             
                
            }
            dataGridView1.DataSource = bs;
            //datesList.Sort();
            var list = datesList.OrderBy(x =>x.Data.Date).ToList(); 

            dataGridView1.AutoGenerateColumns = true;
           
            //dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending);
            foreach(CurrentDateAndTax i in list)
            {
                bs.Add(new DateAndTaxString(i.Data.ToShortDateString(),i.Taxa));
            }
        
        }

        private  void calculateTax(string line)
        {
            string format = "yyyyMMdd";
            double cotacao= Double.Parse(line.Substring(231,13));
            double dias = Double.Parse(line.Substring(388, 5));
          //  MessageBox.Show(line.Substring(231, 13) + " " + line.Substring(388, 5));
            double exp = (252 / dias);
            double div = (10000000 / cotacao);
            double taxaDI = (Math.Pow(div, exp) - 1) * 100; //porcentagem;
            string result = string.Format("{0:0.00}", taxaDI);
           // MessageBox.Show(taxaDI.ToString());
            DateTime data = DateTime.ParseExact(line.Substring(36, 8), format, CultureInfo.InvariantCulture);
            //MessageBox.Show(data.ToShortDateString());
            datesList.Add(new CurrentDateAndTax(data,result));
            listToPlot.Add( new DaysPlusTax((int)dias, taxaDI));
            //data da taxa: 37,44
            //interpolate with dateDate.Ticks -> i think that this will work?;
           // currentDateAndTaxBindingSource.Add(new CurrentDateAndTax(data.ToShortDateString(), result + "%"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // x is a date... how to go through it?
            //interpolação metodo linear
            //ideia: achar os dois valores mais proximos: data menor e data maior do que essa. de algum jeito...
            //distancia entre as datas. se tiver distancia menor que a atual e for menor. se tiver distancia menor que a atual e for maior
            var list = datesList.OrderBy(x => x.Data.Date).ToList();
            long max = long.MaxValue;
            long max2 = long.MaxValue;
            long index = 0;
            long index2 = 0;
            double tax=0;
            double tax2=0;
            foreach (CurrentDateAndTax i in list)
            {
                if((i.Data.Ticks< dateTimePicker2.Value.Ticks) && ((dateTimePicker2.Value.Ticks- i.Data.Ticks) < max))
                {
                    max = dateTimePicker2.Value.Ticks - i.Data.Ticks;
                    index = i.Data.Ticks;
                    tax = Double.Parse(datesList[datesList.IndexOf(i)].Taxa);
                }
                if ((i.Data.Ticks > dateTimePicker2.Value.Ticks) && (( i.Data.Ticks - dateTimePicker2.Value.Ticks  ) < max2))
                {
                    max2 = dateTimePicker2.Value.Ticks - i.Data.Ticks;
                    index2 = i.Data.Ticks;
                    tax2 = Double.Parse(datesList[datesList.IndexOf(i)].Taxa);
                }

              
            }
            double newTax = linear(dateTimePicker2.Value.Ticks, index, index2, tax, tax2);
          //  MessageBox.Show(newTax.ToString());
            textBox1.Text = string.Format("{0:0.00}", newTax) + "%";
            double businessDays = GetDays(dateTimePicker1.Value, dateTimePicker2.Value);
            textBox2.Text = string.Format("{0:0}", businessDays);

        }
        //got this formula from http://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates
        public static double GetDays(DateTime startD, DateTime endD)
        {
            double bussinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) bussinessDays--;
            if ((int)startD.DayOfWeek == 0) bussinessDays--;

            return bussinessDays;
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        //y = c * (d/c)^((x-a) / (b-a))-> Exponential
        // Let x be between a and b. Let y be between c and d.
        //reference: http://www.pmean.com/10/ExponentialInterpolation.html

        static public double exp(double x, double x0, double x1, double y0, double y1)
        {
            double division = ((x-x0)/(x1-x0));
           double expFactor = Math.Pow((y1/y0),(division));
            return (y0 * expFactor);
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
           // Random rdn = new Random();
           // datesListRef = datesList.OrderBy(x => x.Data.Date).ToList();
           // listToPlot = listToPlot.OrderBy(x => dias)
             //datesList.OrderBy(x => x.data)
            //listToPlot.OrderBy<DaysPlusTax>();
           // listToPlot.OrderBy(x=> x.)
            listToPlot = listToPlot.OrderBy(x => x.Days).ToList();
            new ChartForm().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var list = datesList.OrderBy(x => x.Data.Date).ToList();
            long max = long.MaxValue;
            long max2 = long.MaxValue;
            long index = 0;
            long index2 = 0;
            double tax = 0;
            double tax2 = 0;
            foreach (CurrentDateAndTax i in list)
            {
                if ((i.Data.Ticks < dateTimePicker2.Value.Ticks) && ((dateTimePicker2.Value.Ticks - i.Data.Ticks) < max))
                {
                    max = dateTimePicker2.Value.Ticks - i.Data.Ticks;
                    index = i.Data.Ticks;
                    tax = Double.Parse(datesList[datesList.IndexOf(i)].Taxa);
                }
                if ((i.Data.Ticks > dateTimePicker2.Value.Ticks) && ((i.Data.Ticks - dateTimePicker2.Value.Ticks) < max2))
                {
                    max2 = dateTimePicker2.Value.Ticks - i.Data.Ticks;
                    index2 = i.Data.Ticks;
                    tax2 = Double.Parse(datesList[datesList.IndexOf(i)].Taxa);
                }


            }
            double newTax = exp(dateTimePicker2.Value.Ticks, index, index2, tax, tax2);
            //  MessageBox.Show(newTax.ToString());
            textBox1.Text = string.Format("{0:0.00}", newTax) + "%";
            double businessDays = GetDays(dateTimePicker1.Value, dateTimePicker2.Value);
            textBox2.Text = string.Format("{0:0}", businessDays);

        }

       
        
    }
}
