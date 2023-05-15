using IISAutoParts.authCodeDecode;
using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace IISAutoParts
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {

        IISAutoPartsEntities _db;
        users user;
        users new_user;
        public AuthWindow()
        {
            InitializeComponent();
            _db = new IISAutoPartsEntities();
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GetUsersInfo(usernameField.Text, passwordField.Password))
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
        }

        private bool GetUsersInfo(string username, string password)
        {
            password = AuthController.Encrypt(password);
            user = _db.users.Where(x => x.login == username && x.password == password).FirstOrDefault();
            if (user is null)
                return false;
            else
            {
                UserController.userId = user.id;
                user.dateEnter = DateTime.Now;
                _db.users.AddOrUpdate(user);
                _db.SaveChanges();

                loadPermission();

                return true;
            }
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new_user = new users
                {
                    login = usernameField.Text,
                    password = AuthController.Encrypt(passwordField.Password),
                };
                _db.users.Add(new_user);
                _db.SaveChanges();
                CreatePermission();
                MessageBox.Show("Пользователь добавлен");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void loadPermission()
        {
            var _permissions = _db.users.Where(x => x.id == user.id).Select(x => x.permission).FirstOrDefault();
            UserController.permissionList = new List<PermissionList>();
            if (_permissions != null)
            {
                XDocument xdoc = XDocument.Parse(_permissions);
                XElement permission = xdoc.Element("permission");
                UserController.isAdmin = Boolean.Parse(permission.Elements("admin").FirstOrDefault().Value);

                foreach (XElement section in permission.Elements("section"))
                {
                    XAttribute name = section.Attribute("name");
                    XElement read = section.Element("read");
                    XElement add = section.Element("add");
                    XElement edit = section.Element("edit");
                    XElement delete = section.Element("delete");

                    UserController.permissionList.Add(new PermissionList
                    {
                        Sector = name.Value,
                        Read = Boolean.Parse(read.Value),
                        Add = Boolean.Parse(add.Value),
                        Edit = Boolean.Parse(edit.Value),
                        Delete = Boolean.Parse(delete.Value)
                    });
                }
            }
        }


        private void CreatePermission()
        {
            try
            {
                XElement permission = new XElement("permission",
                    new XElement("admin", false));


                var permissions = _db.permissions.ToList();
                
                    
                for (int i = 0; i < permissions.Count; i++)
                {
                    XElement section = new XElement("section", new XAttribute("name", permissions[i].sectorname));

                    XElement read = new XElement("read", true);
                    XElement add = new XElement("add", false);
                    XElement edit = new XElement("edit", false);
                    XElement delete = new XElement("delete", false);

                    section.Add(read);
                    section.Add(add);
                    section.Add(edit);
                    section.Add(delete);

                    permission.Add(section);
                }

                XDocument xmlDocument = new XDocument(permission);

                var user = _db.users.Where(x => x.id == new_user.id).FirstOrDefault();

                user.permission = xmlDocument.ToString();

                _db.users.AddOrUpdate(user);
                _db.SaveChanges();
                MessageBox.Show("Сохранено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
