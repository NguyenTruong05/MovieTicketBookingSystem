using CinemaClient.Services;
using CinemaClient.UI;
using System;

namespace CinemaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            NetworkService networkService = new NetworkService();

            Console.WriteLine("Đang kết nối đến server...");
            if (networkService.Connect("127.0.0.1", 8888))
            {
                Console.WriteLine("Kết nối thành công!");

                // SỬ DỤNG AuthUI THAY VÌ LoginUI
                AuthUI authUI = new AuthUI(networkService);
                if (authUI.ShowAuthMenu())
                {
                    MainMenuUI mainMenu = new MainMenuUI(networkService);
                    mainMenu.Show();
                }

                networkService.Disconnect();
            }
            else
            {
                Console.WriteLine("Không thể kết nối đến server.");
            }

            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}