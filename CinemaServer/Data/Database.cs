using CinemaShared.Models;
using CinemaShared.Enums;
using System;
using System.Collections.Generic;

namespace CinemaServer.Data
{
    public static class Database
    {
        public static List<User> Users = new List<User>();
        public static List<Movie> Movies = new List<Movie>();
        public static List<Showtime> Showtimes = new List<Showtime>();
        public static List<BookingCode> BookingCodes = new List<BookingCode>();

        static Database()
        {
            InitializeSampleData();
        }

        private static void InitializeSampleData()
        {
            // Add default admin user
            Users.Add(new User
            {
                Username = "admin",
                PasswordHash = "admin123",
                PhoneNumber = "0123456789",
                Role = UserRole.Admin
            });

            // Add sample user
            Users.Add(new User
            {
                Username = "user1",
                PasswordHash = "user123",
                PhoneNumber = "0987654321",
                Role = UserRole.Customer
            });

            // THÊM 5 PHIM
            Movies.Add(new Movie { Id = 1, Title = "Avengers: Endgame", Duration = 180, Genre = "Action", AgeRating = "PG-13" });
            Movies.Add(new Movie { Id = 2, Title = "The Batman", Duration = 176, Genre = "Action", AgeRating = "PG-13" });
            Movies.Add(new Movie { Id = 3, Title = "Spider-Man: No Way Home", Duration = 148, Genre = "Adventure", AgeRating = "PG-13" });
            Movies.Add(new Movie { Id = 4, Title = "Avatar: The Way of Water", Duration = 192, Genre = "Sci-Fi", AgeRating = "PG-13" });
            Movies.Add(new Movie { Id = 5, Title = "Black Panther: Wakanda Forever", Duration = 161, Genre = "Action", AgeRating = "PG-13" });

            // TẠO SUẤT CHIẾU HỢP LÝ - 5 PHÒNG, MỖI PHÒNG 5 SUẤT/NGÀY
            DateTime today = DateTime.Today;
            string[] rooms = { "Phòng 1", "Phòng 2", "Phòng 3", "Phòng 4", "Phòng 5" };

            // KHUNG GIỜ CHIẾU HỢP LÝ
            string[] timeSlots = { "08:00", "11:00", "14:00", "17:00", "20:00" };

            int showtimeId = 1;

            // TẠO LỊCH CHIẾU TRONG 3 NGÀY (HÔM NAY, MAI, NGÀY KIA)
            for (int day = 0; day < 3; day++)
            {
                DateTime date = today.AddDays(day);

                // MỖI PHÒNG CHIẾU 1 PHIM TRONG 1 KHUNG GIỜ
                for (int roomIndex = 0; roomIndex < 5; roomIndex++)
                {
                    for (int timeIndex = 0; timeIndex < 5; timeIndex++)
                    {
                        // PHÒNG (roomIndex + 1) CHIẾU PHIM (roomIndex + 1) Ở TẤT CẢ KHUNG GIỜ
                        int movieId = roomIndex + 1;
                        if (movieId > Movies.Count) movieId = Movies.Count;

                        DateTime showtime = DateTime.Parse($"{date:yyyy-MM-dd} {timeSlots[timeIndex]}");

                        var newShowtime = new Showtime
                        {
                            Id = showtimeId++,
                            MovieId = movieId,
                            StartTime = showtime,
                            Room = rooms[roomIndex],
                            Seats = InitializeSeats(6, 6)
                        };
                        Showtimes.Add(newShowtime);
                    }
                }
            }
        }

        private static Seat[,] InitializeSeats(int rows, int cols)
        {
            var seats = new Seat[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    seats[i, j] = new Seat { Row = i, Column = j, IsBooked = false };
                }
            }
            return seats;
        }
    }
}