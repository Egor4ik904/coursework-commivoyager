using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commivoyager
{
    internal class MatrixTools
    {
        public static MatrixTools matrixTools = new MatrixTools();
        public double[] GetColumn(double[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public double[] GetRow(double[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        //функция удаления нужной строки и столбца
        public double[,] RemoveRowAndColumn(double[,] array, int rowIndex, int columnIndex)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            // Создаем новый массив на одну строку и столбец меньше
            double[,] result = new double[rows - 1, cols - 1];

            // Копируем все элементы до удаляемой строки и столбца
            for (int i = 0; i < rowIndex; i++)
            {
                for (int j = 0; j < columnIndex; j++)
                {
                    result[i, j] = array[i, j];
                }
                for (int j = columnIndex + 1; j < cols; j++)
                {
                    result[i, j - 1] = array[i, j];
                }
            }

            // Копируем все элементы после удаляемой строки и столбца
            for (int i = rowIndex + 1; i < rows; i++)
            {
                for (int j = 0; j < columnIndex; j++)
                {
                    result[i - 1, j] = array[i, j];
                }
                for (int j = columnIndex + 1; j < cols; j++)
                {
                    result[i - 1, j - 1] = array[i, j];
                }
            }

            return result;
        }
    }
}
