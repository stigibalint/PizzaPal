using System.Windows;

namespace PizzaPal
{
    public partial class MainWindow : Window
    {
        private bool isLogin = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLogin)
            {
                MessageBox.Show("Bejelentkezés sikeres!");

        
                OrderWindow orderWindow = new OrderWindow();
                orderWindow.Show();

       
                this.Close();
            }
            else
            {
                MessageBox.Show("Regisztráció sikeres!");
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
