using IISAutoParts.Class;
using IISAutoParts.DBcontext.MyEntities;
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
using System.Windows.Forms;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для consumersPage.xaml
    /// </summary>
    public partial class consumersPage : Page
    {
        Paginator paginator;
        IISAutoPartsEntities _dbContext;
        List<customers> _customers = new List<customers>();
        private List<int> selectedIds = new List<int>();

        public consumersPage()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            _customers = fillData();

            paginator = new Paginator(_customers.ToList<object>(), 1, 10);
            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            customersDGV.ItemsSource = paginator.GetTable();

            if (UserController.permissionList.Where(x => x.Sector == "Заказчики").Select(x => x.Add).FirstOrDefault())
                AddnewOrder.IsEnabled = true;
            else
                AddnewOrder.IsEnabled = false;
            if (UserController.permissionList.Where(x => x.Sector == "Заказчики").Select(x => x.Delete).FirstOrDefault())
                deleteBtn.IsEnabled = true;
            else
                deleteBtn.IsEnabled = false;

        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            customersDGV.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            customersDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                customersDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as System.Windows.Controls.CheckBox)?.DataContext as customers;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as System.Windows.Controls.CheckBox)?.DataContext as customers;

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

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
               .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                var deleted = _dbContext.customers.Where(x => selectedIds.Contains(x.id)).ToList();
                _dbContext.customers.RemoveRange(deleted);

                _dbContext.SaveChanges();
                _customers = fillData();

                paginator = new Paginator(_customers.ToList<object>(), paginator.GetPage(), 10);
                customersDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void AddnewOrder_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new consumersAddEdit(0));
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {

            _customers = _dbContext.customers.Where(x => (string.IsNullOrEmpty(customerNameTb.Text) || x.name.ToLower().Contains(customerNameTb.Text.ToLower()))
            && (string.IsNullOrEmpty(addressTb.Text) || x.address.ToLower().Contains(addressTb.Text.ToLower()))).ToList();
            paginator = new Paginator(_customers.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            customersDGV.ItemsSource = paginator.GetTable();
        }

        private void customersDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            customers selectedPart = customersDGV.SelectedItem as customers;

            if (selectedPart != null)
            {
                if (UserController.permissionList.Where(x => x.Sector == "Заказчики").Select(x => x.Edit).FirstOrDefault())
                    FrameController.MainFrame.Navigate(new consumersAddEdit(selectedPart.id));
                else
                    System.Windows.Forms.MessageBox.Show("Недостаточно прав для редактирования компонента");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }

        }
        private List<customers> fillData()
        {
            _customers = _dbContext.customers.ToList();
            return _customers;
        }
    }
}
