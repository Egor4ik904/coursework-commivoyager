using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commivoyager
{
    internal class DBSave
    {

        public void SaveException(Exception ex)
        {
            using (TspContext db = new TspContext())
            {

                UserException exception1 = new UserException
                {
                    Message = ex.Message,
                    TargetSite = ex.TargetSite.ToString(),
                    DateTimeExc = DateTime.Now
                };
                db.UserExceptions.AddRange(exception1);
                db.SaveChanges();
            }
        }
        public void SaveResult(double[,] distanceMatrix, int n, List<int> path, int dist)
        {
            using (TspContext db = new TspContext())
            {

                Request request = new Request
                {
                    Matrix = MatrixToString(distanceMatrix, n),
                    Size = n,
                    BestPath = string.Join(" ", path),
                    BestDistance = dist
                };
                db.Requests.AddRange(request);
                db.SaveChanges();

            }
        }

        //функция отоброжения матрицы
        static string MatrixToString(double[,] distanceMatrix, int n)
        {
            string matrix = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double curentValue = distanceMatrix[i, j];
                    matrix += curentValue + " ";
                }
            }
            return matrix;
        }
    }
}
