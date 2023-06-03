﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using IISAutoParts.DBcontext;
using IISAutoParts.Class;

namespace IISAutoParts.pages
{
    /// <summary>
    /// Логика взаимодействия для ImportAutopartsPage1.xaml
    /// </summary>
    public partial class ImportAutopartsPage1 : Page
    {
        IISAutoPartsEntities _dbContext;

        List<autoparts> _autoparts = new List<autoparts>();
        int idModel;
        public ImportAutopartsPage1(int idModel)
        {
            InitializeComponent();

            _dbContext = new IISAutoPartsEntities();
            this.idModel = idModel;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameController.MainFrame.Navigate(new autopartsPage(this.idModel));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dbContext.autoparts.AddRange(_autoparts);
                _dbContext.SaveChanges();

                var atMod = new List<autopartsModel>();

                foreach (var item in _autoparts)
                {
                    atMod.Add(new autopartsModel
                    {
                        idAutoparts = item.id,
                        idModel = idModel,
                    });
                }

                _dbContext.autopartsModel.AddRange(atMod);
                _dbContext.SaveChanges();
                System.Windows.Forms.MessageBox.Show("Данные добавлены");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файл Excel|*.xls;*.xlsx;*.xlsm";
            openFileDialog.ShowDialog();
            NameFile.Text = openFileDialog.FileName;

            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Open(openFileDialog.FileName);
            try
            {
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];

                var headers = Enumerable.Range(1, worksheet.UsedRange.Columns.Count)
                    .Select(column => (string)(worksheet.Cells[1, column] as Excel.Range)?.Value)
                    .ToArray();

                var data = (object[,])worksheet.UsedRange.Value[Excel.XlRangeValueDataType.xlRangeValueDefault];
                var rows = Enumerable.Range(2, worksheet.UsedRange.Rows.Count - 1)
                    .Select(row => new autoparts()
                    {
                        article = data[row, Array.IndexOf(headers, "Артикул") + 1].ToString(),
                        manufacturer = data[row, Array.IndexOf(headers, "Производитель") + 1].ToString(),
                        name = data[row, Array.IndexOf(headers, "Наименование") + 1].ToString(),
                        description = data[row, Array.IndexOf(headers, "Описание") + 1].ToString(),
                        price = decimal.Parse(data[row, Array.IndexOf(headers, "Цена") + 1].ToString() ?? "0"),
                        year = int.Parse(data[row, Array.IndexOf(headers, "Год выпуска") + 1].ToString() ?? "2020"),
                        count = int.Parse(data[row, Array.IndexOf(headers, "Количество на склад") + 1].ToString() ?? "0"),
                        idCategory = int.Parse(data[row, Array.IndexOf(headers, "Код категории") + 1].ToString() ?? "0"),
                    }).ToList();

                foreach (var item in rows)
                {
                    _autoparts.Add(item);
                }
                autopartDGV.ItemsSource = _autoparts;

                workbook.Close();
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                workbook.Close();
                excelApp.Quit();
                System.Windows.MessageBox.Show(ex.Message);
            }


        }
    }
}
