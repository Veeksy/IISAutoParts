using IISAutoParts.Class;
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
            FrameController.MainFrame = MainFrame;
        }
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
        }
        private void menuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            menuCloseBtn.IsChecked = true;
        }
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new ordersPage());
        }
        private void Delivery_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new providePage());
        }
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new reportsPage());
        }
        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autopartsPage(0));
        }
        private void MainMenu_PaneClosed(object sender, EventArgs e)
        {
            

        }
        private void MainMenu_PaneClosing(object sender, MahApps.Metro.Controls.SplitViewPaneClosingEventArgs e)
        {
            menuOpenBtn.IsChecked = !menuOpenBtn.IsChecked;
        }
        private void Documents_Click(object sender, RoutedEventArgs e)
        {
        }

        private void usersBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new usersListPage());
        }

        private void Cars_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autoPage());

        }
    }
}
