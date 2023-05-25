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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;
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

            if (UserController.permissionList.Where(x => x.Sector == "Отчеты").Select(x => x.Add).FirstOrDefault())
                createReportBtn.IsEnabled = true;
            else
                createReportBtn.IsEnabled = false;
            if (UserController.permissionList.Where(x => x.Sector == "Отчеты").Select(x => x.Delete).FirstOrDefault())
                deleteBtn.IsEnabled = true;
            else
                deleteBtn.IsEnabled = false;

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
                        customerId = clientCb.SelectedValue != null ? ((int)clientCb.SelectedValue) : (0),
                        dateBegin = startDateDt.SelectedDate,
                        dateEnd = endDateDt.SelectedDate,
                    };

                    _dbContext.orderReports.AddOrUpdate(_report);

                    _dbContext.SaveChanges();
                    loadingDoc.Visibility = Visibility.Visible;

                    await System.Threading.Tasks.Task.Run(() => CreateOrderDoc(_report, client));

                    loadingDoc.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Сохранено");
                }
                else
                {
                    string client = clientCb.Text;
                    var _report = new provideReports()
                    {
                        providerId = clientCb.SelectedValue != null ? ((int)clientCb.SelectedValue) : (0),
                        dateBegin = startDateDt.SelectedDate,
                        dateEnd = endDateDt.SelectedDate,
                    };

                    _dbContext.provideReports.AddOrUpdate(_report);

                    _dbContext.SaveChanges();
                    loadingDoc.Visibility = Visibility.Visible;

                    await System.Threading.Tasks.Task.Run(() => CreateProvideDoc(_report, client));

                    loadingDoc.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Сохранено");
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



        private void CreateOrderDoc(orderReports report, string client)
        {
            try
            {
                string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/ReportTemplate.docx");
                string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет №{report.id} по автозапчастям для информационной системы.docx");

                var _report = _dbContext.orderReports.AsNoTracking().Where(x=>x.id == report.id).FirstOrDefault();

                string dateStart = _report.dateBegin.Value.ToString("dd.MM.yyyy");
                string dateEnd = _report.dateEnd.Value.ToString("dd.MM.yyyy");


                List<orderReport> query = (from order in _dbContext.Orders
                            join customer in _dbContext.customers on order.idCustomer equals customer.id into ocJoin
                            from oc in ocJoin.DefaultIfEmpty()
                            join apModel in _dbContext.autopartsModel on order.idAutoparts equals apModel.idAutoparts into oamJoin
                            from oam in oamJoin.DefaultIfEmpty()
                            join carModel in _dbContext.carModels on oam.idModel equals carModel.id into ocmJoin
                            from ocm in ocmJoin.DefaultIfEmpty()
                            join autopart in _dbContext.autoparts on order.idAutoparts equals autopart.id into oaJoin
                            from oa in oaJoin.DefaultIfEmpty()
                            join car in _dbContext.cars on ocm.idCar equals car.id into ocarsJoin
                            from ocars in ocarsJoin.DefaultIfEmpty()
                            where (_report.dateBegin == null || order.dateOrder >= _report.dateBegin) && (_report.dateEnd == null || order.dateOrder <= _report.dateEnd) &&
                            (report.customerId == 0 || order.idCustomer == report.customerId)
                            select new orderReport
                            {
                                car = ocars.name,
                                model = ocm.model,
                                manufacturer = oa.manufacturer,
                                article = oa.article,
                                name = oa.name,
                                price = oa.price,
                                customer = oc.name,
                                countAutoparts = order.countAutoparts,
                                dateOrder = order.dateOrder,
                                totalSum = oa.price * order.countAutoparts
                            }).ToList();




                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);

                decimal totalSum = 0;

                Word.Table table = doc.Tables[1];

                // Заполнение строки заголовков
                for (int i = 0; i < query.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query[i].car;
                    row.Cells[2].Range.Text = query[i].model;
                    row.Cells[3].Range.Text = query[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = query[i].customer;
                    row.Cells[5].Range.Text = query[i].dateOrder.Value.ToString("dd.MM.yyyy");
                    row.Cells[6].Range.Text = query[i].countAutoparts.ToString();
                    row.Cells[7].Range.Text = query[i].price.Value.ToString("F2");
                    row.Cells[8].Range.Text = query[i].totalSum.Value.ToString("F2");
                    totalSum += query[i].totalSum.Value;
                }

                doc.Content.Find.Execute(FindText: "@numberDoc",
                   ReplaceWith: _report.id, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@typereport",
                   ReplaceWith: "заказов", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateStart",
                   ReplaceWith: dateStart, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateEnd",
                   ReplaceWith: dateEnd, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@provider",
                  ReplaceWith: client, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@typecustomer",
                ReplaceWith: "Заказчик", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@totalSum",
                ReplaceWith: totalSum.ToString("F2"), Replace: Word.WdReplace.wdReplaceAll);


                doc.SaveAs(SavePath);
                doc.Close();
                wordApp.Quit();

                byte[] file;

                using (System.IO.FileStream fs = new System.IO.FileStream(SavePath, FileMode.Open))
                {
                    file = new byte[fs.Length];
                    fs.Read(file, 0, file.Length);
                }


                var _doc = _dbContext.orderReports.Where(x => x.id == report.id).FirstOrDefault();
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

        private void CreateProvideDoc(provideReports report, string client)
        {
            try
            {
                string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/ReportTemplate.docx");
                string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет №{report.id} по автозапчастям для информационной системы.docx");

                var _report = _dbContext.provideReports.AsNoTracking().Where(x => x.id == report.id).FirstOrDefault();

                string dateStart = _report.dateBegin.Value.ToString("dd.MM.yyyy");
                string dateEnd = _report.dateEnd.Value.ToString("dd.MM.yyyy");


                List<orderReport> query = (from provide in _dbContext.provide
                                           join customer in _dbContext.providers on provide.idProvider equals customer.id into ocJoin
                                           from oc in ocJoin.DefaultIfEmpty()
                                           join apModel in _dbContext.autopartsModel on provide.idAutoparts equals apModel.idAutoparts into oamJoin
                                           from oam in oamJoin.DefaultIfEmpty()
                                           join carModel in _dbContext.carModels on oam.idModel equals carModel.id into ocmJoin
                                           from ocm in ocmJoin.DefaultIfEmpty()
                                           join autopart in _dbContext.autoparts on provide.idAutoparts equals autopart.id into oaJoin
                                           from oa in oaJoin.DefaultIfEmpty()
                                           join car in _dbContext.cars on ocm.idCar equals car.id into ocarsJoin
                                           from ocars in ocarsJoin.DefaultIfEmpty()
                                           where (_report.dateBegin == null || provide.dateDelivery >= _report.dateBegin) && (_report.dateEnd == null || provide.dateDelivery <= _report.dateEnd) &&
                                            (report.providerId == 0 || provide.idProvider == report.providerId)
                                           select new orderReport
                                           {
                                               car = ocars.name,
                                               model = ocm.model,
                                               manufacturer = oa.manufacturer,
                                               article = oa.article,
                                               name = oa.name,
                                               price = oa.price,
                                               customer = oc.name,
                                               countAutoparts = provide.countAutoparts,
                                               dateOrder = provide.dateDelivery,
                                               totalSum = oa.price * provide.countAutoparts
                                           }).ToList();




                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);

                decimal totalSum = 0;

                Word.Table table = doc.Tables[1];

                // Заполнение строки заголовков
                for (int i = 0; i < query.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query[i].car;
                    row.Cells[2].Range.Text = query[i].model;
                    row.Cells[3].Range.Text = query[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = query[i].customer;
                    row.Cells[5].Range.Text = query[i].dateOrder.Value.ToString("dd.MM.yyyy");
                    row.Cells[6].Range.Text = query[i].countAutoparts.ToString();
                    row.Cells[7].Range.Text = query[i].price.Value.ToString("F2");
                    row.Cells[8].Range.Text = query[i].totalSum.Value.ToString("F2");
                    totalSum += query[i].totalSum.Value;
                }

                doc.Content.Find.Execute(FindText: "@numberDoc",
                   ReplaceWith: _report.id, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@typereport",
                   ReplaceWith: "поставок", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateStart",
                   ReplaceWith: dateStart, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateEnd",
                   ReplaceWith: dateEnd, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@provider",
                  ReplaceWith: client, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@typecustomer",
                ReplaceWith: "Поставщик", Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@totalSum",
                ReplaceWith: totalSum.ToString("F2"), Replace: Word.WdReplace.wdReplaceAll);


                doc.SaveAs(SavePath);
                doc.Close();
                wordApp.Quit();

                byte[] file;

                using (System.IO.FileStream fs = new System.IO.FileStream(SavePath, FileMode.Open))
                {
                    file = new byte[fs.Length];
                    fs.Read(file, 0, file.Length);
                }


                var _doc = _dbContext.provideReports.Where(x => x.id == report.id).FirstOrDefault();
                _doc.doc = file;
                _dbContext.provideReports.AddOrUpdate(_doc);
                _dbContext.SaveChanges();
                Thread.Sleep(10);
                Process.Start(SavePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox
               .Show("Действительно удалить выбранные записи?", "Подтвердите действие",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                if (reportsTypeCb.SelectedIndex == 0)
                {
                    var deleted = _dbContext.orderReports.Where(x => selectedIds.Contains(x.id)).ToList();
                    _dbContext.orderReports.RemoveRange(deleted);

                    _dbContext.SaveChanges();
                    _orderReports = FillDataOrderReports();

                    paginator = new Paginator(_orderReports.ToList<object>(), paginator.GetPage(), 10);
                    reportDGV.ItemsSource = paginator.GetTable();
                }
                else
                {
                    var deleted = _dbContext.provideReports.Where(x => selectedIds.Contains(x.id)).ToList();
                    _dbContext.provideReports.RemoveRange(deleted);

                    _dbContext.SaveChanges();
                    _provideReports = FillDataProvideReports();

                    paginator = new Paginator(_provideReports.ToList<object>(), paginator.GetPage(), 10);
                    reportDGV.ItemsSource = paginator.GetTable();
                }
            }
        }
    }
}
