using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.DBcontext.MyEntities
{
    public class OrdersView
    {
        public int id { get; set; }
        public int? orderNumber { get; set; }
        public string autopartName { get; set; }
        public DateTime? dateOrder { get; set; }
        public int? countAutopart { get; set; }
        public string customer { get; set; }
        public string address { get; set; }
    }
}
