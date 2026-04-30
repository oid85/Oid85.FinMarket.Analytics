using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class ColorPaleteService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService) 
        : IColorPaleteService
    {
        public async Task<(string Color, string Description)> GetColorPeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);
            
            if (metric is null) return (KnownColors.White, "");
            
            if (metric.Pe.HasValue)
            {
                if (metric.Pe.Value <= 0.0)
                    return (KnownColors.Red, "P/E отрицательный. Отрицательный P/E (цена/прибыль) означает, что компания понесла убытки в отчетном периоде, так как чистая прибыль (знаменатель) стала отрицательной. Это сигнал финансовых трудностей, указывающий, что компания не окупается, а теряет акционерный капитал");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Pe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int isMoreThanCount = sectorMetricValues.Count(x => x.Pe!.Value >= metric.Pe.Value);

                double ratio = Convert.ToDouble(isMoreThanCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (KnownColors.Green, $"P/E низкое в секторе - меньше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (KnownColors.LightGreen, $"P/E ниже среднего в секторе - меньше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (KnownColors.Yellow, $"P/E выше среднего  в секторе - меньше, чем у 25% компаний сектора");
                else return (KnownColors.Red, $"P/E высокое в секторе");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorPbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, "");

            if (metric.Pbv.HasValue)
            {
                if (metric.Pbv.Value <= 0.0)
                    return (KnownColors.Red, "P/BV отрицательный. Отрицательный P/BV (Price to Book Value) означает, что собственный капитал компании отрицателен. Это свидетельствует о том, что обязательства компании превышают стоимость всех её активов. Это крайне негативный сигнал, указывающий на то, что бизнес работает исключительно за счет заемных средств и имеет долги, превышающие активы");

                if (metric.Pbv.Value < 1.0) return (KnownColors.Green, $"Стоимость компании меньше её собственного капитала");
                if (metric.Pbv.Value >= 1.0) return (KnownColors.Red, $"Стоимость компании превышает её балансовую стоимость");
            }

            return (KnownColors.White, string.Empty);
        }

        private async Task<FundamentalMetric?> GetMetricAsync(string ticker, string period)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();
            var metric = analyseDataContext.GetFundamentalMetrics(ticker);

            return metric.Find(x => x.Period == period);
        }

        private async Task<List<FundamentalMetric>> GetSectorMetricsAsync(string ticker, string period)
        {                        
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var instruments = (await instrumentRepository.GetInstrumentsAsync())!.Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();

            var instrument = instruments.Find(x => x.Ticker == ticker);
            var sectorTickers = instruments.Where(x => x.Sector == instrument!.Sector).Select(x => x.Ticker).ToList();

            var result = new List<FundamentalMetric>();

            foreach (var sectorTicker in sectorTickers)
            {
                var metrics = analyseDataContext.GetFundamentalMetrics(sectorTicker);
                var metric = metrics.Find(x => x.Period == period);

                if (metric is not null)
                    result.Add(metric);
            }

            return result;
        }
    }
}
