using System;
using System.Linq;
using System.Collections.Generic;
using CinemaShared.Models;
using CinemaServer.Data;

namespace CinemaServer.Services
{
    public static class MovieService
    {
        public static List<Movie> GetAllMovies()
        {
            return Database.Movies;
        }

        public static Movie GetMovieById(int id)
        {
            return Database.Movies.FirstOrDefault(m => m.Id == id);
        }

        public static List<Showtime> GetShowtimesByMovie(int movieId)
        {
            var currentTime = DateTime.Now;
            return Database.Showtimes
                .Where(s => s.MovieId == movieId && s.StartTime > currentTime)
                .ToList();
        }

        public static Showtime GetShowtimeById(int showtimeId)
        {
            return Database.Showtimes.FirstOrDefault(s => s.Id == showtimeId);
        }

        public static bool IsShowtimeValid(int showtimeId)
        {
            var showtime = GetShowtimeById(showtimeId);
            if (showtime == null) return false;

            return showtime.StartTime > DateTime.Now;
        }
    }
}