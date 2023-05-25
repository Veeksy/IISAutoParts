using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
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
using System.Xml;
using CheckBox = System.Windows.Controls.CheckBox;
using Page = System.Windows.Controls.Page;

namespace IISAutoParts.pages
{

    /// <summary>
    /// Логика взаимодействия для autopartsPage.xaml
    /// </summary>
    public partial class autopartsPage : Page
    {
        IISAutoPartsEntities _dbContext;
        Paginator paginator;

        List<autoparts> _autoparts = new List<autoparts>();
        List<int?> ids = new List<int?>();

        private List<int> selectedIds = new List<int>();

        private int CarModel;

        public autopartsPage(int id)
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();
            this.CarModel = id;

            ids = _dbContext.autopartsModel.AsNoTracking().Where(x=>x.idModel == CarModel).Select(x=>x.idAutoparts).ToList();

            _autoparts = _dbContext.autoparts.Where(x=> ids.Contains(x.id)).ToList();

            paginator = new Paginator(_autoparts.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            autopartDGV.ItemsSource = paginator.GetTable();

            categoryCb.ItemsSource = _dbContext.autopartsCategory.ToList();
            categoryCb.DisplayMemberPath = "title";
            categoryCb.SelectedValuePath = "id";

            if (UserController.permissionList.Where(x=>x.Sector == "Каталог автозапчастей").Select(x => x.Add).FirstOrDefault())
                AddnewAutoParts.IsEnabled = true;
            else
                AddnewAutoParts.IsEnabled = false;

            if (UserController.permissionList.Where(x => x.Sector == "Каталог автозапчастей").Select(x => x.Delete).FirstOrDefault())
                deleteElements.IsEnabled = true;
            else
                deleteElements.IsEnabled = false;

        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            autopartDGV.ItemsSource = paginator.GetTable();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            autopartDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                autopartDGV.ItemsSource = paginator.GetTable();
            }

        }



        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void autopartDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            autoparts selectedPart = autopartDGV.SelectedItem as autoparts;

            if (selectedPart != null)
            {
                if (UserController.permissionList.Where(x => x.Sector == "Каталог автозапчастей").Select(x => x.Edit).FirstOrDefault())
                    FrameController.MainFrame.Navigate(new autopartsAddEdit(CarModel, selectedPart.id));
                else
                    System.Windows.Forms.MessageBox.Show("Недостаточно прав для редактирования компонента");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось открыть выбранный объект.");
            }

            //private async void GetAutoParts()
            //{
            //    var cars = await _dbContext.cars.AsNoTracking().ToListAsync();
            //    byte[] buffer = cars.Select(x => x.photo).FirstOrDefault();
            //    MemoryStream byteStream = new MemoryStream(buffer);
            //    BitmapImage image = new BitmapImage();
            //    image.BeginInit();
            //    image.StreamSource = byteStream;
            //    image.EndInit(); // itemSource = image
            //}

        }

        private void AddnewAutoParts_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autopartsAddEdit(CarModel, 0));
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (autoparts item in autopartDGV.Items)
            {
                CheckBox checkBox = autopartDGV.Columns[0].GetCellContent(item) as CheckBox;
                if (checkBox != null)
                {
                    checkBox.IsChecked = true;
                    selectedIds.Add(item.id);
                }
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

        private void deleteElements_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
                .Show("Действительно удалить выбранные записи?", "Подтвердите действие", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                var deleted = _autoparts.Where(x => selectedIds.Contains(x.id)).ToList();
                _dbContext.autoparts.RemoveRange(deleted);

                _dbContext.SaveChanges();
                _autoparts = _dbContext.autoparts.Where(x => ids.Contains(x.id)).ToList();


                paginator = new Paginator(_autoparts.ToList<object>(), paginator.GetPage(), 10);
                autopartDGV.ItemsSource = paginator.GetTable();
            }



        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {
            _autoparts = _autoparts = _dbContext.autoparts.Where(x => ids.Contains(x.id)).ToList();
            int category = categoryCb.SelectedValue != null ? ((int)categoryCb.SelectedValue) : (-1); 

            _autoparts = _autoparts.Where(x => 
            (string.IsNullOrEmpty(manufacturerTb.Text) || x.manufacturer.ToLower().Contains(manufacturerTb.Text.ToLower())) 
            && (string.IsNullOrEmpty(nameTb.Text) || x.name.ToLower().Contains(nameTb.Text.ToLower()))
            && (string.IsNullOrEmpty(minPrice.Text) || x.price >= Convert.ToInt32(minPrice.Text))
            && (string.IsNullOrEmpty(maxPrice.Text) || x.price <= Convert.ToInt32(maxPrice.Text)) &&
            (category == -1 || x.idCategory == category) 
            && (string.IsNullOrEmpty(articleTb.Text) || x.article.Contains(articleTb.Text))).ToList();


            paginator = new Paginator(_autoparts.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            autopartDGV.ItemsSource = paginator.GetTable();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            var carId = _dbContext.carModels.Where(x => x.id == CarModel).Select(x=>x.idCar).FirstOrDefault(); 
            FrameController.MainFrame.Navigate(new autoModelsPage((int)carId));
        }
    }
}
