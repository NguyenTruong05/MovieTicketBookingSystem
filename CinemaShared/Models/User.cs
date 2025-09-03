using CinemaShared.Enums;

namespace CinemaShared.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;

        // THÊM 2 TRƯỜNG MỚI CHO CÂU HỎI BẢO MẬT
        public string SecurityQuestion { get; set; } = string.Empty;
        public string SecurityAnswer { get; set; } = string.Empty;
    }

}