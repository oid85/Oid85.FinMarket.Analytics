namespace Oid85.FinMarket.Analytics.Core.Models
{
    /// <summary>
    /// Инструмент
    /// </summary>
    public class Instrument
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тикер
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Инструмент выбран
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Инструмент в портфеле
        /// </summary>
        public bool InPortfolio { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
    }
}
