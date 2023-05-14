using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.DBcontext.MyEntities
{
    public class ReportsView
    {
        public int id { get; set; }
        public DateTime? dateBegin { get; set; }
        public DateTime? dateEnd { get; set; }
        public int? customerId { get; set; }
        public string customerName { get; set; }


    }
}
