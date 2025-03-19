using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    /// <summary>
    /// Интерфейс, описывающий задачу оптимизации.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Вычисляет значение целевой функции для заданной точки.
        /// </summary>
        /// <param name="point">Точка с параметрами для вычисления.</param>
        /// <returns>Значение целевой функции.</returns>
        public double CalculateObjectiveFunction(FuncPoint point);

        /// <summary>
        /// Возвращает формулу в задании в виде строки
        /// </summary>
        /// <returns>Значение формулы.</returns>
        public string GetFormula();

        /// <summary>
        /// Проверяет выполнение ограничений первого рода (диапазоны значений переменных).
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если выполняются ограничения первого рода.</returns>
        public bool CheckFirstOrderConstraints(FuncPoint point);

        /// <summary>
        /// Проверяет выполнение ограничений второго рода (дополнительные условия).
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если выполняются ограничения второго рода.</returns>
        public bool CheckSecondOrderConstraints(FuncPoint point);

        /// <summary>
        /// Возвращает нижние границы параметров в виде именованного кортежа.
        /// </summary>
        /// <returns>Кортеж с нижними границами (FirstLower, SecondLower).</returns>
        public (double FirstLower, double SecondLower) GetLowerBounds();

        /// <summary>
        /// Возвращает верхние границы параметров в виде именованного кортежа.
        /// </summary>
        /// <returns>Кортеж с верхними границами (FirstUpper, SecondUpper).</returns>
        public (double FirstUpper, double SecondUpper) GetUpperBounds();

        /// <summary>
        /// Возвращает строку с названиями параметров и их значениями.
        /// </summary>
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
    }
}
