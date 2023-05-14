using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.DBcontext.MyEntities
{
    public class UserList
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public DateTime? DateEnter { get; set; }
    }

    public class PermissionList
    {
        public int Id { get; set; }
        public string Sector{ get; set; }
        public bool Read { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }

    }
}
