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

namespace DICurve
{
    public partial class Form1 : Form
    {
        private ProgressBar progressBar;
        public Form1()
        {
           
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            datesList = new List<CurrentDateAndTax>();
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
            MessageBox.Show("Clicou no primeiro botão! A data é :" + time);
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

            //using (var client = new WebClient())
            //{
            //    client.DownloadFile("http://i.imgur.com/dAXfnw2.jpg", "a.jpg");
            //}


            using (WebClient wc = new WebClient())
            {
                time = dateTimePicker1.Value.ToString("yyMMdd");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                wc.DownloadFile(new System.Uri("http://www.bmf.com.br/ftp/ContratosPregaoFinal/BF" + time + ".ex_"),
                "BF" + time + ".exe");
               // wc.DownloadDataAsync(new System.Uri("http://i.imgur.com/dAXfnw2.jpg"),"F\\DIData");
                MessageBox.Show("Foi");
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
            //String line;
            //FileStream currentFile = new FileStream("BD_Final.txt", FileMode.Open, FileAccess.Read);
          //  StreamReader currentFile = new StreamReader("BD_Final.txt");
            string[] lines = System.IO.File.ReadAllLines("BD_Final.txt");
            //tem 5 colunas, separar cada linha nessas cinco
           // while ((line = currentFile.ReadLine()) != null)
            foreach (string line in lines)
            {
                //divide into columns
                string code = line.Substring(21, 3);
                string[] columns = line.Split(' ');
                if (code=="DI1")
                { 
                    calculateTax(line); 
                }
                   
                //foreach (string column in columns)
                //{
                //    //if (column.StartsWith("D1"))
                //    //{
                //    //   // Console.WriteLine("\t" + line);
                //    //    string teste = line.Substring(36, 8);
                //    //    //Console.WriteLine(teste);
                //    //    calculateTax(line);
                //    //}
                //    ////emerc data1data2+numbers+numbers
                //    /*looking into the full string...:
                //      dias uteis ate vencimento: começa no 389, vai até 387
                //     * data da taxa: 37,44 -> VEM COMO ENTRADA, SERA USADA PRA ACHAR
                //     * cotação ajuste: 232, 244
                //     */
                    
                //}
                
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

        private void calculateTax(string line)
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
            //data da taxa: 37,44
           // currentDateAndTaxBindingSource.Add(new CurrentDateAndTax(data.ToShortDateString(), result + "%"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // x is a date... how to go through it?
            //interpolação metodo linear
            //ideia: achar os dois valores mais proximos: data menor e data maior do que essa. de algum jeito...
            //distancia entre as datas. se tiver distancia menor que a atual e for menor. se tiver distancia menor que a atual e for maior
        }


        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

    }
}
