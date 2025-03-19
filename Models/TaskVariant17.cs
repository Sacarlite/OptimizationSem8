using System;
using System.Reflection;
using Models.Interface;

namespace Models
{
    /// <summary>
    /// Реализация задачи оптимизации для варианта №17.
    /// </summary>
    public class TaskVariant17 : ITask
    {
        private readonly string formula = "S = α*G*( (T2- β* A)^N +  µ *exp(T1+T2) ^N +∆ *( T2- T1))";
        private readonly double _alpha;
        private readonly double _beta;
        private readonly double _mu;
        private readonly double _delta;
        private readonly double _g; // Расход реакционной массы, кг/ч
        private readonly double _a; // Давление, кПа
        private readonly int _n;    // Количество теплообменных устройств
        private readonly double _mindiff;

        // Поля для границ параметров (ограничения первого рода)
        private readonly double _firstLowerBound; // Нижняя граница первой переменной
        private readonly double _firstUpperBound; // Верхняя граница первой переменной
        private readonly double _secondLowerBound; // Нижняя граница второй переменной
        private readonly double _secondUpperBound; // Верхняя граница второй переменной

        // Свойства для доступа к полям
        public double Alpha => _alpha;
        public double Beta => _beta;
        public double Mu => _mu;
        public double Delta => _delta;
        public double G => _g; // Расход реакционной массы, кг/ч
        public double A => _a; // Давление, кПа
        public int N => _n;    // Количество теплообменных устройств
        public double MinDiff => _mindiff;

        // Свойства для границ параметров
        public double FirstLowerBound => _firstLowerBound; // Нижняя граница первой переменной
        public double FirstUpperBound => _firstUpperBound; // Верхняя граница первой переменной
        public double SecondLowerBound => _secondLowerBound; // Нижняя граница второй переменной
        public double SecondUpperBound => _secondUpperBound; // Верхняя граница второй переменной

        /// <summary>
        /// Конструктор с параметрами по умолчанию, как в задании.
        /// </summary>
        /// <param name="alpha">Коэффициент Alpha</param>
        /// <param name="beta">Коэффициент Beta</param>
        /// <param name="mu">Коэффициент Mu</param>
        /// <param name="delta">Коэффициент Delta</param>
        /// <param name="g">Расход реакционной массы</param>
        /// <param name="a">Давление</param>
        /// <param name="n">Количество теплообменных устройств</param>
        /// <param name="firstLowerBound">Нижняя граница первой переменной</param>
        /// <param name="firstUpperBound">Верхняя граница первой переменной</param>
        /// <param name="secondLowerBound">Нижняя граница второй переменной</param>
        /// <param name="secondUpperBound">Верхняя граница второй переменной</param>
        public TaskVariant17(double alpha = 1.0, double beta = 1.0, double mu = 1.0, double delta = 1.0,
            double g = 1.0, double a = 1.0, int n = 2, double firstLowerBound = -18, double firstUpperBound = 7.0,
            double secondLowerBound = -8.0, double secondUpperBound = 8.0, double mindiff=2)
        {
            _alpha = alpha;
            _beta = beta;
            _mu = mu;
            _delta = delta;
            _g = g;
            _a = a;
            _n = n;
            _mindiff = mindiff;
            _firstLowerBound = firstLowerBound;
            _firstUpperBound = firstUpperBound;
            _secondLowerBound = secondLowerBound;
            _secondUpperBound = secondUpperBound;
        }

        public double CalculateObjectiveFunction(FuncPoint point)
        {
            double term1 = Math.Pow(point.Second - _beta * _a, _n);           // (Second - β*A)^N
            double term2 = _mu * Math.Pow(Math.Exp(point.First + point.Second), _n); // μ * exp(First + Second)^N
            double term3 = _delta * (point.Second - point.First);           // Δ * (Second - First)

            return _alpha * _g * (term1 + term2 + term3);                    // S = α * G * (term1 + term2 + term3)
        }

        public bool CheckFirstOrderConstraints(FuncPoint point)
        {
            return point.First >= _firstLowerBound && point.First <= _firstUpperBound && // -18 ≤ First ≤ 7
                   point.Second >= _secondLowerBound && point.Second <= _secondUpperBound; // -8 ≤ Second ≤ 8
        }

        public bool CheckSecondOrderConstraints(FuncPoint point)
        {
            return point.Second - point.First >= _mindiff;
        }

        public (double FirstLower, double SecondLower) GetLowerBounds()
        {
            return (FirstLower: _firstLowerBound, SecondLower: _secondLowerBound);
        }

        public (double FirstUpper, double SecondUpper) GetUpperBounds()
        {
            return (FirstUpper: _firstUpperBound, SecondUpper: _secondUpperBound);
        }

        public string GetParametersString()
        {
            // Получаем все свойства текущего объекта
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Формируем строку с параметрами
            var parameters = new List<string>();
            foreach (var property in properties)
            {
                // Получаем значение свойства
                var value = property.GetValue(this);
                // Добавляем имя свойства и его значение в список
                parameters.Add($"{property.Name}: {value}");
            }

            // Объединяем параметры в одну строку
            return string.Join(", ", parameters);
        }
        public string GetFormula()
        {
            return formula;
        }
    }
}