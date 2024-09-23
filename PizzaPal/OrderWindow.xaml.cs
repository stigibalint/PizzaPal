using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PizzaPal
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public string UserName { get; set; }

        public OrderWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateUserInterface();
        }

        private void UpdateUserInterface()
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                UserNameTextBlock.Text = $"{UserName}!"; 
                                                                   
            }
            else
            {
                UserNameTextBlock.Text = "Bejelentkezés";  
            }
        }
        private void shorCart(object sender, RoutedEventArgs e)
        {
            Cart popup = new Cart();
            popup.ShowDialog();
        }
        private void btnProfil(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserName))
            {

                ProfilWindow profilWindow = new ProfilWindow();
                profilWindow.Show();
            }
            else
            {
            
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
            }

            this.Close();  
        }
    }
}