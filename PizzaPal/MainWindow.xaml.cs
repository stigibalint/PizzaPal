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
        private string bejelentkezettId;


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
                        username VARCHAR(255) NOT NULL UNIQUE,
                        password VARCHAR(255) NOT NULL,
                        email VARCHAR(255) NOT NULL UNIQUE,
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
                        FOREIGN KEY (felhasznaloId) REFERENCES Felhasznalok(felhasznaloId),
                        FOREIGN KEY (pizzaId) REFERENCES Pizza(pizzaId)
                    );";
                command.ExecuteNonQuery();

            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTXT.Text;
            string password = Password.Password;

            if (isLogin)
            {
                string jogosultsag = LoginUser(username, password);

                if (jogosultsag != null)
                {
                    MessageBox.Show("Bejelentkezés sikeres!");

                    if (jogosultsag == "admin")
                    {
                        AdminPanel adminPanel = new AdminPanel();
                        adminPanel.Show();
                    }
                    else
                    {
                        OrderWindow orderWindow = new OrderWindow();
                        orderWindow.Id = bejelentkezettId.ToString();
                        orderWindow.UserName = username;
                        orderWindow.Show();
                    }

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


        private string LoginUser(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT password, Jogosultsag, felhasznaloId  FROM Felhasznalok WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHashedPassword = reader.GetString(0);
                        string jogosultsag = reader.GetString(1);
                        string hashedInputPassword = HashPassword(username, password);

                        if (storedHashedPassword == hashedInputPassword)
                        {
                            bejelentkezettId = reader.GetString(2);
                            return jogosultsag; 
                        }
                    }
                }
            }
            return null; 
        }


        private string HashPassword(string username, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[usernameBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(usernameBytes, 0, combinedBytes, 0, usernameBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, usernameBytes.Length, passwordBytes.Length);

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
