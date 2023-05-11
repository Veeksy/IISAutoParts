using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для reportsPage.xaml
    /// </summary>
    public partial class reportsPage : Page
    {
        IISAutoPartsEntities _dbContext;
        Paginator paginator;

        private List<int> selectedIds = new List<int>();

        List<ReportsView> _orderReports = new List<ReportsView>();
        List<ReportsView> _provideReports = new List<ReportsView>();

        public reportsPage()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            _orderReports = _dbContext.Orders.Join(_dbContext.orderReports, x => x.id, y => y.orderId, (x, y) =>
            new ReportsView()
            {
                id = x.id,
                numberDoc = (int)x.orderNumber,
                customer = (int)x.idCustomer,
                //dateInterval = $"{Convert.ToDateTime(y.dateBegin).ToString("dd.MM.yyyy")}-{Convert.ToDateTime(y.dateEnd).ToString("dd.MM.yyyy")}",
            }).ToList();

            reportsTypeCb.SelectedIndex = 0;

            paginator = new Paginator(_orderReports.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            reportDGV.ItemsSource = paginator.GetTable();

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as ReportsView;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as ReportsView;

            // Удалить ID элемента из списка выбранных элементов, если он был добавлен ранее
            if (row != null && selectedIds.Contains(row.id))
            {
                selectedIds.Remove(row.id);
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            reportDGV.ItemsSource = paginator.GetTable();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            reportDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                reportDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void reportsTypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIds.Clear();
            if (reportsTypeCb.SelectedIndex == 0)
            {
                _orderReports = _dbContext.Orders.Join(_dbContext.orderReports, x => x.id, y => y.orderId, (x, y) =>
                new ReportsView()
                {
                    id = x.id,
                    numberDoc = (int)x.orderNumber,
                    customer = (int)x.idCustomer,
                    //dateInterval = $"{Convert.ToDateTime(y.dateBegin).ToString("dd.MM.yyyy")}-{Convert.ToDateTime(y.dateEnd).ToString("dd.MM.yyyy")}",
                }).ToList();

                paginator = new Paginator(_orderReports.ToList<object>(), 1, 10);

                pageNumber.Text = paginator.GetPage().ToString();
                countPage.Content = paginator.GetCountpage();

                reportDGV.ItemsSource = paginator.GetTable();
            }
            else
            {
                _provideReports = _dbContext.provide.Join(_dbContext.provideReports, x => x.id, y => y.provideId, (x, y) =>
                 new ReportsView()
                 {
                     id = x.id,
                     numberDoc = (int)x.provideNumber,
                     customer = (int)x.idProvider,
                     //dateInterval = $"{Convert.ToDateTime(y.dateBegin).ToString("dd.MM.yyyy")}-{Convert.ToDateTime(y.dateEnd).ToString("dd.MM.yyyy")}",
                 }).ToList();

                paginator = new Paginator(_provideReports.ToList<object>(), 1, 10);

                pageNumber.Text = paginator.GetPage().ToString();
                countPage.Content = paginator.GetCountpage();

                reportDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void createReportBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
