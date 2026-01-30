namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class CreateOrUpdateAnalyticFundamentalParameterRequest
    {
        public string Ticker { get; set; }
        public double? Pe2019 { get; set; }
        public double? Pe2020 { get; set; }
        public double? Pe2021 { get; set; }
        public double? Pe2022 { get; set; }
        public double? Pe2023 { get; set; }
        public double? Pe2024 { get; set; }
        public double? Pe2025 { get; set; }
    }
}
