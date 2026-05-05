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
        public async Task<(double Ratio, string Color, string Description)> GetColorPeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);
            
            if (metric is null) return (0.0, KnownColors.White, string.Empty);
            
            if (metric.Pe.HasValue)
            {
                if (metric.Pe.Value <= 0.0)
                    return (0.0, KnownColors.Red, "P/E отрицательный. Отрицательный P/E (цена/прибыль) означает, что компания понесла убытки в отчетном периоде, так как чистая прибыль (знаменатель) стала отрицательной. Это сигнал финансовых трудностей, указывающий, что компания не окупается, а теряет акционерный капитал");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Pe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.Pe!.Value >= metric.Pe.Value || x.Pe!.Value < 0.0);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (ratio, KnownColors.Green, $"P/E низкое в секторе - меньше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (ratio, KnownColors.LightGreen, $"P/E ниже среднего в секторе - меньше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (ratio, KnownColors.Yellow, $"P/E выше среднего в секторе - меньше, чем у 25% компаний сектора");
                else return (ratio, KnownColors.Red, $"P/E высокое в секторе");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorPbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (metric.Pbv.HasValue)
            {
                if (metric.Pbv.Value <= 0.0) return (0.0, KnownColors.Red, "P/BV отрицательный. Отрицательный P/BV означает, что собственный капитал компании отрицателен. Это свидетельствует о том, что обязательства компании превышают стоимость всех её активов. Это крайне негативный сигнал, указывающий на то, что бизнес работает исключительно за счет заемных средств и имеет долги, превышающие активы");
                if (metric.Pbv.Value < 1.0) return (1.0, KnownColors.Green, $"Стоимость компании меньше её собственного капитала");
                if (metric.Pbv.Value >= 1.0) return (0.0, KnownColors.Red, $"Стоимость компании превышает её балансовую стоимость");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.Revenue.HasValue)
            {
                if (metric.Revenue.Value <= 0.0) return (KnownColors.Red, "Отрицательная выручка");
                if (metric.Revenue.Value > 0.0) return (KnownColors.Green, "Положительная выручка");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorNetProfitAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return (0.0, KnownColors.White, string.Empty);
            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (prevMetric.NetProfit.HasValue && metric.NetProfit.HasValue)
            {
                if (metric.NetProfit.Value <= 0.0) return (0.0, KnownColors.Red, "Отрицательная чистая прибыль");
                if (prevMetric.NetProfit.Value > 0.0 && metric.NetProfit.Value > 0.0 && metric.NetProfit.Value > prevMetric.NetProfit.Value) return (1.0, KnownColors.Green, "Рост чистой прибыли");
                if (prevMetric.NetProfit.Value > 0.0 && metric.NetProfit.Value > 0.0 && metric.NetProfit.Value <= prevMetric.NetProfit.Value) return (0.75, KnownColors.Yellow, "Падение чистой прибыли");                
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorFcfAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return (0.0, KnownColors.White, string.Empty);
            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (prevMetric.Fcf.HasValue && metric.Fcf.HasValue)
            {
                if (metric.Fcf.Value <= 0.0) return (0.0, KnownColors.Red, "Отрицательный FCF");
                if (prevMetric.Fcf.Value > 0.0 && metric.Fcf.Value > 0.0 && metric.Fcf.Value > prevMetric.Fcf.Value) return (1.0, KnownColors.Green, "Рост FCF");
                if (prevMetric.Fcf.Value > 0.0 && metric.Fcf.Value > 0.0 && metric.Fcf.Value <= prevMetric.Fcf.Value) return (0.75, KnownColors.Yellow, "Падение FCF");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorEpsAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return (0.0, KnownColors.White, string.Empty);
            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (prevMetric.Eps.HasValue && metric.Eps.HasValue)
            {
                if (metric.Eps.Value <= 0.0) return (0.0, KnownColors.Red, "Отрицательная EPS");
                if (prevMetric.Eps.Value > 0.0 && metric.Eps.Value > 0.0 && metric.Eps.Value > prevMetric.Eps.Value) return (1.0, KnownColors.Green, "Рост EPS");
                if (prevMetric.Eps.Value > 0.0 && metric.Eps.Value > 0.0 && metric.Eps.Value <= prevMetric.Eps.Value) return (0.75, KnownColors.Yellow, "Падение EPS");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorNetDebtAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.NetDebt.HasValue)
            {
                if (metric.NetDebt.Value <= 0.0) return (KnownColors.Green, "Отрицательная долг");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorRoaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.Roa.HasValue)
            {
                if (metric.Roa.Value <= 0.0)
                    return (KnownColors.Red, "ROA отрицательный");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roa.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.Roa!.Value <= metric.Roa.Value || x.Roa!.Value < 0.0);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (KnownColors.Green, $"ROA высокое в секторе - выше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (KnownColors.LightGreen, $"ROA выше среднего в секторе - выше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (KnownColors.Yellow, $"ROA ниже среднего в секторе - выше, чем у 25% компаний сектора");
                else return (KnownColors.Red, $"ROA низкое в секторе");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorRoeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.Roe.HasValue)
            {
                if (metric.Roe.Value <= 0.0)
                    return (KnownColors.Red, "ROE отрицательный");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.Roe!.Value <= metric.Roe.Value || x.Roe!.Value < 0.0);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (KnownColors.Green, $"ROE высокое в секторе - выше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (KnownColors.LightGreen, $"ROE выше среднего в секторе - выше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (KnownColors.Yellow, $"ROE ниже среднего в секторе - выше, чем у 25% компаний сектора");
                else return (KnownColors.Red, $"ROE низкое в секторе");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorEvEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (metric.EvEbitda.HasValue)
            {
                if (metric.EvEbitda.Value <= 0.0)
                    return (0.0, KnownColors.Red, "EV/EBITDA отрицательный");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EvEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.EvEbitda!.Value >= metric.EvEbitda.Value || x.EvEbitda!.Value < 0.0);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (ratio, KnownColors.Green, $"EV/EBITDA низкое в секторе - меньше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (ratio, KnownColors.LightGreen, $"EV/EBITDA ниже среднего в секторе - меньше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (ratio, KnownColors.Yellow, $"EV/EBITDA выше среднего в секторе - меньше, чем у 25% компаний сектора");
                else return (ratio, KnownColors.Red, $"EV/EBITDA высокое в секторе");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(double Ratio, string Color, string Description)> GetColorNetDebtEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (0.0, KnownColors.White, string.Empty);

            if (metric.NetDebtEbitda.HasValue)
            {
                if (metric.NetDebtEbitda.Value <= 0.0)
                    return (1.0, KnownColors.Green, "NetDebt/EBITDA отрицательный");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.NetDebtEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.NetDebtEbitda!.Value >= metric.NetDebtEbitda.Value);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (ratio, KnownColors.Green, $"NetDebt/EBITDA низкое в секторе - меньше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (ratio, KnownColors.LightGreen, $"NetDebt/EBITDA ниже среднего в секторе - меньше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (ratio, KnownColors.Yellow, $"NetDebt/EBITDA выше среднего в секторе - меньше, чем у 25% компаний сектора");
                else return (ratio, KnownColors.Red, $"NetDebt/EBITDA высокое в секторе");
            }

            return (0.0, KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorEbitdaRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.EbitdaRevenue.HasValue)
            {
                if (metric.EbitdaRevenue.Value <= 0.0)
                    return (KnownColors.Red, "EBITDA Margin отрицательный");

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EbitdaRevenue.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.EbitdaRevenue!.Value <= metric.EbitdaRevenue.Value || x.EbitdaRevenue!.Value < 0.0);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) return (KnownColors.Green, $"EBITDA Margin высокое в секторе - выше, чем у 75% компаний сектора");
                else if (ratio > 0.5) return (KnownColors.LightGreen, $"EBITDA Margin выше среднего в секторе - выше, чем у 50% компаний сектора");
                else if (ratio > 0.25) return (KnownColors.Yellow, $"EBITDA Margin ниже среднего в секторе - выше, чем у 25% компаний сектора");
                else return (KnownColors.Red, $"EBITDA Margin низкое в секторе");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorDividendYieldAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.DividendYield.HasValue)
            {
                if (metric.DividendYield.Value == 0.0) return (KnownColors.Red, "Дивидендов нет");
                if (metric.DividendYield.Value > 10.0) return (KnownColors.Green, "Дивидендная доходность больше 10 %");
                if (metric.DividendYield.Value > 0.0) return (KnownColors.Yellow, "Дивидендная доходность до 10 %");
            }

            return (KnownColors.White, string.Empty);
        }

        public async Task<(string Color, string Description)> GetColorDeltaMinMaxAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return (KnownColors.White, string.Empty);

            if (metric.DeltaMinMax.HasValue)
            {                
                if (metric.DeltaMinMax.Value < 0.0) return (KnownColors.Red, "Падение цены");
                if (metric.DeltaMinMax.Value > 0.0) return (KnownColors.Green, "Рост цены");
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
