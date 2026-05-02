namespace Oid85.FinMarket.Analytics.Core.Models
{
    /// <summary>
    /// Числовой параметр для вывода на экран
    /// </summary>
    public class AnalyseParameter<T>
    {
        /// <summary>
        /// Значение
        /// </summary>
        public T? Value { get; set; } = default;

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// Цвет фона
        /// </summary>
        public string ColorFill { get; set; }  = "#FFFFFF";

        /// <summary>
        /// Оценочный коэффициент
        /// </summary>
        public double Ratio { get; set; } = 0.0;
    }
}
