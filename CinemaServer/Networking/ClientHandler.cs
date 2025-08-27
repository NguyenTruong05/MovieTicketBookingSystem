using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using CinemaShared.Models;
using CinemaShared.Constants;
using CinemaServer.Services;
using CinemaServer.Data;
using CinemaShared.Enums;

namespace CinemaServer.Networking
{
    public class ClientHandler
    {
        private TcpClient _tcpClient;
        private string _currentUsername;

        public ClientHandler(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void HandleClient()
        {
            try
            {
                NetworkStream stream = _tcpClient.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");

                    string response = ProcessMessage(message);
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client handling error: {ex.Message}");
            }
            finally
            {
                _tcpClient?.Close();
                Console.WriteLine($"Client disconnected: {_currentUsername}");
            }
        }

        private string ProcessMessage(string message)
        {
            try
            {
                string[] parts = message.Split('|');
                string action = parts[0];

                if (action == MessageTypes.LoginRequest)
                {
                    return HandleLogin(parts[1], parts[2]);
                }
                else if (action == MessageTypes.RegisterRequest)
                {
                    return HandleRegister(parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                else if (action == MessageTypes.GetMoviesRequest)
                {
                    return HandleGetMovies();
                }
                else if (action == MessageTypes.GetShowtimesRequest)
                {
                    int movieId = int.Parse(parts[1]);
                    return HandleGetShowtimes(movieId);
                }
                else if (action == MessageTypes.GetSeatMapRequest)
                {
                    int showtimeId = int.Parse(parts[1]);
                    return HandleGetSeatMap(showtimeId);
                }
                else if (action == MessageTypes.BookSeatRequest)
                {
                    int stId = int.Parse(parts[1]);
                    int row = int.Parse(parts[2]);
                    int col = int.Parse(parts[3]);
                    return HandleBookSeat(stId, row, col);
                }
                else if (action == MessageTypes.ForgotPasswordRequest)
                {
                    return HandleForgotPassword(parts[1], parts[2]);
                }
                else if (action == MessageTypes.ResetPasswordRequest)
                {
                    return HandleResetPassword(parts[1], parts[2], parts[3]);
                }
                else if (action == MessageTypes.GetBookingCodeRequest)
                {
                    int showtimeId = int.Parse(parts[1]);
                    int row = int.Parse(parts[2]);
                    int col = int.Parse(parts[3]);
                    return HandleGetBookingCode(showtimeId, row, col);
                }
                else
                {
                    return "ERROR|Invalid action";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        private string HandleLogin(string username, string password)
        {
            User user;
            bool success = AuthService.AuthenticateUser(username, password, out user);

            if (success)
            {
                _currentUsername = username;
                string role = user.Role == UserRole.Admin ? "ADMIN" : "USER";
                return $"{MessageTypes.LoginResponse}|SUCCESS|{role}";
            }
            return $"{MessageTypes.LoginResponse}|FAILED";
        }

        private string HandleRegister(string username, string password, string phone, string question, string answer)
        {
            var newUser = new User
            {
                Username = username,
                PasswordHash = password,
                PhoneNumber = phone,
                SecurityQuestion = question,
                SecurityAnswer = answer,
                Role = UserRole.Customer
            };

            string result = AuthService.RegisterUser(newUser);
            return $"{MessageTypes.RegisterResponse}|{result}";
        }

        private string HandleGetMovies()
        {
            var movies = Database.Movies;
            string response = $"{MessageTypes.GetMoviesResponse}";
            foreach (var movie in movies)
            {
                response += $"|{movie.Id},{movie.Title},{movie.Duration},{movie.Genre},{movie.AgeRating}";
            }
            return response;
        }

        private string HandleGetShowtimes(int movieId)
        {
            var showtimes = MovieService.GetShowtimesByMovie(movieId);
            string response = $"{MessageTypes.GetShowtimesResponse}";

            foreach (var st in showtimes)
            {
                string status = st.StartTime > DateTime.Now ? "SẮP CHIẾU" : "ĐÃ CHIẾU";
                response += $"|{st.Id},{st.StartTime:yyyy-MM-dd HH:mm},{st.Room},{status}";
            }

            return response;
        }

        private string HandleGetSeatMap(int showtimeId)
        {
            var showtime = Database.Showtimes.FirstOrDefault(s => s.Id == showtimeId);
            if (showtime == null) return $"{MessageTypes.GetSeatMapResponse}|ERROR|Showtime not found";

            string response = $"{MessageTypes.GetSeatMapResponse}";
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var seat = showtime.Seats[i, j];
                    response += $"|{i},{j},{(seat.IsBooked ? "1" : "0")}";
                }
            }
            return response;
        }

        private string HandleBookSeat(int showtimeId, int row, int col)
        {
            // KIỂM TRA SUẤT CHIẾU CÒN HỢP LỆ KHÔNG
            if (!MovieService.IsShowtimeValid(showtimeId))
            {
                return $"{MessageTypes.BookSeatResponse}|ERROR|Suất chiếu đã bắt đầu hoặc kết thúc";
            }

            var showtime = Database.Showtimes.FirstOrDefault(s => s.Id == showtimeId);
            if (showtime == null) return $"{MessageTypes.BookSeatResponse}|ERROR|Showtime not found";

            if (row < 0 || row >= 6 || col < 0 || col >= 6)
                return $"{MessageTypes.BookSeatResponse}|ERROR|Invalid seat";

            if (showtime.Seats[row, col].IsBooked)
                return $"{MessageTypes.BookSeatResponse}|ERROR|Seat already booked";

            // Đặt ghế và tạo mã code
            showtime.Seats[row, col].IsBooked = true;
            showtime.Seats[row, col].BookedByUsername = _currentUsername;

            string bookingCode;
            BookingService.AddBookingCode(showtimeId, row, col, _currentUsername, out bookingCode);

            BroadcastSeatUpdate(showtimeId, row, col, true);

            return $"{MessageTypes.BookSeatResponse}|SUCCESS|{row},{col}|{bookingCode}";
        }

        private string HandleForgotPassword(string username, string answer)
        {
            User user;
            bool success = AuthService.VerifySecurityAnswer(username, answer, out user);

            if (success)
            {
                return $"{MessageTypes.ForgotPasswordResponse}|SUCCESS|{user.SecurityQuestion}";
            }
            return $"{MessageTypes.ForgotPasswordResponse}|FAILED";
        }

        private string HandleResetPassword(string username, string answer, string newPassword)
        {
            User user;
            bool verifySuccess = AuthService.VerifySecurityAnswer(username, answer, out user);

            if (verifySuccess)
            {
                bool resetResult = AuthService.ResetPassword(username, newPassword);
                return $"{MessageTypes.ResetPasswordResponse}|{(resetResult ? "SUCCESS" : "FAILED")}";
            }
            return $"{MessageTypes.ResetPasswordResponse}|FAILED";
        }

        private string HandleGetBookingCode(int showtimeId, int row, int col)
        {
            string code = BookingService.GetBookingCode(showtimeId, row, col);
            return $"{MessageTypes.GetBookingCodeResponse}|{code}";
        }

        private void BroadcastSeatUpdate(int showtimeId, int row, int col, bool isBooked)
        {
            string updateMessage = $"{MessageTypes.BookSeatResponse}|UPDATE|{showtimeId}|{row}|{col}|{(isBooked ? "1" : "0")}";
            Console.WriteLine($"Broadcasting: {updateMessage}");
        }
    }
}