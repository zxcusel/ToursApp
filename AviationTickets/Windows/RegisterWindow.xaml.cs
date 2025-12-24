using System.Data.SQLite;
using System.Windows;
using AviationTickets.Data;

namespace AviationTickets.Windows
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                var cmd = new SQLiteCommand(
                    "INSERT INTO Users (Username, Password, FullName, Email) VALUES (@u,@p,@f,@e)",
                    conn);

                cmd.Parameters.AddWithValue("@u", UsernameTextBox.Text);
                cmd.Parameters.AddWithValue("@p", PasswordBox.Password);
                cmd.Parameters.AddWithValue("@f", FullNameTextBox.Text);
                cmd.Parameters.AddWithValue("@e", EmailTextBox.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Регистрация успешна");
            Close();
        }
    }
}
