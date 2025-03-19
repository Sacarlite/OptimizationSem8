using Models;
using Models.Interface;
using NLog;

public class FullSearchMethod : MethodModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ITask task;    // Задача оптимизации
    private readonly double step;   // Шаг дискретизации для поиска
    private readonly bool maximize; // Флаг для максимизации или минимизации
    private readonly int precision; // Погрешность округления (количество знаков после запятой)
    private readonly bool iterationMode; // Флаг ограничения на количество итераций
    private readonly int iterationCount; // Максимальное количество итераций

    public FullSearchMethod()
    {
        MethodName = "Метод полного сканирования";
    }

    /// <summary>
    /// Конструктор метода полного перебора.
    /// </summary>
    /// <param name="task">Задача оптимизации, реализующая интерфейс ITask.</param>
    /// <param name="step">Шаг дискретизации для параметров.</param>
    /// <param name="maximize">Флаг для максимизации или минимизации.</param>
    /// <param name="precision">Погрешность округления (количество знаков после запятой).</param>
    /// <param name="iterations">Кортеж, содержащий флаг ограничения итераций и максимальное количество итераций.</param>
    public FullSearchMethod((bool iterationMode, int iterationCount) iterations, ITask task, double step = 0.1, bool maximize = false, int precision = 4)
    {
        this.task = task ?? throw new ArgumentNullException(nameof(task));
        this.step = step;
        this.maximize = maximize;
        this.precision = precision;
        this.iterationMode = iterations.iterationMode;
        this.iterationCount = iterations.iterationCount;

        Logger.Info($"Инициализация FullSearchMethod: step={step}, maximize={maximize}, precision={precision}, iterationMode={iterationMode}, iterationCount={iterationCount}");
        Logger.Debug($"Объект задачи: {task.GetType().Name}");
    }

    /// <summary>
    /// Выполняет оптимизацию методом полного перебора.
    /// </summary>
    /// <returns>Оптимальные параметры в виде именованного кортежа (First, Second) с наилучшим значением целевой функции.</returns>
    public FuncPoint Optimize()
    {
        Logger.Info("Начало оптимизации методом полного перебора");

        var lowerBounds = task.GetLowerBounds();
        var upperBounds = task.GetUpperBounds();
        Logger.Debug($"Границы: LowerBounds=({lowerBounds.FirstLower}, {lowerBounds.SecondLower}), UpperBounds=({upperBounds.FirstUpper}, {upperBounds.SecondUpper})");
        Logger.Debug($"Шаг дискретизации: {step}");

        // Список для хранения всех точек, удовлетворяющих ограничениям
        var validPoints = new List<FuncPoint>();

        int currentIteration = 0;
        bool isIterationLimitReached = false;

        // Перебор всех точек
        for (double first = lowerBounds.FirstLower; first <= upperBounds.FirstUpper && !isIterationLimitReached; first = Math.Round(first + step, precision))
        {
            for (double second = lowerBounds.SecondLower; second <= upperBounds.SecondUpper; second = Math.Round(second + step, precision))
            {
                currentIteration++;
                Logger.Debug($"Итерация {currentIteration}: First={first}, Second={second}");

                // Проверка ограничения на количество итераций
                if (iterationMode && currentIteration >= iterationCount)
                {
                    Logger.Info($"Достигнуто ограничение на количество итераций: {iterationCount}");
                    isIterationLimitReached = true;
                    break;
                }

                var point = new FuncPoint(first, second);
                Logger.Debug($"Создание точки: First={point.First}, Second={point.Second}");

                // Проверка ограничений
                bool firstOrderValid = task.CheckFirstOrderConstraints(point);
                bool secondOrderValid = task.CheckSecondOrderConstraints(point);
                Logger.Debug($"Проверка ограничений: FirstOrder={firstOrderValid}, SecondOrder={secondOrderValid}");

                if (firstOrderValid && secondOrderValid)
                {
                    // Вычисление значения целевой функции
                    double funcValue = task.CalculateObjectiveFunction(point);
                    funcValue = Math.Round(funcValue, precision);
                    Logger.Debug($"Значение функции: FuncNum={funcValue}");

                    // Сохранение точки
                    point.First = Math.Round(first, precision);
                    point.Second = Math.Round(second, precision);
                    point.FuncNum = funcValue;
                    validPoints.Add(point);
                }
                else
                {
                    Logger.Debug("Точка не удовлетворяет ограничениям, пропускаем");
                }
            }
        }

        // Если нет подходящих точек
        if (validPoints.Count == 0)
        {
            Logger.Error("Оптимальное значение не найдено.");
            throw new Exception("Ошибка, оптимальное значение не было найдено");
        }

        // Нахождение оптимальной точки с помощью LINQ
        FuncPoint bestPoint = maximize
            ? validPoints.OrderByDescending(p => p.FuncNum).First()
            : validPoints.OrderBy(p => p.FuncNum).First();

        Logger.Info($"Оптимизация завершена. Лучшее значение: First={bestPoint.First}, Second={bestPoint.Second}, FuncNum={bestPoint.FuncNum}");
        Logger.Debug($"Итоговое количество итераций: {currentIteration}");

        return bestPoint;
    }
}