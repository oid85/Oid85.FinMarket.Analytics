using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Entities
{
    /// <summary>
    /// Инструмент
    /// </summary>
    public class InstrumentEntity : BaseEntity
    {
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

        /// <summary>
        /// Ручной коэффициент
        /// </summary>
        public double ManualCoefficient { get; set; } = 1.0;

        /// <summary>
        /// Сектор
        /// </summary>
        public string Sector { get; set; }
    }
}
