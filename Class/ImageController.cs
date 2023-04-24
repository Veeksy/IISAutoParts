using IISAutoParts.DBcontext;
using IISAutoParts.pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IISAutoParts.Class
{
    public static class ImageController
    {
        public static byte[] ImageData { get; set; }

        public static BitmapImage ReturnImageFromDataBase(byte[] bytes)
        {
            byte[] buffer = bytes;
            MemoryStream byteStream = new MemoryStream(buffer);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
            return image;
        }

        public static byte[] ReturnChoosedFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // создаем диалоговое окно
            openFileDialog.ShowDialog(); // показываем
            byte[] image_bytes = File.ReadAllBytes(openFileDialog.FileName); // получаем байты выбранного файла
            return image_bytes;
        }




    }
}
