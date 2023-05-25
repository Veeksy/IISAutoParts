using IISAutoParts.Class;
using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для provideAddEdit.xaml
    /// </summary>
    public partial class provideAddEdit : Page
    {
        IISAutoPartsEntities _dbContext;

        provide _provide = new provide();
        List<autoparts> _autoparts;
        List<providers> _providers;


        public provideAddEdit()
        {
            InitializeComponent();
            _dbContext = new IISAutoPartsEntities();

            _autoparts = _dbContext.autoparts.AsNoTracking().ToList();
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
                _provide.idAutoparts = (int)autopartCb.SelectedValue;
                _provide.dateDelivery = DateOrderTb.SelectedDate;

                _provide.provideNumber = Convert.ToInt32(orderNumberTb.Text);
                _provide.idProvider = (int)ProvidersCb.SelectedValue;
                _provide.countAutoparts = Convert.ToInt32(countTb.Text);

                var autopart = _dbContext.autoparts.AsNoTracking().Where(x => x.id == (int)autopartCb.SelectedValue).FirstOrDefault();

                autopart.count += Convert.ToInt32(countTb.Text);
                _dbContext.autoparts.AddOrUpdate(autopart);


                _dbContext.provide.AddOrUpdate(_provide);

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

                    var part = _autoparts.Where(x => x.id == _provide.idAutoparts).FirstOrDefault();
                    doc.Content.Find.Execute(FindText: "@autopart",
                        ReplaceWith: $@"{part.manufacturer} {part.name}", Replace: Word.WdReplace.wdReplaceAll);
                    doc.Content.Find.Execute(FindText: "@count",
                        ReplaceWith: _provide.countAutoparts, Replace: Word.WdReplace.wdReplaceAll);
                    doc.Content.Find.Execute(FindText: "@dateOrder",
                        ReplaceWith: Convert.ToDateTime(_provide.dateDelivery).ToString("dd.MM.yyyy"), Replace: Word.WdReplace.wdReplaceAll);
                    doc.Content.Find.Execute(FindText: "@price",
                       ReplaceWith: $@"{part.price.Value.ToString("F2")} руб.", Replace: Word.WdReplace.wdReplaceAll);
                    doc.Content.Find.Execute(FindText: "@TotalPrice",
                       ReplaceWith: $@"{(_provide.countAutoparts * part.price).Value.ToString("F2")} руб.", Replace: Word.WdReplace.wdReplaceAll);

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

    }
}
