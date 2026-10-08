using System;
using System.Data;
using System.Windows;
using WelkinRestaurant.DataAccess;
using WelkinRestaurant.Helpers;
using WelkinRestaurant.Models;
using MySql.Data.MySqlClient;

namespace WelkinRestaurant.Views
{
    public partial class LoginWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            errorBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            errorBorder.Visibility = Visibility.Collapsed;
            txtError.Text = "";
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем предыдущую ошибку
            HideError();

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль!");
                return;
            }

            try
            {
                string query = "SELECT id, login, role, full_name, password_hash, salt FROM users WHERE login = @login";
                var dt = db.ExecuteQuery(query, new MySqlParameter("@login", login));

                if (dt.Rows.Count == 1)
                {
                    string storedHash = dt.Rows[0]["password_hash"].ToString();
                    string salt = dt.Rows[0]["salt"].ToString();

                    if (PasswordHelper.VerifyPassword(password, storedHash, salt))
                    {
                        User user = new User
                        {
                            Id = Convert.ToInt32(dt.Rows[0]["id"]),
                            Login = dt.Rows[0]["login"].ToString(),
                            Role = dt.Rows[0]["role"].ToString(),
                            FullName = dt.Rows[0]["full_name"].ToString()
                        };

                        MainWindow mainWindow = new MainWindow(user);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("Неверный пароль!");
                        txtPassword.Password = ""; // Очищаем поле пароля
                    }
                }
                else
                {
                    ShowError("Пользователь не найден!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к БД: " + ex.Message);
            }
        }

        private void OpenTests_Click(object sender, RoutedEventArgs e)
        {
            var testWindow = new TestRunnerWindow();
            testWindow.Show();
        }
    }
}