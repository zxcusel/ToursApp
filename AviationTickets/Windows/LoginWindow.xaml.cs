using System.Data.SQLite;
using System.Windows;
using AviationTickets.Data;
using AviationTickets.Models;

namespace AviationTickets.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                var cmd = new SQLiteCommand(
                    "SELECT * FROM Users WHERE Username=@u AND Password=@p", conn);

                cmd.Parameters.AddWithValue("@u", UsernameTextBox.Text);
                cmd.Parameters.AddWithValue("@p", PasswordBox.Password);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        FullName = reader.GetString(3),
                        Email = reader.GetString(4)
                    };

                    new MainWindow(user).Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
        }
    }
}
