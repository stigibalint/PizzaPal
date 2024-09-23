using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace PizzaPal
{
    public partial class MainWindow : Window
    {
        private bool isLogin = true;
        private string connectionString = "Server=localhost;Database=PizzaPalDB;Uid=root;Pwd=;";

        public MainWindow()
        {
            InitializeComponent();
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"


                   CREATE TABLE IF NOT EXISTS Felhasznalok (
    felhasznaloId INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    Jogosultsag ENUM('admin', 'user') NOT NULL
);

CREATE TABLE IF NOT EXISTS Pizza (
    pizzaId INT PRIMARY KEY AUTO_INCREMENT,
    pizzaNev VARCHAR(255) NOT NULL,
    egysegAr INT NOT NULL
);

CREATE TABLE IF NOT EXISTS Alapanyag (
    alapanyagId INT PRIMARY KEY AUTO_INCREMENT,
    alapanyagNev VARCHAR(255) NOT NULL,
    alapanyagMennyiseg INT NOT NULL
);

CREATE TABLE IF NOT EXISTS PizzaAlapanyag (
    pizzaId INT,
    alapanyagId INT,
    mennyiseg INT NOT NULL,
    PRIMARY KEY (pizzaId, alapanyagId),
    FOREIGN KEY (pizzaId) REFERENCES Pizza(pizzaId),
    FOREIGN KEY (alapanyagId) REFERENCES Alapanyag(alapanyagId)
);

CREATE TABLE IF NOT EXISTS Rendeles (
    rendelesId INT PRIMARY KEY AUTO_INCREMENT,
    felhasznaloId INT,
    pizzaId INT,
    mennyiseg INT NOT NULL,
    datum DATE NOT NULL,
    cim VARCHAR(255) NOT NULL,
    FOREIGN KEY (felhasznaloId) REFERENCES Felhasznalok(felhasznaloId), -- Corrected reference
    FOREIGN KEY (pizzaId) REFERENCES Pizza(pizzaId));";
                command.ExecuteNonQuery();
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTXT.Text;
            string password = Password.Password; 

            if (isLogin)
            {
                if (LoginUser(username, password))
                {
                    MessageBox.Show("Bejelentkezés sikeres!");
                    OrderWindow orderWindow = new OrderWindow();
                    orderWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hibás felhasználónév vagy jelszó!");
                }
            }
            else
            {
                string email = EmailTextBox.Text;
                if (RegisterUser(username, email, password))
                {
                    MessageBox.Show("Regisztráció sikeres!");
                }
                else
                {
                    MessageBox.Show("A regisztráció sikertelen, próbáld újra!");
                }
            }
        }

        private bool RegisterUser(string username, string email, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    var command = connection.CreateCommand();
                    string hashedPassword = HashPassword(username, password);
                    command.CommandText = "INSERT INTO Felhasznalok (username, email, password, Jogosultsag) VALUES (@username, @email, @password, 'user')";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", hashedPassword);
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Registration error: {ex.Message}");
                    return false;
                }
            }
        }

        private bool LoginUser(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT password FROM Felhasznalok WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHashedPassword = reader.GetString(0);
                        string hashedInputPassword = HashPassword(username, password);
                        return storedHashedPassword == hashedInputPassword;
                    }
                    return false; // Username not found
                }
            }
        }

        private string HashPassword(string username, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine username and password
                byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[usernameBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(usernameBytes, 0, combinedBytes, 0, usernameBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, usernameBytes.Length, passwordBytes.Length);

                // Hash the combined bytes
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLogin)
            {
                HeaderText.Text = "Regisztrálj új fiókot!";
                ActionButton.Content = "Regisztráció";
                ToggleButton.Content = "Vissza a bejelentkezéshez";
                EmailStack.Visibility = Visibility.Visible;
                EmailTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                HeaderText.Text = "Üdvözlünk! Jelentkezz be a fiókodba.";
                ActionButton.Content = "Bejelentkezés";
                ToggleButton.Content = "Regisztráció";
                EmailStack.Visibility = Visibility.Collapsed;
                EmailTextBox.Visibility = Visibility.Collapsed;
            }
            isLogin = !isLogin;
        }
    }
}
