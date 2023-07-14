using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace rekenmachine
{
    public partial class WeightWindow : Window
    {
        private string connectionString = "Server=localhost;Database=Rekenmachine ;Uid=root;Pwd=Test@1234";
        MySqlConnection connection;
        public WeightWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            DisplayLastConversions();

        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFromUnit.SelectedItem != null && cmbToUnit.SelectedItem != null)
            {
                double inputValue;
                if (double.TryParse(txtInput.Text, out inputValue))
                {
                    var fromUnit = ((ComboBoxItem)cmbFromUnit.SelectedItem).Content.ToString();
                    var toUnit = ((ComboBoxItem)cmbToUnit.SelectedItem).Content.ToString();

                    double outputValue;

                    if (fromUnit == "Kilogram" && toUnit == "Pound")
                    {
                        outputValue = ConvertToPound(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Pound" && toUnit == "Kilogram")
                    {
                        outputValue = ConvertToKilo(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Kilogram" && toUnit == "Gram")
                    {
                        outputValue = ConvertToGram(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Gram" && toUnit == "Kilogram")
                    {
                        outputValue = ConvertToKiloFromGram(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Pound" && toUnit == "Gram")
                    {
                        outputValue = ConvertToGramFromPound(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Gram" && toUnit == "Pound")
                    {
                        outputValue = ConvertToPoundFromGram(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Kilogram" && toUnit == "Ounce")
                    {
                        outputValue = ConvertToOunceFromKilo(inputValue);
                        SaveConversionToDatabase(inputValue, outputValue, fromUnit, toUnit);
                    }
                    else if (fromUnit == "Ounce" && toUnit == "Kilogram")
                    {
                        outputValue = ConvertToKiloFromOunce(inputValue);
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
        double poundconvert = 0;
        double kilogramconvert = 0;
        private double ConvertToPound(double Kilogram)
        {
            return Kilogram * poundconvert;
        }

        private double ConvertToKilo(double pound)
        {
            return pound * kilogramconvert;
        }
        private double ConvertToGram(double kilogram)
        {
            return kilogram * 1000;
        }

        private double ConvertToKiloFromGram(double gram)
        {
            return gram / 1000;
        }

        private double ConvertToGramFromPound(double pound)
        {
            return pound * 453.592;
        }

        private double ConvertToPoundFromGram(double gram)
        {
            return gram / 453.592;
        }

        private double ConvertToOunceFromKilo(double kilogram)
        {
            return kilogram * 35.274;
        }

        private double ConvertToKiloFromOunce(double ounce)
        {
            return ounce / 35.274;
        }

        private void SaveConversionToDatabase(double inputValue, double outputValue, string fromUnit, string toUnit)
        {


            try
            {
                connection.Open();
                string query = "INSERT INTO conversieweight (input_value, output_value, from_unit, to_unit) VALUES (@inputValue, @outputValue, @fromUnit, @toUnit)";
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
                case "valuta":
                    ValutaWindow ValutaWindow = new ValutaWindow();
                    ValutaWindow.Show();
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
                string query = "SELECT * FROM conversieweight ORDER BY id  DESC LIMIT 10";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<string> conversions = new List<string>();
                while (reader.Read())
                {
                    double inputValue = reader.GetDouble("input_value");
                    double outputValue = reader.GetDouble("output_value");
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
