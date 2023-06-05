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

            var dateBegin = startDateDt.SelectedDate.ToString();
            var dateEnd = endDateDt.SelectedDate.ToString();

            if (string.IsNullOrEmpty(dateBegin) || string.IsNullOrEmpty(dateEnd))
                MessageBox.Show("Необходимо указать период.");
            else
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
        }

        private List<ReportsView> FillDataOrderReports()
        {
            var _reports = (from or in _dbContext.orderReports
                            join c in _dbContext.customers on or.customerId equals c.id into jC
                            from c in jC.DefaultIfEmpty()
                            select new ReportsView
                            {
                                id = or.id,
                                customerId = or.customerId,
                                customerName = c != null ? c.name : null,
                                dateBegin = or.dateBegin,
                                dateEnd = or.dateEnd,
                            }).ToList();


            return _reports;
        }

        private List<ReportsView> FillDataProvideReports()
        {
            var _reports = (from or in _dbContext.provideReports
                            join c in _dbContext.providers on or.providerId equals c.id into jC
                            from c in jC.DefaultIfEmpty()
                            select new ReportsView
                            {
                                id = or.id,
                                customerId = or.providerId,
                                customerName = c != null ? c.name : null,
                                dateBegin = or.dateBegin,
                                dateEnd = or.dateEnd,
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


                var _orders = _dbContext.Orders.Join(_dbContext.customers, x=>x.idCustomer, y=>y.id, (x, y) => new
                {
                    id = x.id,
                    dateOrder = x.dateOrder,
                    idCustomer = x.idCustomer,
                    customerName = y.name,
                }).Where(x=> (_report.dateBegin == null || x.dateOrder >= _report.dateBegin) 
                    && (_report.dateEnd == null
                    || x.dateOrder <= _report.dateEnd) &&
                (_report.customerId == 0 || x.idCustomer == _report.customerId)).ToList();

                var parts = _orders.Join(_dbContext.ListOrder, x=>x.id, y=>y.idOrder, (x, y) => new
                {
                    id = y.idAutopart,
                    count = y.countAutoparts,
                    customerId = x.idCustomer,
                    dateOrder = x.dateOrder,
                    customerName = x.customerName,
                }).Join(_dbContext.autoparts, x => x.id, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = y.article,
                    name = y.name,
                    manufacturer = y.manufacturer,
                    count = x.count,
                    price = y.price,
                    dateOrder = x.dateOrder,
                    customerName = x.customerName,
                }).ToList();

                var query = parts.Join(_dbContext.autopartsModel, x => x.id, y => y.idAutoparts, (x, y) => new
                {
                    modelId = y.idModel,
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    dateOrder = x.dateOrder,
                    customerName = x.customerName,
                }).Distinct().ToList();

                var query1 = query.Join(_dbContext.carModels, x => x.modelId, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = y.model,
                    carId = y.idCar,
                    dateOrder = x.dateOrder,
                    customerName = x.customerName,
                }).ToList();

                var query2 = query1.Join(_dbContext.cars, x => x.carId, y => y.id, (x, y) => new
                {
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = x.model,
                    car = y.name,
                    dateOrder = x.dateOrder,
                    customerName = x.customerName,
                }).ToList();





                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);

                decimal totalSum = 0;

                Word.Table table = doc.Tables[1];

                // Заполнение строки заголовков
                for (int i = 0; i < query.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query2[i].car;
                    row.Cells[2].Range.Text = query2[i].model;
                    row.Cells[3].Range.Text = query2[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = query2[i].customerName;
                    row.Cells[5].Range.Text = query2[i].dateOrder.Value.ToString("dd.MM.yyyy");
                    row.Cells[6].Range.Text = query2[i].count.ToString();
                    row.Cells[7].Range.Text = query2[i].price.Value.ToString("F2");
                    row.Cells[8].Range.Text = (query2[i].price * query2[i].count).Value.ToString("F2");
                    totalSum += (query2[i].price * query2[i].count).Value;
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


                var _provide = _dbContext.provide.Join(_dbContext.providers, x => x.idProvider, y => y.id, (x, y) => new
                {
                    id = x.id,
                    dateDelivery = x.dateDelivery,
                    idProvider = x.idProvider,
                    customerName = y.name,
                }).Where(x => (_report.dateBegin == null || x.dateDelivery >= _report.dateBegin)
                    && (_report.dateEnd == null
                    || x.dateDelivery <= _report.dateEnd) &&
                (_report.providerId == 0 || x.idProvider == _report.providerId)).ToList();

                var parts = _provide.Join(_dbContext.ListProvide, x => x.id, y => y.idProvide, (x, y) => new
                {
                    id = y.idAutoparts,
                    count = y.countAutoparts,
                    idProvider = x.idProvider,
                    dateDelivery = x.dateDelivery,
                    customerName = x.customerName,
                }).Join(_dbContext.autoparts, x => x.id, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = y.article,
                    name = y.name,
                    manufacturer = y.manufacturer,
                    count = x.count,
                    price = y.price,
                    dateDelivery = x.dateDelivery,
                    customerName = x.customerName,
                }).ToList();

                var query = parts.Join(_dbContext.autopartsModel, x => x.id, y => y.idAutoparts, (x, y) => new
                {
                    modelId = y.idModel,
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    dateDelivery = x.dateDelivery,
                    customerName = x.customerName,
                }).Distinct().ToList();

                var query1 = query.Join(_dbContext.carModels, x => x.modelId, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = y.model,
                    carId = y.idCar,
                    dateDelivery = x.dateDelivery,
                    customerName = x.customerName,
                }).ToList();

                var query2 = query1.Join(_dbContext.cars, x => x.carId, y => y.id, (x, y) => new
                {
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = x.model,
                    car = y.name,
                    dateDelivery = x.dateDelivery,
                    customerName = x.customerName,
                }).ToList();





                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);

                decimal totalSum = 0;

                Word.Table table = doc.Tables[1];

                // Заполнение строки заголовков
                for (int i = 0; i < query.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query2[i].car;
                    row.Cells[2].Range.Text = query2[i].model;
                    row.Cells[3].Range.Text = query2[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = query2[i].customerName;
                    row.Cells[5].Range.Text = query2[i].dateDelivery.Value.ToString("dd.MM.yyyy");
                    row.Cells[6].Range.Text = query2[i].count.ToString();
                    row.Cells[7].Range.Text = query2[i].price.Value.ToString("F2");
                    row.Cells[8].Range.Text = (query2[i].price * query2[i].count).Value.ToString("F2");
                    totalSum += (query2[i].price * query2[i].count).Value;
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

        private void reportDGV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReportsView selectedPart = reportDGV.SelectedItem as ReportsView;


            if (reportsTypeCb.SelectedIndex == 0)
            {
                if (selectedPart != null)
                {
                    byte[] file = _dbContext.orderReports.AsNoTracking()
                        .Where(x => x.id == selectedPart.id).Select(x => x.doc).FirstOrDefault();
                    if (file is null)
                    {
                        MessageBox.Show("Файлы отсутствуют");
                    }
                    else
                    {
                        string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет №{selectedPart.id} по автозапчастям для информационной системы.docx");

                        File.WriteAllBytes(SavePath, file);
                        Process.Start(SavePath);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось открыть выбранный объект.");
                }
            }
            else
            {
                if (selectedPart != null)
                {
                    byte[] file = _dbContext.provideReports.AsNoTracking()
                        .Where(x => x.id == selectedPart.id).Select(x => x.doc).FirstOrDefault();
                    if (file is null)
                    {
                        MessageBox.Show("Файлы отсутствуют");
                    }
                    else
                    {
                        string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет №{selectedPart.id} по автозапчастям для информационной системы.docx");

                        File.WriteAllBytes(SavePath, file);
                        Process.Start(SavePath);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось открыть выбранный объект.");
                }
            }
        }

        private async void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadingDoc.Visibility = Visibility.Visible;

                await System.Threading.Tasks.Task.Run(() => CreateDoc());

                loadingDoc.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            //AutopartsReport
        }

        private void CreateDoc()
        {
            string Template = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/AutopartsReport.docx");
            string SavePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Отчет по остаткам автозапчастей для информационной системы.docx");

            Word.Application wordApp = new Word.Application();
            Word.Document doc = null;
            try
            {

                doc = wordApp.Documents.Open(Template);


                var parts = _dbContext.autoparts.ToList();


                var query = parts.Join(_dbContext.autopartsModel, x => x.id, y => y.idAutoparts, (x, y) => new
                {
                    modelId = y.idModel,
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                }).Distinct().ToList();

                var query1 = query.Join(_dbContext.carModels, x => x.modelId, y => y.id, (x, y) => new
                {
                    id = x.id,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = y.model,
                    carId = y.idCar,
                }).ToList();

                var query2 = query1.Join(_dbContext.cars, x => x.carId, y => y.id, (x, y) => new
                {
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = x.count,
                    price = x.price,
                    model = x.model,
                    car = y.name,
                }).OrderBy(x => x.car).ToList();


                Word.Table table = doc.Tables[1];
                // Заполнение строки заголовков
                for (int i = 0; i < query2.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query2[i].car;
                    row.Cells[2].Range.Text = query2[i].model;
                    row.Cells[3].Range.Text = query2[i].article + " " + query[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = (query2[i].count.HasValue ? query2[i].count : 0).ToString();
                    row.Cells[5].Range.Text = query2[i].price.Value.ToString("F2");
                    row.Cells[6].Range.Text = (query2[i].price.Value * (query2[i].count.HasValue ? query2[i].count : 0)).Value.ToString("F2");
                }
                doc.SaveAs(SavePath);
                Thread.Sleep(10);
                Process.Start(SavePath);
                doc.Close();
                wordApp.Quit();
            }
            catch (Exception ex)
            {
                doc.Close();
                wordApp.Quit();
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
