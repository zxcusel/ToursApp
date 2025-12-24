using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using AviationTickets.Data;
using AviationTickets.Models;

namespace AviationTickets.Windows
{
    public partial class BookingsWindow : Window
    {
        private User currentUser;
        private Booking selectedBooking;

        public BookingsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadBookings();
        }

        private void LoadBookings()
        {
            var bookings = new List<Booking>();

            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                var cmd = new SQLiteCommand(@"
                    SELECT b.Id, b.UserId, b.FlightId, b.BookingDate,
                           b.PassengerName, b.SeatNumber,
                           f.FlightNumber, f.Departure, f.Destination, f.DepartureDate
                    FROM Bookings b
                    JOIN Flights f ON b.FlightId = f.Id
                    WHERE b.UserId = @uid", conn);

                cmd.Parameters.AddWithValue("@uid", currentUser.Id);

                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    bookings.Add(new Booking
                    {
                        Id = r.GetInt32(0),
                        UserId = r.GetInt32(1),
                        FlightId = r.GetInt32(2),
                        BookingDate = r.GetString(3),
                        PassengerName = r.GetString(4),
                        SeatNumber = r.GetString(5),
                        FlightNumber = r.GetString(6),
                        Route = r.GetString(7) + " → " + r.GetString(8),
                        DepartureDate = r.GetString(9)
                    });
                }
            }

            BookingsDataGrid.ItemsSource = bookings;
        }

        private void BookingsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedBooking = BookingsDataGrid.SelectedItem as Booking;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBooking == null)
            {
                MessageBox.Show("Выберите бронирование");
                return;
            }

            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                new SQLiteCommand(
                    "DELETE FROM Bookings WHERE Id=@id", conn)
                {
                    Parameters = { new SQLiteParameter("@id", selectedBooking.Id) }
                }.ExecuteNonQuery();

                new SQLiteCommand(
                    "UPDATE Flights SET AvailableSeats = AvailableSeats + 1 WHERE Id=@fid", conn)
                {
                    Parameters = { new SQLiteParameter("@fid", selectedBooking.FlightId) }
                }.ExecuteNonQuery();
            }

            LoadBookings();
        }
    }
}
