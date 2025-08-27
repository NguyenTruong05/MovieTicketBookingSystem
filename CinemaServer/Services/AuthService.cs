using System.Linq;
using CinemaServer.Data;
using CinemaShared.Models;

namespace CinemaServer.Services
{
    public static class AuthService
    {
        public static bool AuthenticateUser(string username, string password, out User user)
        {
            user = Database.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            return user != null;
        }

        public static string RegisterUser(User newUser)
        {
            if (Database.Users.Any(u => u.Username == newUser.Username))
                return "USERNAME_EXISTS";

            Database.Users.Add(newUser);
            return "SUCCESS";
        }

        public static bool VerifySecurityAnswer(string username, string answer, out User user)
        {
            user = Database.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            return user.SecurityAnswer.ToLower() == answer.ToLower();
        }

        public static bool ResetPassword(string username, string newPassword)
        {
            var user = Database.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            user.PasswordHash = newPassword;
            return true;
        }
    }
}