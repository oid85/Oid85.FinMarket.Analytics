namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public GetAnalyticFundamentalParameterListItemValueResponse Pe { get; set; } = new();
    }

    public class GetAnalyticFundamentalParameterListItemValueResponse
    {
        public double? _2019 { get; set; } = null;
        public double? _2020 { get; set; } = null;
        public double? _2021 { get; set; } = null;
        public double? _2022 { get; set; } = null;
        public double? _2023 { get; set; } = null;
        public double? _2024 { get; set; } = null;
        public double? _2025 { get; set; } = null;
    }
}
