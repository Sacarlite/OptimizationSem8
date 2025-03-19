using System.Windows;
using Models;
using OptimizationSem8.DbConnector;
using OptimizationSem8.ViewModels;
using OptimizationSem8.Views;


namespace OptimizationSem8
{
    public class Bootstrapper
    {
        public Window Run()
        {
            // Инициализация NLog с помощью LoggerSetup
            try
            {
                LoggerSetup.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации логирования: {ex.Message}");
                throw;
            }
            try
            {
                DbInitializer.Initialize(new AppDbContext());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
                throw;
            }
            return new LoginWindow(new LoginViewModel());
        }
    }
}
