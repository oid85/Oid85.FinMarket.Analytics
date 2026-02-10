namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class CreateOrUpdateFundamentalParameterRequest
    {
        public List<CreateOrUpdateFundamentalParameterItemRequest> FundamentalParameters { get; set; } = [];
    }

    public class CreateOrUpdateFundamentalParameterItemRequest
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
