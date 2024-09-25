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
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
        }
        private void PizzaManagement_Click(object sender, RoutedEventArgs e)
        {
            PizzaManagementSection.Visibility = Visibility.Visible;
            UserManagementSection.Visibility = Visibility.Collapsed;
        }

        private void UserManagement_Click(object sender, RoutedEventArgs e)
        {
            PizzaManagementSection.Visibility = Visibility.Collapsed;
            UserManagementSection.Visibility = Visibility.Visible;
        }

    }
}
