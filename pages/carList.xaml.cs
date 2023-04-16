using IISAutoParts.DBcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
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
using Image = System.Windows.Controls.Image;
using Label = System.Windows.Controls.Label;

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

            var cars = _dbContext.cars.AsNoTracking().ToList();
            List<WrapPanel> wrap_panels = new List<WrapPanel>();
            List<Label> labels = new List<Label>();
            List<Image> images = new List<Image>();

            foreach (var item in cars)
            {

                byte[] buffer = item.photo;
                MemoryStream byteStream = new MemoryStream(buffer);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = byteStream;
                image.EndInit();


                //wrap_panels.Add(new WrapPanel());
                labels.Add(new Label() { Content = item.name });
                //var img = new Image() { Source = image };

                


            }
            
            //for (int r = 0; r < 3; r++)
            //{
            //    for (int c = 0; c < 3; c++)
            //    {
            //        Grid.SetRow(labels[c], r);
            //        Grid.SetColumn(labels[c], c);
            //        carGrid.Children.Add(labels[c]);
            //    }
                   
            //}




        }


        private async void GetAutoParts()
        {
            var cars = await _dbContext.cars.AsNoTracking().ToListAsync();
            byte[] buffer = cars.Select(x => x.photo).FirstOrDefault();
            MemoryStream byteStream = new MemoryStream(buffer);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit(); // itemSource = image
        }


    }
}
