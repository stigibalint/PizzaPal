﻿using System;
using System.Windows;
using System.Windows.Controls;
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
                ProfileWindow profilWindow = new ProfileWindow();
                profilWindow.Show();
                this.Close();
            }
        }

        private void shorCart(object sender, RoutedEventArgs e)
        {
            Cart popup = new Cart();
            popup.ShowDialog();
        }
    }
}