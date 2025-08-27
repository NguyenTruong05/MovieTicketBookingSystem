using CinemaClient.Services;
using CinemaShared.Constants;
using System;
using System.Threading;

namespace CinemaClient.UI
{
    public class AuthUI
    {
        private NetworkService _networkService;

        public AuthUI(NetworkService networkService)
        {
            _networkService = networkService;
        }

        public bool ShowAuthMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CHÀO MỪNG ĐẾN RẠP PHIM ===");
                Console.WriteLine("1. Đăng nhập");
                Console.WriteLine("2. Đăng ký");
                Console.WriteLine("3. Quên mật khẩu");
                Console.WriteLine("4. Thoát");
                Console.Write("Chọn: ");

                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    if (ShowLogin()) return true;
                }
                else if (choice == "2")
                {
                    ShowRegister();
                }
                else if (choice == "3")
                {
                    ShowForgotPassword();
                }
                else if (choice == "4")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    Thread.Sleep(1000);
                }
            }
        }

        public bool ShowLogin()
        {
            Console.Clear();
            Console.WriteLine("=== ĐĂNG NHẬP ===");
            Console.Write("Tên đăng nhập: ");
            string username = Console.ReadLine();
            Console.Write("Mật khẩu: ");
            string password = Console.ReadLine();

            string response = _networkService.SendMessage($"{MessageTypes.LoginRequest}|{username}|{password}");
            string[] parts = response.Split('|');

            if (parts.Length >= 2 && parts[1] == "SUCCESS")
            {
                Console.WriteLine("Đăng nhập thành công!");
                _networkService.SetCurrentUser(username);
                Thread.Sleep(1000);
                return true;
            }

            Console.WriteLine("Đăng nhập thất bại!");
            Thread.Sleep(1000);
            return false;
        }

        public void ShowRegister()
        {
            Console.Clear();
            Console.WriteLine("=== ĐĂNG KÝ TÀI KHOẢN ===");

            Console.Write("Tên đăng nhập: ");
            string username = Console.ReadLine();
            Console.Write("Mật khẩu: ");
            string password = Console.ReadLine();
            Console.Write("Số điện thoại: ");
            string phone = Console.ReadLine();

            Console.WriteLine("\n=== CÂU HỎI BẢO MẬT ===");
            Console.WriteLine("Chọn câu hỏi bảo mật:");
            Console.WriteLine("1. Tên thú cưng đầu tiên của bạn?");
            Console.WriteLine("2. Tên trường cấp 1 của bạn?");
            Console.WriteLine("3. Tên người bạn thân nhất thời thơ ấu?");
            Console.Write("Chọn số câu hỏi (1-3): ");

            string questionChoice = Console.ReadLine();
            string question = "";

            if (questionChoice == "1")
                question = "Tên thú cưng đầu tiên của bạn?";
            else if (questionChoice == "2")
                question = "Tên trường cấp 1 của bạn?";
            else if (questionChoice == "3")
                question = "Tên người bạn thân nhất thời thơ ấu?";
            else
                question = "Câu hỏi mặc định";

            Console.Write("Câu trả lời: ");
            string answer = Console.ReadLine();

            string response = _networkService.SendMessage(
                $"{MessageTypes.RegisterRequest}|{username}|{password}|{phone}|{question}|{answer}");

            string[] parts = response.Split('|');
            if (parts.Length >= 2 && parts[1] == "SUCCESS")
            {
                Console.WriteLine("Đăng ký thành công! Vui lòng đăng nhập.");
            }
            else
            {
                Console.WriteLine("Đăng ký thất bại! Tên đăng nhập đã tồn tại.");
            }
            Thread.Sleep(2000);
        }

        public void ShowForgotPassword()
        {
            Console.Clear();
            Console.WriteLine("=== QUÊN MẬT KHẨU ===");

            Console.Write("Tên đăng nhập: ");
            string username = Console.ReadLine();

            string response = _networkService.SendMessage($"{MessageTypes.ForgotPasswordRequest}|{username}|");
            string[] parts = response.Split('|');

            if (parts.Length >= 2 && parts[1] == "SUCCESS")
            {
                string question = parts.Length >= 3 ? parts[2] : "";
                Console.WriteLine($"Câu hỏi bảo mật: {question}");
                Console.Write("Câu trả lời: ");
                string answer = Console.ReadLine();

                Console.Write("Mật khẩu mới: ");
                string newPassword = Console.ReadLine();

                string resetResponse = _networkService.SendMessage(
                    $"{MessageTypes.ResetPasswordRequest}|{username}|{answer}|{newPassword}");

                string[] resetParts = resetResponse.Split('|');
                if (resetParts.Length >= 2 && resetParts[1] == "SUCCESS")
                {
                    Console.WriteLine("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");
                }
                else
                {
                    Console.WriteLine("Câu trả lời không đúng! Không thể đổi mật khẩu.");
                }
            }
            else
            {
                Console.WriteLine("Tên đăng nhập không tồn tại!");
            }
            Thread.Sleep(2000);
        }
    }
}