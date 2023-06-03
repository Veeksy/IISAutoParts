using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<PermissionList> permissionList = permissionDGV.ItemsSource as List<PermissionList>;

                XElement permission = new XElement("permission",
                    new XElement("admin", IsAdminChb.IsChecked));

                for (int i = 0; i < permissionList.Count; i++)
                {
                    XElement section = new XElement("section", new XAttribute("name", permissionList[i].Sector));

                    XElement read = new XElement("read", permissionList[i].Read);
                    XElement add = new XElement("add", permissionList[i].Add);
                    XElement edit = new XElement("edit", permissionList[i].Edit);
                    XElement delete = new XElement("delete", permissionList[i].Delete);

                    section.Add(read);
                    section.Add(add);
                    section.Add(edit);
                    section.Add(delete);

                    permission.Add(section);
                }

                XDocument xmlDocument = new XDocument(permission);

                var user = _dbContext.users.Where(x => x.id == userId).FirstOrDefault();

                user.permission = xmlDocument.ToString();
                user.name = nameUserTb.Text;
                _dbContext.users.AddOrUpdate(user);
                _dbContext.SaveChanges();
                MessageBox.Show("Сохранено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void loadPermission()
        {
            var user = _dbContext.users.Where(x => x.id == userId).FirstOrDefault();
            nameUserTb.Text = user.name;
            var _permissions = user.permission;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new usersListPage());
        }
    }
}
