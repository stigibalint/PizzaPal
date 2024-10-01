using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Media;
using System.Windows.Media.Imaging;
using FontAwesome.WPF;
using MySql.Data.MySqlClient;
using static PizzaPal.AdminPanel;
using System.Windows.Media.Effects;
using System.Windows.Media;



namespace PizzaPal
{
    public partial class OrderWindow : Window
    {
        private string _userName;
        private List<CartItem> _cartItems = new List<CartItem>();

        private int _totalPrice = 0;
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
            addToCartButton.Click += (s, e) => AddToCart(pizza);
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(nameTextBlock);
            stackPanel.Children.Add(priceTextBlock);
            stackPanel.Children.Add(ingredientsTextBlock);
            stackPanel.Children.Add(addToCartButton);

            border.Child = stackPanel;

            return border;
        }

        private void UpdateCart()
        {
            CartItemsPanel.Children.Clear();
            _totalPrice = 0;

            int totalQuantity = 0;

            foreach (var cartItem in _cartItems)
            {
                var cardBorder = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(5),
                    CornerRadius = new CornerRadius(8),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Colors.White)
                };

        
                var cardGrid = new Grid
                {
                    Margin = new Thickness(0),
                    RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }
            },
                    ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(100) }, 
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, 
                new ColumnDefinition { Width = new GridLength(120) } 
            }
                };

                var image = new Image
                {
                    Source = new BitmapImage(new Uri($"Images/{cartItem.Pizza.PizzaNev.ToLower()}.jpg", UriKind.Relative)),
                    Height = 100,
                    Width = 100,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                var detailsStackPanel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0)
                };

         
                var pizzaNameTextBlock = new TextBlock
                {
                    Text = $"{cartItem.Pizza.PizzaNev} x {cartItem.Quantity}",
                    FontSize = 18, 
                    FontWeight = FontWeights.Bold,
                };

           
         
               
                var removeButton = new Button
                {
                    Content = "Eltávolítás",
                    Margin = new Thickness(5),
                    Width = 120,
                    Height = 35, 
                    Background = new SolidColorBrush(Color.FromRgb(226, 27, 34)),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(5),
                    BorderThickness = new Thickness(0),
                    FontSize = 14 
                };
                removeButton.Click += (s, e) => RemoveFromCart(cartItem.Pizza.PizzaId);

               
                Grid.SetColumn(image, 0);
                cardGrid.Children.Add(image);

              
                Grid.SetColumn(detailsStackPanel, 1);
                detailsStackPanel.Children.Add(pizzaNameTextBlock);
              
                cardGrid.Children.Add(detailsStackPanel);

              
                Grid.SetColumn(removeButton, 2);
                cardGrid.Children.Add(removeButton);

                cardBorder.Child = cardGrid;

                CartItemsPanel.Children.Add(cardBorder);

                _totalPrice += cartItem.Pizza.EgysegAr * cartItem.Quantity;
                totalQuantity += cartItem.Quantity;
            }

            TotalPriceText.Text = $"{_totalPrice} Ft";

            var cartItemCountTextBlock = (TextBlock)FindName("cartItemCountTextBlock");
            cartItemCountTextBlock.Text = totalQuantity.ToString();
            cartItemCountTextBlock.Visibility = totalQuantity > 0 ? Visibility.Visible : Visibility.Hidden;
        }
        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count > 0)
            {
                ProcessPayment();
            }
            else
            {
                MessageBox.Show("A kosár üres. Kérjük, adjon hozzá termékeket a rendeléshez.", "Üres kosár", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ProcessPayment()
        {
            string userAddress = GetUserAddress();
            if (string.IsNullOrWhiteSpace(userAddress))
            {
                MessageBox.Show("Kérjük, adja meg a szállítási címet!", "Hiányzó cím", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cartItem in _cartItems)
                        {
                        
                            string insertOrderQuery = @"
                        INSERT INTO Rendeles (felhasznaloId, pizzaId, mennyiseg, datum, cim)
                        VALUES (@userId, @pizzaId, @quantity, @date, @address)";

                            using (var command = new MySqlCommand(insertOrderQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@userId", GetUserIdByUsername(_userName));
                                command.Parameters.AddWithValue("@pizzaId", cartItem.Pizza.PizzaId);
                                command.Parameters.AddWithValue("@quantity", cartItem.Quantity);
                                command.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                command.Parameters.AddWithValue("@address", userAddress);
                                command.ExecuteNonQuery();
                            }


                            string updateIngredientsQuery = @"
                        UPDATE Alapanyag a
                        JOIN PizzaAlapanyag pa ON a.alapanyagId = pa.alapanyagId
                        SET a.alapanyagMennyiseg = a.alapanyagMennyiseg - (pa.mennyiseg * @orderQuantity)
                        WHERE pa.pizzaId = @pizzaId";

                            using (var command = new MySqlCommand(updateIngredientsQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@orderQuantity", cartItem.Quantity);
                                command.Parameters.AddWithValue("@pizzaId", cartItem.Pizza.PizzaId);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Rendelés sikeresen feldolgozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        _cartItems.Clear();
                        UpdateCart();
                        ClearAddressFields();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Hiba történt a rendelés feldolgozása során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private string GetUserAddress()
        {
            string street = StreetAddressTextBox.Text.Trim();
            string city = CityTextBox.Text.Trim();
            string zipCode = ZipCodeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zipCode))
            {
                return string.Empty;
            }

            return $"{street}, {zipCode} {city}";
        }

        private void ClearAddressFields()
        {
            StreetAddressTextBox.Clear();
            CityTextBox.Clear();
            ZipCodeTextBox.Clear();
        }
        private int GetUserIdByUsername(string username)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT felhasznaloId FROM Felhasznalok WHERE username = @username";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }
        private void AddToCart(Pizza pizza)
        {
            var existingItem = _cartItems.FirstOrDefault(ci => ci.Pizza.PizzaId == pizza.PizzaId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _cartItems.Add(new CartItem { Pizza = pizza, Quantity = 1 });
            }
            UpdateCart();
        }

        private void RemoveFromCart(int pizzaId)
        {
            var cartItem = _cartItems.FirstOrDefault(ci => ci.Pizza.PizzaId == pizzaId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _cartItems.Remove(cartItem);
                }
            }
            UpdateCart(); 
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
        private void CartPage(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBlock.Text != "" && UserNameTextBlock.Text != "Bejelentkezés") {
                wpPizzak.Visibility = Visibility.Hidden;
                PizzaList.Visibility = Visibility.Hidden;
                CartSection.Visibility = Visibility.Visible;
          
            }
            else
            {
                MessageBox.Show("Bejelentkezés szükséges!");
            }
   
        }
            private void Close_Click(object sender, RoutedEventArgs e)
            {
                Application.Current.Shutdown();
            }
        private void LogOutUser(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
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
                    wpPizzak.Visibility= Visibility.Hidden;
                    CartSection.Visibility = Visibility.Hidden;
                    ProfilSzerkesztoGrid.Visibility = Visibility.Visible;
                }
            }

        private void BackToPizzaList_Click(object sender, RoutedEventArgs e)
            {
     
                PizzaList.Visibility = Visibility.Visible;
                wpPizzak.Visibility = Visibility.Visible;
                CartSection.Visibility = Visibility.Hidden;
                ProfilSzerkesztoGrid.Visibility = Visibility.Hidden;
            }
          
        }
    public class CartItem
    {
        public Pizza Pizza { get; set; }
        public int Quantity { get; set; }
    }

}
