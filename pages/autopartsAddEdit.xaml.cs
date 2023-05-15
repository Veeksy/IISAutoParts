using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для autopartsAddEdit.xaml
    /// </summary>
    public partial class autopartsAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;

        autoparts autopart;

        int CarModel;

        public autopartsAddEdit(int CarModel, int? IdAutoPart = 0)
        {
            InitializeComponent();
            try
            {
                _dbContext = new IISAutoPartsEntities();
                this.CarModel = CarModel;

                autopart = _dbContext.autoparts.Where(x => x.id == IdAutoPart).FirstOrDefault();

                var category = _dbContext.autopartsCategory.ToList();
                CategoryCb.ItemsSource = category;
                CategoryCb.DisplayMemberPath = "title";
                CategoryCb.SelectedValuePath = "id";

                if (autopart != null)
                {
                    manufacturerTb.Text = autopart.manufacturer;
                    nameTb.Text = autopart.name;
                    priceTb.Text = $"{autopart.price:F2} руб.";
                    yearTb.Text = autopart.year.ToString();
                    descriptionTb.Text = autopart.description;
                    CategoryCb.SelectedValue = autopart.idCategory;
                    if (!(autopart.image is null))
                        autopartImage.Source = ImageController.ReturnImageFromDataBase(autopart.image);
                }
                else
                {
                    autopart = new autoparts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе данных. " + ex.Message);
            }

            this.CarModel = CarModel;
        }

        private void ChangeImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                autopart.image = ImageController.ReturnChoosedFile();
                autopartImage.Source = ImageController.ReturnImageFromDataBase(autopart.image);
            }
            catch (Exception ex) {
                MessageBox.Show("Формат не поддерживается. " + ex.Message);

            }

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                autopart.manufacturer = manufacturerTb.Text;
                autopart.price = Convert.ToDecimal(priceTb.Text.Split(' ')[0]);
                 autopart.year = Convert.ToInt32(yearTb.Text);
                autopart.name = nameTb.Text;
                autopart.description = descriptionTb.Text;
                autopart.idCategory = Convert.ToInt32(CategoryCb.SelectedValue);
                autopart.count = autopart.count ?? 0;
                _dbContext.autoparts.AddOrUpdate(autopart);
                _dbContext.SaveChanges();

                var _modelAutoPart = new autopartsModel()
                {
                    idAutoparts = autopart.id,
                    idModel = CarModel,
                };

                _dbContext.autopartsModel.AddOrUpdate(_modelAutoPart);
                _dbContext.SaveChanges();


                MessageBox.Show("Сохранено");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка сохранения. " + ex.Message);
            }
            

        }

        private void priceTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void DeleteImageBtn_Click(object sender, RoutedEventArgs e)
        {
            autopart.image = null;
            autopartImage.Source = null;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autopartsPage(CarModel));
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
}
