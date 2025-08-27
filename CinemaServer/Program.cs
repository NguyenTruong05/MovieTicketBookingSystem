using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CinemaShared;
using Newtonsoft.Json;

namespace CinemaServer
{
    class Server
    {
        private static TcpListener listener;
        private static List<Movie> movies = new List<Movie>();
        private static List<Showtime> showtimes = new List<Showtime>();

        static void Main(string[] args)
        {
            InitData();

            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server started on port 5000...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                Thread t = new Thread(() => HandleClient(client));
                t.Start();
            }
        }

        static void InitData()
        {
            movies.Add(new Movie { Id = 1, Title = "Inception" });
            movies.Add(new Movie { Id = 2, Title = "Interstellar" });

            showtimes.Add(new Showtime
            {
                Id = 1,
                MovieId = 1,
                Time = DateTime.Now.AddHours(2),
                Seats = new List<Seat>
                {
                    new Seat{ Id = 1, IsBooked = false },
                    new Seat{ Id = 2, IsBooked = false }
                }
            });

            showtimes.Add(new Showtime
            {
                Id = 2,
                MovieId = 2,
                Time = DateTime.Now.AddHours(3),
                Seats = new List<Seat>
                {
                    new Seat{ Id = 1, IsBooked = false },
                    new Seat{ Id = 2, IsBooked = false }
                }
            });
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int byteCount;

            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string requestJson = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Request request = JsonConvert.DeserializeObject<Request>(requestJson);

                Response response = ProcessRequest(request);

                string responseJson = JsonConvert.SerializeObject(response);
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
        }

        static Response ProcessRequest(Request req)
        {
            switch (req.Action)
            {
                case "GetMovies":
                    return new Response
                    {
                        Success = true,
                        Data = JsonConvert.SerializeObject(movies)
                    };

                case "GetShowtimes":
                    int movieId = int.Parse(req.Data);
                    var list = showtimes.FindAll(s => s.MovieId == movieId);
                    return new Response
                    {
                        Success = true,
                        Data = JsonConvert.SerializeObject(list)
                    };

                case "BookSeat":
                    var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(req.Data);
                    int showtimeId = data["ShowtimeId"];
                    int seatId = data["SeatId"];

                    Showtime showtime = showtimes.Find(s => s.Id == showtimeId);
                    if (showtime != null)
                    {
                        Seat seat = showtime.Seats.Find(s => s.Id == seatId);
                        if (seat != null && !seat.IsBooked)
                        {
                            seat.IsBooked = true;
                            return new Response { Success = true, Message = "Đặt vé thành công!" };
                        }
                        else
                        {
                            return new Response { Success = false, Message = "Ghế đã được đặt." };
                        }
                    }
                    return new Response { Success = false, Message = "Suất chiếu không tồn tại." };

                default:
                    return new Response { Success = false, Message = "Yêu cầu không hợp lệ." };
            }
        }
    }
}
