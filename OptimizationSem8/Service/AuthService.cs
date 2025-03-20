using Models.DbModels;
using OptimizationSem8.DbConnector;

namespace OptimizationSem8.Service
{
    public class AuthService
    {
        private AppDbContext _context;

        public AuthService()
        {
        }
        public User Login(string username, string password)
        {
            _context = new AppDbContext();
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
