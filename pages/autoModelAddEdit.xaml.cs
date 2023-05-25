using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для autoModelAddEdit.xaml
    /// </summary>
    public partial class autoModelAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;
        private int modelId;
        private int carId;

        private carModels _carModels;
        public autoModelAddEdit(int modelId, int carId)
        {
            InitializeComponent();
            this.modelId = modelId;
            this.carId = carId;
            this._dbContext = new IISAutoPartsEntities();
            try
            {
                this._carModels = getCarModels();

                nameTb.Text = _carModels.model;
                if (!(_carModels.photo is null))
                    CarImage.Source = ImageController.ReturnImageFromDataBase(_carModels.photo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private carModels getCarModels()
        {
            var carModels = new carModels();
            if (modelId != 0)
                carModels = _dbContext.carModels.AsNoTracking().Where(x => x.id == modelId).FirstOrDefault();
            return carModels;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autoModelsPage(carId));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _carModels.model = nameTb.Text;
                _carModels.idCar = carId;
                _dbContext.carModels.AddOrUpdate(_carModels);
                _dbContext.SaveChanges();
                MessageBox.Show("Сохранено");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void ChangeImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _carModels.photo = ImageController.ReturnChoosedFile();
                CarImage.Source = ImageController.ReturnImageFromDataBase(_carModels.photo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Формат не поддерживается. " + ex.Message);
            }
        }

        private void DeleteImageBtn_Click(object sender, RoutedEventArgs e)
        {
            _carModels.photo = null;
            CarImage.Source = null;
        }
    }
}
