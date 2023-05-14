using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для userPermissionPage.xaml
    /// </summary>
    public partial class userPermissionPage : Page
    {
        IISAutoPartsEntities _dbContext = new IISAutoPartsEntities();

        int userId = 0;

        public userPermissionPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            loadPermission();
        }

        private void loadPermission()
        {
            var _permissions = _dbContext.users.Where(x => x.id == userId).Select(x => x.permission).FirstOrDefault();

            List<PermissionList> permissionList = new List<PermissionList>();

            XDocument xdoc = XDocument.Parse(_permissions);
            XElement permission = xdoc.Element("permission");
            IsAdminChb.IsChecked = Boolean.Parse(permission.Elements("admin").FirstOrDefault().Value);
            foreach (XElement section in permission.Elements("section"))
            {

                XAttribute name = section.Attribute("name");
                XElement read = section.Element("read");
                XElement add = section.Element("add");
                XElement edit = section.Element("edit");
                XElement delete = section.Element("delete");

                permissionList.Add(new PermissionList
                {
                    Sector = name.Value,
                    Read = Boolean.Parse(read.Value),
                    Add = Boolean.Parse(add.Value),
                    Edit = Boolean.Parse(edit.Value),
                    Delete = Boolean.Parse(delete.Value)
                });
                permissionDGV.ItemsSource = permissionList;
            }
        }

    }
}
