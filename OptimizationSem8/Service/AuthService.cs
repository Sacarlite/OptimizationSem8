using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DbModels;
using OptimizationSem8.DbConnector;

namespace OptimizationSem8.Service
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService()
        {
            _context = new AppDbContext();
        }

        // Регистрация нового пользователя с указанием роли (по умолчанию User)
        public bool Register(string username, string password, string email, Role role = Role.User)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                return false; // Пользователь с таким именем уже существует
            }

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role // Устанавливаем роль
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        // Вход в систему
        public User Login(string username, string password)
        {
            var users = _context.Users.ToList();
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return null; // Пользователь не найден
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isPasswordValid ? user : null; // Возвращаем пользователя, если пароль верный
        }
    }
}
