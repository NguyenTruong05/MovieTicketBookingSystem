using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using CinemaShared;
using Newtonsoft.Json;

namespace CinemaClient
{
    class Client
    {
        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = client.GetStream();

                Console.WriteLine("Kết nối server thành công!");

                // Lấy danh sách phim
                Request req = new Request { Action = "GetMovies" };
                SendRequest(stream, req);
                Response res = ReceiveResponse(stream);

                var movies = JsonConvert.DeserializeObject<List<Movie>>(res.Data);
                Console.WriteLine("Danh sách phim:");
                foreach (var m in movies)
                    Console.WriteLine($"{m.Id}. {m.Title}");

                Console.Write("Chọn ID phim: ");
                int movieId = int.Parse(Console.ReadLine());

                // Lấy suất chiếu
                req = new Request { Action = "GetShowtimes", Data = movieId.ToString() };
                SendRequest(stream, req);
                res = ReceiveResponse(stream);

                var showtimes = JsonConvert.DeserializeObject<List<Showtime>>(res.Data);
                foreach (var s in showtimes)
                {
                    Console.WriteLine($"Suất {s.Id} - {s.Time}");
                    foreach (var seat in s.Seats)
                        Console.WriteLine($"Ghế {seat.Id} - {(seat.IsBooked ? "Đã đặt" : "Trống")}");
                }

                Console.Write("Chọn ID suất chiếu: ");
                int showtimeId = int.Parse(Console.ReadLine());
                Console.Write("Chọn ID ghế: ");
                int seatId = int.Parse(Console.ReadLine());

                var bookData = new Dictionary<string, int> { { "ShowtimeId", showtimeId }, { "SeatId", seatId } };
                req = new Request { Action = "BookSeat", Data = JsonConvert.SerializeObject(bookData) };
                SendRequest(stream, req);
                res = ReceiveResponse(stream);

                Console.WriteLine(res.Message);

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        static void SendRequest(NetworkStream stream, Request req)
        {
            string json = JsonConvert.SerializeObject(req);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
        }

        static Response ReceiveResponse(NetworkStream stream)
        {
            byte[] buffer = new byte[4096];
            int byteCount = stream.Read(buffer, 0, buffer.Length);
            string json = Encoding.UTF8.GetString(buffer, 0, byteCount);
            return JsonConvert.DeserializeObject<Response>(json);
        }
    }
}
