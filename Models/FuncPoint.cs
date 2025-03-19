namespace Models
{
    /// <summary>
    /// Класс, представляющий точку в пространстве параметров функции.
    /// </summary>
    public class FuncPoint
    {
        public double First { get; set; }  // Первая координата
        public double Second { get; set; } // Вторая координата
        public double FuncNum { get; set; } // Значение функции
        /// <summary>
        /// Конструктор для создания точки с заданными координатами.
        /// </summary>
        public FuncPoint(double first, double second)
        {
            First = first;
            Second = second;
        }
    }
}
