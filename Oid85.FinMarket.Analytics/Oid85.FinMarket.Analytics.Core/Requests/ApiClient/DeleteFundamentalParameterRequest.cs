namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class DeleteFundamentalParameterRequest
    {
        public List<DeleteFundamentalParameterItemRequest> FundamentalParameters { get; set; } = [];
    }

    public class DeleteFundamentalParameterItemRequest
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
    }
}
