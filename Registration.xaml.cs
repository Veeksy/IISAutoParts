using IISAutoParts.authCodeDecode;
using IISAutoParts.DBcontext;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace IISAutoParts
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        IISAutoPartsEntities _db;
        users new_user;
        bool trust = false;
        public Registration()
        {
            InitializeComponent();
            _db = new IISAutoPartsEntities();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _db.users.Where(x=>x.login == usernameField.Text).FirstOrDefault();
                if (user != null)
                {
                    MessageBox.Show("Логин занят");
                }
                else
                {
                    if (passwordField2.Password == passwordField1.Password && trust)
                    {
                        new_user = new users
                        {
                            login = usernameField.Text,
                            name = nameField.Text,
                            password = AuthController.Encrypt(passwordField1.Password),
                        };
                        _db.users.Add(new_user);
                        _db.SaveChanges();
                        CreatePermission();
                        MessageBox.Show("Пользователь добавлен");
                    }
                    else
                    {
                        resultView.Content = "Пароль должен иметь одну заглавную букву и состоять из 8 символов!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        private void passwordField2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordField2.Password == passwordField1.Password)
            {
                resultView.Content = "";
                if (!trust)
                {
                    resultView.Content = "Пароль должен иметь одну заглавную букву и состоять из 8 символов!";
                }
            }
            else
            {
                resultView.Content = "Пароли не совпадают";
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
                    XElement add = new XElement("add", true);
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

        private void passwordField1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordField2.Password == passwordField1.Password)
            {
                resultView.Content = "";
                if (!trust)
                {
                    resultView.Content = "Пароль должен иметь одну заглавную букву и состоять из 8 символов!";
                }
            }
            else
            {
                resultView.Content = "Пароли не совпадают";
            }
            if (passwordField1.Password.Length < 8)
            {
                resultView.Content = "Пароль должен иметь одну заглавную букву и состоять из 8 символов!";
                trust = false;
            }
            else
            {
                
                for (int i = 0; i < passwordField1.Password.Length; i++)
                {
                    if (!Char.IsDigit(passwordField1.Password[i]) && Char.IsUpper(passwordField1.Password, i))
                    {
                        trust = true;
                    }
                }
            }
        }
    }
}
