using IISAutoParts.authCodeDecode;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace IISAutoParts
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {

        IISAutoPartsEntities _db;

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
            var user =  _db.users.Where(x=>x.login == username && x.password == password).FirstOrDefault();
            if (user is null)
                return false;
            else return true;
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            _db.users.Add(new users
            {
                login = usernameField.Text,
                password = AuthController.Encrypt(passwordField.Password),
                role = "Admin",
            });
            _db.SaveChanges();
        }
    }
}
