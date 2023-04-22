﻿using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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
        Paginator paginator;

        public carList()
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();

            var autoparts = _dbContext.autoparts.AsNoTracking().ToList<object>();

            paginator = new Paginator(autoparts, 1, 10);

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
