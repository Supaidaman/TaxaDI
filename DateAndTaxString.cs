using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICurve
{
    public class DateAndTaxString
    {
        public DateAndTaxString(string date, string tax)
        {
            this.data = date;
            this.taxa = tax + "%";
        }
        public string data;
        public string Data { get { return data; } set { data = value; } }


        public string taxa;
        public string Taxa { get { return taxa; } set { taxa = value; } }

    }
}
