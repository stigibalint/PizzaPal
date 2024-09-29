using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FontAwesome.WPF;

namespace PizzaPal
{
    public partial class OrderWindow : Window
    {
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                UpdateUserInterface();
            }
        }

        public OrderWindow()
        {
            InitializeComponent();
        }

        private void UpdateUserInterface()
        {
            if (!string.IsNullOrEmpty(_userName))
            {
                UserNameTextBlock.Text = _userName;
                var parentStackPanel = (StackPanel)UserNameTextBlock.Parent;
                var iconElement = (ImageAwesome)parentStackPanel.Children[0];
            
            }
            else
            {
                UserNameTextBlock.Text = "Bejelentkezés";
                var parentStackPanel = (StackPanel)UserNameTextBlock.Parent;
                var iconElement = (ImageAwesome)parentStackPanel.Children[0];
               
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

        private void btnProfil(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userName))
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                PizzaList.Visibility = Visibility.Hidden;
                ProfilSzerkesztoGrid.Visibility = Visibility.Visible;
            }
        }
        private void BackToPizzaList_Click(object sender, RoutedEventArgs e)
        {
          
            PizzaList.Visibility = Visibility.Visible;
            ProfilSzerkesztoGrid.Visibility = Visibility.Hidden;
        }
        private void shorCart(object sender, RoutedEventArgs e)
        {
            Cart popup = new Cart();
            popup.ShowDialog();
        }
    }
}