using System.Threading.Tasks;
using ForGit.Data;
using ForGit.Models;
using Microsoft.EntityFrameworkCore;

namespace ForGit.Services
{
    public class AuthResult
    {
        public bool Success { get; init; }
        public string? Error { get; init; }
        public User? User { get; init; }

        public static AuthResult Ok(User user) => new() { Success = true, User = user };
        public static AuthResult Fail(string error) => new() { Success = false, Error = error };
    }

    public static class AuthService
    {
        private const string DefaultRoleName = "Пользователь";

        public static async Task<AuthResult> RegisterAsync(string firstName, string middleName, string lastName,
            string email, string password)
        {
            email = email.Trim().ToLowerInvariant();

            await using var db = new AppDbContext();

            if (await db.Logins.AnyAsync(l => l.Email == email))
                return AuthResult.Fail("Пользователь с таким email уже существует");

            var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoleName);
            if (role == null)
            {
                role = new Roles { Name = DefaultRoleName };
                db.Roles.Add(role);
            }

            var user = new User
            {
                FirstName = firstName.Trim(),
                MiddleName = middleName.Trim(),
                LastName = lastName.Trim(),
                Roles = role
            };

            db.Logins.Add(new Logins
            {
                Email = email,
                Password = PasswordHasher.Hash(password),
                User = user
            });

            await db.SaveChangesAsync();
            return AuthResult.Ok(user);
        }

        public static async Task<AuthResult> LoginAsync(string email, string password)
        {
            email = email.Trim().ToLowerInvariant();

            await using var db = new AppDbContext();

            var login = await db.Logins
                .Include(l => l.User)
                .ThenInclude(u => u.Roles)
                .FirstOrDefaultAsync(l => l.Email == email);

            if (login == null || !PasswordHasher.Verify(password, login.Password))
                return AuthResult.Fail("Неверный email или пароль");

            return AuthResult.Ok(login.User);
        }
    }
}
