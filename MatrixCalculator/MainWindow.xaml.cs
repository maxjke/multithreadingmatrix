using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatrixCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void InitializeMatrix(DataGrid grid, int rows, int columns)
        {
            grid.Columns.Clear();
            for (int i = 0; i < columns; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = $"Col {i + 1}",
                    Binding = new Binding($"[{i}]") 
                };
                grid.Columns.Add(column);
            }

            var data = new List<List<string>>();
            for (int i = 0; i < rows; i++)
            {
                var newRow = new List<string>(new string[columns]);
                data.Add(newRow);
            }
            grid.ItemsSource = data;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Rows.Text, out int rows) && int.TryParse(Columns.Text, out int columns))
            {
                InitializeMatrix(MatrixA, rows, columns);
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for rows and columns.");
            }


        }

        private void CreateB_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsB.Text, out int rows) && int.TryParse(ColumnsB.Text, out int columns))
            {
                InitializeMatrix(MatrixB, rows, columns);
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for rows and columns.");
            }

        }
       


        private void MultiplyRow(int row, List<List<string>> dataA, List<List<string>> dataB, List<List<string>> resultData)
        {
            for (int j = 0; j < dataB[0].Count; j++)
            {
                int sum = 0;
                for (int k = 0; k < dataA[row].Count; k++)
                {
                    if (int.TryParse(dataA[row][k], out int numA) && int.TryParse(dataB[k][j], out int numB))
                    {
                        sum += numA * numB;
                    }
                    else
                    {
                        MessageBox.Show($"One of the elements at Matrix A row {row}, column {k} or Matrix B row {k}, column {j} is not a valid integer.", "Error");
                        return;
                    }
                }
                resultData[row][j] = sum.ToString();
            }
        }


        private void SumButton_Click(object sender, RoutedEventArgs e)
        {

            var dataA = (List<List<string>>)MatrixA.ItemsSource;
            var dataB = (List<List<string>>)MatrixB.ItemsSource;

            if (dataA.Count != dataB.Count || dataA[0].Count != dataB[0].Count)
            {
                MessageBox.Show("Matrices must be of the same dimensions.", "Error");
                return;
            }

            var resultData = new List<List<string>>();
            for (int i = 0; i < dataA.Count; i++)
            {
                var newRow = new List<string>();
                for (int j = 0; j < dataA[i].Count; j++)
                {
                    if (int.TryParse(dataA[i][j], out int numA) && int.TryParse(dataB[i][j], out int numB))
                    {
                        int sum = numA + numB;
                        newRow.Add(sum.ToString());
                    }
                    else
                    {
                        MessageBox.Show($"One of the elements at row {i}, column {j} is not a valid integer.", "Error");
                        return;
                    }
                }
                resultData.Add(newRow);
            }

            InitializeMatrix(ResultMatrix, dataA.Count, dataA[0].Count);
            ResultMatrix.ItemsSource = resultData;
        }

        private void MatrixMultiplier_Click(object sender, RoutedEventArgs e)
        {
            var dataA = (List<List<string>>)MatrixA.ItemsSource;
            var dataB = (List<List<string>>)MatrixB.ItemsSource;

            if (dataA[0].Count != dataB.Count)
            {
                MessageBox.Show("Number of columns in Matrix A must match number of rows in Matrix B for multiplication.", "Error");
                return;
            }

            var resultData = new List<List<string>>(dataA.Count);
            var threads = new List<Thread>(dataA.Count);

            for (int i = 0; i < dataA.Count; i++)
            {
                resultData.Add(new List<string>(new string[dataB[0].Count]));
                int row = i;
                Thread thread = new Thread(() => MultiplyRow(row, dataA, dataB, resultData));
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            InitializeMatrix(ResultMatrix, dataA.Count, dataB[0].Count);
            ResultMatrix.ItemsSource = resultData;
        }
    }
}

