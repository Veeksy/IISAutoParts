using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        List<autoparts> autoparts = new List<autoparts>();

        public autopartsPage()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            autoparts = _dbContext.autoparts.AsNoTracking().ToList();

            paginator = new Paginator(autoparts.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            autopartDGV.ItemsSource = paginator.GetTable();
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
                FrameController.MainFrame.Navigate(new autopartsAddEdit(selectedPart.id));
            }
            else
            {
                MessageBox.Show("Не удалось открыть выбранный объект.");
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

        private void selectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AddnewAutoParts_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autopartsAddEdit(0));
        }
    }
}
