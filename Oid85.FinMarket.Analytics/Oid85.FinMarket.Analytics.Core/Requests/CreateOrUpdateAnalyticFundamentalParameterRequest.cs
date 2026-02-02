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

        public double? Ebitda2019 { get; set; }
        public double? Ebitda2020 { get; set; }
        public double? Ebitda2021 { get; set; }
        public double? Ebitda2022 { get; set; }
        public double? Ebitda2023 { get; set; }
        public double? Ebitda2024 { get; set; }
        public double? Ebitda2025 { get; set; }

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
