using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using TodoWPFApp.DTOs;

namespace TodoWPFApp
{

    public partial class LoginRegisterWindow : Window
    {
        private readonly HttpClient _client;
        public LoginRegisterWindow()
        {
            InitializeComponent();
            _client = new HttpClient() { BaseAddress = new Uri("http://localhost:5221") };
        }

        /* private void Border_MouseDown(object sender, MouseButtonEventArgs e)
         {
             if (e.ChangedButton == MouseButton.Left)
             {
                 this.DragMove();
             }

         }*/

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = LoginUsernameTextBox.Text;
                var password = LoginPasswordBox.Password;

                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };

                var response = await _client.PostAsJsonAsync("api/user/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var userDtoW = await response.Content.ReadFromJsonAsync<UserDtoW>();
                    App.LoggedInUserId = userDtoW.Id;

                    MessageBox.Show($"Login successful. Welcome {userDtoW.Username}!");

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Login failed: " + errorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = RegisterUsernameTextBox.Text;
            var password = RegisterPasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            var registerDto = new RegisterDto
            {
                Username = username,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var response = await _client.PostAsJsonAsync("api/user/register", registerDto);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Registration successful.");
                AuthTabControl.SelectedIndex = 1;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show("Registration failed: " + errorMessage);
            }
        }
    }
}
