using System.Windows;
using Models;
using NLog;

namespace OptimizationSem8
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Bootstrapper? _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Запуск приложения
            _bootstrapper = new Bootstrapper();
            MainWindow = _bootstrapper.Run();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Завершение работы NLog
            LoggerSetup.Shutdown();
            base.OnExit(e);
        }
    }
}