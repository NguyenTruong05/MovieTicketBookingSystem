using System;
using System.Collections.Generic;

namespace CinemaShared
{
    // Yêu cầu/Phản hồi gửi qua socket
    [Serializable]
    public class Request
    {
        public string Action { get; set; } // ví dụ: "GetMovies", "GetShowtimes", "BookSeat"
        public string Data { get; set; }   // dữ liệu kèm theo (JSON string)
    }

    [Serializable]
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    // Model phim
    [Serializable]
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    // Suất chiếu
    [Serializable]
    public class Showtime
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime Time { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }

    // Ghế
    [Serializable]
    public class Seat
    {
        public int Id { get; set; }
        public bool IsBooked { get; set; }
    }
}
