using System;
using System.Data.SqlClient;
using System.Windows;

namespace HR_Department
{
    public partial class MainWindow : Window
    {
        public static string CurrentUserFullName { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            StatusMainBox.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                StatusMainBox.Text = "Введите логин";
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                StatusMainBox.Text = "Введите пароль";
                return;
            }

            if (PasswordBox.Password != PasswordBox2.Password)
            {
                StatusMainBox.Text = "Пароли не совпадают";
                return;
            }

            AuthenticateUser(LoginBox.Text, PasswordBox.Password);
        }

        private void AuthenticateUser(string login, string password)
        {
            try
            {
                StatusMainBox.Text = string.Empty;
                string query = "SELECT * FROM admin_table WHERE login = @login AND password = @password";

                using (SqlConnection connection = new SqlConnection(DatabaseHelper.connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        CurrentUserFullName = reader["full_name"].ToString();
                        reader.Close();

                        var tableWindow = new TableWindow();
                        tableWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        StatusMainBox.Text = "Неверный логин или пароль";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMainBox.FontSize = 13;
                StatusMainBox.Text = $"{ex.Message}";
            }
        }

    }
}

