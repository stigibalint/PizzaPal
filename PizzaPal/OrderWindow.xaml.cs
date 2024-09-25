using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FontAwesome.WPF;
using MySql.Data.MySqlClient;

namespace PizzaPal
{
    public partial class OrderWindow : Window
    {
        private string _userName;
        public string Id;
        private string connectionString = "Server=localhost;Database=PizzaPalDB;Uid=root;Pwd=;";
        public List<Pizza> _Pizzak;
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
            LoadPizzas();
        }

        private void LoadPizzas()
        {
            _Pizzak = new List<Pizza>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT pizzaId, pizzaNev, egysegAr FROM Pizza";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _Pizzak.Add(new Pizza
                        {
                            PizzaId = reader.GetInt32("pizzaId"),
                            PizzaNev = reader.GetString("pizzaNev"),
                            EgysegAr = reader.GetInt32("egysegAr")
                        });
                    }
                }
            }

            PopulatePizzaCards();
        }

        private void PopulatePizzaCards()
        {
            foreach (var pizza in _Pizzak)
            {
                var pizzaCard = CreatePizzaCard(pizza);
                wpPizzak.Children.Add(pizzaCard);
            }
        }

        private UIElement CreatePizzaCard(Pizza pizza)
        {
            var border = new Border
            {
                Style = (Style)FindResource("PizzaCardStyle"),
                Padding = new Thickness(10)
            };

            var stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

            var image = new Image
            {
                Style = (Style)FindResource("PizzaImageStyle"),
                Source = new BitmapImage(new Uri($"Images/{pizza.PizzaNev.ToLower()}.jpg", UriKind.Relative))
            };

            var nameTextBlock = new TextBlock
            {
                Text = pizza.PizzaNev,
                Style = (Style)FindResource("PizzaNameStyle"),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var priceTextBlock = new TextBlock
            {
                Text = $"{pizza.EgysegAr} Ft",
                Style = (Style)FindResource("PizzaPriceStyle"),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var addToCartButton = new Button
            {
                Content = "Kosárba",
                Style = (Style)FindResource("AddToCartButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(nameTextBlock);
            stackPanel.Children.Add(priceTextBlock);
            stackPanel.Children.Add(addToCartButton);

            border.Child = stackPanel;

            return border;
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