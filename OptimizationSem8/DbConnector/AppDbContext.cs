using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.DbModels;

namespace OptimizationSem8.DbConnector
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated(); // Создаём базу данных, если её нет
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app_users.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Начальные данные с ролями
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = Role.Admin // Админ
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = Role.User // Обычный пользователь
                }
            );
        }
    }
}
