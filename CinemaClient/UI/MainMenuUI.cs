using CinemaClient.Services;
using System.Threading;
using System;

namespace CinemaClient.UI
{
    public class MainMenuUI
    {
        private NetworkService _networkService;
        private MovieUI _movieUI;
        private BookingUI _bookingUI;

        public MainMenuUI(NetworkService networkService)
        {
            _networkService = networkService;
            _movieUI = new MovieUI(networkService);
            _bookingUI = new BookingUI(networkService);
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MENU CHÍNH ===");
                Console.WriteLine("1. Xem danh sách phim");
                Console.WriteLine("2. Đặt vé");
                Console.WriteLine("3. Thoát");
                Console.Write("Chọn: ");

                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    _movieUI.ShowMoviesOnly(); // Chỉ xem, không chọn
                }
                else if (choice == "2")
                {
                    _bookingUI.ShowBookingProcess(); // Đặt vé với luồng đầy đủ
                }
                else if (choice == "3")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}