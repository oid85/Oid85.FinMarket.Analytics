namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalParameterListResponse
    {
        public GetFundamentalParameterListResult Result { get; set; }        
    }

    public class GetFundamentalParameterListResult
    {
        public List<GetFundamentalParameterListItemResponse> FundamentalParameters { get; set; }
    }

    public class GetFundamentalParameterListItemResponse
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
    }
}
