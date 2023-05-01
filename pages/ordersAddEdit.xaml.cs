using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для ordersAddEdit.xaml
    /// </summary>
    public partial class ordersAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;

        Orders _order;

        public ordersAddEdit(int? orderId = 0)
        {
            InitializeComponent();
            try
            {
                _dbContext = new IISAutoPartsEntities();
                _order = _dbContext.Orders.Where(x=>x.id == orderId).FirstOrDefault();
                var _autoparts = _dbContext.autoparts.AsNoTracking().ToList<object>();
                var _customers = _dbContext.customers.AsNoTracking().ToList<object>();

                orderNumberTb.Text = _order.orderNumber.ToString();
                autopartCb.ItemsSource = _autoparts;
                autopartCb.SelectedItem = _order.idAutoparts;
                autopartCb.DisplayMemberPath = "[manufacturer name price руб.]";
                autopartCb.SelectedValuePath = "id";

                DateOrderTb.Text = _order.dateOrder.ToString();
                countTb.Text = _order.countAutoparts.ToString();
                CustomerCb.ItemsSource = _customers;
                CustomerCb.SelectedItem = _order.idCustomer;
                autopartCb.DisplayMemberPath = "name";
                autopartCb.SelectedValuePath = "id";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе данных. " + ex.Message);
            }

            

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка сохранения. " + ex.Message);
            }
            

        }

        private void priceTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new ordersPage());
        }

    }
}
