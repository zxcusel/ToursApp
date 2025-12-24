namespace AviationTickets.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public string DepartureDate { get; set; }
        public double Price { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public string BookingDate { get; set; }
        public string PassengerName { get; set; }
        public string SeatNumber { get; set; }
        public string FlightNumber { get; set; }
        public string Route { get; set; }
        public string DepartureDate { get; set; }
    }
}