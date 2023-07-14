using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace rekenmachine
{
    public partial class NummerWindow : Window
    {
        private string connectionString = "Server=localhost;Database=Rekenmachine;Uid=root;Pwd=Test@1234";
        MySqlConnection connection;

        public NummerWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            DisplayLastConversions();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFromUnit.SelectedItem != null && cmbToUnit.SelectedItem != null)
            {
                int inputValue;
                if (int.TryParse(txtInput.Text, out inputValue))
                {
                    var fromUnit = ((ComboBoxItem)cmbFromUnit.SelectedItem).Content.ToString();
                    var toUnit = ((ComboBoxItem)cmbToUnit.SelectedItem).Content.ToString();

                    string outputValue;

                    if (fromUnit == "Decimaal" && toUnit == "Hexadecimaal")
                    {
                        outputValue = ConvertToHexadecimal(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Decimaal" && toUnit == "Octaal")
                    {
                        outputValue = ConvertToOctal(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Decimaal" && toUnit == "Binair")
                    {
                        outputValue = ConvertToBinary(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Hexadecimaal" && toUnit == "Decimaal")
                    {
                        outputValue = ConvertToDecimalFromHexadecimal(txtInput.Text);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Octaal" && toUnit == "Decimaal")
                    {
                        outputValue = ConvertToDecimalFromOctal(txtInput.Text);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Binair" && toUnit == "Decimaal")
                    {
                        outputValue = ConvertToDecimalFromBinary(txtInput.Text);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else
                    {
                        // Ongeldige conversie
                        MessageBox.Show("Ongeldige conversie!");
                        return;
                    }

                    txtOutput.Text = outputValue;
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

        private string ConvertToHexadecimal(int decimalValue)
        {
            return decimalValue.ToString("X");
        }

        private string ConvertToOctal(int decimalValue)
        {
            return Convert.ToString(decimalValue, 8);
        }

        private string ConvertToBinary(int decimalValue)
        {
            return Convert.ToString(decimalValue, 2);
        }

        private string ConvertToDecimalFromHexadecimal(string hexadecimalValue)
        {
            return Convert.ToInt32(hexadecimalValue, 16).ToString();
        }

        private string ConvertToDecimalFromOctal(string octalValue)
        {
            return Convert.ToInt32(octalValue, 8).ToString();
        }

        private string ConvertToDecimalFromBinary(string binaryValue)
        {
            return Convert.ToInt32(binaryValue, 2).ToString();
        }

        private void SaveConversionToDatabase(int inputValue, string outputValue, string fromUnit, string toUnit)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO conversienummer (input_value, output_value, from_unit, to_unit) VALUES (@inputValue, @outputValue, @fromUnit, @toUnit)";
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
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    break;
                case "Valuta":
                    ValutaWindow valutaWindow = new ValutaWindow();
                    valutaWindow.Show();
                    this.Close();
                    break;
                case "Gewicht":
                    WeightWindow weightWindow = new WeightWindow();
                    weightWindow.Show();
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
                string query = "SELECT * FROM conversienummer ORDER BY id DESC LIMIT 10";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<string> conversions = new List<string>();
                while (reader.Read())
                {
                    int inputValue = reader.GetInt32("input_value");
                    string outputValue = reader.GetString("output_value");
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
