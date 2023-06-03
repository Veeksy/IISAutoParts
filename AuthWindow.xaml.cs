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
using System.Web.Caching;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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


        private int tryEnter = 3;

        public bool IsAccept = true;
        private Captcha capcha;

        public AuthWindow()
        {
            InitializeComponent();
            _db = new IISAutoPartsEntities();

            capcha = new Captcha(5, CapchaCanvas);
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAccept)
            {
                MessageBox.Show("Символы не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                RegenearteCapcha();
            }
            else
            {
                if (GetUsersInfo(usernameField.Text, passwordField.Password))
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный пароль");
                    tryEnter -= 1;
                    if (tryEnter <= 0)
                    {
                        IsAccept = false;
                        capchaGrid.Visibility = Visibility.Visible;
                        RegenearteCapcha();
                    }
                }
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
                UserController.userName = user.name;
                user.dateEnter = DateTime.UtcNow;
                _db.users.AddOrUpdate(user);
                _db.SaveChanges();

                loadPermission();

                return true;
            }
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void RegenearteCapcha()
        {
            capcha.GenerateNew();
            AnswerTextBox.Text = "";
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

        private void NewCapchButton(object sender, RoutedEventArgs e)
        {
            RegenearteCapcha();
        }

        private void SendButton(object sender, RoutedEventArgs e)
        {
            if (capcha.CheckCapcha(AnswerTextBox.Text))
            {
                IsAccept = true;
                capchaGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Символы не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                RegenearteCapcha();
            }
        }
    }
}
