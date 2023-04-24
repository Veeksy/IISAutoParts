using IISAutoParts.pages;
using System;
using System.Collections.Generic;
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

namespace IISAutoParts
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            menuCloseBtn.IsChecked = !menuCloseBtn.IsChecked;
        }

        private void menuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            menuOpenBtn.IsChecked = !menuOpenBtn.IsChecked;
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ordersPage());
        }

        private void Delivery_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new autopartsAddEdit());
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new autopartsPage());
        }
    }
}
