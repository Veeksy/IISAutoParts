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


        public ordersAddEdit(int? orderId = 0)
        {
            InitializeComponent();
            try
            {
                _dbContext = new IISAutoPartsEntities();
                _order = _dbContext.Orders.Where(x => x.id == orderId).FirstOrDefault();
                _autoparts = _dbContext.autoparts.AsNoTracking().ToList();
                _customers = _dbContext.customers.AsNoTracking().ToList();

                if (_order != null)
                {
                    orderNumberTb.Text = _order.orderNumber.ToString();
                    autopartCb.ItemsSource = _autoparts;
                    autopartCb.SelectedValue = _order.idAutoparts;
                    autopartCb.DisplayMemberPath = "name";
                    autopartCb.SelectedValuePath = "id";

                    DateOrderTb.SelectedDate = _order.dateOrder ?? DateTime.Now;
                    countTb.Text = _order.countAutoparts.ToString();
                    CustomerCb.ItemsSource = _customers;
                    CustomerCb.SelectedValue = _order.idCustomer;
                    CustomerCb.DisplayMemberPath = "name";
                    CustomerCb.SelectedValuePath = "id";
                }
                else
                {
                    _order = new Orders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе данных. " + ex.Message);
            }



        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _order.idAutoparts = (int)autopartCb.SelectedValue;
                _order.dateOrder = DateOrderTb.SelectedDate;

                _order.orderNumber = Convert.ToInt32(orderNumberTb.Text);
                _order.idCustomer = (int)CustomerCb.SelectedValue;
                _order.countAutoparts = Convert.ToInt32(countTb.Text);
                _dbContext.Orders.AddOrUpdate(_order);
                _dbContext.SaveChanges();
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

        private async void SaveDocBtn_Click(object sender, RoutedEventArgs e)
        {

            loadingDoc.Visibility = Visibility.Visible;

            await Task.Run(() => CreateDoc());

            loadingDoc.Visibility = Visibility.Collapsed;
        }

        private void CreateDoc()
        {
            try
            {
                var _doc = _dbContext.OrdersDoc.AsNoTracking().Where(x => x.idOrders == _order.id).FirstOrDefault();

                if (_order.id != 0)
                {

                    string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/OrderTemplate.docx");
                    string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Акт№{_order.orderNumber} заказа автозапчастей для информационной системы.docx");

                    Word.Application wordApp = new Word.Application();

                    Word.Document doc = wordApp.Documents.Open(Template);

                    doc.Content.Find.Execute(FindText: "@numberDoc",
                        ReplaceWith: _order.orderNumber.ToString(), Replace: Word.WdReplace.wdReplaceAll);

                    doc.Content.Find.Execute(FindText: "@customer",
                        ReplaceWith: _customers.Where(x => x.id == _order.idCustomer).Select(x => x.name).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);

                    doc.Content.Find.Execute(FindText: "@address",
                        ReplaceWith: _customers.Where(x => x.id == _order.idCustomer).Select(x => x.address).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);

                    var part = _autoparts.Where(x => x.id == _order.idAutoparts).FirstOrDefault();

                    doc.Content.Find.Execute(FindText: "@autopart",
                        ReplaceWith: $@"{part.manufacturer} {part.name}", Replace: Word.WdReplace.wdReplaceAll);


                    doc.Content.Find.Execute(FindText: "@count",
                        ReplaceWith: _order.countAutoparts, Replace: Word.WdReplace.wdReplaceAll);

                    doc.Content.Find.Execute(FindText: "@dateOrder",
                        ReplaceWith: Convert.ToDateTime(_order.dateOrder).ToString("dd.MM.yyyy"), Replace: Word.WdReplace.wdReplaceAll);

                    doc.Content.Find.Execute(FindText: "@price",
                       ReplaceWith: $@"{part.price} руб.", Replace: Word.WdReplace.wdReplaceAll);

                    doc.Content.Find.Execute(FindText: "@TotalPrice",
                       ReplaceWith: $@"{(_order.countAutoparts * part.price).ToString()} руб.", Replace: Word.WdReplace.wdReplaceAll);


                    doc.SaveAs(SavePath);

                    doc.Close();
                    wordApp.Quit();

                    byte[] file;

                    using (System.IO.FileStream fs = new System.IO.FileStream(SavePath, FileMode.Open))
                    {
                        file = new byte[fs.Length];
                        fs.Read(file, 0, file.Length);
                    }

                    if (_doc != null)
                    {
                        _doc.doc = file;
                    }
                    else
                    {
                        _doc = new OrdersDoc()
                        {
                            idOrders = _order.id,
                            doc = file,
                        };
                    }
                    _dbContext.OrdersDoc.AddOrUpdate(_doc);
                    _dbContext.SaveChanges();
                    Thread.Sleep(10);
                    Process.Start(SavePath);
                }
                else
                {
                    MessageBox.Show("Сначала необходимо сохранить заказ.");
                }

                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}

