namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double? Pe2019 { get; set; }
        public double? Pe2020 { get; set; }
        public double? Pe2021 { get; set; }
        public double? Pe2022 { get; set; }
        public double? Pe2023 { get; set; }
        public double? Pe2024 { get; set; }
        public double? Pe2025 { get; set; }
        public double? Revenue2019 { get; set; }
        public double? Revenue2020 { get; set; }
        public double? Revenue2021 { get; set; }
        public double? Revenue2022 { get; set; }
        public double? Revenue2023 { get; set; }
        public double? Revenue2024 { get; set; }
        public double? Revenue2025 { get; set; }
        public double? NetProfit2019 { get; set; }
        public double? NetProfit2020 { get; set; }
        public double? NetProfit2021 { get; set; }
        public double? NetProfit2022 { get; set; }
        public double? NetProfit2023 { get; set; }
        public double? NetProfit2024 { get; set; }
        public double? NetProfit2025 { get; set; }
    }
}
