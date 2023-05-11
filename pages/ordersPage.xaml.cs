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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;

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
        private List<int> selectedIds = new List<int>();

        public ordersPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            orders = fillData();


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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as OrdersView;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as OrdersView;

            // Удалить ID элемента из списка выбранных элементов, если он был добавлен ранее
            if (row != null && selectedIds.Contains(row.id))
            {
                selectedIds.Remove(row.id);
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

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
               .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                var deleted = _dbContext.Orders.Where(x => selectedIds.Contains(x.id)).ToList();
                _dbContext.Orders.RemoveRange(deleted);

                _dbContext.SaveChanges();
                orders = fillData();

                paginator = new Paginator(orders.ToList<object>(), paginator.GetPage(), 10);
                ordersDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            int? _autopart = (int?)autopartCb.SelectedValue;
            int? _customer = (int?)customerCb.SelectedValue;
            DateTime? _startDate = startDateDt.SelectedDate;
            DateTime? _endDate = endDateDt.SelectedDate;

            orders = fillData();

            orders = orders.Where(x => (string.IsNullOrEmpty(numberOrder.Text)
            || numberOrder.Text.Contains(x.orderNumber.GetValueOrDefault().ToString())) &&
            (_autopart == null || x.autopartId == _autopart) && (_customer == null || x.customerId == _customer)
            && (_startDate == null || x.dateOrder >= _startDate) && (_endDate == null || x.dateOrder <= _endDate)).ToList();


            paginator = new Paginator(orders.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            ordersDGV.ItemsSource = paginator.GetTable();

        }

        private List<OrdersView> fillData()
        {
            var _Orders = _dbContext.Orders.AsNoTracking()
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
            return _Orders;
        }

        
    }
}
