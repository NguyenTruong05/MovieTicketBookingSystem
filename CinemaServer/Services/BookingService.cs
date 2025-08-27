using System;
using System.Linq;
using System.Collections.Generic;
using CinemaShared.Models;
using CinemaServer.Data;

namespace CinemaServer.Services
{
    public static class BookingService
    {
        private static Random _random = new Random();

        public static string GenerateUniqueCode(int showtimeId)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code;
            HashSet<string> existingCodes;

            do
            {
                code = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());

                existingCodes = new HashSet<string>(
                    Database.BookingCodes
                        .Where(b => b.ShowtimeId == showtimeId)
                        .Select(b => b.Code)
                );
            }
            while (existingCodes.Contains(code));

            return code;
        }

        public static bool AddBookingCode(int showtimeId, int row, int col, string username, out string code)
        {
            code = GenerateUniqueCode(showtimeId);

            var bookingCode = new BookingCode
            {
                ShowtimeId = showtimeId,
                Row = row,
                Column = col,
                Code = code,
                Username = username
            };

            Database.BookingCodes.Add(bookingCode);
            return true;
        }

        public static string GetBookingCode(int showtimeId, int row, int col)
        {
            var code = Database.BookingCodes.FirstOrDefault(
                b => b.ShowtimeId == showtimeId && b.Row == row && b.Column == col);

            return code?.Code ?? "NOT_FOUND";
        }
    }
}