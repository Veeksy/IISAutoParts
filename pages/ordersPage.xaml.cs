using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ordersPage.xaml
    /// </summary>
    public partial class ordersPage : Page
    {
        Paginator paginator;
        IISAutoPartsEntities _dbContext;
        List<OrdersView> orders = new List<OrdersView>();

        public ordersPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            orders = _dbContext.Orders.AsNoTracking()
                .Join(_dbContext.autoparts,
                x => x.idAutoparts, y=> y.id, (x, y) => new {
                    id = x.id,
                    number = x.orderNumber,
                    autopart = y.manufacturer + " " + y.name,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutoparts,
                    customer = x.idCustomer,
                }
                ).Join(_dbContext.customers, x=> x.customer, y=>y.id, (x, y) => new OrdersView
                {
                    id = x.id,
                    orderNumber = x.number,
                    autopartName = x.autopart,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutopart,
                    customer = y.name,
                    address = y.address,
                }).ToList();


            autopartCb.ItemsSource = _dbContext.autoparts.AsNoTracking().ToList();
            autopartCb.DisplayMemberPath = "name";
            autopartCb.SelectedValuePath = "id";

            customerCb.ItemsSource = _dbContext.customers.AsNoTracking().ToList();
            customerCb.DisplayMemberPath = "name";
            customerCb.SelectedValuePath = "id";

            paginator = new Paginator(orders.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            ordersDGV.ItemsSource = paginator.GetTable();

        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            ordersDGV.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            ordersDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                ordersDGV.ItemsSource = paginator.GetTable();
            }
        }



        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void AddnewOrder_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new ordersAddEdit(0));
        }

        private void ordersDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OrdersView selectedPart = ordersDGV.SelectedItem as OrdersView;

            if (selectedPart != null)
            {
                byte[] file = _dbContext.OrdersDoc.AsNoTracking()
                    .Where(x => x.idOrders == selectedPart.id).Select(x=>x.doc).FirstOrDefault();
                if (file is null)
                {
                    MessageBox.Show("Файлы отсутствуют");
                }
                else
                {
                    string SavePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Акт№{selectedPart.orderNumber} заказа автозапчастей для информационной системы.docx");

                    File.WriteAllBytes(SavePath, file);
                    Process.Start(SavePath);
                }
            }
            else
            {
                MessageBox.Show("Не удалось открыть выбранный объект.");
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            int? _autopart = (int?)autopartCb.SelectedValue;
            int? _customer = (int?)customerCb.SelectedValue;
            DateTime? _startDate = startDateDt.SelectedDate;
            DateTime? _endDate = endDateDt.SelectedDate;

            orders = _dbContext.Orders.AsNoTracking()
                .Join(_dbContext.autoparts,
                x => x.idAutoparts, y => y.id, (x, y) => new {
                    id = x.id,
                    number = x.orderNumber,
                    autopart = y.manufacturer + " " + y.name,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutoparts,
                    customer = x.idCustomer,
                }
                ).Join(_dbContext.customers, x => x.customer, y => y.id, (x, y) => new OrdersView
                {
                    id = x.id,
                    orderNumber = x.number,
                    autopartName = x.autopart,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutopart,
                    customer = y.name,
                    address = y.address,
                }).ToList();

            orders = orders.Where(x => (string.IsNullOrEmpty(numberOrder.Text)
            || numberOrder.Text.Contains(x.orderNumber.GetValueOrDefault().ToString())) &&
            (_autopart == null || x.autopartId == _autopart) && (_customer == null || x.customerId == _customer)
            && (_startDate == null || x.dateOrder >= _startDate) && (_endDate == null || x.dateOrder <= _endDate)).ToList();


            paginator = new Paginator(orders.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            ordersDGV.ItemsSource = paginator.GetTable();

        }
    }
}
