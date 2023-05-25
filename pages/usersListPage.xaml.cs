using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для usersListPage.xaml
    /// </summary>
    public partial class usersListPage : Page
    {
        IISAutoPartsEntities _dbContext;
        public usersListPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            var users = _dbContext.users.Select(x=> new UserList
            {
                Id = x.id,
                Name = x.login,
                DateEnter = x.dateEnter.Value,
            }).ToList();

            UsersDGV.ItemsSource = users;
            
        }

        private void UsersDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserList selectedPart = UsersDGV.SelectedItem as UserList;

            if (selectedPart != null)
            {
                FrameController.MainFrame.Navigate(new userPermissionPage(selectedPart.Id));
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }
        }
    }
}
