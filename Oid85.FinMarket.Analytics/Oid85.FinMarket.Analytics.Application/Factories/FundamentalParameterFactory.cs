using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Factories
{
    public class FundamentalParameterFactory(
        IDataService dataService,
        IColorPaleteService colorPaleteService) 
        : IFundamentalParameterFactory
    {
        public async Task<Parameter<double?>?> CreatePeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorPeAsync(ticker, period);

            var displayParameter = new Parameter<double?>
            {
                Value = metric.Pe,
                Description = description,
                ColorFill = color
            };

            return displayParameter;
        }

        public async Task<Parameter<double?>?> CreatePbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return null;

            var (color, description) = await colorPaleteService.GetColorPbvAsync(ticker, period);

            var displayParameter = new Parameter<double?>
            {
                Value = metric.Pbv,
                Description = description,
                ColorFill = color
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
