namespace Oid85.FinMarket.Analytics.Core.Models
{
    /// <summary>
    /// Числовой параметр для вывода на экран
    /// </summary>
    public class NumberParameter
    {
        /// <summary>
        /// Значение
        /// </summary>
        public double? Value { get; set; } = null;

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// Цвет фона
        /// </summary>
        public string ColorFill { get; set; }  = "#FFFFFF";

        /// <summary>
        /// Цвет текста
        /// </summary>
        public string ColorStroke { get; set; } = "#000000";
    }
}
