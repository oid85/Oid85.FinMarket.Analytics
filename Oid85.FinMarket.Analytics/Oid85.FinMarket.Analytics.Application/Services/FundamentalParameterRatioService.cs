using System.Drawing;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class FundamentalParameterRatioService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService)
        : IFundamentalParameterRatioService
    {
        public async Task<FundamentalParameterRatio> GetRatioPeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Pe.HasValue)
            {
                if (metric.Pe.Value <= 0.0)
                    return new() 
                    {
                        Ratio = 0.0,
                        Color = KnownColors.Red,
                        Description = $"P/E отрицательный ({metric.Pe.Value})",
                        Text = $"P/E отрицательный ({metric.Pe.Value}). Компания понесла убытки в отчетном периоде. Сигнал финансовых трудностей, указывающий, что компания не окупается, а теряет акционерный капитал"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Pe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.Pe!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.Pe!.Value > 0.0).Count(x => x.Pe!.Value >= metric.Pe.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Green, 
                        Description = $"P/E низкое в секторе ({metric.Pe.Value})а",
                        Text = $"P/E низкое в секторе ({metric.Pe.Value}) - меньше, чем у 75% компаний сектора"
                    };
                
                else if (ratio > 0.5) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.LightGreen, 
                        Description = $"P/E ниже среднего в секторе ({metric.Pe.Value})",
                        Text = $"P/E ниже среднего в секторе ({metric.Pe.Value}) - меньше, чем у 50% компаний сектора"
                    };
                
                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Yellow, 
                        Description = $"P/E выше среднего в секторе ({metric.Pe.Value})",
                        Text = $"P/E выше среднего в секторе ({metric.Pe.Value}) - меньше, чем у 25% компаний сектора"
                    };
                
                else 
                    return new() 
                    {
                        Ratio = 0.0,
                        Color = KnownColors.Red, 
                        Description = $"P/E высокое в секторе ({metric.Pe.Value})",
                        Text = $"P/E высокое в секторе ({metric.Pe.Value})"
                    };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioPbvAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Pbv.HasValue)
            {
                if (metric.Pbv.Value <= 0.0) 
                    return new() 
                    { 
                        Ratio = 0.0, 
                        Color = KnownColors.Red,
                        Description = $"P/BV отрицательный ({metric.Pbv.Value})",
                        Text = $"P/BV отрицательный ({metric.Pbv.Value}). Собственный капитал компании отрицателен. Обязательства компании превышают стоимость всех её активов. Негативный сигнал, бизнес работает за счет заемных средств и имеет долги, превышающие активы"
                    };

                if (metric.Pbv.Value < 1.0)
                    return new()
                    {
                        Ratio = 1.0,
                        Color = KnownColors.Green,
                        Description = $"P/BV меньше 1.0 ({metric.Pbv.Value})",
                        Text = $"P/BV меньше 1.0 ({metric.Pbv.Value}). Стоимость компании меньше её собственного капитала"
                    };
               
                if (metric.Pbv.Value >= 1.0) 
                    return new() 
                    { 
                        Ratio = 0.0, 
                        Color = KnownColors.Red,
                        Description = $"P/BV больше 1.0 ({metric.Pbv.Value})",
                        Text = $"P/BV больше 1.0 ({metric.Pbv.Value}). Стоимость компании превышает её балансовую стоимость"
                    };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Revenue.HasValue)
            {
                if (metric.Revenue.Value <= 0.0) 
                    return new() { Color = KnownColors.Red, Description = "Отриц. выручка" };

                if (metric.Revenue.Value > 0.0) 
                    return new() { Color = KnownColors.Green, Description = "Полож. выручка" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioNetProfitAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return new();
            if (metric is null) return new();

            if (prevMetric.NetProfit.HasValue && metric.NetProfit.HasValue)
            {
                if (metric.NetProfit.Value <= 0.0) 
                    return new() { Color = KnownColors.Red, Description = "Отриц. чистая прибыль" };
                
                if (metric.NetProfit.Value > 0.0 && metric.NetProfit.Value > prevMetric.NetProfit.Value) 
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = "Рост чистой прибыли" };
                
                if (metric.NetProfit.Value <= prevMetric.NetProfit.Value)
                    return new() { Ratio = 0.75, Color = KnownColors.Yellow, Description = "Падение чистой прибыли" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioFcfAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return new();
            if (metric is null) return new();

            if (prevMetric.Fcf.HasValue && metric.Fcf.HasValue)
            {
                if (metric.Fcf.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "Отриц. FCF" };

                if (metric.Fcf.Value > 0.0 && metric.Fcf.Value > prevMetric.Fcf.Value)
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = "Рост FCF" };

                if (metric.Fcf.Value <= prevMetric.Fcf.Value)
                    return new() { Ratio = 0.75, Color = KnownColors.Yellow, Description = "Падение FCF" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetColorEpsAsync(string ticker, string period)
        {
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevMetric is null) return new();
            if (metric is null) return new();

            if (prevMetric.Eps.HasValue && metric.Eps.HasValue)
            {
                if (metric.Eps.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "Отриц. EPS" };

                if (metric.Eps.Value > 0.0 && metric.Eps.Value > prevMetric.Eps.Value)
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = "Рост EPS" };

                if (metric.Eps.Value <= prevMetric.Eps.Value)
                    return new() { Ratio = 0.75, Color = KnownColors.Yellow, Description = "Падение EPS" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioNetDebtAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.NetDebt.HasValue)
            {
                if (metric.NetDebt.Value <= 0.0) return new() { Color = KnownColors.Green, Description = "Отриц. долг" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioRoaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Roa.HasValue)
            {
                if (metric.Roa.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "ROA отриц." };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roa.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.Roa!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.Roa!.Value > 0.0).Count(x => x.Roa!.Value <= metric.Roa.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() { Ratio = ratio, Color = KnownColors.Green, Description = $"ROA высокое в секторе - выше, чем у 75% компаний сектора" };
                
                else if (ratio > 0.5) 
                    return new() { Ratio = ratio, Color = KnownColors.LightGreen, Description = $"ROA выше среднего в секторе - выше, чем у 50% компаний сектора" };
                
                else if (ratio > 0.25) 
                    return new() { Ratio = ratio, Color = KnownColors.Yellow, Description = $"ROA ниже среднего в секторе - выше, чем у 25% компаний сектора" };
                
                else 
                    return new() { Color = KnownColors.Red, Description = $"ROA низкое в секторе" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioRoeAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Roe.HasValue)
            {
                if (metric.Roe.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "ROE отриц." };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.Roe!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.Roe!.Value > 0.0).Count(x => x.Roe!.Value <= metric.Roe.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() { Ratio = ratio, Color = KnownColors.Green, Description = $"ROE высокое в секторе - выше, чем у 75% компаний сектора" };
                
                else if (ratio > 0.5) 
                    return new() { Ratio = ratio, Color = KnownColors.LightGreen, Description = $"ROE выше среднего в секторе - выше, чем у 50% компаний сектора" };
                
                else if (ratio > 0.25) 
                    return new() { Ratio = ratio, Color = KnownColors.Yellow, Description = $"ROE ниже среднего в секторе - выше, чем у 25% компаний сектора" };
                
                else 
                    return new() { Color = KnownColors.Red, Description = $"ROE низкое в секторе" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioEvEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.EvEbitda.HasValue)
            {
                if (metric.EvEbitda.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "EV/EBITDA отриц." };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EvEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.EvEbitda!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.EvEbitda!.Value > 0.0).Count(x => x.EvEbitda!.Value >= metric.EvEbitda.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() { Ratio = ratio, Color = KnownColors.Green, Description = $"EV/EBITDA низкое в секторе - меньше, чем у 75% компаний сектора" };
                
                else if (ratio > 0.5) 
                    return new() { Ratio = ratio, Color = KnownColors.LightGreen, Description = $"EV/EBITDA ниже среднего в секторе - меньше, чем у 50% компаний сектора" };
                
                else if (ratio > 0.25) 
                    return new() { Ratio = ratio, Color = KnownColors.Yellow, Description = $"EV/EBITDA выше среднего в секторе - меньше, чем у 25% компаний сектора" };
                
                else 
                    return new() { Ratio = ratio, Color = KnownColors.Red, Description = $"EV/EBITDA высокое в секторе" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioNetDebtEbitdaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.NetDebtEbitda.HasValue)
            {
                if (metric.NetDebtEbitda.Value <= 0.0)
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = "NetDebt/EBITDA отриц." };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.NetDebtEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.NetDebtEbitda!.Value >= metric.NetDebtEbitda.Value);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() { Ratio = ratio, Color = KnownColors.Green, Description = $"NetDebt/EBITDA низкое в секторе - меньше, чем у 75% компаний сектора" };

                else if (ratio > 0.5) 
                    return new() { Ratio = ratio, Color = KnownColors.LightGreen, Description = $"NetDebt/EBITDA ниже среднего в секторе - меньше, чем у 50% компаний сектора" };

                else if (ratio > 0.25) 
                    return new() { Ratio = ratio, Color = KnownColors.Yellow, Description = $"NetDebt/EBITDA выше среднего в секторе - меньше, чем у 25% компаний сектора" };

                else 
                    return new() { Ratio = ratio, Color = KnownColors.Red, Description = $"NetDebt/EBITDA высокое в секторе" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioDebtRatioAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.DebtRatio.HasValue)
            {
                if (metric.DebtRatio.Value >= 0.7) 
                    return new() { Ratio = 0.0, Color = KnownColors.Yellow, Description = $"Высокая долговая нагрузка" };
                
                else 
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = $"Нормальная долговая нагрузка" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioDebtEquityAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.DebtEquity.HasValue)
            {
                if (metric.DebtEquity.Value >= 2.0)
                    return new() { Ratio = 0.0, Color = KnownColors.Yellow, Description = $"Высокая долговая нагрузка" };

                else
                    return new() { Ratio = 1.0, Color = KnownColors.Green, Description = $"Нормальная долговая нагрузка" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioEbitdaRevenueAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.EbitdaRevenue.HasValue)
            {
                if (metric.EbitdaRevenue.Value <= 0.0)
                    return new() { Color = KnownColors.Red, Description = "EBITDA Margin отриц." };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EbitdaRevenue.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.EbitdaRevenue!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.EbitdaRevenue!.Value > 0.0).Count(x => x.EbitdaRevenue!.Value <= metric.EbitdaRevenue.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() { Color = KnownColors.Green, Description = $"EBITDA Margin высокое в секторе - выше, чем у 75% компаний сектора" };

                else if (ratio > 0.5) 
                    return new() { Color = KnownColors.LightGreen, Description = $"EBITDA Margin выше среднего в секторе - выше, чем у 50% компаний сектора" };

                else if (ratio > 0.25) 
                    return new() { Color = KnownColors.Yellow, Description = $"EBITDA Margin ниже среднего в секторе - выше, чем у 25% компаний сектора" };

                else 
                    return new() { Color = KnownColors.Red, Description = $"EBITDA Margin низкое в секторе" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioDividendYieldAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.DividendYield.HasValue)
            {
                if (metric.DividendYield.Value == 0.0) 
                    return new() { Color = KnownColors.Red, Description = "Дивидендов нет" };

                if (metric.DividendYield.Value > 10.0) 
                    return new() { Color = KnownColors.Green, Description = "Дивидендная доходность больше 10 %" };

                if (metric.DividendYield.Value > 0.0) 
                    return new() { Color = KnownColors.Yellow, Description = "Дивидендная доходность до 10 %" };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioDeltaMinMaxAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.DeltaMinMax.HasValue)
            {
                if (metric.DeltaMinMax.Value < 0.0) 
                    return new() { Color = KnownColors.Red, Description = "Падение цены" };

                if (metric.DeltaMinMax.Value > 0.0) 
                    return new() { Color = KnownColors.Green, Description = "Рост цены" };
            }

            return new();
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
