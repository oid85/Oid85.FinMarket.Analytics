namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public double? Pe2019 { get; set; }
        public double? Pe2020 { get; set; }
        public double? Pe2021 { get; set; }
        public double? Pe2022 { get; set; }
        public double? Pe2023 { get; set; }
        public double? Pe2024 { get; set; }
        public double? Pe2025 { get; set; }
    }
}
