using System.CodeDom;
using System.Data.SqlClient;
using System.Windows;

namespace HR_Department
{
    /*
            23.01.2026 
            https://github.com/dan1eew/HR-Department
    */
    public partial class MainWindow : Window
    {
        public static string CurrentAdminFullName { get; private set; }
        public MainWindow() => InitializeComponent();
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

            AuthenticateAdmin(LoginBox.Text, PasswordBox.Password);
        }

        /// <summary>Метод аутентификации</summary>
        private void AuthenticateAdmin(string text, string password)
        {
            using var con = new SqlConnection(DatabaseHelper.connectionString);
            using var cmd = new SqlCommand(
                "SELECT full_name FROM admin_table WHERE login=@l AND password=@p", con);

            cmd.Parameters.AddWithValue("@l", LoginBox.Text);
            cmd.Parameters.AddWithValue("@p", PasswordBox.Password);

            con.Open();
            var name = cmd.ExecuteScalar();

            if (name == null)
            {
                StatusMainBox.Text = "Неверный логин или пароль";
                return;
            }

            CurrentAdminFullName = name.ToString();
            new TableWindow().Show();
            Close();
        }

        private void ExitClick(object sender, RoutedEventArgs e) => this.Close();
    }
}
