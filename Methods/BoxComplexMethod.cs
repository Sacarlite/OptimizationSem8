using System;
using System.Linq;
using Models.Interface;
using Models;
using NLog;
using System.Collections.Generic;

namespace Methods
{
    /// <summary>
    /// Класс, реализующий комплекс-метод Бокса для оптимизации.
    /// </summary>
    public class BoxComplexMethod:MethodModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITask task;
        private readonly int n = 2;       // Количество переменных
        private readonly int complexSize; // Размер комплекса (N ≥ n + 1)
        private readonly double epsilon;  // Точность сходимости
        private readonly int precision;   // Погрешность округления (количество знаков после запятой)
        private readonly Random random;
        private readonly bool findMaximum; // Если true - ищем максимум, если false - минимум

        
        public BoxComplexMethod(ITask task, bool findMaximum = false, int complexSize = 5, double epsilon = 0.1, int precision = 2)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.complexSize = complexSize >= n + 1 ? complexSize : n + 1;
            this.epsilon = epsilon;
            this.precision = precision;
            this.random = new Random();
            this.findMaximum = findMaximum;

            Logger.Info($"Инициализация BoxComplexMethod: findMaximum={findMaximum}, complexSize={complexSize}, epsilon={epsilon}, precision={precision}");
            Logger.Debug($"Объект задачи: {task.GetType().Name}");
            Logger.Debug($"Размерность: n={n}");
        }
        public BoxComplexMethod()
        {
            MethodName = "Комплексный метод Бокса";

        }
        /// <summary>
        /// Выполняет оптимизацию методом Бокса.
        /// </summary>
        /// <returns>Оптимальные параметры в виде объекта FuncPoint с округлением до заданной погрешности.</returns>
        public FuncPoint Optimize()
        {
            Logger.Info("Начало оптимизации методом Бокса");
            var lowerBounds = task.GetLowerBounds();
            var upperBounds = task.GetUpperBounds();
            Logger.Debug($"Границы: LowerBounds=({lowerBounds.FirstLower}, {lowerBounds.SecondLower}), UpperBounds=({upperBounds.FirstUpper}, {upperBounds.SecondUpper})");

            // Создание начального комплекса
            Logger.Debug("Создание начального комплекса");
            FuncPoint[] complex = CreateInitialComplex(lowerBounds, upperBounds);

            // Основной цикл метода Бокса
            int iterationLimit = 1000; // Ограничение на количество итераций для предотвращения зацикливания
            int iterations = 0;

            while (true)
            {
                Logger.Debug($"Итерация {iterations + 1}/{iterationLimit}");
                Logger.Debug("Поиск индексов лучшей и худшей точек");
                (int bestIndex, int worstIndex) = FindBestAndWorstIndices(complex);

                Logger.Debug("Вычисление центра комплекса");
                double[] center = CalculateCenter(complex, worstIndex);

                Logger.Debug("Проверка сходимости");
                if (IsConverged(complex, center, bestIndex, worstIndex))
                {
                    Logger.Info($"Сходимость достигнута на итерации {iterations + 1} с погрешностью {epsilon}");
                    break;
                }

                Logger.Debug("Генерация новой точки");
                FuncPoint newPoint = GenerateNewPoint(center, complex[worstIndex]);
                Logger.Debug("Корректировка новой точки для соответствия ограничениям");
                AdjustPointForConstraints(ref newPoint, lowerBounds, upperBounds, center, complex[bestIndex], complex[worstIndex].FuncNum);

                Logger.Debug($"Замена худшей точки (индекс {worstIndex}) на новую: First={newPoint.First}, Second={newPoint.Second}, FuncNum={newPoint.FuncNum}");
                complex[worstIndex] = newPoint;
                iterations++;
            }

            if (iterations >= iterationLimit)
            {
                Logger.Error($"Метод не сошелся за {iterationLimit} итераций.");
                throw new Exception("Метод не сошелся в пределах заданного количества итераций.");
            }

            // Возвращаем лучшую точку с округлением
            Logger.Debug("Поиск финального лучшего индекса");
            int finalBestIndex = FindBestAndWorstIndices(complex).maxNumIndex;
            FuncPoint result = new FuncPoint(
                Math.Round(complex[finalBestIndex].First, precision),
                Math.Round(complex[finalBestIndex].Second, precision)
            );
            // Округляем значение функции с учётом precision
            double rawFuncValue = task.CalculateObjectiveFunction(result);
            result.FuncNum = Math.Round(rawFuncValue, precision);
            Logger.Debug($"Финальное значение функции (до округления): {rawFuncValue}");
            Logger.Debug($"Финальное значение функции (после округления): {result.FuncNum}");

            Logger.Info($"Оптимизация завершена. Результат: First={result.First}, Second={result.Second}, FuncNum={result.FuncNum}");
            Logger.Debug($"Финальная точка (без округления): First={complex[finalBestIndex].First}, Second={complex[finalBestIndex].Second}, FuncNum={complex[finalBestIndex].FuncNum}");
            return result;
        }

        /// <summary>
        /// Создаёт начальный комплекс точек.
        /// </summary>
        private FuncPoint[] CreateInitialComplex(
            (double FirstLower, double SecondLower) lowerBounds,
            (double FirstUpper, double SecondUpper) upperBounds)
        {
            FuncPoint[] complex = new FuncPoint[complexSize];
            Logger.Debug($"Создание комплекса из {complexSize} точек");

            for (int j = 0; j < complexSize; j++)
            {
                Logger.Debug($"Попытка создания точки {j + 1}/{complexSize}");
                FuncPoint point;
                bool validPoint = false;
                int attempts = 0;
                const int maxAttempts = 100;

                while (!validPoint && attempts < maxAttempts)
                {
                    Logger.Debug($"Попытка {attempts + 1}/{maxAttempts}");
                    double first = lowerBounds.FirstLower + random.NextDouble() * (upperBounds.FirstUpper - lowerBounds.FirstLower);
                    double second = lowerBounds.SecondLower + random.NextDouble() * (upperBounds.SecondUpper - lowerBounds.SecondLower);
                    Logger.Debug($"Сгенерированы координаты: First={first}, Second={second}");

                    point = new FuncPoint(first, second);
                    double rawFuncValue = task.CalculateObjectiveFunction(point);
                    point.FuncNum = Math.Round(rawFuncValue, precision); // Округление значения функции
                    Logger.Debug($"Значение функции (до округления): FuncNum={rawFuncValue}");
                    Logger.Debug($"Значение функции (после округления): FuncNum={point.FuncNum}");

                    bool firstOrderValid = task.CheckFirstOrderConstraints(point);
                    bool secondOrderValid = task.CheckSecondOrderConstraints(point);
                    Logger.Debug($"Проверка ограничений: FirstOrder={firstOrderValid}, SecondOrder={secondOrderValid}");

                    if (firstOrderValid && secondOrderValid)
                    {
                        complex[j] = point;
                        validPoint = true;
                        Logger.Debug($"Точка {j + 1} создана: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");
                    }
                    attempts++;
                }

                if (!validPoint)
                {
                    Logger.Error($"Не удалось создать точку {j + 1} после {maxAttempts} попыток.");
                    throw new Exception("Не удалось сформировать начальный комплекс: слишком строгие ограничения.");
                }
            }

            Logger.Debug("Начальный комплекс создан:");
            for (int i = 0; i < complex.Length; i++)
            {
                Logger.Debug($"Точка {i + 1}: First={complex[i].First}, Second={complex[i].Second}, FuncNum={complex[i].FuncNum}");
            }
            return complex;
        }

        /// <summary>
        /// Находит индексы лучшей и худшей точек в комплексе.
        /// </summary>
        private (int maxNumIndex, int minNumIndex) FindBestAndWorstIndices(FuncPoint[] complex)
        {
            Logger.Debug("Поиск лучшей и худшей точек в комплексе");
            for (int i = 0; i < complex.Length; i++)
            {
                Logger.Debug($"Точка {i}: First={complex[i].First}, Second={complex[i].Second}, FuncNum={complex[i].FuncNum}");
            }

            var minNumIndex = complex.Select((point, index) => new { point, index })
                .OrderBy(p => findMaximum ? p.point.FuncNum : -p.point.FuncNum)
                .First().index;
            var maxNumIndex = complex.Select((point, index) => new { point, index })
                .OrderBy(p => findMaximum ? -p.point.FuncNum : p.point.FuncNum)
                .First().index;
            Logger.Debug($"Лучшая точка: индекс={maxNumIndex}, First={complex[maxNumIndex].First}, Second={complex[maxNumIndex].Second}, FuncNum={complex[maxNumIndex].FuncNum}");
            Logger.Debug($"Худшая точка: индекс={minNumIndex}, First={complex[minNumIndex].First}, Second={complex[minNumIndex].Second}, FuncNum={complex[minNumIndex].FuncNum}");
            return (maxNumIndex, minNumIndex);
        }

        /// <summary>
        /// Вычисляет центр комплекса, исключая худшую точку.
        /// </summary>
        private double[] CalculateCenter(FuncPoint[] complex, int worstIndex)
        {
            Logger.Debug($"Вычисление центра комплекса, исключая худшую точку (индекс {worstIndex})");
            double[] center = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                int count = 0;
                for (int j = 0; j < complexSize; j++)
                {
                    if (j != worstIndex)
                    {
                        sum += i == 0 ? complex[j].First : complex[j].Second;
                        count++;
                    }
                }
                center[i] = sum / count;
                Logger.Debug($"Центр по оси {i}: {center[i]} (сумма={sum}, количество точек={count})");
            }
            Logger.Debug($"Центр комплекса: First={center[0]}, Second={center[1]}");
            return center;
        }

        /// <summary>
        /// Проверяет, достигнута ли сходимость.
        /// </summary>
        private bool IsConverged(FuncPoint[] complex, double[] center, int bestIndex, int worstIndex)
        {
            Logger.Debug("Проверка сходимости комплекса");
            double bestToCenter = Math.Sqrt(
                (complex[bestIndex].First - center[0]) * (complex[bestIndex].First - center[0]) +
                (complex[bestIndex].Second - center[1]) * (complex[bestIndex].Second - center[1])
            );
            double worstToCenter = Math.Sqrt(
                (complex[worstIndex].First - center[0]) * (complex[worstIndex].First - center[0]) +
                (complex[worstIndex].Second - center[1]) * (complex[worstIndex].Second - center[1])
            );
            double b = Math.Sqrt(0.5 * (bestToCenter * bestToCenter + worstToCenter * worstToCenter));
            Logger.Debug($"Расстояние лучшей точки до центра: {bestToCenter}");
            Logger.Debug($"Расстояние худшей точки до центра: {worstToCenter}");
            Logger.Debug($"Критерий сходимости b: {b}, epsilon={epsilon}");

            bool converged = b < epsilon;
            Logger.Debug($"Сходимость достигнута: {converged}");
            return converged;
        }

        /// <summary>
        /// Генерирует новую точку путём отражения худшей точки через центр.
        /// </summary>
        private FuncPoint GenerateNewPoint(double[] center, FuncPoint worstPoint)
        {
            Logger.Debug($"Генерация новой точки через отражение худшей точки: First={worstPoint.First}, Second={worstPoint.Second}");
            double first = 2.3 * center[0] - 1.3 * worstPoint.First;
            double second = 2.3 * center[1] - 1.3 * worstPoint.Second;
            var newPoint = new FuncPoint(first, second);
            double rawFuncValue = task.CalculateObjectiveFunction(newPoint);
            newPoint.FuncNum = Math.Round(rawFuncValue, precision); // Округление значения функции
            Logger.Debug($"Значение функции (до округления): FuncNum={rawFuncValue}");
            Logger.Debug($"Новая точка (после округления): First={newPoint.First}, Second={newPoint.Second}, FuncNum={newPoint.FuncNum}");
            return newPoint;
            // Проверка на близость к нулю
            if (Math.Abs(first) < 1e-10) first = 0;
            if (Math.Abs(second) < 1e-10) second = 0;
        }

        /// <summary>
        /// Корректирует точку, чтобы она удовлетворяла ограничениям, и обновляет её значение.
        /// </summary>
        private void AdjustPointForConstraints(
            ref FuncPoint point,
            (double FirstLower, double SecondLower) lowerBounds,
            (double FirstUpper, double SecondUpper) upperBounds,
            double[] center,
            FuncPoint bestPoint,
            double worstValue)
        {
            Logger.Debug($"Корректировка точки: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");

            // Корректировка ограничений первого рода
            bool firstOrderValid = task.CheckFirstOrderConstraints(point);
            Logger.Debug($"Проверка ограничений первого рода: {firstOrderValid}");
            if (!firstOrderValid)
            {
                Logger.Debug("Точка нарушает ограничения первого рода. Корректировка...");
                if (point.First < lowerBounds.FirstLower)
                {
                    Logger.Debug($"First ({point.First}) < {lowerBounds.FirstLower}. Установка First={lowerBounds.FirstLower}");
                    point.First = lowerBounds.FirstLower;
                }
                if (point.First > upperBounds.FirstUpper)
                {
                    Logger.Debug($"First ({point.First}) > {upperBounds.FirstUpper}. Установка First={upperBounds.FirstUpper}");
                    point.First = upperBounds.FirstUpper;
                }
                if (point.Second < lowerBounds.SecondLower)
                {
                    Logger.Debug($"Second ({point.Second}) < {lowerBounds.SecondLower}. Установка Second={lowerBounds.SecondLower}");
                    point.Second = lowerBounds.SecondLower;
                }
                if (point.Second > upperBounds.SecondUpper)
                {
                    Logger.Debug($"Second ({point.Second}) > {upperBounds.SecondUpper}. Установка Second={upperBounds.SecondUpper}");
                    point.Second = upperBounds.SecondUpper;
                }
                // Проверка на близость к нулю
                if (Math.Abs(point.First) < 1e-10) point.First = 0;
                if (Math.Abs(point.Second) < 1e-10) point.Second = 0;
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision); // Округление после корректировки
                Logger.Debug($"Значение функции (до округления): FuncNum={rawFuncValue}");
                Logger.Debug($"Точка после корректировки первого рода: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");
            }

            // Корректировка ограничений второго рода
            int secondOrderAttempts = 0;
            const int maxSecondOrderAttempts = 100;
            bool secondOrderValid = task.CheckSecondOrderConstraints(point);
            Logger.Debug($"Проверка ограничений второго рода: {secondOrderValid}");
            while (!secondOrderValid && secondOrderAttempts < maxSecondOrderAttempts)
            {
                Logger.Debug($"Попытка {secondOrderAttempts + 1}/{maxSecondOrderAttempts} корректировки второго рода");
                point.First = (point.First + center[0]) / 2;
                point.Second = (point.Second + center[1]) / 2;
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision); // Округление после корректировки
                Logger.Debug($"Значение функции (до округления): FuncNum={rawFuncValue}");
                Logger.Debug($"Точка после корректировки: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");
                secondOrderValid = task.CheckSecondOrderConstraints(point);
                Logger.Debug($"Проверка ограничений второго рода: {secondOrderValid}");
                secondOrderAttempts++;
            }

            if (!secondOrderValid)
            {
                Logger.Warn($"Не удалось скорректировать точку для ограничений второго рода после {maxSecondOrderAttempts} попыток.");
            }

            // Улучшение точки в зависимости от цели (максимум или минимум)
            int improvementAttempts = 0;
            const int maxImprovementAttempts = 50;
            bool improvementCondition = findMaximum ? point.FuncNum < bestPoint.FuncNum : point.FuncNum > bestPoint.FuncNum;
            Logger.Debug($"Проверка необходимости улучшения точки: {improvementCondition} (findMaximum={findMaximum}, point.FuncNum={point.FuncNum}, bestPoint.FuncNum={bestPoint.FuncNum})");
            while (improvementCondition && improvementAttempts < maxImprovementAttempts)
            {
                Logger.Debug($"Попытка {improvementAttempts + 1}/{maxImprovementAttempts} улучшения точки");
                point.First = (point.First + bestPoint.First) / 2;
                point.Second = (point.Second + bestPoint.Second) / 2;
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision); // Округление после улучшения
                Logger.Debug($"Значение функции (до округления): FuncNum={rawFuncValue}");
                Logger.Debug($"Точка после улучшения: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");
                improvementCondition = findMaximum ? point.FuncNum < bestPoint.FuncNum : point.FuncNum > bestPoint.FuncNum;
                Logger.Debug($"Проверка необходимости дальнейшего улучшения: {improvementCondition}");
                improvementAttempts++;
            }

            if (improvementAttempts >= maxImprovementAttempts)
            {
                Logger.Warn($"Достигнуто максимальное количество попыток улучшения ({maxImprovementAttempts}).");
            }

            Logger.Debug($"Финальная скорректированная точка: First={point.First}, Second={point.Second}, FuncNum={point.FuncNum}");
        }
    }
}