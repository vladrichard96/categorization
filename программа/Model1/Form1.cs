using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Model1
{
    public partial class Form1 : Form
    {
        public static int BestMethod;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
        }
        private string[,] array;
        void ArrayToDataGridView(string[,] array, DataGridView dgv)
        {
        {
         for (int i = 0; i < dgv.Columns.Count; i++)
        {
        dgv.Columns.RemoveAt(i);
        }
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
        dgv.Rows.RemoveAt(i);
        }
        }
            dgv.Controls.Clear();
            for (int i = 0; i < array.GetLength(1); i++)
            {
                dgv.Columns.Add("" + (i + 1), "М" + (i + 1));
                dgv.Columns["" + (i + 1)].Width = 105;
            }
            for (int i = 0; i < array.GetLength(0) - 2; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = string.Format("K" + (i + 1).ToString(), "0");
            }
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    try
                    {
                        dgv.Rows[i].Cells[j].Value = array[i, j];
                    }
                    catch (Exception) { }
        }
        string[,] DataGridViewToArray(DataGridView dgv)
        {
            string[,] refArray = new string[dgv.RowCount-1, dgv.ColumnCount];
            for (int i = 0; i < dgv.RowCount-1; i++)
                for (int j = 0; j < dgv.ColumnCount; j++)
                    refArray[i, j] = dgv.Rows[i].Cells[j].Value.ToString();
            return refArray;
        }
        string[,] GetArray(string path, ListBox lb)
        {
            lb.Items.Clear();
            string[] col = File.ReadAllLines(@path, Encoding.Default);
            string temp = col[0];
            string[] lines = temp.Split(',');
            lb.Items.Add("Методы:");
            string[] tempRow = col[0].Split(',','"');
            for (int j = 0; j < tempRow.Length; j++)
            {
                lb.Items.Add(tempRow[j]);
            }
            lb.Items.Add("Критерии:");
            tempRow = col[1].Split(',', '"');
            for (int j = 0; j < tempRow.Length; j++)
            {
                lb.Items.Add(tempRow[j]);
            }
            string[,] tempArray = new string[col.Length, lines.Length];
            for (int i = 2; i < col.Length; i++)
            {
                string[] row = col[i].Split(',');
                for (int j = 0; j < lines.Length; j++)
                {
                    tempArray[i-2, j] = row[j];
                }
            }
            return tempArray;
        }
        void Analize(string[,] array, DataGridView dgv, Chart ch, ListBox lb)
        {
            double[,] doubleArray = new double[array.GetLength(0)-2, array.GetLength(1)];
            double[] colWeights = new double[array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0)-2; i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    doubleArray[i, j] = Double.Parse(dataGridView1.Rows[i].Cells[j].Value.ToString().Replace('.', ','));                    
                }
            }
            for (int i = 0; i < array.GetLength(0)-2; i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    colWeights[j] += Math.Pow(1 - doubleArray[i, j], 2);
                }
            }
            double min = double.MaxValue;
            int minIndex = 0;
            int check = 0;
            int[] arrayMinIndxs = new int[0];
            for (int i = 0; i < colWeights.Length; i++)
                if (min > colWeights[i])
                {
                    min = colWeights[i];
                    minIndex = i;
                    BestMethod = minIndex;
                }
            for (int i = 0; i < colWeights.Length; i++)
            {
                if (min == colWeights[i])
                {
                    Array.Resize(ref arrayMinIndxs, arrayMinIndxs.Length + 1);
                    arrayMinIndxs[check] = i;
                    check++;
                }
            }
            lb.Items.Add("");
            ch.Series.Clear();
            foreach (var series in ch.Series)
            {
                series.Points.Clear();
            }
            Series s1 = new Series("Лучшие методы");
            ch.Series.Add(s1);
            ch.ChartAreas[0].AxisY.Maximum = 4;
            ch.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            ch.ChartAreas[0].AxisX.Interval = 1;
            for (int i = 0; i < colWeights.Length; i++)
            {
                ch.Series["Лучшие методы"].Points.AddXY(" ", colWeights[i]);
            }
                if (arrayMinIndxs.Length == 1)
            {
                dgv.Columns[minIndex].DefaultCellStyle.BackColor = Color.LightGray;
                ch.Series["Лучшие методы"].Points[minIndex].Color = Color.Gold;
                lb.Items.Add("Наилучшее средство: М" + (minIndex + 1) + " с вектором = " + colWeights[minIndex]);
            }
            else
            {
                lb.Items.Add("Наилучшие средства:");
                for (int i = 0; i < arrayMinIndxs.Length; i++)
                {
                    lb.Items.Add("М" + (arrayMinIndxs[i] + 1) + " с вектором  = " + Math.Round(colWeights[arrayMinIndxs[i]], 1));
                    dgv.Columns[arrayMinIndxs[i]].DefaultCellStyle.BackColor = Color.LightGray;
                    ch.Series["Лучшие методы"].Points[arrayMinIndxs[i]].Color = Color.Gold;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                else MessageBox.Show("Файл не выбран");
            }
            try
            {
                array = GetArray(path, listBox1);
                ArrayToDataGridView(array, dataGridView1);
                button2.Enabled = true;
            }
            catch (Exception) { MessageBox.Show("Ошибка файла"); button2.Enabled = false; 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            Analize(array, dataGridView1, chart1, listBox1);
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }
    }
}
