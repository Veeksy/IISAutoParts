using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
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
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using Path = System.IO.Path;
using System.Threading;
using IISAutoParts.DBcontext.MyEntities;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для ordersAddEdit.xaml
    /// </summary>
    public partial class ordersAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;

        Orders _order;
        List<autoparts> _autoparts;
        List<customers> _customers;

        List<ListOrder> _listOrder = new List<ListOrder>();
        List<listAutopartOrder> lists = new List<listAutopartOrder>();
        public ordersAddEdit(int? orderId = 0)
        {
            InitializeComponent();
            try
            {
                _dbContext = new IISAutoPartsEntities();
                
                _order = _dbContext.Orders.OrderByDescending(x=>x.id).FirstOrDefault();

                orderNumberTb.Text = (Convert.ToInt32(_order.orderNumber) + 1).ToString();

                _autoparts = _dbContext.autoparts.AsNoTracking().ToList();
                _customers = _dbContext.customers.AsNoTracking().ToList();

                CarCb.ItemsSource = _dbContext.cars.AsNoTracking().ToList();
                CarCb.DisplayMemberPath = "name";
                CarCb.SelectedValuePath = "id";

                CustomerCb.ItemsSource = _customers;
                CustomerCb.DisplayMemberPath = "name";
                CustomerCb.SelectedValuePath = "id";

                _order = new Orders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе данных. " + ex.Message);
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _order.dateOrder = DateOrderTb.SelectedDate;

                _order.orderNumber = string.IsNullOrEmpty(orderNumberTb.Text) ? (0) : (Convert.ToInt32(orderNumberTb.Text));
                _order.idCustomer = (int)CustomerCb.SelectedValue;
                _dbContext.Orders.AddOrUpdate(_order);
                _dbContext.SaveChanges();

                foreach (var item in _listOrder)
                {
                    item.idOrder = _order.id;
                }

                _dbContext.ListOrder.AddRange(_listOrder);

                var __aut = _autoparts.Where(x => _listOrder.Select(y => y.idAutopart).Contains(x.id)).ToList();

                foreach (var item in __aut)
                {
                    item.count -= _listOrder.Where(x => x.idAutopart == item.id).Select(x => x.countAutoparts).FirstOrDefault();
                    _dbContext.autoparts.AddOrUpdate(item);
                }


                _dbContext.SaveChanges();

                loadingDoc.Visibility = Visibility.Visible;

                await Task.Run(() => CreateDoc());

                loadingDoc.Visibility = Visibility.Collapsed;
                MessageBox.Show("Сохранено");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка сохранения. " + ex.Message);
            }
        }

        private void priceTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new ordersPage());
        }

        private void CreateDoc()
        {
            string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/OrderTemplate.docx");
            string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Акт№{_order.orderNumber} заказа автозапчастей для информационной системы.docx");

            Word.Application wordApp = new Word.Application();
            Word.Document doc = null;
            try
            {

                doc = wordApp.Documents.Open(Template);

                doc.Content.Find.Execute(FindText: "@numberDoc",
                    ReplaceWith: _order.orderNumber.ToString(), Replace: Word.WdReplace.wdReplaceAll);

                doc.Content.Find.Execute(FindText: "@customer",
                    ReplaceWith: _customers.Where(x => x.id == _order.idCustomer).Select(x => x.name).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);

                doc.Content.Find.Execute(FindText: "@address",
                    ReplaceWith: _customers.Where(x => x.id == _order.idCustomer).Select(x => x.address).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);

                doc.Content.Find.Execute(FindText: "@dateOrder",
                    ReplaceWith: Convert.ToDateTime(_order.dateOrder).ToString("dd.MM.yyyy"), Replace: Word.WdReplace.wdReplaceAll);


                var parts = _dbContext.autoparts.Join(_dbContext.ListOrder, x => x.id, y => y.idAutopart, (x, y) => new
                {
                    id = y.idAutopart,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = y.countAutoparts,
                    price = x.price,
                    orderId = y.idOrder

                }).Where(x => x.orderId == _order.id).ToList();


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
               }).ToList();


                Word.Table table = doc.Tables[1];
                decimal totalSum = 0;
                // Заполнение строки заголовков
                for (int i = 0; i < query2.Count(); i++)
                {
                    var row = table.Rows.Add();
                    row.Cells[1].Range.Text = query2[i].car;
                    row.Cells[2].Range.Text = query2[i].model;
                    row.Cells[3].Range.Text = query2[i].article + " " + query[i].manufacturer + " " + query[i].name;
                    row.Cells[4].Range.Text = query2[i].count.Value.ToString();
                    row.Cells[5].Range.Text = query2[i].price.Value.ToString("F2");
                    row.Cells[6].Range.Text = "";
                    row.Cells[7].Range.Text = (query2[i].price * query2[i].count).Value.ToString("F2");
                    totalSum += (query2[i].price * query2[i].count) ?? 0;
                }
                doc.Content.Find.Execute(FindText: "@totalSum",
                   ReplaceWith: $@"{totalSum.ToString("F2")} руб.", Replace: Word.WdReplace.wdReplaceAll);

                doc.SaveAs(SavePath);

                doc.Close();
                wordApp.Quit();

                byte[] file;

                using (System.IO.FileStream fs = new System.IO.FileStream(SavePath, FileMode.Open))
                {
                    file = new byte[fs.Length];
                    fs.Read(file, 0, file.Length);
                }

                var _doc = new OrdersDoc()
                {
                    idOrders = _order.id,
                    doc = file,
                };
                _dbContext.OrdersDoc.AddOrUpdate(_doc);
                _dbContext.SaveChanges();
                Thread.Sleep(10);
                Process.Start(SavePath);
            }
            catch (Exception ex)
            {
                doc.Close();
                wordApp.Quit();
                MessageBox.Show(ex.Message);
            }
        }

        private void CarCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CarCb.SelectedValue != null)
                {
                    modelCarCb.ItemsSource = _dbContext.carModels.AsNoTracking().Where(x => x.idCar == (int)CarCb.SelectedValue).ToList();
                    modelCarCb.DisplayMemberPath = "model";
                    modelCarCb.SelectedValuePath = "id";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void modelCarCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (modelCarCb.SelectedValue != null)
                {
                    var autoparts = _dbContext.autopartsModel.Where(x => x.idModel == (int)modelCarCb.SelectedValue).Select(x => x.idAutoparts).ToList();
                    autopartCb.ItemsSource = _dbContext.autoparts.AsNoTracking().Where(x => autoparts.Contains(x.id)).ToList();
                    autopartCb.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void AddAutopart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var at = _autoparts.Where(x=>x.id == (int)autopartCb.SelectedValue).FirstOrDefault();


                if ((at.count ?? 0) - Convert.ToInt32(countTb.Text) < 0)
                {
                    MessageBox.Show($"Невозможно добавить запчапсть {at.manufacturer} {at.name}, " +
                        $"так как на складе осталось всего {at.count ?? 0}.");
                }
                else
                {
                    if(_listOrder.Select(x=>x.idAutopart).Contains(at.id))
                    {
                        MessageBox.Show("Эта запчасть уже добавлена");
                    }
                    else
                    {
                        
                        _listOrder.Add(new ListOrder()
                        {
                            idAutopart = (int)autopartCb.SelectedValue,
                            countAutoparts = Convert.ToInt32(countTb.Text),
                        });

                        var autoparts = _dbContext.autoparts.ToList();

                        lists = autoparts.Join(_listOrder, x => x.id, y => y.idAutopart, (x, y) => new listAutopartOrder()
                        {
                            id = x.id,
                            name = x.article + " " + x.manufacturer + " " + x.name,
                            count = y.countAutoparts ?? 0,
                        }).ToList();


                        ListAutoparts.ItemsSource = lists;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(/*"Пустое поле с запчастью."*/ ex.Message);
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var item = button.DataContext as listAutopartOrder; // Получите элемент, связанный с кнопкой
            _listOrder.Remove(_listOrder.Where(x => x.idAutopart == item.id).FirstOrDefault());

            lists.Remove(lists.Where(x => x.id == item.id).FirstOrDefault());

            ListAutoparts.ItemsSource = lists;

            ListAutoparts.Items.Refresh();
        }
    }
}

