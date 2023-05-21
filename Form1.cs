using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Commivoyager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int n = (int)numericUpDown1.Value;
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = n;
            dataGridView1.ColumnCount = n;

            fillFiagonal(n);
            NumberRowsAndColumns(dataGridView1);
        }

        private void fillFiagonal(double n)
        {
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Rows[i].Cells[i].Value = 0;
                dataGridView1.Rows[i].Cells[i].ReadOnly = true;
            }
        }

        private void NumberRowsAndColumns(DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                dataGridView.Columns[j].HeaderText = (j + 1).ToString();
            }
            // Запретить сортировку
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSPSolver solver = new TSPSolver();
            double[,] distnceMatrix = solver.ReadInputData(dataGridView1, (int)numericUpDown1.Value);
            List<int> result = null;
            double pathLength = 0;
            if (solver.Start(dataGridView1, (int)numericUpDown1.Value, out result, out pathLength))
            {
                label2.Text = "Оптимальный маршрут: " + result[result.Count - 1] + "->" + result[0] + " ";
                for (int i = 0; i < result.Count; i += 2)
                {
                    int element = result[i];
                    label2.Text += result[i] + "->" + result[i + 1] + " ";
                }
                label3.Text = "Дистанция минимального маршрута - " + pathLength;
                solver = null;
                if ((int)numericUpDown1.Value < 10)
                {
                    TSPVisualizer tspVisualizer = new TSPVisualizer(distnceMatrix, pictureBox1);
                    tspVisualizer.UpdatePath(result);
                }
                DBSave dbSave = new DBSave();
                dbSave.SaveResult(distnceMatrix, (int)numericUpDown1.Value, result, Convert.ToInt32(pathLength));

            }


        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.RowCount = 3;
            fillFiagonal(3);
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
