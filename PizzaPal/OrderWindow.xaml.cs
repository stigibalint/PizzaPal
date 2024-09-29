using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FontAwesome.WPF;
using MySql.Data.MySqlClient;
using static PizzaPal.AdminPanel;

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
            CreateDatabase();
            LoadPizzas();
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
        private string GetPizzaIngredients(int pizzaId)
        {
            var ingredients = new List<string>();


            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT a.alapanyagNev FROM PizzaAlapanyag pa JOIN Alapanyag a ON pa.alapanyagId = a.alapanyagId WHERE pa.pizzaId = @pizzaId", connection);
                command.Parameters.AddWithValue("@pizzaId", pizzaId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredients.Add(reader["alapanyagNev"].ToString());
                    }
                }
            }

            return string.Join(", ", ingredients); 
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
                // Fetch ingredients and set to Alapanyagok property
                pizza.Alapanyagok = GetPizzaIngredients(pizza.PizzaId);

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

            var stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

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

            
            var ingredientsTextBlock = new TextBlock
            {
                Text = pizza.Alapanyagok,
                Style = (Style)FindResource("PizzaIngredientsStyle"),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
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
            stackPanel.Children.Add(ingredientsTextBlock); 
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