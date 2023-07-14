using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace rekenmachine
{
    public partial class ValutaWindow : Window
    {
        private string connectionString = "Server=localhost;Database=Rekenmachine ;Uid=root;Pwd=Test@1234";
        MySqlConnection connection;
        public ValutaWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            DisplayLastConversions();

        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFromUnit.SelectedItem != null && cmbToUnit.SelectedItem != null)
            {
                decimal inputValue;
                if (decimal.TryParse(txtInput.Text, out inputValue))
                {
                    var fromUnit = ((ComboBoxItem)cmbFromUnit.SelectedItem).Content.ToString();
                    var toUnit = ((ComboBoxItem)cmbToUnit.SelectedItem).Content.ToString();

                    decimal outputValue;

                    if (fromUnit == "Dollar" && toUnit == "Euro")
                    {
                        outputValue = ConvertToEuro(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Euro" && toUnit == "Dollar")
                    {
                        outputValue = ConvertToDollar(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else
                    {
                        // Ongeldige conversie
                        MessageBox.Show("Ongeldige conversie!");
                        return;
                    }

                    txtOutput.Text = outputValue.ToString();
                    DisplayLastConversions();
                }
                else
                {
                    MessageBox.Show("Ongeldige invoerwaarde!");
                }
            }
            else
            {
                MessageBox.Show("Selecteer een eenheid!");
            }
        }

        private decimal ConvertToEuro(decimal dollar)
        {
            return dollar * euroconvert;
        }

        private decimal ConvertToDollar(decimal euro)
        {
            return euro * dollarconvert;
        }
        decimal euroconvert = 0.89m;
        decimal dollarconvert = 1.12m;

        private void SaveConversionToDatabase(decimal inputValue, decimal outputValue, string fromUnit, string toUnit)
        {


            try
            {
                connection.Open();
                string query = "INSERT INTO conversievaluta (input_value, output_value, from_unit, to_unit) VALUES (@inputValue, @outputValue, @fromUnit, @toUnit)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@inputValue", inputValue);
                command.Parameters.AddWithValue("@outputValue", outputValue);
                command.Parameters.AddWithValue("@fromUnit", fromUnit);
                command.Parameters.AddWithValue("@toUnit", toUnit);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het opslaan van de conversie in de database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0];
            string tag = (string)selectedItem.Tag;

            switch (tag)
            {
                case "Rekenmachine":
                    MainWindow MainWindow = new MainWindow();
                    MainWindow.Show();
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

        private void DisplayLastConversions()
        {

            try
            {
                connection.Open();
                string query = "SELECT * FROM conversievaluta ORDER BY id  DESC LIMIT 10";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<string> conversions = new List<string>();
                while (reader.Read())
                {
                    decimal inputValue = reader.GetDecimal("input_value");
                    decimal outputValue = reader.GetDecimal("output_value");
                    string fromUnit = reader.GetString("from_unit");
                    string toUnit = reader.GetString("to_unit");

                    string conversion = $"{inputValue} {fromUnit} => {outputValue} {toUnit}";
                    conversions.Add(conversion);
                }

                txtLastConversions.Text = string.Join(Environment.NewLine, conversions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het ophalen van de conversies uit de database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }

    }
}
