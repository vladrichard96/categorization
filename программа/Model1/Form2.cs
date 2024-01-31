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
    public partial class Form2 : Form
    {
        private string[,] array;
        public Form2()
        {
            InitializeComponent();
            comboBox1.Items.Add("ИАФ (Идентификация и аутентификация)");
            comboBox1.Items.Add("УПД (Управление доступом)");
            comboBox1.Items.Add("АУД (Аудит безопасности)");
            comboBox1.Items.Add("ОЦЛ (Обеспечение целостности)");
            comboBox1.Items.Add("ОДТ (Обеспечение безопасности)");
            button2.Enabled = false;
        }
        string[,] GetArray(string path, ListBox lb)
        {
            lb.Items.Clear();
            string[] col = File.ReadAllLines(@path, Encoding.Default);
            string temp = col[0];
            string[] lines = temp.Split(',');
            lb.Items.Add("Объекты:");
            string[] tempRow = col[0].Split(',', '"');
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
            string[,] tempArray = new string[col.Length - 4, lines.Length];
            for (int i = 2; i < col.Length - 2; i++)
            {
                string[] row = col[i].Split(',');
                for (int j = 0; j < lines.Length; j++)
                {
                    tempArray[i - 2, j] = row[j];
                }
            }
            return tempArray;
        }
        void ArrayToDataGridView(string[,] array, DataGridView dgv, string path)
        {
            string[] col = File.ReadAllLines(@path, Encoding.Default);
            string[] columns = col[col.Length - 2].Split(',');
            string[] rows = col[col.Length - 1].Split(',');
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
                dgv.Columns.Add("" + (i + 1), columns[i]);
                dgv.Columns["" + (i + 1)].Width = 75;
            }
            for (int i = 0; i < array.GetLength(0); i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = string.Format(rows[i], "0");
            }
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    try
                    {
                        dgv.Rows[i].Cells[j].Value = array[i, j];
                    }
                    catch (Exception) { }
        }
        string[] CreateArray(string[,] array, int i, int j, int plus)
        {
            string[] arr = new string[i];
            for (int n = 0; n < arr.Length; n++)
            {
                arr[n] = array[n + plus, j];
            }
            return arr;
        }
        int GetMinimumFromArray(string[] array)
        {
            int min = 4;
            for (int i = 0; i<array.Length; i++)
            {
                if (min > Convert.ToInt32(array[i]) && Convert.ToInt32(array[i]) > 0)
                {
                    min = Convert.ToInt32(array[i]);
                }
            }
            if (min == 4) return 0;
            else return min;
        }
        int GetCategory(int[] array)
        {
            int min = 4;
            for (int i = 0; i < array.Length; i++)
            {
                if (min > array[i] && array[i] > 0)
                {
                    min = array[i];
                }
            }
            if (min == 4) return 0;
            else return min;
        }
        int Categorize(string[,] array, int j)
        {
            string[] array1 = CreateArray(array, 8, j, 0);
            string[] array2 = CreateArray(array, 2, j, 8);
            string[] array3 = CreateArray(array, 8, j, 10);
            string[] array4 = CreateArray(array, 2, j, 18);
            string[] array5 = CreateArray(array, 4, j, 20);
            int[] arrayminimals = new int[] { GetMinimumFromArray(array1), GetMinimumFromArray(array2), GetMinimumFromArray(array3), GetMinimumFromArray(array4), GetMinimumFromArray(array5) };
            return GetCategory(arrayminimals);
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
                try
                {
                    array = GetArray(path, listBox1);
                    ArrayToDataGridView(array, dataGridView1, path);
                    button2.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка файла"); button2.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                dataGridView2.Columns.RemoveAt(i);
            }
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows.RemoveAt(i);
            }
            dataGridView2.Controls.Clear();
            for (int i = 0; i < 4; i++)
            {
                dataGridView2.Columns.Add("" + (i + 1), "О" + (i + 1));
                dataGridView2.Columns["" + (i + 1)].Width = 75;
            }
            dataGridView2.Rows[0].HeaderCell.Value = string.Format("Кат", "0");
            for (int i = 0; i < 1; i++)
            for (int j = 0; j < 4; j++)
            try
            {
            dataGridView2.Rows[i].Cells[j].Value = Categorize(array, j);
            }
            catch (Exception) { }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        listBox2.Items.Clear();
                        if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0") listBox2.Items.Add("Данный объект непрокатегорирован");
                        else { listBox2.Items.Add("Регламентация правил и процедур идентификации и аутентификации;");
                                    listBox2.Items.Add("Идентификация и аутентификация пользователей и инициируемых ими процессов;");
                                    listBox2.Items.Add("Идентификация и аутентификация устройств;");
                                    listBox2.Items.Add("Управление идентификаторами;");
                                    listBox2.Items.Add("Управление средствами аутентификации;");
                                    listBox2.Items.Add("Идентификация и аутентификация внешних пользователей;");
                                    listBox2.Items.Add("Защита аутентификационной информации при передаче."); }
                        break;
                    }
                case 1:
                    {
                        listBox2.Items.Clear();
                        if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0") listBox2.Items.Add("Данный объект непрокатегорирован");
                        else
                        {
                            listBox2.Items.Add("Регламентация правил и процедур управления доступом;");
                            listBox2.Items.Add("Управление учетными записями пользователей;");
                            listBox2.Items.Add("Реализация модели управления доступом;");
                            listBox2.Items.Add("Доверенная загрузка;");
                            listBox2.Items.Add("Разделение полномочий (ролей) пользователей;");
                            listBox2.Items.Add("Назначение минимально необходимых прав и привилегий;");
                            listBox2.Items.Add("Ограничение неуспешных попыток доступа в информационную (автоматизированную) систему;");
                            listBox2.Items.Add("Ограничение числа параллельных сеансов доступа;");
                            listBox2.Items.Add("Блокирование сеанса доступа пользователя при неактивности;");
                            listBox2.Items.Add("Управление действиями пользователей до идентификации и аутентификации;");
                            listBox2.Items.Add("Реализация защищенного удаленного доступа;");
                            listBox2.Items.Add("Контроль доступа из внешних информационных (автоматизированных) систем;");
                            if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "3")
                            {
                                listBox2.Items.Remove("Доверенная загрузка;");
                                listBox2.Items.Remove("Ограничение числа параллельных сеансов доступа;");
                            }
                            else if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "2")
                            {
                                listBox2.Items.Remove("Ограничение числа параллельных сеансов доступа;");
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        listBox2.Items.Clear();
                        if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0") listBox2.Items.Add("Данный объект непрокатегорирован");
                        else
                        {
                            listBox2.Items.Add("Регламентация правил и процедур аудита безопасности;");
                            listBox2.Items.Add("Инвентаризация информационных ресурсов;");
                            listBox2.Items.Add("Анализ уязвимостей и их устранение;");
                            listBox2.Items.Add("Генерирование временных меток и (или) синхронизация системного времени;");
                            listBox2.Items.Add("Регистрация событий безопасности;");
                            listBox2.Items.Add("Контроль и анализ сетевого трафика;");
                            listBox2.Items.Add("Защита информации о событиях безопасности;");
                            listBox2.Items.Add("Мониторинг безопасности;");
                            listBox2.Items.Add("Реагирование на сбои при регистрации событий безопасности;");
                            listBox2.Items.Add("Анализ действий отдельных пользователей;");
                            listBox2.Items.Add("Проведение внутренних аудитов;");
                            if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "1")
                            {
                                listBox2.Items.Remove("Контроль и анализ сетевого трафика;");
                                listBox2.Items.Remove("Анализ действий отдельных пользователей;");
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        listBox2.Items.Clear();
                        if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0") listBox2.Items.Add("Данный объект непрокатегорирован");
                        else
                        {
                            listBox2.Items.Add("Регламентация правил и процедур обеспечения целостности;");
                            listBox2.Items.Add("Контроль целостности программного обеспечения;");
                            listBox2.Items.Add("Ограничения по вводу информации в информационную (автоматизированную) систему;");
                            listBox2.Items.Add("Контроль данных, вводимых в информационную (автоматизированную) систему;");
                            listBox2.Items.Add("Контроль ошибочных действий пользователей по вводу и (или) передаче информации и предупреждение пользователей об ошибочных действиях;");
                            if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "3")
                            {
                                listBox2.Items.Remove("Ограничения по вводу информации в информационную (автоматизированную) систему;");
                                listBox2.Items.Remove("Контроль данных, вводимых в информационную (автоматизированную) систему;");
                                listBox2.Items.Remove("Контроль ошибочных действий пользователей по вводу и (или) передаче информации и предупреждение пользователей об ошибочных действиях;");
                            }
                            else if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "2")
                            {
                                listBox2.Items.Remove("Ограничения по вводу информации в информационную (автоматизированную) систему;");
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        listBox2.Items.Clear();
                        if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0") listBox2.Items.Add("Данный объект непрокатегорирован");
                        else
                        {
                            listBox2.Items.Add("Регламентация правил и процедур обеспечения доступности;");
                            listBox2.Items.Add("Использование отказоустойчивых технических средств;");
                            listBox2.Items.Add("Резервирование средств и систем;");
                            listBox2.Items.Add("Контроль безотказного функционирования средств и систем;");
                            listBox2.Items.Add("Резервное копирование информации;");
                            listBox2.Items.Add("Обеспечение возможности восстановления информации;");
                            listBox2.Items.Add("Обеспечение возможности восстановления программного обеспечения при нештатных ситуациях;");
                            listBox2.Items.Add("Контроль предоставляемых вычислительных ресурсов и каналов связи;");
                            if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "3")
                            {
                                listBox2.Items.Remove("Использование отказоустойчивых технических средств;");
                                listBox2.Items.Remove("Резервирование средств и систем;");
                                listBox2.Items.Remove("Контроль безотказного функционирования средств и систем;");
                            }
                        }
                        break;
                    }
            }
        }
    }
}
