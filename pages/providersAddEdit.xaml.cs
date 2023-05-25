using IISAutoParts.Class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для providersAddEdit.xaml
    /// </summary>
    public partial class providersAddEdit : Page
    {
        providers provider;
        IISAutoPartsEntities _dbContext;
        public providersAddEdit(int id)
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            if (id > 0)
            {
                provider = _dbContext.providers.Where(x=>x.id == id).FirstOrDefault();
                nameProviderTb.Text = provider.name;
                addressTb.Text = provider.address;
            }
            else 
            {
                provider = new providers();
            }   
            

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                provider.name = nameProviderTb.Text;
                provider.address = addressTb.Text;

                _dbContext.providers.AddOrUpdate(provider);
                _dbContext.SaveChanges();
                MessageBox.Show("Сохранено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new providersPage());
        }
    }
}
