using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IISAutoParts.Class
{
    public static class FrameController
    {
        public static Frame MainFrame { get; set; }
    }
    public static class UserController { 
        public static int userId { get; set; }
        public static string userName { get; set; }
        public static bool isAdmin { get; set; }
        public static List<PermissionList> permissionList { get; set; }
    }
}
