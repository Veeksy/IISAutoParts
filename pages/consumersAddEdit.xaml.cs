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
    /// Логика взаимодействия для consumersAddEdit.xaml
    /// </summary>
    public partial class consumersAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;
        customers customer;

        public consumersAddEdit(int id)
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            if (id > 0) {
                customer = _dbContext.customers.Where(x=>x.id == id).FirstOrDefault();
                nameCustomerTb.Text = customer.name;
                addressTb.Text = customer.address;
            }
            else
            {
                customer = new customers();
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new consumersPage());
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                customer.name = nameCustomerTb.Text;
                customer.address = addressTb.Text;
                _dbContext.customers.AddOrUpdate(customer);
                _dbContext.SaveChangesAsync();
                MessageBox.Show("Сохранено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
