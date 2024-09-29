using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Data;

namespace PizzaPal
{
    public partial class AdminPanel : Window
    {
        private string connectionString = "Server=localhost;Database=PizzaPalDB;Uid=root;Pwd=;";

        public AdminPanel()
        {
            InitializeComponent();
            LoadAlapanyagList();
            LoadAlapanyagComboBox();
            LoadPizzaList();
            AlapanyagList.CanUserAddRows = false;
            PizzaDataGrid.CanUserAddRows = false;
        }

        private void PizzaManagement_Click(object sender, RoutedEventArgs e)
        {
            PizzaManagementSection.Visibility = Visibility.Visible;
            InventoryManagementSection.Visibility = Visibility.Collapsed;



        }

        private void UserManagement_Click(object sender, RoutedEventArgs e)
        {
            PizzaManagementSection.Visibility = Visibility.Collapsed;
            InventoryManagementSection.Visibility = Visibility.Collapsed;



        }

        private void InventoryManagement_Click(object sender, RoutedEventArgs e)
        {
            PizzaManagementSection.Visibility = Visibility.Collapsed;
            InventoryManagementSection.Visibility = Visibility.Visible;


        }

        private void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            var alapanyagok = AlapanyagList.ItemsSource as List<Alapanyag>;
            if (alapanyagok == null) return;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var alapanyag in alapanyagok)
                        {
                            var command = connection.CreateCommand();
                            command.Transaction = transaction;
                            command.CommandText = "UPDATE Alapanyag SET alapanyagNev = @nev, alapanyagMennyiseg = @mennyiseg WHERE alapanyagId = @id";
                            command.Parameters.AddWithValue("@nev", alapanyag.AlapanyagNev);
                            command.Parameters.AddWithValue("@mennyiseg", alapanyag.AlapanyagMennyiseg);
                            command.Parameters.AddWithValue("@id", alapanyag.AlapanyagId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Az alapanyagok sikeresen frissítve!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Hiba történt a frissítés során: {ex.Message}");
                    }
                }
            }

            LoadAlapanyagList();
        }

        private void DeleteAlapanyag_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Alapanyag alapanyag)
            {
                var result = MessageBox.Show($"Biztosan törölni szeretné a(z) {alapanyag.AlapanyagNev} alapanyagot? Ez eltávolítja az összes pizzát, ami tartalmazza ezt az alapanyagot.", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                var command = connection.CreateCommand();
                                command.Transaction = transaction;


                                command.CommandText = "DELETE FROM PizzaAlapanyag WHERE alapanyagId = @id";
                                command.Parameters.AddWithValue("@id", alapanyag.AlapanyagId);
                                command.ExecuteNonQuery();


                                command.CommandText = "DELETE FROM Alapanyag WHERE alapanyagId = @id";
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Az alapanyag és a kapcsolódó pizza összetevők sikeresen törölve!");
                                    LoadAlapanyagList();
                                    LoadPizzaList();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Nem sikerült törölni az alapanyagot.");
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                MessageBox.Show($"Hiba történt a törlés során: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        private void LoadAlapanyagList()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Alapanyag";
                var reader = command.ExecuteReader();
                var alapanyagList = new List<Alapanyag>();
                while (reader.Read())
                {
                    alapanyagList.Add(new Alapanyag
                    {
                        AlapanyagId = reader.GetInt32("alapanyagId"),
                        AlapanyagNev = reader.GetString("alapanyagNev"),
                        AlapanyagMennyiseg = reader.GetInt32("alapanyagMennyiseg")
                    });
                }
                AlapanyagList.ItemsSource = alapanyagList;
            }
        }


        private void LoadAlapanyagComboBox()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Alapanyag";
                var reader = command.ExecuteReader();
                var alapanyagList = new List<Alapanyag>();
                while (reader.Read())
                {
                    alapanyagList.Add(new Alapanyag
                    {
                        AlapanyagId = reader.GetInt32("alapanyagId"),
                        AlapanyagNev = reader.GetString("alapanyagNev"),
                        AlapanyagMennyiseg = reader.GetInt32("alapanyagMennyiseg")
                    });
                }
                AlapanyagListBox.ItemsSource = alapanyagList;
                AlapanyagListBox.DisplayMemberPath = "AlapanyagNev";
                AlapanyagListBox.SelectedValuePath = "AlapanyagId";
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SavePizza_Click(object sender, RoutedEventArgs e)
        {
            var pizzaNev = PizzaNevTextBox.Text;
            int egysegAr;
            var selectedAlapanyagok = AlapanyagListBox.SelectedItems.Cast<Alapanyag>().ToList();

            if (string.IsNullOrWhiteSpace(pizzaNev))
            {
                MessageBox.Show("Kérlek, add meg a pizza nevét.");
                return;
            }

            if (!selectedAlapanyagok.Any())
            {
                MessageBox.Show("Kérlek, válassz ki legalább egy alapanyagot.");
                return;
            }

            try
            {
                egysegAr = int.Parse(PizzaArTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Kérlek, adj meg egy érvényes árat (szám).");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    command.CommandText = "SELECT COUNT(*) FROM Pizza WHERE pizzaNev = @pizzaNev";
                    command.Parameters.AddWithValue("@pizzaNev", pizzaNev);
                    var count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Ez a pizza név már létezik, kérlek adj meg egy másik nevet.");
                        return;
                    }

                    command.CommandText = "INSERT INTO Pizza (pizzaNev, egysegAr) VALUES (@pizzaNev, @egysegAr); SELECT LAST_INSERT_ID();";
                    command.Parameters.AddWithValue("@egysegAr", egysegAr);
                    var pizzaId = Convert.ToInt32(command.ExecuteScalar());

                    foreach (var alapanyag in selectedAlapanyagok)
                    {
                        command.CommandText = "INSERT INTO PizzaAlapanyag (pizzaId, alapanyagId, mennyiseg) VALUES (@pizzaId, @alapanyagId, @mennyiseg)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@pizzaId", pizzaId);
                        command.Parameters.AddWithValue("@alapanyagId", alapanyag.AlapanyagId);
                        command.Parameters.AddWithValue("@mennyiseg", 1);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Pizza sikeresen hozzáadva!");
                    LoadPizzaList();

                    PizzaNevTextBox.Clear();
                    PizzaArTextBox.Clear();
                    AlapanyagListBox.SelectedItems.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                }
            }
        }


        private void UploadPizzaImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = Path.GetFileName(openFileDialog.FileName);


                var projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                var destPath = Path.Combine(projectPath, "Images", fileName);


                Directory.CreateDirectory(Path.Combine(projectPath, "Images"));
                File.Copy(openFileDialog.FileName, destPath, true);

                MessageBox.Show("Kép sikeresen feltöltve az Images mappába a projekt gyökérmappájában!");
            }
        }


        private void AddAlapanyag_Click(object sender, RoutedEventArgs e)
        {
            var alapanyagNev = AlapanyagNevTextBox.Text;
            int mennyiseg;

            if (string.IsNullOrWhiteSpace(alapanyagNev))
            {
                MessageBox.Show("Kérlek, add meg az alapanyag nevét.");
                return;
            }

            if (!int.TryParse(AlapanyagMennyisegTextBox.Text, out mennyiseg) || mennyiseg <= 0)
            {
                MessageBox.Show("Kérlek, adj meg egy érvényes, pozitív mennyiséget.");
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Adatbázis kapcsolat megnyitva.");

                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Alapanyag (alapanyagNev, alapanyagMennyiseg) VALUES (@alapanyagNev, @mennyiseg)";
                    command.Parameters.AddWithValue("@alapanyagNev", alapanyagNev);
                    command.Parameters.AddWithValue("@mennyiseg", mennyiseg);

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Érintett sorok száma: {rowsAffected}");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Az alapanyag sikeresen hozzáadva az adatbázishoz!");
                        LoadAlapanyagList(); 
                    }
                    else
                    {
                        MessageBox.Show("Nem sikerült hozzáadni az alapanyagot az adatbázishoz.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL hiba történt: {ex.Message}\nHibakód: {ex.Number}");
                Console.WriteLine($"Részletes MySQL hiba: {ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Általános hiba történt: {ex.Message}");
                Console.WriteLine($"Részletes hiba: {ex}");
            }


            AlapanyagNevTextBox.Clear();
            AlapanyagMennyisegTextBox.Clear();
            LoadAlapanyagComboBox();
        }


        private void LogOut(object sender, RoutedEventArgs e)

        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
            
            private void UpdatePizza_Click(object sender, RoutedEventArgs e)
        {
            if (PizzaDataGrid.SelectedItem is Pizza selectedPizza)
            {
    
                var pizzaNev = selectedPizza.PizzaNev;
                var egysegAr = selectedPizza.EgysegAr;

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Pizza SET pizzaNev = @pizzaNev, egysegAr = @egysegAr WHERE pizzaId = @pizzaId";
                    command.Parameters.AddWithValue("@pizzaNev", pizzaNev);
                    command.Parameters.AddWithValue("@egysegAr", egysegAr);
                    command.Parameters.AddWithValue("@pizzaId", selectedPizza.PizzaId);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Pizza sikeresen frissítve!");
                        LoadPizzaList();
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt a pizza frissítése során.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Kérlek válassz ki egy pizzát a módosításhoz.");
            }
        }


        private void LoadAlapanyagListForPizza(int pizzaId)
        {

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT a.alapanyagId, a.alapanyagNev FROM PizzaAlapanyag pa JOIN Alapanyag a ON pa.alapanyagId = a.alapanyagId WHERE pa.pizzaId = @pizzaId";
                command.Parameters.AddWithValue("@pizzaId", pizzaId);
                var reader = command.ExecuteReader();
                var selectedAlapanyagok = new List<Alapanyag>();
                while (reader.Read())
                {
                    selectedAlapanyagok.Add(new Alapanyag
                    {
                        AlapanyagId = reader.GetInt32("alapanyagId"),
                        AlapanyagNev = reader.GetString("alapanyagNev"),
                    });
                }
                AlapanyagListBox.ItemsSource = selectedAlapanyagok;
            }
        }
        private void PizzaDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PizzaDataGrid.SelectedItem is Pizza selectedPizza)
            {
                PizzaNevTextBox.Text = selectedPizza.PizzaNev;
                PizzaArTextBox.Text = selectedPizza.EgysegAr.ToString();
                LoadAlapanyagListForPizza(selectedPizza.PizzaId); 
            }
        }

        private void LoadPizzaList()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT p.pizzaId, p.pizzaNev, p.egysegAr, 
                   GROUP_CONCAT(a.alapanyagNev SEPARATOR ', ') AS Alapanyagok
            FROM Pizza p
            LEFT JOIN PizzaAlapanyag pa ON p.pizzaId = pa.pizzaId
            LEFT JOIN Alapanyag a ON pa.alapanyagId = a.alapanyagId
            GROUP BY p.pizzaId";

                var reader = command.ExecuteReader();
                var pizzaList = new List<Pizza>();
                while (reader.Read())
                {
                    pizzaList.Add(new Pizza
                    {
                        PizzaId = reader.GetInt32("pizzaId"),
                        PizzaNev = reader.GetString("pizzaNev"),
                        EgysegAr = reader.GetInt32("egysegAr"),
                        Alapanyagok = reader.IsDBNull("Alapanyagok") ? "" : reader.GetString("Alapanyagok")
                    });
                }
                PizzaDataGrid.ItemsSource = pizzaList;
            }
        }

        private void DeletePizza_Click(object sender, RoutedEventArgs e)
        {
            var pizza = ((Button)sender).DataContext as Pizza;
            if (pizza != null)
            {
                var result = MessageBox.Show($"Biztosan törölni szeretnéd ezt a pizzát: {pizza.PizzaNev}?", "Megerősítés", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            var command = connection.CreateCommand();
                            command.Transaction = transaction;

                  
                            command.CommandText = "DELETE FROM PizzaAlapanyag WHERE pizzaId = @pizzaId";
                            command.Parameters.AddWithValue("@pizzaId", pizza.PizzaId);
                            command.ExecuteNonQuery();

                   
                            command.CommandText = "DELETE FROM Pizza WHERE pizzaId = @pizzaId";
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            MessageBox.Show("Pizza sikeresen törölve!");
                            LoadPizzaList();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Hiba történt: {ex.Message}");
                        }
                    }
                }
            }
        }

        public class Pizza
        {
            public int PizzaId { get; set; }
            public string PizzaNev { get; set; }
            public int EgysegAr { get; set; }
            public string Alapanyagok { get; set; }
        }

        public class Felhasznalo
        {
            public int FelhasznaloId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Jogosultsag { get; set; }
        }

        public class Alapanyag
        {
            public int AlapanyagId { get; set; }
            public string AlapanyagNev { get; set; }
            public int AlapanyagMennyiseg { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
