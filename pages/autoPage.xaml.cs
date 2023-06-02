using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using Word = Microsoft.Office.Interop.Word;

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

        private async void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadingDoc.Visibility = Visibility.Visible;

                await System.Threading.Tasks.Task.Run(() => CreateDoc());

                loadingDoc.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            //AutopartsReport
        }

        private void CreateDoc()
        {
            string Template = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/AutopartsReport.docx");
            string SavePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет по остаткам автозапчастей для информационной системы.docx");

            Word.Application wordApp = new Word.Application();
            Word.Document doc = null;
            try
            {

                doc = wordApp.Documents.Open(Template);


                var parts = _dbContext.autoparts.ToList();


                var query = parts.Join(_dbContext.autopartsModel, x => x.id, y => y.idAutoparts, (x, y) => new
                {
                    modelId = y.idModel,
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                }).Distinct().ToList();

                var query1 = query.Join(_dbContext.carModels, x => x.modelId, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = y.model,
                    carId = y.idCar,
                }).ToList();

                var query2 = query1.Join(_dbContext.cars, x => x.carId, y => y.id, (x, y) => new
                {
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = x.model,
                    car = y.name,
                }).OrderBy(x=>x.car).ToList();


                Word.Table table = doc.Tables[1];
                // Заполнение строки заголовков
                for (int i = 0; i < query2.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query2[i].car;
                    row.Cells[2].Range.Text = query2[i].model;
                    row.Cells[3].Range.Text = query2[i].article + " " + query[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = (query2[i].count.HasValue ? query2[i].count : 0).ToString();
                    row.Cells[5].Range.Text = query2[i].price.Value.ToString("F2");
                    row.Cells[6].Range.Text = (query2[i].price.Value * (query2[i].count.HasValue ? query2[i].count : 0)).Value.ToString("F2");
                }
                doc.SaveAs(SavePath);
                Thread.Sleep(10);
                Process.Start(SavePath);
                doc.Close();
                wordApp.Quit();
            }
            catch (Exception ex) 
            {
                doc.Close();
                wordApp.Quit();
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void carImages_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
        }
    }
}
