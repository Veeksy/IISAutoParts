using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// Логика взаимодействия для CarAddEdit.xaml
    /// </summary>
    public partial class CarAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;
        cars car;


        private int CarId;

        public CarAddEdit(int CarId)
        {
            InitializeComponent();
            this.CarId = CarId;
            _dbContext = new IISAutoPartsEntities();
            Countries countries = new Countries();

            car = _dbContext.cars.Where(x => x.id == CarId).FirstOrDefault();
            countryCb.ItemsSource = countries.countries;

            if (car == null)
            {
                car = new cars();
            }
            else
            {
                nameTb.Text = car.name;
                descriptionTb.Text = car.description;
                countryCb.SelectedValue = car.country;
                if (!(car.photo is null))
                    CarImage.Source = ImageController.ReturnImageFromDataBase(car.photo);
            }

            
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autoPage());
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                car.name = nameTb.Text;
                car.description = descriptionTb.Text;
                car.country = (string)countryCb.SelectedValue;
                _dbContext.cars.AddOrUpdate(car);
                _dbContext.SaveChanges();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
            }


        }

        private void ChangeImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                car.photo = ImageController.ReturnChoosedFile();
                CarImage.Source = ImageController.ReturnImageFromDataBase(car.photo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Формат не поддерживается. " + ex.Message);
            }
        }

        private void DeleteImageBtn_Click(object sender, RoutedEventArgs e)
        {
            car.photo = null;
            CarImage.Source = null;
        }
    }
}
