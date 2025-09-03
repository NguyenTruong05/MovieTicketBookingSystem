namespace CinemaShared.Models
{
    public class Seat
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsBooked { get; set; }
        public string BookedByUsername { get; set; } = string.Empty;
    }

}