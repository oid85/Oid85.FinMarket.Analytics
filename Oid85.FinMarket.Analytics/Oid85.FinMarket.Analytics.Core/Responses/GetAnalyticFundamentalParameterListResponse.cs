namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public int Number { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public bool InPortfolio { get; set; }

        /// <summary>
        /// Расчетный рейтинг
        /// </summary>
        public double? Score { get; set; }

        /// <summary>
        /// Доля в индексе MOEX
        /// </summary>
        public double? Moex { get; set; }

        /// <summary>
        /// Изменение относительно индекса полной доходности
        /// </summary>
        public double BenchmarkChange { get; set; }

        public double? Price2015 { get; set; }
        public double? Price2016 { get; set; }
        public double? Price2017 { get; set; }
        public double? Price2018 { get; set; }
        public double? Price2019 { get; set; }
        public double? Price2020 { get; set; }
        public double? Price2021 { get; set; }
        public double? Price2022 { get; set; }
        public double? Price2023 { get; set; }
        public double? Price2024 { get; set; }
        public double? Price2025 { get; set; }
        public double? Price2026 { get; set; }

        public double? Pe2015 { get; set; }
        public double? Pe2016 { get; set; }
        public double? Pe2017 { get; set; }
        public double? Pe2018 { get; set; }
        public double? Pe2019 { get; set; }
        public double? Pe2020 { get; set; }
        public double? Pe2021 { get; set; }
        public double? Pe2022 { get; set; }
        public double? Pe2023 { get; set; }
        public double? Pe2024 { get; set; }
        public double? Pe2025 { get; set; }
        public double? Pe2026 { get; set; }

        public double? Ebitda2015 { get; set; }
        public double? Ebitda2016 { get; set; }
        public double? Ebitda2017 { get; set; }
        public double? Ebitda2018 { get; set; }
        public double? Ebitda2019 { get; set; }
        public double? Ebitda2020 { get; set; }
        public double? Ebitda2021 { get; set; }
        public double? Ebitda2022 { get; set; }
        public double? Ebitda2023 { get; set; }
        public double? Ebitda2024 { get; set; }
        public double? Ebitda2025 { get; set; }
        public double? Ebitda2026 { get; set; }

        public double? Revenue2015 { get; set; }
        public double? Revenue2016 { get; set; }
        public double? Revenue2017 { get; set; }
        public double? Revenue2018 { get; set; }
        public double? Revenue2019 { get; set; }
        public double? Revenue2020 { get; set; }
        public double? Revenue2021 { get; set; }
        public double? Revenue2022 { get; set; }
        public double? Revenue2023 { get; set; }
        public double? Revenue2024 { get; set; }
        public double? Revenue2025 { get; set; }
        public double? Revenue2026 { get; set; }

        public double? NetProfit2015 { get; set; }
        public double? NetProfit2016 { get; set; }
        public double? NetProfit2017 { get; set; }
        public double? NetProfit2018 { get; set; }
        public double? NetProfit2019 { get; set; }
        public double? NetProfit2020 { get; set; }
        public double? NetProfit2021 { get; set; }
        public double? NetProfit2022 { get; set; }
        public double? NetProfit2023 { get; set; }
        public double? NetProfit2024 { get; set; }
        public double? NetProfit2025 { get; set; }
        public double? NetProfit2026 { get; set; }

        public double? Ev2015 { get; set; }
        public double? Ev2016 { get; set; }
        public double? Ev2017 { get; set; }
        public double? Ev2018 { get; set; }
        public double? Ev2019 { get; set; }
        public double? Ev2020 { get; set; }
        public double? Ev2021 { get; set; }
        public double? Ev2022 { get; set; }
        public double? Ev2023 { get; set; }
        public double? Ev2024 { get; set; }
        public double? Ev2025 { get; set; }
        public double? Ev2026 { get; set; }

        public double? NetDebt2015 { get; set; }
        public double? NetDebt2016 { get; set; }
        public double? NetDebt2017 { get; set; }
        public double? NetDebt2018 { get; set; }
        public double? NetDebt2019 { get; set; }
        public double? NetDebt2020 { get; set; }
        public double? NetDebt2021 { get; set; }
        public double? NetDebt2022 { get; set; }
        public double? NetDebt2023 { get; set; }
        public double? NetDebt2024 { get; set; }
        public double? NetDebt2025 { get; set; }
        public double? NetDebt2026 { get; set; }

        public double? MarketCap2015 { get; set; }
        public double? MarketCap2016 { get; set; }
        public double? MarketCap2017 { get; set; }
        public double? MarketCap2018 { get; set; }
        public double? MarketCap2019 { get; set; }
        public double? MarketCap2020 { get; set; }
        public double? MarketCap2021 { get; set; }
        public double? MarketCap2022 { get; set; }
        public double? MarketCap2023 { get; set; }
        public double? MarketCap2024 { get; set; }
        public double? MarketCap2025 { get; set; }
        public double? MarketCap2026 { get; set; }

        public double? DividendYield2015 { get; set; }
        public double? DividendYield2016 { get; set; }
        public double? DividendYield2017 { get; set; }
        public double? DividendYield2018 { get; set; }
        public double? DividendYield2019 { get; set; }
        public double? DividendYield2020 { get; set; }
        public double? DividendYield2021 { get; set; }
        public double? DividendYield2022 { get; set; }
        public double? DividendYield2023 { get; set; }
        public double? DividendYield2024 { get; set; }
        public double? DividendYield2025 { get; set; }
        public double? DividendYield2026 { get; set; }

        public double? Roa2015 { get; set; }
        public double? Roa2016 { get; set; }
        public double? Roa2017 { get; set; }
        public double? Roa2018 { get; set; }
        public double? Roa2019 { get; set; }
        public double? Roa2020 { get; set; }
        public double? Roa2021 { get; set; }
        public double? Roa2022 { get; set; }
        public double? Roa2023 { get; set; }
        public double? Roa2024 { get; set; }
        public double? Roa2025 { get; set; }
        public double? Roa2026 { get; set; }

        public double? Pbv2015 { get; set; }
        public double? Pbv2016 { get; set; }
        public double? Pbv2017 { get; set; }
        public double? Pbv2018 { get; set; }
        public double? Pbv2019 { get; set; }
        public double? Pbv2020 { get; set; }
        public double? Pbv2021 { get; set; }
        public double? Pbv2022 { get; set; }
        public double? Pbv2023 { get; set; }
        public double? Pbv2024 { get; set; }
        public double? Pbv2025 { get; set; }
        public double? Pbv2026 { get; set; }

        public double? EvEbitda2015 { get; set; }
        public double? EvEbitda2016 { get; set; }
        public double? EvEbitda2017 { get; set; }
        public double? EvEbitda2018 { get; set; }
        public double? EvEbitda2019 { get; set; }
        public double? EvEbitda2020 { get; set; }
        public double? EvEbitda2021 { get; set; }
        public double? EvEbitda2022 { get; set; }
        public double? EvEbitda2023 { get; set; }
        public double? EvEbitda2024 { get; set; }
        public double? EvEbitda2025 { get; set; }
        public double? EvEbitda2026 { get; set; }

        public double? NetDebtEbitda2015 { get; set; }
        public double? NetDebtEbitda2016 { get; set; }
        public double? NetDebtEbitda2017 { get; set; }
        public double? NetDebtEbitda2018 { get; set; }
        public double? NetDebtEbitda2019 { get; set; }
        public double? NetDebtEbitda2020 { get; set; }
        public double? NetDebtEbitda2021 { get; set; }
        public double? NetDebtEbitda2022 { get; set; }
        public double? NetDebtEbitda2023 { get; set; }
        public double? NetDebtEbitda2024 { get; set; }
        public double? NetDebtEbitda2025 { get; set; }
        public double? NetDebtEbitda2026 { get; set; }

        public double? EbitdaRevenue2015 { get; set; }
        public double? EbitdaRevenue2016 { get; set; }
        public double? EbitdaRevenue2017 { get; set; }
        public double? EbitdaRevenue2018 { get; set; }
        public double? EbitdaRevenue2019 { get; set; }
        public double? EbitdaRevenue2020 { get; set; }
        public double? EbitdaRevenue2021 { get; set; }
        public double? EbitdaRevenue2022 { get; set; }
        public double? EbitdaRevenue2023 { get; set; }
        public double? EbitdaRevenue2024 { get; set; }
        public double? EbitdaRevenue2025 { get; set; }
        public double? EbitdaRevenue2026 { get; set; }

        public double? DeltaMinMax2015 { get; set; }
        public double? DeltaMinMax2016 { get; set; }
        public double? DeltaMinMax2017 { get; set; }
        public double? DeltaMinMax2018 { get; set; }
        public double? DeltaMinMax2019 { get; set; }
        public double? DeltaMinMax2020 { get; set; }
        public double? DeltaMinMax2021 { get; set; }
        public double? DeltaMinMax2022 { get; set; }
        public double? DeltaMinMax2023 { get; set; }
        public double? DeltaMinMax2024 { get; set; }
        public double? DeltaMinMax2025 { get; set; }
        public double? DeltaMinMax2026 { get; set; }
    }
}
