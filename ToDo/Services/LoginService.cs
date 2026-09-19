using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Services
{
    public class LoginService
    {
        public static Profile CurrentProfile { get; set; }
        private static string _testEmail;
        private static string _testCode;
        private static DateTime _codeTime;
        private readonly EmailService _emailService = new EmailService();

        public async Task<string> LoginAsync(string email, string password)
        {
            using (var db = new AppDbContext())
            {
                var user = await db.Profiles.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                    return "Incorrect email or password!";

                CurrentProfile = user;
                return null;
            }
        }

        public async Task<string> RegisterAsync(string login, string email, string password)
        {
            using (var db = new AppDbContext())
            {
                if (await db.Profiles.AnyAsync(u => u.Email == email))
                    return "A user with this email address is already registered.";

                if (await db.Profiles.AnyAsync(u => u.Login == login))
                    return "A user with this login has already been registered.";

                db.Profiles.Add(new Profile
                {
                    Login = login,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });
                await db.SaveChangesAsync();
                return null;
            }
        }

        public async Task<string> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (CurrentProfile == null)
                return "You are not logged in.";

            using (var db = new AppDbContext())
            {
                var user = await db.Profiles.FirstOrDefaultAsync(u => u.IdProfile == CurrentProfile.IdProfile);
                if (user == null)
                    return "User not found.";

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                    return "Current password is incorrect.";

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await db.SaveChangesAsync();

                CurrentProfile = user;
                return null;
            }
        }

        public void Logout()
        {
            CurrentProfile = null;
        }

        public async Task<string> DeleteAccountAsync(string password)
        {
            if (CurrentProfile == null)
                return "You are not logged in.";

            using (var db = new AppDbContext())
            {
                var user = await db.Profiles.FirstOrDefaultAsync(u => u.IdProfile == CurrentProfile.IdProfile);
                if (user == null)
                    return "User not found.";

                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                    return "Incorrect password.";

                var tasks = db.UserTasks.Where(t => t.ProfileId == user.IdProfile);
                db.UserTasks.RemoveRange(tasks);

                db.Profiles.Remove(user);
                await db.SaveChangesAsync();

                CurrentProfile = null;
                return null;
            }
        }

        public async Task<string> SendResetCodeAsync(string email)
        {
            using (var db = new AppDbContext())
            {
                var user = await db.Profiles.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return "No user with such an email address was found.";

                var rnd = new Random();
                _testCode = rnd.Next(100000, 999999).ToString();
                _testEmail = email;
                _codeTime = DateTime.Now.AddMinutes(5);

                try
                {
                    await _emailService.SendCodeAsync(email, _testCode);
                    return null;
                }
                catch (Exception ex)
                {
                    return $"The email could not be sent: {ex.Message}";
                }
            }
        }

        public async Task<string> ResetPasswordAsync(string email, string code, string newPassword)
        {
            if (_testEmail != email)
                return "First, request the code for this email.";

            if (DateTime.Now > _codeTime)
                return "The code has expired. Request a new one.";

            if (_testCode != code)
                return "Invalid code.";

            using (var db = new AppDbContext())
            {
                var user = await db.Profiles.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return "The user was not found.";

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await db.SaveChangesAsync();

                _testEmail = null;
                _testCode = null;
                return null;
            }
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }
    }
}
