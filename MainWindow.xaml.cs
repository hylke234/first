using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySqlConnector;

namespace rekenmachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = "Server=localhost;Database=Rekenmachine ;Uid=root;Pwd=Test@1234";
        double first;
        double second;
        char op;
        MySqlConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            List<string> last10Calculations = GetLast10Calculations();
            history.Text = string.Join(Environment.NewLine, last10Calculations);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Result.Text += button.Content.ToString();
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, out first))
            {
                op = '/';
                Result.Clear();
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, out first))
            {
                op = '-';
                Result.Clear();
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, out first))
            {
                op = '+';
                Result.Clear();
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, out first))
            {
                op = '*';
                Result.Clear();
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void Equalsto_Click(object sender, RoutedEventArgs e)
        {
            double result = 0;
            if (double.TryParse(Result.Text, out second))
            {
                if (op == '+')
                {
                    result = first + second;
                }
                else if (op == '-')
                {
                    result = first - second;
                }
                else if (op == '*')
                {
                    result = first * second;
                }
                else if (op == '/')
                {
                    if (second != 0)
                    {
                        result = first / second;
                    }
                    else
                    {
                        ShowErrorMessage("Delen door nul is niet toegestaan.");
                        return;
                    }
                }
                Result.Text = Math.Round(result, 2).ToString();

                // Opslaan van de som in de database
                SaveCalculation(first, op, second, result);
                List<string> last10Calculations = GetLast10Calculations();
                history.Text = string.Join(Environment.NewLine, last10Calculations);
            }
            else
            {
                ShowErrorMessage();
            }
        }

        private void SaveCalculation(double firstNumber, char op, double secondNumber, double result)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO calculations (first_number, operator, second_number, result) VALUES (@first, @op, @second, @result)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@first", firstNumber);
                command.Parameters.AddWithValue("@op", op);
                command.Parameters.AddWithValue("@second", secondNumber);
                command.Parameters.AddWithValue("@result", result);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Fout bij het opslaan van de som: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void DELETE_Click(object sender, RoutedEventArgs e)
        {
            Result.Clear();
        }

        private void ShowErrorMessage(string message = "Er is een fout opgetreden.")
        {
            MessageBox.Show(message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0];
            string tag = (string)selectedItem.Tag;

            switch (tag)
            {
                case "eu_to_usd":
                    ValutaWindow ValutaWindow = new ValutaWindow();
                    ValutaWindow.Show();
                    this.Close();
                    break;
                case "gbp_to_kg":
                    WeightWindow WeightWindow = new WeightWindow();
                    WeightWindow.Show();
                    this.Close();
                    break;
                case "deg_to_rad":
                    AlgebraWindow AlgebraWindow = new AlgebraWindow();
                    AlgebraWindow.Show();
                    this.Close();
                    break;
                case "Volume":
                    VolumeWindow VolumeWindow = new VolumeWindow();
                    VolumeWindow.Show();
                    this.Close();
                    break;
                case "Nummer":
                    NummerWindow NummerWindow = new NummerWindow();
                    NummerWindow.Show();
                    this.Close();
                    break;
                case "Afstand":
                    DistanceWindow DistanceWindow = new DistanceWindow();
                    DistanceWindow.Show();
                    this.Close();
                    break;
                default:
                    break;
            }
        }
        private List<string> GetLast10Calculations()
        {
            List<string> calculations = new List<string>();

            try
            {
                string query = "SELECT CONCAT(first_number, ' ', operator, ' ', second_number, ' = ', result) AS calculation FROM calculations ORDER BY id DESC LIMIT 10";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string calculation = reader["calculation"].ToString();
                        calculations.Add(calculation);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Fout bij het ophalen van de geschiedenis: " + ex.Message);
            }

            return calculations;
        }

    }
}
