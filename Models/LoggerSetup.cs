using NLog;
using NLog.Config;
using NLog.Targets;

namespace Models
{
    public static class LoggerSetup
    {
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized)
            {
                return; // Предотвращаем повторную инициализацию
            }

            try
            {
                // Создаём папку logs в корневой директории проекта
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logsPath = Path.Combine(baseDirectory, "logs");
                Directory.CreateDirectory(logsPath);

                // Создаём конфигурацию NLog программно
                var config = new LoggingConfiguration();

                // Настройка цели для консоли
                var consoleTarget = new ColoredConsoleTarget("logconsole")
                {
                    Layout = "${longdate} | ${level:uppercase=true} | ${message} ${exception:format=tostring}"
                };
                config.AddTarget(consoleTarget);

                // Настройка цели для файла с уникальным именем для каждого запуска
                var fileTarget = new FileTarget("logfile")
                {
                    FileName = Path.Combine(logsPath, $"optimization-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log"),
                    Layout = "${longdate} | ${level:uppercase=true} | ${message} ${exception:format=tostring}"
                };
                config.AddTarget(fileTarget);

                // Правила логирования в зависимости от режима сборки
#if DEBUG
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget); // Включаем все уровни в Debug
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("NLog инициализирован программно (режим Debug с детальным логированием).");
#else
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget); // Только Info и выше в Release
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("NLog инициализирован программно (режим Release с базовым логированием).");
#endif

                logger.Info($"Логи будут записываться в {logsPath} с уникальным именем файла.");
                LogManager.Configuration = config;
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации NLog: {ex.Message}");
                throw;
            }
        }

        public static void Shutdown()
        {
            if (_isInitialized)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("Завершение работы NLog.");
                LogManager.Shutdown();
                _isInitialized = false;
            }
        }
    }
}