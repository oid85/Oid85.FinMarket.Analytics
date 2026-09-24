namespace Oid85.FinMarket.Analytics.Core.Requests
{
    /// <summary>
    /// Списки тикеров
    /// </summary>
    public class CreateWeekTradesPostRequest
    {
        /// <summary>
        /// Тикеры, по которым были получены дивиденды
        /// </summary>
        public string ReceivedDividendTickers { get; set; } = string.Empty;

        /// <summary>
        /// Тикеры купленных облигаций
        /// </summary>
        public string BuyBondTickers { get; set; } = string.Empty;

        /// <summary>
        /// Тикеры купленных акций
        /// </summary>
        public string BuyShareTickers { get; set; } = string.Empty;
    }
}
