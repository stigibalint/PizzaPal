using MySql.Data.MySqlClient; 
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
                        felhasznaloId INT AUTO_INCREMENT PRIMARY KEY,
                        username VARCHAR(255) NOT NULL UNIQUE,
                        password VARCHAR(255) NOT NULL,
                        email VARCHAR(255) NOT NULL UNIQUE,
                        adminJogosultsag BOOLEAN NOT NULL
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
                if (LoginUser(username, password))
                {
                    MessageBox.Show("Bejelentkezés sikeres!");

                    // Create an instance of OrderWindow and set the UserName property
                    OrderWindow orderWindow = new OrderWindow();
                    orderWindow.UserName = username;  // Set the username
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

        private bool LoginUser(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Felhasznalok WHERE username = @username AND password = @password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                using (var reader = command.ExecuteReader())
                {
                    return reader.HasRows; 
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
                    command.CommandText = "INSERT INTO Felhasznalok (username, email, password, adminJogosultsag) VALUES (@username, @email, @password, 0)";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery(); 
                    return true;
                }
                catch (MySqlException)
                {
                    return false; 
                }
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
