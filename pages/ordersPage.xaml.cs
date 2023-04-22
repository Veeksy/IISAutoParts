using IISAutoParts.Class;
using IISAutoParts.DBcontext;
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
    /// Логика взаимодействия для ordersPage.xaml
    /// </summary>
    public partial class ordersPage : Page
    {
        Paginator paginator;
        IISAutoPartsEntities _dbContext;

        public ordersPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            var orders = _dbContext.Orders.AsNoTracking()
                .Join(_dbContext.autoparts,
                x => x.idAutoparts, y=> y.id, (x, y) => new {
                    number = x.orderNumber,
                    autopart = y.manufacturer + " " + y.name,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutoparts,
                    customer = x.idCustomer,

                }
                ).Join(_dbContext.customers, x=> x.customer, y=>y.id, (x, y) => new
                {
                    orderNumber = x.number,
                    autopartName = x.autopart,
                    dateOrder = x.dateOrder,
                    countAutopart = x.countAutopart,
                    customer = y.name,
                    address = y.address,
                }).ToList<object>();

            paginator = new Paginator(orders, 1, 10);

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
    }
}
