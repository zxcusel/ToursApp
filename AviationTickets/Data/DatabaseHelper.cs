using System;
using System.Data.SQLite;
using System.IO;

namespace AviationTickets.Data
{
    public static class DatabaseHelper
    {
        private static readonly string dbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aviation.db");

        public static string ConnectionString =>
            $"Data Source={dbPath};Version=3;";

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                CreateTables(conn);
                InsertSampleData(conn);
            }
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            string users = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT UNIQUE NOT NULL,
    Password TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL
);";

            string flights = @"
CREATE TABLE IF NOT EXISTS Flights (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FlightNumber TEXT NOT NULL,
    Departure TEXT NOT NULL,
    Destination TEXT NOT NULL,
    DepartureDate TEXT NOT NULL,
    Price REAL NOT NULL,
    AvailableSeats INTEGER NOT NULL
);";

            string bookings = @"
CREATE TABLE IF NOT EXISTS Bookings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    FlightId INTEGER NOT NULL,
    BookingDate TEXT NOT NULL,
    PassengerName TEXT NOT NULL,
    SeatNumber TEXT NOT NULL
);";

            new SQLiteCommand(users, conn).ExecuteNonQuery();
            new SQLiteCommand(flights, conn).ExecuteNonQuery();
            new SQLiteCommand(bookings, conn).ExecuteNonQuery();
        }

        private static void InsertSampleData(SQLiteConnection conn)
        {
            string insertFlights = @"
INSERT OR IGNORE INTO Flights
(Id, FlightNumber, Departure, Destination, DepartureDate, Price, AvailableSeats)
VALUES
(1,'SU101','Москва','Санкт-Петербург','2026-01-15 10:30',4500,45),
(2,'SU202','Москва','Екатеринбург','2026-01-16 14:00',7800,32),
(3,'SU303','Санкт-Петербург','Сочи','2026-01-17 08:15',9200,28),
(4,'SU403','Краснодар','Сочи','2026-01-19 18:15',9200,28),
(5,'SU503','Казань','Волгоград','2026-01-20 13:15',9200,28),
(6,'SU603','Псков','Москва','2026-02-01 6:50',9200,28);";

            new SQLiteCommand(insertFlights, conn).ExecuteNonQuery();
        }
    }
}
