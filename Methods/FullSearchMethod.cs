using System;
using Models;
using Models.Interface;
using NLog;

namespace Methods
{
    /// <summary>
    /// Класс для выполнения оптимизации методом полного перебора.
    /// </summary>
    public class FullSearchMethod : MethodModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITask task;    // Задача оптимизации
        private readonly double step;   // Шаг дискретизации для поиска
        private readonly bool maximize; // Флаг для максимизации или минимизации
        private readonly int precision; // Погрешность округления (количество знаков после запятой)

        public FullSearchMethod() {
            MethodName = "Метод полного сканирования";
        }
        /// <summary>
        /// Конструктор метода полного перебора.
        /// </summary>
        /// <param name="task">Задача оптимизации, реализующая интерфейс ITask.</param>
        /// <param name="step">Шаг дискретизации для параметров.</param>
        /// <param name="maximize">Флаг для максимизации или минимизации.</param>
        /// <param name="precision">Погрешность округления (количество знаков после запятой).</param>
        public FullSearchMethod(ITask task, double step = 0.1, bool maximize = false, int precision = 4)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.step = step;
            this.maximize = maximize;
            this.precision = precision;

            Logger.Info($"Инициализация FullSearchMethod: step={step}, maximize={maximize}, precision={precision}");
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

            double bestFuncValue = maximize ? double.MinValue : double.MaxValue;
            FuncPoint bestPoint = null;

            // Перебор всех возможных точек в пределах ограничений
            int firstSteps = (int)((upperBounds.FirstUpper - lowerBounds.FirstLower) / step) + 1;
            int secondSteps = (int)((upperBounds.SecondUpper - lowerBounds.SecondLower) / step) + 1;
            Logger.Debug($"Общее количество итераций: {firstSteps} x {secondSteps} = {firstSteps * secondSteps}");

            int iterationCount = 0;
            for (double first = lowerBounds.FirstLower; first <= upperBounds.FirstUpper; first += step)
            {
                for (double second = lowerBounds.SecondLower; second <= upperBounds.SecondUpper; second += step)
                {
                    iterationCount++;
                    Logger.Debug($"Итерация {iterationCount}: First={first}, Second={second}");

                    // Создаем точку с текущими значениями
                    var point = new FuncPoint(first, second);
                    Logger.Debug($"Создание точки: First={point.First}, Second={point.Second}");

                    // Вычисляем значение целевой функции
                    double funcValue = task.CalculateObjectiveFunction(point);
                    Logger.Debug($"Значение функции (до округления): FuncNum={funcValue}");

                    // Округляем значение функции для согласованности с precision
                    funcValue = Math.Round(funcValue, precision);
                    Logger.Debug($"Значение функции (после округления): FuncNum={funcValue}");

                    // Проверка, если найдено лучшее значение
                    if ((maximize && funcValue > bestFuncValue) || (!maximize && funcValue < bestFuncValue))
                    {
                        bool firstOrderValid = task.CheckFirstOrderConstraints(point);
                        bool secondOrderValid = task.CheckSecondOrderConstraints(point);
                        Logger.Debug($"Проверка ограничений: FirstOrder={firstOrderValid}, SecondOrder={secondOrderValid}");

                        if (firstOrderValid && secondOrderValid)
                        {
                            Logger.Debug("Точка удовлетворяет ограничениям, обновление лучшего значения");
                            // Округляем точку до заданной погрешности
                            point.First = Math.Round(first, precision);
                            point.Second = Math.Round(second, precision);
                            point.FuncNum = funcValue; // Используем уже округленное значение

                            bestPoint = point;
                            bestFuncValue = funcValue;
                            Logger.Debug($"Обновлено лучшее значение: First={bestPoint.First}, Second={bestPoint.Second}, FuncNum={bestFuncValue}");
                        }
                        else
                        {
                            Logger.Debug("Точка не удовлетворяет ограничениям, пропускаем");
                        }
                    }
                }
            }

            if (bestPoint == null)
            {
                Logger.Error("Оптимальное значение не найдено.");
                throw new ArgumentNullException("Ошибка, значение не было найдено");
            }

            Logger.Info($"Оптимизация завершена. Лучшее значение: First={bestPoint.First}, Second={bestPoint.Second}, FuncNum={bestPoint.FuncNum}");
            Logger.Debug($"Итоговое количество итераций: {iterationCount}");
            return bestPoint;
        }
    }
}