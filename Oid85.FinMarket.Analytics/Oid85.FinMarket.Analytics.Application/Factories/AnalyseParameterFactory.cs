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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
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
                Ratio = parameterRatio.Ratio.RoundTo(2)
            };

            return displayParameter;
        }

        public async Task<AnalyseRatioParameter<bool?>?> CreateDividendAristocratAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;

            var metric01 = await GetMetricAsync(ticker, (int.Parse(predictYear)).ToString());
            var metric02 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 1).ToString());
            var metric03 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 2).ToString());
            var metric04 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 3).ToString());
            var metric05 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 4).ToString());

            int count = 0;

            if (metric01 is not null && metric01.Dividend.HasValue && metric01.Dividend.Value > 0) count++;
            if (metric02 is not null && metric02.Dividend.HasValue && metric02.Dividend.Value > 0) count++;
            if (metric03 is not null && metric03.Dividend.HasValue && metric03.Dividend.Value > 0) count++;
            if (metric04 is not null && metric04.Dividend.HasValue && metric04.Dividend.Value > 0) count++;
            if (metric05 is not null && metric05.Dividend.HasValue && metric05.Dividend.Value > 0) count++;

            bool isDividendAristokrat = count >= 4;

            var displayParameter = new AnalyseRatioParameter<bool?>
            {
                Value = isDividendAristokrat,
                Description = isDividendAristokrat ? "Дивидендный аристократ" : string.Empty,
                ColorFill = isDividendAristokrat ? KnownColors.Green : KnownColors.White,
                Ratio = isDividendAristokrat ? 1.0 : 0.0
            };

            return displayParameter;
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
