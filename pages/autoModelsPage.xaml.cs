using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using MaterialDesignThemes.Wpf;
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
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для autoModelsPage.xaml
    /// </summary>
    public partial class autoModelsPage : Page
    {
        IISAutoPartsEntities _dbContext;
        Paginator paginator;
        private int carId;

        List<carModels> _carsModels = new List<carModels>();

        private List<int> selectedIds = new List<int>();
        public autoModelsPage(int carId)
        {
            InitializeComponent();

            this.carId = carId;

            _dbContext = new IISAutoPartsEntities();
            _carsModels = _dbContext.carModels.ToList();

            paginator = new Paginator(_carsModels.Where(x=>x.idCar == carId).ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            carModelList.ItemsSource = paginator.GetTable();
            
            if (UserController.permissionList.Where(x => x.Sector == "Модель авто").Select(x => x.Add).FirstOrDefault())
                AddnewAutoParts.IsEnabled = true;
            else
                AddnewAutoParts.IsEnabled = false;
        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {
            _carsModels = _dbContext.carModels.Where(x => (string.IsNullOrEmpty(nameTb.Text) || x.model.ToLower().Contains(nameTb.Text.ToLower()))).ToList();

            paginator = new Paginator(_carsModels.ToList<object>(), 1, 10);
            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            carModelList.ItemsSource = paginator.GetTable();
        }
        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            carModelList.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            carModelList.ItemsSource = paginator.GetTable();
        }


        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void AddnewAutoParts_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autoModelAddEdit(0, carId));
        }

        private void deleteElements_Click(object sender, RoutedEventArgs e)
        {
            if (UserController.permissionList.Where(x => x.Sector == "Модель авто").Select(x => x.Delete).FirstOrDefault())
            {
                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
                string listViewId = button.Tag.ToString();

                if (listViewId != null)
                {
                    System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
                    .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        var deleted = _carsModels.Where(x => x.id == int.Parse(listViewId)).FirstOrDefault();
                        _dbContext.carModels.Remove(deleted);
                        _dbContext.SaveChanges();
                        _carsModels = _dbContext.carModels.ToList();

                        paginator = new Paginator(_carsModels.Where(x => x.idCar == carId).ToList<object>(), 1, 10);

                        pageNumber.Text = paginator.GetPage().ToString();
                        countPage.Content = paginator.GetCountpage();

                        carModelList.ItemsSource = paginator.GetTable();
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
                carModelList.ItemsSource = paginator.GetTable();
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

        private void carModelList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            carModels selectedPart = carModelList.SelectedItem as carModels;

            if (selectedPart != null)
            {
                if (UserController.permissionList.Where(x => x.Sector == "Каталог автозапчастей").Select(x => x.Read).FirstOrDefault())
                    FrameController.MainFrame.Navigate(new autopartsPage(selectedPart.id));
                else
                    MessageBox.Show("Недостаточно прав для просмотра запчастей");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }

        }

        private void EditElement_Click(object sender, RoutedEventArgs e)
        {
            if (UserController.permissionList.Where(x => x.Sector == "Модель авто").Select(x => x.Edit).FirstOrDefault())
            {
                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
                string listViewId = button.Tag.ToString();

                if (listViewId != null)
                {
                    FrameController.MainFrame.Navigate(new autoModelAddEdit(int.Parse(listViewId), carId));
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

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autoPage());
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
        }
    }
}
