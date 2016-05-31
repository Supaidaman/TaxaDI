using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICurve
{
    public class CurrentDateAndTax
    {
        public CurrentDateAndTax(DateTime date, string tax)
    {
        this.data = date;
        this.taxa = tax;
    }
        public DateTime data;
        public DateTime Data { get { return data; } set { data = value; } }


        public string taxa;
        public string Taxa { get { return taxa; } set { taxa = value; } }
    
    }
}
