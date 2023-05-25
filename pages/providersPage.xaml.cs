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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для providersPage.xaml
    /// </summary>
    public partial class providersPage : Page
    {
        Paginator paginator;
        IISAutoPartsEntities _dbContext;
        List<providers> _providers = new List<providers>();
        private List<int> selectedIds = new List<int>();

        public providersPage()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            _providers = _dbContext.providers.ToList();

            paginator = new Paginator(_providers.ToList<object>(), 1, 10);
            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            providersDGV.ItemsSource = paginator.GetTable();

            if (UserController.permissionList.Where(x => x.Sector == "Поставщики").Select(x => x.Add).FirstOrDefault())
                AddnewOrder.IsEnabled = true;
            else
                AddnewOrder.IsEnabled = false;
            if (UserController.permissionList.Where(x => x.Sector == "Поставщики").Select(x => x.Delete).FirstOrDefault())
                deleteBtn.IsEnabled = true;
            else
                deleteBtn.IsEnabled = false;


        }

        private void providersDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            providers selectedPart = providersDGV.SelectedItem as providers;

            if (selectedPart != null)
            {
                if (UserController.permissionList.Where(x => x.Sector == "Поставщики").Select(x => x.Edit).FirstOrDefault())
                    FrameController.MainFrame.Navigate(new providersAddEdit(selectedPart.id));
                else
                    System.Windows.Forms.MessageBox.Show("Недостаточно прав для редактирования компонента");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            providersDGV.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            providersDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                providersDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as System.Windows.Controls.CheckBox)?.DataContext as providers;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as System.Windows.Controls.CheckBox)?.DataContext as providers;

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

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            _providers = _dbContext.providers.Where(x => (string.IsNullOrEmpty(customerNameTb.Text) || x.name.ToLower().Contains(customerNameTb.Text.ToLower()))
           && (string.IsNullOrEmpty(addressTb.Text) || x.address.ToLower().Contains(addressTb.Text.ToLower()))).ToList();
            paginator = new Paginator(_providers.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            providersDGV.ItemsSource = paginator.GetTable();
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
              .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                var deleted = _dbContext.providers.Where(x => selectedIds.Contains(x.id)).ToList();
                _dbContext.providers.RemoveRange(deleted);

                _dbContext.SaveChanges();
                _providers = _dbContext.providers.ToList();


                paginator = new Paginator(_providers.ToList<object>(), paginator.GetPage(), 10);
                providersDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void AddnewOrder_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new providersAddEdit(0));
        }
    }
}
