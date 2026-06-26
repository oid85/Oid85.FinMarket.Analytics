using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Factories
{
    public class AnalyseParameterFactory(
        IDataService dataService,
        IFundamentalParameterRatioService fundamentalParameterRatioService,
        IParameterRepository parameterRepository)
        : IAnalyseParameterFactory
    {
        public async Task<AnalyseRatioParameter<double?>?> CreateMarketCapAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioMarketCapAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.MarketCap,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreatePeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioPeAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Pe,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreatePbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioPbvAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Pbv,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioRevenueAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Revenue,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateNetProfitAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioNetProfitAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.NetProfit,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateFcfAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioFcfAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Fcf,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateEpsAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetColorEpsAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Eps,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateNetDebtAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioNetDebtAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.NetDebt,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateRoaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioRoaAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Roa,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateRoeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioRoeAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.Roe,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateEvEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioEvEbitdaAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.EvEbitda,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateNetDebtEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioNetDebtEbitdaAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.NetDebtEbitda,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateDebtRatioAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioDebtRatioAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.DebtRatio,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateDebtEquityAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioDebtEquityAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.DebtEquity,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateEbitdaRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioEbitdaRevenueAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.EbitdaRevenue,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateOwnCapitalNumberSharesAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.OwnCapitalNumberShares,
                Description = string.Empty,
                ColorFill = KnownColors.White
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateDividendYieldAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioDividendYieldAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.DividendYield,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<double?>?> CreateDeltaMinMaxAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            var parameterRatio = await fundamentalParameterRatioService.GetRatioDeltaMinMaxAsync(ticker, period);

            var displayParameter = new AnalyseRatioParameter<double?>
            {
                Value = metric.DeltaMinMax,
                Description = parameterRatio.Description,
                ColorFill = parameterRatio.Color,
                Ratio = parameterRatio.Ratio.RoundTo(2),
                Text = parameterRatio.Text
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<bool?>?> CreateDividendAristocratAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;

            var metric_0 = await GetMetricAsync(ticker, (int.Parse(ttmYear)).ToString());
            var metric_1 = await GetMetricAsync(ticker, (int.Parse(ttmYear) - 1).ToString());
            var metric_2 = await GetMetricAsync(ticker, (int.Parse(ttmYear) - 2).ToString());
            var metric_3 = await GetMetricAsync(ticker, (int.Parse(ttmYear) - 3).ToString());
            var metric_4 = await GetMetricAsync(ticker, (int.Parse(ttmYear) - 4).ToString());

            int count = 0;

            if (metric_0 is not null && metric_0.Dividend.HasValue && metric_0.Dividend.Value > 0) count++;
            if (metric_1 is not null && metric_1.Dividend.HasValue && metric_1.Dividend.Value > 0) count++;
            if (metric_2 is not null && metric_2.Dividend.HasValue && metric_2.Dividend.Value > 0) count++;
            if (metric_3 is not null && metric_3.Dividend.HasValue && metric_3.Dividend.Value > 0) count++;
            if (metric_4 is not null && metric_4.Dividend.HasValue && metric_4.Dividend.Value > 0) count++;

            if (count == 5)
                return new ()
                {
                    Value = true,
                    Ratio = 1.0,
                    ColorFill = KnownColors.Green,
                    Description = "✅ 🏆 Выплаты каждый год без отмены",                                        
                    Text = "✅ Компания платит дивиденды более 5 лет подряд. 🏆 Выплаты каждый год без отмены"
                };

            if (count == 4)
                return new()
                {
                    Value = false,
                    Ratio = 0.75,
                    ColorFill = KnownColors.LightGreen,
                    Description = "✅ Стабильные дивиденды",
                    Text = "✅ Компания за последние 5 лет пропустила выплату дивидендов только 1 раз"
                };

            if (count == 3 || count == 2 || count == 1)
                return new()
                {
                    Value = false,
                    Ratio = 0.5,
                    ColorFill = KnownColors.Yellow,
                    Description = "⚠️ Дивиденды не стабильны",
                    Text = "⚠️ Компания за последние 5 лет платит дивиденды не каждый год"
                };

            return new()
            {
                Value = false,
                Ratio = 0.0,
                ColorFill = KnownColors.Red,
                Description = "❗ Дивидендов нет",
                Text = "❗ Компания за последние 5 лет дивиденды не выплачивала"
            };
        }

        private async Task<FundamentalMetric?> GetMetricAsync(string ticker, string period)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();
            var metrics = analyseDataContext.GetFundamentalMetrics(ticker);
            var metric = metrics.Find(x => x.Period == period);

            return metric;
        }
    }
}
