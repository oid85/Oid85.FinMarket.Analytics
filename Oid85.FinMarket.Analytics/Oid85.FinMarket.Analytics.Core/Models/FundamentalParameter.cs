using Oid85.FinMarket.Analytics.Core.Models.Base;

namespace Oid85.FinMarket.Analytics.Core.Models
{
    /// <summary>
    /// Фундаментальный параметр
    /// </summary>
    public class FundamentalParameter : BaseModel
    {
        /// <summary>
        /// Тикер
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value { get; set; }
    }
}
