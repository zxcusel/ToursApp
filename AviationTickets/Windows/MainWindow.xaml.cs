using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using AviationTickets.Data;
using AviationTickets.Models;

namespace AviationTickets.Windows
{
    public partial class MainWindow : Window
    {
        private User currentUser;
        private Flight selectedFlight;

        public MainWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadFlights();
        }

        private void LoadFlights(string dep = "", string dest = "")
        {
            var flights = new List<Flight>();

            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                string sql = "SELECT * FROM Flights WHERE AvailableSeats > 0";
                if (dep != "") sql += " AND Departure LIKE @d";
                if (dest != "") sql += " AND Destination LIKE @to";

                var cmd = new SQLiteCommand(sql, conn);

                if (dep != "") cmd.Parameters.AddWithValue("@d", "%" + dep + "%");
                if (dest != "") cmd.Parameters.AddWithValue("@to", "%" + dest + "%");

                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    flights.Add(new Flight
                    {
                        Id = r.GetInt32(0),
                        FlightNumber = r.GetString(1),
                        Departure = r.GetString(2),
                        Destination = r.GetString(3),
                        DepartureDate = r.GetString(4),
                        Price = r.GetDouble(5),
                        AvailableSeats = r.GetInt32(6)
                    });
                }
            }

            FlightsDataGrid.ItemsSource = flights;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFlights(DepartureTextBox.Text, DestinationTextBox.Text);
        }

        private void FlightsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedFlight = FlightsDataGrid.SelectedItem as Flight;
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFlight == null) return;

            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                new SQLiteCommand(
                    "INSERT INTO Bookings (UserId, FlightId, BookingDate, PassengerName, SeatNumber) VALUES (@u,@f,@d,@p,@s)",
                    conn)
                {
                    Parameters =
                    {
                        new SQLiteParameter("@u", currentUser.Id),
                        new SQLiteParameter("@f", selectedFlight.Id),
                        new SQLiteParameter("@d", DateTime.Now.ToString()),
                        new SQLiteParameter("@p", PassengerNameTextBox.Text),
                        new SQLiteParameter("@s", SeatNumberTextBox.Text)
                    }
                }.ExecuteNonQuery();

                new SQLiteCommand(
                    "UPDATE Flights SET AvailableSeats = AvailableSeats - 1 WHERE Id=@id",
                    conn)
                {
                    Parameters = { new SQLiteParameter("@id", selectedFlight.Id) }
                }.ExecuteNonQuery();
            }

            LoadFlights();
        }

        private void MyBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            new BookingsWindow(currentUser).Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
