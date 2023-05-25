using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.DBcontext.MyEntities
{
    public class orderReport
    {
        public string car { get;set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
        public string article { get; set; }
        public string name { get; set; }
        public decimal? price { get; set; }
        public string customer { get; set; }
        public int? countAutoparts { get; set; }
        public DateTime? dateOrder { get; set; }
        public decimal? totalSum { get; set; } 
    }
}
