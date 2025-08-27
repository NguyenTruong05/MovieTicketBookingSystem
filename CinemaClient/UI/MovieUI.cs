using CinemaClient.Services;
using System;
using System.Threading;

namespace CinemaClient.UI
{
    public class MovieUI
    {
        private NetworkService _networkService;

        public MovieUI(NetworkService networkService)
        {
            _networkService = networkService;
        }

        public int ShowMoviesAndSelect()
        {
            int selectedMovieId = -1;

            while (selectedMovieId == -1)
            {
                Console.Clear();
                Console.WriteLine("=== DANH SÁCH PHIM ===");
                Console.WriteLine("0. Quay lại menu chính");
                Console.WriteLine("----------------------");

                string response = _networkService.SendMessage("GET_MOVIES_REQ");
                string[] parts = response.Split('|');

                if (parts[0] == "GET_MOVIES_RES" && parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string[] movieData = parts[i].Split(',');
                        Console.WriteLine($"{movieData[0]}. {movieData[1]} ({movieData[2]}phút) - {movieData[3]} - {movieData[4]}");
                    }

                    Console.WriteLine("\n----------------------");
                    Console.Write("Chọn ID phim (hoặc 0 để quay lại): ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        if (choice == 0)
                        {
                            return -1; // Quay lại menu chính
                        }

                        // Kiểm tra xem ID phim có hợp lệ không
                        bool isValidMovie = false;
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] movieData = parts[i].Split(',');
                            if (int.Parse(movieData[0]) == choice)
                            {
                                isValidMovie = true;
                                selectedMovieId = choice;
                                break;
                            }
                        }

                        if (!isValidMovie)
                        {
                            Console.WriteLine("ID phim không hợp lệ! Vui lòng chọn lại.");
                            Thread.Sleep(1500);
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
                    Console.WriteLine("Không có phim nào.");
                    Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                    Console.ReadKey();
                    return -1;
                }
            }

            return selectedMovieId;
        }

        public void ShowMoviesOnly()
        {
            Console.Clear();
            Console.WriteLine("=== DANH SÁCH PHIM ===");
            Console.WriteLine("----------------------");

            string response = _networkService.SendMessage("GET_MOVIES_REQ");
            string[] parts = response.Split('|');

            if (parts[0] == "GET_MOVIES_RES" && parts.Length > 1)
            {
                for (int i = 1; i < parts.Length; i++)
                {
                    string[] movieData = parts[i].Split(',');
                    Console.WriteLine($"{movieData[0]}. {movieData[1]} ({movieData[2]}phút) - {movieData[3]} - {movieData[4]}");
                }
            }
            else
            {
                Console.WriteLine("Không có phim nào.");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
    }
}