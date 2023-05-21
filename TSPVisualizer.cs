using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commivoyager
{
    public class TSPVisualizer
    {
        public List<int> SimplifyPath(List<int> path)
        {
            List<int> simplifiedPath = new List<int>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] != path[i + 1])
                {
                    simplifiedPath.Add(path[i]);
                }
            }
            simplifiedPath.Add(path[path.Count - 1]);
            simplifiedPath.Insert(0, path[path.Count - 1]);
            return simplifiedPath;
        }

        private const int CityRadius = 20; // радиус окружности, представляющей город
        private const int CityMargin = 30; // отступ между городами

        private readonly double[,] distanceMatrix;
        private readonly List<Point> cityLocations;

        private readonly PictureBox pictureBox;
        private readonly Bitmap bitmap;

        private List<int> path;

        public TSPVisualizer(double[,] distanceMatrix, PictureBox pictureBox)
        {
            this.distanceMatrix = distanceMatrix;
            this.pictureBox = pictureBox;
            cityLocations = new List<Point>();

            // создаем новое изображение и устанавливаем его в качестве изображения pictureBox
            bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = bitmap;

            // вычисляем местоположение городов на pictureBox
            CalculateCityLocations();
        }

        // метод для вычисления местоположения городов на pictureBox
        private void CalculateCityLocations()
        {
            int numCities = distanceMatrix.GetLength(0);
            int numCols = (int)Math.Ceiling(Math.Sqrt(numCities)); // количество столбцов на pictureBox
            int numRows = (int)Math.Ceiling((double)numCities / numCols); // количество строк на pictureBox

            // вычисляем размеры pictureBox
            int width = numCols * CityRadius * 2 + (numCols + 1) * CityMargin;
            int height = numRows * CityRadius * 2 + (numRows + 1) * CityMargin;
            pictureBox.Width = width;
            pictureBox.Height = height;

            // вычисляем местоположение каждого города на pictureBox
            for (int i = 0; i < numCities; i++)
            {
                int colIndex = i % numCols;
                int rowIndex = i / numCols;
                int x = (colIndex + 1) * CityMargin + colIndex * CityRadius * 2;
                int y = (rowIndex + 1) * CityMargin + rowIndex * CityRadius * 2;
                cityLocations.Add(new Point(x, y));
            }
        }

        // метод для отрисовки городов на pictureBox
        private void DrawCities()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < distanceMatrix.GetLength(0); i++)
                {
                    Point cityLocation = cityLocations[i];
                    g.FillEllipse(Brushes.Blue, cityLocation.X - CityRadius, cityLocation.Y - CityRadius, CityRadius * 2,
                        CityRadius * 2);
                    g.DrawString((i + 1).ToString(), new Font("Arial", 10), Brushes.White,
                        new PointF(cityLocation.X - 7, cityLocation.Y - 7));
                }
            }
        }

        // метод для отрисовки пути на pictureBox
        private void DrawPath()
        {
            if (path == null || path.Count == 0)
            {
                return;
            }

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                int ArrowLength = 12;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    int fromCityIndex = path[i];
                    int toCityIndex = path[i + 1];
                    Point fromCityLocation = cityLocations[fromCityIndex - 1];
                    Point toCityLocation = cityLocations[toCityIndex - 1];
                    DrawArrow(g, Pens.Red, fromCityLocation, toCityLocation);
                    double angle = Math.Atan2(toCityLocation.Y - fromCityLocation.Y, toCityLocation.X - fromCityLocation.X);
                    double x1 = Math.Cos(angle + Math.PI / 6) * ArrowLength + toCityLocation.X;
                    double y1 = Math.Sin(angle + Math.PI / 6) * ArrowLength + toCityLocation.Y;
                    double x2 = Math.Cos(angle - Math.PI / 6) * ArrowLength + toCityLocation.X;
                    double y2 = Math.Sin(angle - Math.PI / 6) * ArrowLength + toCityLocation.Y;
                    g.DrawLine(Pens.Red, toCityLocation, new Point((int)x1, (int)y1));
                    g.DrawLine(Pens.Red, toCityLocation, new Point((int)x2, (int)y2));
                }
                // соединяем последний город с первым городом, чтобы закончить цикл
                int firstCityIndex = path[0];
                int lastCityIndex = path[path.Count - 1];
                Point firstCityLocation = cityLocations[firstCityIndex];
                Point lastCityLocation = cityLocations[lastCityIndex];
                g.DrawLine(Pens.Black, lastCityLocation, firstCityLocation);
            }

        }

        private void DrawArrow(Graphics g, Pen pen, Point from, Point to)
        {
            const int ArrowSize = CityRadius * 2; // размер стрелки

            // вычисляем направление и длину линии
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);

            // вычисляем координаты конца линии
            var x2 = to.X - (int)(dx / length * CityRadius);
            var y2 = to.Y - (int)(dy / length * CityRadius);

            // вычисляем координаты стрелки
            var x3 = to.X - (int)(dx / length * (CityRadius + ArrowSize));
            var y3 = to.Y - (int)(dy / length * (CityRadius + ArrowSize));
            var x4 = to.X - (int)(dx / length * (CityRadius + ArrowSize / 2));
            var y4 = to.Y - (int)(dy / length * (CityRadius + ArrowSize / 2));

            // рисуем линию
            g.DrawLine(pen, from, new Point(x2, y2));

            // рисуем стрелку
            var points = new[] { new Point(x3, y3), to, new Point(x4, y4) };
            g.FillPolygon(Brushes.Red, points);
            g.DrawPolygon(pen, points);
        }

        // метод для обновления pictureBox с новым маршрутом
        public void UpdatePath(List<int> newPath)
        {
            path = SimplifyPath(newPath);
            DrawCities();
            DrawPath();
            pictureBox.Refresh();
        }
    }
}
