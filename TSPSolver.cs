using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commivoyager
{


    public class TSPSolver
    {

        private static double H;

        private static List<int> Str;
        private static List<int> Stb;

        private static List<int> res = new List<int>();
        private static List<int> result = new List<int>();
        public static double PathLenght;
        private static double[,] StartMatrix;
        private static double[,] matrix;

        public bool Start(DataGridView dataGridView, int n, out List<int> EndResult, out double EndPathLength)
        {
            try
            {
                result = new List<int>();
                res = new List<int>();
                PathLenght = 0;
                H = 0;
                Str = new List<int>();
                Stb = new List<int>();
                //Инициализируем массивы для сохранения индексов
                for (int i = 0; i < n; i++)
                {
                    Str.Add(i);
                    Stb.Add(i);
                }
                matrix = ReadInputData(dataGridView, n);
                StartMatrix = (double[,])matrix.Clone();
                //заполнение диагнонали
                for (int i = 0; i < n; i++)
                {
                    matrix[i, i] = double.PositiveInfinity;
                }

                Solve(matrix, n);

                MakeRes();
                EndResult = result;
                EndPathLength = PathLenght;
                return true;
            }
            catch (Exception ex)
            {
                DBSave dBSave = new DBSave();
                dBSave.SaveException(ex);
                EndResult = null;
                EndPathLength = 0;
                MessageBox.Show("Неккоректный ввод!", "Ошибка");
                return false;
            }
        }


        //функция считывания матрицы
        public double[,] ReadInputData(DataGridView dataGridView, int n)
        {

            try
            {
                double[,] distanceMatrix = new double[n, n]; // матрица расстояний
                for (int i = 0; i < n; i++)
                {

                    for (int j = 0; j < n; j++)
                    {
                        if (dataGridView.Rows[i].Cells[j].Value != null)
                        {
                            distanceMatrix[i, j] = double.Parse(dataGridView.Rows[i].Cells[j].Value.ToString());
                        }
                        else return null;

                    }
                }
                return distanceMatrix;
            }
            catch (Exception ex)
            {
                DBSave dBSave = new DBSave();
                dBSave.SaveException(ex);
                return null;
            }
        }


        //Функция нахождения минимального элемента, исключая текущий элемент
        static double ExceptMin(double[] lst, int myIndex)
        {
            double minVal = double.MaxValue; // задаем начальное значение минимального элемента как максимальное значение типа int
            for (int i = 0; i < lst.Length; i++)
            {
                if (i != myIndex && lst[i] < minVal) // если текущий индекс не равен исходному и текущий элемент меньше текущего минимального, обновляем минимальное значение
                {
                    minVal = lst[i];
                }
            }
            return minVal; // возвращаем минимальное значение
        }



        static void Solve(double[,] matrix, int n)
        {
            MatrixTools matrixTools = new MatrixTools();
            while (true)
            {
                // Редуцируем
                // --------------------------------------
                // Вычитаем минимальный элемент в строках
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    double temp = matrixTools.GetRow(matrix,i).Min();
                    H += temp;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        matrix[i,j] -= temp;
                    }
                }


                // Вычитаем минимальный элемент в столбцах    
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    double temp = matrixTools.GetColumn(matrix, i).Min();
                    H += temp;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        matrix[j, i] -= temp;
                    }
                }
                // --------------------------------------

                // Оцениваем нулевые клетки и ищем нулевую клетку с максимальной оценкой
                // --------------------------------------
                double nullMax = 0;
                int index1 = 0;
                int index2 = 0;
                double tmp = 0;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (matrix[i, j] == 0)
                        {
                            tmp = ExceptMin(matrixTools.GetRow(matrix, i), j) + ExceptMin(matrixTools.GetColumn(matrix,j), i);
                            if (tmp >= nullMax)
                            {
                                nullMax = tmp;
                                index1 = i;
                                index2 = j;
                            }
                        }
                    }

                }

                // находим нужный путь и записываем его в res, удаляем все ненужное
                res.Add(Str[index1] + 1);
                res.Add(Stb[index2] + 1);
                int oldIndex1 = Str[index1];
                int oldIndex2 = Stb[index2];
                if (Str.Contains(oldIndex2) && Stb.Contains(oldIndex1))
                {
                    int NewIndex1 = Str.IndexOf(oldIndex2);
                    int NewIndex2 = Stb.IndexOf(oldIndex1);
                    matrix[NewIndex1,NewIndex2] = double.PositiveInfinity;
                }
                Str.RemoveAt(index1);
                Stb.RemoveAt(index2);
                matrix = matrixTools.RemoveRowAndColumn(matrix, index1, index2);
                if (matrix.Length == 1)
                {
                    break;
                }         
            }
           
        }

        public void MakeRes()
        {
            //Формируем порядок пути
            for (int i = 0; i < res.Count - 1; i += 2)
            {
                if (res.Count(x => x == res[i]) < 2)
                {
                    result.Add(res[i]);
                    result.Add(res[i + 1]);
                }
            }
            for (int i = 0; i < res.Count - 1; i += 2)
            {
                for (int j = 0; j < res.Count - 1; j += 2)
                {
                    if (result[result.Count - 1] == res[j])
                    {
                        result.Add(res[j]);
                        result.Add(res[j + 1]);
                    }
                }
            }

            //Считаем длину пути
            for (int i = 0; i < result.Count - 1; i += 2)
            {
                if (i == result.Count - 2)
                {
                    PathLenght += StartMatrix[result[i] - 1, result[i + 1] - 1];
                    PathLenght += StartMatrix[result[i + 1] - 1, result[0] - 1];
                }
                else
                {
                    PathLenght += StartMatrix[result[i] - 1, result[i + 1] - 1];
                }
            }
        }


        
    }

}
