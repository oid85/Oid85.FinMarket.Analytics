namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetForecastListResponse
    {
        public GetForecastListResult Result { get; set; } = new();
    }

    public class GetForecastListResult
    {
        public List<GetForecastListItemResponse> Forecasts { get; set; } = [];
    }

    public class GetForecastListItemResponse
    {
        public string Ticker { get; set; }
        public double CurrentPrice { get; set; }
        public double ConsensusPrice { get; set; }
        public double MinTarget { get; set; }
        public double MaxTarget { get; set; }
        public double PriceChange { get; set; }
        public double PriceChangeRel { get; set; }
        public string RecommendationString { get; set; }        
    }
}
