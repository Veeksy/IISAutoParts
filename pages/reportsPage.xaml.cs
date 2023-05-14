using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Path = System.IO.Path;
using Word = Microsoft.Office.Interop.Word;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для reportsPage.xaml
    /// </summary>
    public partial class reportsPage : Page
    {
        IISAutoPartsEntities _dbContext;
        Paginator paginator;

        private List<int> selectedIds = new List<int>();

        List<ReportsView> _orderReports = new List<ReportsView>();
        List<ReportsView> _provideReports = new List<ReportsView>();

        public reportsPage()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            _orderReports = FillDataOrderReports();

            reportsTypeCb.SelectedIndex = 0;

            paginator = new Paginator(_orderReports.ToList<object>(), 1, 10);

            pageNumber.Text = paginator.GetPage().ToString();
            countPage.Content = paginator.GetCountpage();

            reportDGV.ItemsSource = paginator.GetTable();

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as ReportsView;

            // Добавить ID элемента в список выбранных элементов, если он еще не был добавлен
            if (row != null && !selectedIds.Contains(row.id))
            {
                selectedIds.Add(row.id);
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получить строку DataGrid, соответствующую этому CheckBox
            var row = (sender as CheckBox)?.DataContext as ReportsView;

            // Удалить ID элемента из списка выбранных элементов, если он был добавлен ранее
            if (row != null && selectedIds.Contains(row.id))
            {
                selectedIds.Remove(row.id);
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.PreviousPage();
            pageNumber.Text = paginator.GetPage().ToString();
            reportDGV.ItemsSource = paginator.GetTable();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            paginator.NextPage();
            pageNumber.Text = paginator.GetPage().ToString();
            reportDGV.ItemsSource = paginator.GetTable();
        }

        private void pageNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void pageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageNumber.Text != null && pageNumber.Text != "")
            {
                paginator.SetPage(Convert.ToInt32(pageNumber.Text));
                reportDGV.ItemsSource = paginator.GetTable();
            }
        }

        private void reportsTypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIds.Clear();
            if (reportsTypeCb.SelectedIndex == 0)
            {
                _orderReports = FillDataOrderReports();

                paginator = new Paginator(_orderReports.ToList<object>(), 1, 10);

                pageNumber.Text = paginator.GetPage().ToString();
                countPage.Content = paginator.GetCountpage();

                reportDGV.ItemsSource = paginator.GetTable();

                clientCb.ItemsSource = _dbContext.customers.AsNoTracking().ToList();
                clientCb.SelectedValuePath = "id";
                clientCb.DisplayMemberPath = "name";

            }
            else
            {
                _provideReports = FillDataProvideReports();

                paginator = new Paginator(_provideReports.ToList<object>(), 1, 10);

                pageNumber.Text = paginator.GetPage().ToString();
                countPage.Content = paginator.GetCountpage();

                reportDGV.ItemsSource = paginator.GetTable();

                clientCb.ItemsSource = _dbContext.providers.AsNoTracking().ToList();
                clientCb.SelectedValuePath = "id";
                clientCb.DisplayMemberPath = "name";
            }
        }

        private async void createReportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (reportsTypeCb.SelectedIndex == 0)
                {
                    string client = clientCb.Text;
                    var _report = new orderReports()
                    {
                        customerId = (int)clientCb.SelectedValue,
                        dateBegin = startDateDt.SelectedDate,
                        dateEnd = endDateDt.SelectedDate,
                    };

                    _dbContext.orderReports.AddOrUpdate(_report);

                    _dbContext.SaveChanges();
                    loadingDoc.Visibility = Visibility.Visible;

                    await System.Threading.Tasks.Task.Run(() => CreateOrderDoc(_report.id, client));

                    loadingDoc.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Сохранено");
                }
                else
                {
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка сохранения. " + ex.Message);
            }
        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {

        }


        private List<ReportsView> FillDataOrderReports()
        {
            var datet = DateTime.Now;
            var _reports = _dbContext.orderReports.Join(_dbContext.customers, x => x.customerId, y => y.id, (x, y) => new ReportsView
            {
                id = x.id,
                customerId = x.customerId,
                customerName = y.name,
                dateBegin = x.dateBegin,
                dateEnd = x.dateEnd,
            }).ToList();
            return _reports;
        }

        private List<ReportsView> FillDataProvideReports()
        {
            var _reports = _dbContext.provideReports.Join(_dbContext.providers, x => x.providerId, y => y.id, (x, y) => new ReportsView
            {
                id = x.id,
                customerId = x.providerId,
                customerName = y.name,
                dateBegin = x.dateBegin,
                dateEnd = x.dateEnd,
            }).ToList();
            return _reports;
        }



        private void CreateOrderDoc(int id, string client)
        {
            try
            {
                string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/ReportTemplate.docx");
                string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет №{id} по автозапчастям для информационной системы.docx");

                var _report = _dbContext.orderReports.AsNoTracking().Where(x=>x.id == id).FirstOrDefault();

                string dateStart = _report.dateBegin.Value.ToString("dd.MM.yyyy");
                string dateEnd = _report.dateEnd.Value.ToString("dd.MM.yyyy");



                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);
                doc.Content.Find.Execute(FindText: "@typereport",
                    ReplaceWith: "Заказы", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@type1report",
                   ReplaceWith: "заказов", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateStart",
                   ReplaceWith: dateStart, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateEnd",
                   ReplaceWith: dateEnd, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@provider",
                  ReplaceWith: client, Replace: Word.WdReplace.wdReplaceAll);
                     doc.Content.Find.Execute(FindText: "@typecustomer",
                  ReplaceWith: "Заказчик", Replace: Word.WdReplace.wdReplaceAll);


                //var ordersPeriod = _dbContext.Orders.AsNoTracking()
                //   .Where(x => x.dateOrder >= startDateDt.SelectedDate
                //   && x.dateOrder <= endDateDt.SelectedDate)
                //   .Join(_dbContext.autoparts, x=>x.idAutoparts, y=>y.id, (x, y) =>
                //   {
                       
                //   }).ToList();

                Word.Table table = doc.Tables[0];
                

                doc.SaveAs(SavePath);

                doc.Close();
                wordApp.Quit();

                byte[] file;

                using (System.IO.FileStream fs = new System.IO.FileStream(SavePath, FileMode.Open))
                {
                    file = new byte[fs.Length];
                    fs.Read(file, 0, file.Length);
                }


                var _doc = _dbContext.orderReports.Where(x => x.id == id).FirstOrDefault();
                _doc.doc = file;
                _dbContext.orderReports.AddOrUpdate(_doc);
                _dbContext.SaveChanges();
                Thread.Sleep(10);
                Process.Start(SavePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
