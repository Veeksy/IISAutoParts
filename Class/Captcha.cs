using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IISAutoParts.Class
{
    public class Captcha
    {
        private const string SYMVOLS_CAPCHA = "QWERTYUIOPASDFGHJKLZXCVBNM123456789";

        private Random random;

        private string Answer;

        public int CountSymvol { get; set; }

        private Canvas canvas;

        public Captcha(int countSynvol, Canvas canvas)
        {
            this.canvas = canvas;

            random = new Random();
            CountSymvol = countSynvol;

            GenerateRandomString();
        }

        private Color GetRundomColor()
        {
            switch (random.Next(1, 5))
            {
                case 1:
                    return Colors.Black;
                case 2:
                    return Colors.Orange;
                case 3:
                    return Colors.Aqua;
                default:
                    return Colors.Red;
            }
        }

        private void GenerateRandomString()
        {
            string capchaString = "";

            for (int i = 0; i < CountSymvol; i++)
                capchaString += SYMVOLS_CAPCHA[random.Next(0, SYMVOLS_CAPCHA.Length)];

            Answer = capchaString;
        }

        private void DrawLine(Point point1, Point point2, double thickness = 5)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(GetRundomColor());

            line.X1 = point1.X;
            line.X2 = point2.X;

            line.Y1 = point1.Y;
            line.Y2 = point2.Y;

            line.StrokeThickness = thickness;
            canvas.Children.Add(line);
        }

        private void CanvasUpdate()
        {
            canvas.Children.Clear();

            var widht = 10;

            for (int i = 0; i < CountSymvol; i++)
            {
                TextBlock letter = new TextBlock();
                letter.Text = Answer[i].ToString();
                letter.FontSize = 42;
                letter.Foreground = new SolidColorBrush(GetRundomColor());

                Canvas.SetLeft(letter, widht);
                Canvas.SetTop(letter, random.Next(-5, 15));
                canvas.Children.Add(letter);

                widht += 30 + random.Next(0, 30);
            }

            DrawLine(new Point(0, 0), new Point(canvas.RenderSize.Width, canvas.RenderSize.Height));
            DrawLine(new Point(0, canvas.RenderSize.Height), new Point(canvas.RenderSize.Width, 0));
        }

        public void GenerateNew()
        {
            GenerateRandomString();
            CanvasUpdate();
        }

        public bool CheckCapcha(string userInput)
        {
            return userInput.ToLower() == Answer.ToLower();
        }
    }
}
