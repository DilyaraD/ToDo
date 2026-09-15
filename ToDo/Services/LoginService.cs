using System;
using System.Data.Entity;
using System.Threading.Tasks;
using BCrypt.Net;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Services
{
    public class LoginService
    {
        public static Profile CurrentProfile { get; set; }

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
    }
}
