using System.Drawing;
using Hangfire.Dashboard;
using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Factories
{
    public class AnalyseParameterFactory(
        IDataService dataService,
        IColorPaleteService colorPaleteService,
        IParameterRepository parameterRepository) 
        : IAnalyseParameterFactory
    {
        public async Task<AnalyseParameter<double?>?> CreatePeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (ratio, color, description) = await colorPaleteService.GetColorPeAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Pe,
                Description = description,
                ColorFill = color,
                Ratio = ratio
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreatePbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (ratio, color, description) = await colorPaleteService.GetColorPbvAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Pbv,
                Description = description,
                ColorFill = color,
                Ratio = ratio
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorRevenueAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Revenue,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateNetProfitAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorNetProfitAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.NetProfit,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateFcfAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorFcfAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Fcf,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateEpsAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorEpsAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Eps,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateNetDebtAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorNetDebtAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.NetDebt,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateRoaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorRoaAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Roa,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateRoeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorRoeAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.Roe,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateEvEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (ratio, color, description) = await colorPaleteService.GetColorEvEbitdaAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.EvEbitda,
                Description = description,
                ColorFill = color,
                Ratio = ratio
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateNetDebtEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (ratio, color, description) = await colorPaleteService.GetColorNetDebtEbitdaAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.NetDebtEbitda,
                Description = description,
                ColorFill = color,
                Ratio = ratio
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateEbitdaRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorEbitdaRevenueAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.EbitdaRevenue,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateDividendYieldAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorDividendYieldAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.DividendYield,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<double?>?> CreateDeltaMinMaxAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorDeltaMinMaxAsync(ticker, period);

            var displayParameter = new AnalyseParameter<double?>
            {
                Value = metric.DeltaMinMax,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<AnalyseParameter<bool?>?> CreateDividendAristocratAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;

            var metric1 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 1).ToString());
            var metric2 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 2).ToString());
            var metric3 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 3).ToString());
            var metric4 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 4).ToString());
            var metric5 = await GetMetricAsync(ticker, (int.Parse(predictYear) - 5).ToString());
            
            int count = 0;

            if (metric1 is not null && metric1.Dividend.HasValue && metric1.Dividend.Value > 0) count++;
            if (metric2 is not null && metric2.Dividend.HasValue && metric2.Dividend.Value > 0) count++;
            if (metric3 is not null && metric3.Dividend.HasValue && metric3.Dividend.Value > 0) count++;
            if (metric4 is not null && metric4.Dividend.HasValue && metric4.Dividend.Value > 0) count++;
            if (metric5 is not null && metric5.Dividend.HasValue && metric5.Dividend.Value > 0) count++;

            bool isDividendAristokrat = count >= 4;

            var displayParameter = new AnalyseParameter<bool?>
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
