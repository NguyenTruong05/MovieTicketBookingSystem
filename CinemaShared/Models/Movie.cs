namespace CinemaShared.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; } // in minutes
        public string Genre { get; set; } = string.Empty;
        public string AgeRating { get; set; } = string.Empty;
    }
}