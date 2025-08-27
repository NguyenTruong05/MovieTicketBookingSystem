namespace CinemaShared.Models
{
    public class BookingCode
    {
        public int ShowtimeId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}