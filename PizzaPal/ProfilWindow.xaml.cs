using MySql.Data.MySqlClient;
using System.Windows;

namespace PizzaPal
{
    public partial class ProfileWindow : Window
    {
        private string connectionString = "Server=localhost;Database=PizzaPalDB;Uid=root;Pwd=;";
        private string currentUsername;

        public ProfileWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            LoadProfileDetails();
        }

        private void LoadProfileDetails()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT username, email FROM Felhasznalok WHERE username = @username";
                command.Parameters.AddWithValue("@username", currentUsername);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UsernameTextBox.Text = reader.GetString(0);
                        EmailTextBox.Text = reader.GetString(1);
                    }
                }
            }
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text;
            string newEmail = EmailTextBox.Text;
            string newPassword = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek meg!");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Felhasznalok SET username = @newUsername, email = @newEmail, password = @newPassword WHERE username = @currentUsername";
                command.Parameters.AddWithValue("@newUsername", newUsername);
                command.Parameters.AddWithValue("@newEmail", newEmail);
                command.Parameters.AddWithValue("@newPassword", HashPassword(newUsername, newPassword));
                command.Parameters.AddWithValue("@currentUsername", currentUsername);

                command.ExecuteNonQuery();
                MessageBox.Show("Profil sikeresen frissítve!");
            }
        }

        private string HashPassword(string username, string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] usernameBytes = System.Text.Encoding.UTF8.GetBytes(username);
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[usernameBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(usernameBytes, 0, combinedBytes, 0, usernameBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, usernameBytes.Length, passwordBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return System.Convert.ToBase64String(hashBytes);
            }
        }
    }
}
