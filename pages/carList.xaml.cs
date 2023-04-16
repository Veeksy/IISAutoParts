using IISAutoParts.DBcontext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
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

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для carList.xaml
    /// </summary>
    public partial class carList : Page
    {
        IISAutoPartsEntities _dbContext;
        public carList()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();
        }


        private async void GetAutoParts()
        {
            var cars = await _dbContext.cars.AsNoTracking().ToListAsync();
            dgvCars.ItemsSource = cars;

        }

        private void dgvCars_Loaded(object sender, RoutedEventArgs e)
        {
            GetAutoParts();
        }
    }
}
