using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DbModels;
using System.Windows;

namespace OptimizationSem8.DbConnector
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Проверяем, есть ли уже пользователи в базе данных
            if (!context.Users.Any())
            {
                // Добавляем начальных пользователей
                context.Users.AddRange(
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Role = Role.Admin
                    },
                    new User
                    {
                        Id = 2,
                        Username = "user",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                        Role = Role.User
                    }
                );

                // Сохраняем изменения в базе данных
                context.SaveChanges();

                // Сообщение для первого запуска
                string message = "Программа запущена впервые. Были созданы следующие пользователи:\n\n" +
                                 "1. Логин: admin, Пароль: admin123, Роль: Администратор\n" +
                                 "2. Логин: user, Пароль: user123, Роль: Обычный пользователь\n\n" +
                                 "Рекомендуется сменить пароли после первого входа.";

                // Использование MessageBox для вывода сообщения
                MessageBox.Show(message, "Первый запуск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
