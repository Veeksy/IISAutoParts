using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using IISAutoParts.DBcontext.MyEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для provideAddEdit.xaml
    /// </summary>
    public partial class provideAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;

        provide _provide = new provide();
        List<providers> _providers;

        List<ListProvide> _listProvide = new List<ListProvide>();
        List<listAutopartOrder> lists = new List<listAutopartOrder>();
        public provideAddEdit()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            var s = _dbContext.provide.OrderByDescending(x => x.id).FirstOrDefault();

            orderNumberTb.Text = (Convert.ToInt32(s.provideNumber) + 1).ToString();


            
            _providers = _dbContext.providers.AsNoTracking().ToList();

            CarCb.ItemsSource = _dbContext.cars.AsNoTracking().ToList();
            CarCb.DisplayMemberPath = "name";
            CarCb.SelectedValuePath = "id";

            ProvidersCb.ItemsSource = _providers;
            ProvidersCb.DisplayMemberPath = "name";
            ProvidersCb.SelectedValuePath = "id";


        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new providePage());
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _provide.dateDelivery = DateOrderTb.SelectedDate;

                _provide.provideNumber = Convert.ToInt32(orderNumberTb.Text);
                _provide.idProvider = (int)ProvidersCb.SelectedValue;

                _dbContext.provide.AddOrUpdate(_provide);

                


                foreach (var item in _listProvide)
                {
                    var at = _dbContext.autoparts.AsNoTracking().Where(x=>x.id == item.idAutoparts).FirstOrDefault();
                    at.count += item.countAutoparts;
                    _dbContext.autoparts.AddOrUpdate(at);
                }

                _dbContext.SaveChanges();

                foreach (var item in _listProvide)
                {
                    item.idProvide = _provide.id;
                }

                _dbContext.ListProvide.AddRange(_listProvide);
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

        private void CreateDoc()
        {
            try
            {
                string Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/ProvideTemplate.docx");
                string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Templates/Акт№{_provide.provideNumber} поставки автозапчастей для информационной системы.docx");

                Word.Application wordApp = new Word.Application();

                Word.Document doc = wordApp.Documents.Open(Template);
                doc.Content.Find.Execute(FindText: "@numberDoc",
                    ReplaceWith: _provide.provideNumber.ToString(), Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@customer",
                    ReplaceWith: _providers.Where(x => x.id == _provide.idProvider).Select(x => x.name).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@address",
                    ReplaceWith: _providers.Where(x => x.id == _provide.idProvider).Select(x => x.address).FirstOrDefault(), Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@dateOrder",
                    ReplaceWith: Convert.ToDateTime(_provide.dateDelivery).ToString("dd.MM.yyyy"), Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "@user",
                    ReplaceWith: UserController.userName, Replace: Word.WdReplace.wdReplaceAll);

                var parts = _dbContext.autoparts.Join(_dbContext.ListProvide, x => x.id, y => y.idAutoparts, (x, y) => new
                {
                    id = y.idAutoparts,
                    article = x.article,
                    name = x.name,
                    manufacturer = x.manufacturer,
                    count = y.countAutoparts,
                    price = x.price,
                    idProvide = y.idProvide

                }).Where(x => x.idProvide == _provide.id).ToList();


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


                var _doc = new provideDoc()
                {
                    provideId = _provide.id,
                    doc = file,
                };
                _dbContext.provideDoc.AddOrUpdate(_doc);
                _dbContext.SaveChanges();
                Thread.Sleep(10);
                Process.Start(SavePath);

            }
            catch (Exception ex)
            {
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
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var item = button.DataContext as listAutopartOrder; // Получите элемент, связанный с кнопкой
            _listProvide.Remove(_listProvide.Where(x => x.idAutoparts == item.id).FirstOrDefault());
            lists.Remove(lists.Where(x => x.id == item.id).FirstOrDefault());

            ListAutoparts.ItemsSource = lists;

            ListAutoparts.Items.Refresh();
        }

        private void AddAutopart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var at = _dbContext.autoparts.Where(x => x.id == (int)autopartCb.SelectedValue).FirstOrDefault();

                if (_listProvide.Select(x => x.idAutoparts).Contains(at.id))
                {
                    MessageBox.Show("Эта запчасть уже добавлена");
                }
                else
                {

                    _listProvide.Add(new ListProvide()
                    {
                        idAutoparts = (int)autopartCb.SelectedValue,
                        countAutoparts = Convert.ToInt32(countTb.Text),
                    });

                    var autoparts = _dbContext.autoparts.ToList();

                    lists = autoparts.Join(_listProvide, x => x.id, y => y.idAutoparts, (x, y) => new listAutopartOrder()
                    {
                        id = x.id,
                        name = x.article + " " + x.manufacturer + " " + x.name,
                        count = y.countAutoparts ?? 0,
                    }).ToList();

                    ListAutoparts.ItemsSource = lists;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(/*"Пустое поле с запчастью."*/ ex.Message);
            }

        }
    }
}
