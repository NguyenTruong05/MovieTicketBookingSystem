using CinemaClient.Services;
using System;

namespace CinemaClient.UI
{
    public class LoginUI
    {
        private NetworkService _networkService;

        public LoginUI(NetworkService networkService)
        {
            _networkService = networkService;
        }

        public bool Show()
        {
            Console.Clear();
            Console.WriteLine("=== ĐĂNG NHẬP HỆ THỐNG ===");
            Console.Write("Tên đăng nhập: ");
            string username = Console.ReadLine();
            Console.Write("Mật khẩu: ");
            string password = Console.ReadLine();

            string message = $"LOGIN|{username}|{password}";
            string response = _networkService.SendMessage(message);

            if (response == "LOGIN_SUCCESS")
            {
                Console.WriteLine("Đăng nhập thành công!");
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else
            {
                Console.WriteLine("Đăng nhập thất bại! Vui lòng thử lại.");
                System.Threading.Thread.Sleep(1000);
                return false;
            }
        }
    }
}