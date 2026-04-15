namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetFundamentalParameterListResponse
    {
        public GetFundamentalParameterListResult Result { get; set; }
    }

    public class GetFundamentalParameterListResult
    {
        public List<FundamentalParameterListItem> FundamentalParameters { get; set; }
    }

    public class FundamentalParameterListItem
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

        /// <summary>
        /// Доп. данные
        /// </summary>
        public string ExtData { get; set; }
    }
}
