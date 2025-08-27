using System;

namespace CinemaShared.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime StartTime { get; set; }
        public string Room { get; set; } = string.Empty;
        public Seat[,] Seats { get; set; } = new Seat[6, 6];
    }
}