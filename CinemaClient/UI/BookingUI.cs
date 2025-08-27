using CinemaClient.Services;
using System;
using System.Threading;

namespace CinemaClient.UI
{
    public class BookingUI
    {
        private NetworkService _networkService;
        private MovieUI _movieUI;

        public BookingUI(NetworkService networkService)
        {
            _networkService = networkService;
            _movieUI = new MovieUI(networkService);
        }

        public void ShowBookingProcess()
        {
            int movieId = _movieUI.ShowMoviesAndSelect();
            if (movieId == -1) return;

            int showtimeId = ShowShowtimesAndSelect(movieId);
            if (showtimeId == -1) return;

            ShowSeatMap(showtimeId);
        }

        private int ShowShowtimesAndSelect(int movieId)
        {
            int selectedShowtimeId = -1;

            while (selectedShowtimeId == -1)
            {
                Console.Clear();
                Console.WriteLine("=== SUẤT CHIẾU ===");
                Console.WriteLine("0. Quay lại chọn phim");
                Console.WriteLine("-------------------");

                string movieResponse = _networkService.SendMessage("GET_MOVIES_REQ");
                string[] movieParts = movieResponse.Split('|');
                if (movieParts[0] == "GET_MOVIES_RES" && movieParts.Length > 1)
                {
                    foreach (var part in movieParts)
                    {
                        if (part.Contains($"{movieId},"))
                        {
                            string[] movieData = part.Split(',');
                            Console.WriteLine($"Phim: {movieData[1]}");
                            break;
                        }
                    }
                }

                Console.WriteLine("-------------------");
                Console.WriteLine("(Các suất đã chiếu sẽ không thể đặt)");
                Console.WriteLine("-------------------");

                string response = _networkService.SendMessage($"GET_SHOWTIMES_REQ|{movieId}");
                string[] parts = response.Split('|');

                if (parts[0] == "GET_SHOWTIMES_RES" && parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string[] showtimeData = parts[i].Split(',');
                        string status = showtimeData.Length >= 4 ? showtimeData[3] : "";

                        // ĐỔI MÀU THEO TRẠNG THÁI
                        if (status == "ĐÃ CHIẾU")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{showtimeData[0]}. {showtimeData[1]} - {showtimeData[2]} - {status}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{showtimeData[0]}. {showtimeData[1]} - {showtimeData[2]} - {status}");
                            Console.ResetColor();
                        }
                    }

                    Console.WriteLine("\n-------------------");
                    Console.Write("Chọn ID suất chiếu (chỉ chọn suất 'SẮP CHIẾU', hoặc 0 để quay lại): ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        if (choice == 0) return -1;

                        bool isValidShowtime = false;
                        bool isAvailable = false;

                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] showtimeData = parts[i].Split(',');
                            if (int.Parse(showtimeData[0]) == choice)
                            {
                                isValidShowtime = true;
                                isAvailable = showtimeData.Length >= 4 && showtimeData[3] == "SẮP CHIẾU";
                                break;
                            }
                        }

                        if (!isValidShowtime)
                        {
                            Console.WriteLine("ID suất chiếu không hợp lệ!");
                            Thread.Sleep(1500);
                        }
                        else if (!isAvailable)
                        {
                            Console.WriteLine("Suất chiếu này đã bắt đầu hoặc kết thúc, không thể đặt!");
                            Thread.Sleep(1500);
                        }
                        else
                        {
                            selectedShowtimeId = choice;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vui lòng nhập số!");
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    Console.WriteLine("Không có suất chiếu nào cho phim này.");
                    Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                    Console.ReadKey();
                    return -1;
                }
            }

            return selectedShowtimeId;
        }

        private void ShowSeatMap(int showtimeId)
        {
            Console.Clear();
            Console.WriteLine("=== SƠ ĐỒ GHẾ ===");
            Console.WriteLine("(X: Đã đặt, O: Còn trống)");
            Console.WriteLine("  1 2 3 4 5 6");

            string response = _networkService.SendMessage($"GET_SEATMAP_REQ|{showtimeId}");
            string[] parts = response.Split('|');

            if (parts[0] == "GET_SEATMAP_RES" && parts.Length > 1)
            {
                char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F' };
                int index = 1;

                for (int i = 0; i < 6; i++)
                {
                    Console.Write($"{rows[i]} ");
                    for (int j = 0; j < 6; j++)
                    {
                        string[] seatData = parts[index++].Split(',');
                        string status = seatData[2] == "1" ? "X" : "O";
                        Console.Write($"{status} ");
                    }
                    Console.WriteLine();
                }

                // Chọn ghế
                Console.Write("\nNhập hàng (A-F, hoặc 0 để quay lại): ");
                string rowInput = Console.ReadLine().ToUpper();

                if (rowInput == "0") return;

                if (rowInput.Length == 0 || Array.IndexOf(rows, rowInput[0]) == -1)
                {
                    Console.WriteLine("Hàng không hợp lệ!");
                    Thread.Sleep(1500);
                    return;
                }

                Console.Write("Nhập cột (1-6): ");
                if (!int.TryParse(Console.ReadLine(), out int col) || col < 1 || col > 6)
                {
                    Console.WriteLine("Cột không hợp lệ!");
                    Thread.Sleep(1500);
                    return;
                }

                int rowIndex = Array.IndexOf(rows, rowInput[0]);
                BookSeat(showtimeId, rowIndex, col - 1);
            }
            else
            {
                Console.WriteLine("Không thể tải sơ đồ ghế.");
                Thread.Sleep(1500);
            }
        }

        private void BookSeat(int showtimeId, int row, int col)
        {
            string response = _networkService.SendMessage($"BOOK_SEAT_REQ|{showtimeId}|{row}|{col}");
            string[] parts = response.Split('|');

            if (parts[0] == "BOOK_SEAT_RES" && parts[1] == "SUCCESS")
            {
                string bookingCode = parts.Length >= 4 ? parts[3] : "";

                Console.WriteLine("\n✅ ĐẶT VÉ THÀNH CÔNG!");
                Console.WriteLine("========================");
                Console.WriteLine($"Vị trí: {(char)('A' + row)}{col + 1}");
                Console.WriteLine($"Mã code: {bookingCode}");
                Console.WriteLine("========================");
                Console.WriteLine("Lưu ý: Mang mã code này đến rạp để nhận vé!");
            }
            else
            {
                Console.WriteLine($"\n❌ Lỗi: {parts[2]}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
    }
}