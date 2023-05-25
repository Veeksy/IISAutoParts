using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для autoPage.xaml
    /// </summary>
    public partial class autoPage : Page
    {
        IISAutoPartsEntities _dbContext;
        Paginator paginator;

        List<cars> _cars = new List<cars>();

        private List<int> selectedIds = new List<int>();

        public autoPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();
            _cars = _dbContext.cars.ToList();

            paginator = new Paginator(_cars.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            carList.ItemsSource = paginator.GetTable();

            Countries countries = new Countries();

            countryCb.ItemsSource = countries.countries;

            if (UserController.permissionList.Where(x => x.Sector == "Бренд").Select(x => x.Add).FirstOrDefault())
                AddnewAutoParts.IsEnabled = true;
            else
                AddnewAutoParts.IsEnabled = false;

        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {
            _cars = _dbContext.cars.Where(x => (string.IsNullOrEmpty(nameTb.Text) || x.name.ToLower().Contains(nameTb.Text.ToLower()))
            && (string.IsNullOrEmpty(countryCb.Text) || x.country.Contains(countryCb.Text))).ToList();


            paginator = new Paginator(_cars.ToList<object>(), 1, 10);
            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            carList.ItemsSource = paginator.GetTable();
        }
        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            carList.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            carList.ItemsSource = paginator.GetTable();
        }


        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void AddnewAutoParts_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new CarAddEdit(0));
        }

        private void deleteElements_Click(object sender, RoutedEventArgs e)
        {
            
            if (UserController.permissionList.Where(x => x.Sector == "Бренд").Select(x => x.Delete).FirstOrDefault())
            {

                Button button = sender as Button;
                string listViewId = button.Tag.ToString();

                if (listViewId != null)
                {
                    System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
                    .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        var deleted = _cars.Where(x => x.id == int.Parse(listViewId)).FirstOrDefault();
                        _dbContext.cars.Remove(deleted);
                        _dbContext.SaveChanges();

                        _cars = _dbContext.cars.ToList();

                        paginator = new Paginator(_cars.ToList<object>(), 1, 10);

                        pageNumber.Text = paginator.GetPage().ToString();
                        countPage.Content = paginator.GetCountpage();

                        carList.ItemsSource = paginator.GetTable();
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Недостаточно прав для совершения действия");
            }
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                carList.ItemsSource = paginator.GetTable();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as autoparts;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as autoparts;

            // Удалить ID элемента из списка выбранных элементов, если он был добавлен ранее
            if (row != null && selectedIds.Contains(row.id))
            {
                selectedIds.Remove(row.id);
            }
        }

        private void carList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cars selectedPart = carList.SelectedItem as cars;

            if (selectedPart != null)
            {
                if (UserController.permissionList.Where(x => x.Sector == "Модель авто").Select(x => x.Read).FirstOrDefault())
                    FrameController.MainFrame.Navigate(new autoModelsPage(selectedPart.id));
                else
                    System.Windows.MessageBox.Show("Недостаточно прав для просмотра моделей автомобилей");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }

        }

        private void EditElement_Click(object sender, RoutedEventArgs e)
        {
            if (UserController.permissionList.Where(x => x.Sector == "Бренд").Select(x => x.Edit).FirstOrDefault())
            {
                Button button = sender as Button;
                string listViewId = button.Tag.ToString();

                if (listViewId != null)
                {
                    FrameController.MainFrame.Navigate(new CarAddEdit(int.Parse(listViewId)));
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Недостаточно прав для совершения действия");
            }
        }
    }
}
