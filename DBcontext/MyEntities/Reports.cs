﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.DBcontext.MyEntities
{
    public class ReportsView
    {
        public int id { get; set; }
        public int numberDoc{ get; set; }
        public string dateInterval { get; set; }
        public int customer { get; set; }

    }
}
