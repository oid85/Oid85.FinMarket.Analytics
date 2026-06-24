using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class FundamentalParameterRatioService(
        IStorageApiClient storageApiClient,
        IInstrumentRepository instrumentRepository,
        IDataService dataService)
        : IFundamentalParameterRatioService
    {
        public async Task<FundamentalParameterRatio> GetRatioMarketCapAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.MarketCap.HasValue)
            {
                if (metric.MarketCap.Value >= 700.0)
                    return new()
                    {
                        Ratio = 1.0,
                        Color = KnownColors.Green,
                        Description = "✅ Большая",
                        Text = $"✅ Компания с большой капитализацией ({metric.MarketCap.Value} млрд. руб)"
                    };

                if (metric.MarketCap.Value >= 150.0)
                    return new()
                    {
                        Ratio = 0.75,
                        Color = KnownColors.LightGreen,
                        Description = "✅ Средняя",
                        Text = $"✅ Компания со средней капитализацией ({metric.MarketCap.Value} млрд. руб)"
                    };

                return new()
                {
                    Ratio = 0.5,
                    Color = KnownColors.Yellow,
                    Description = "⚠️ Малая",
                    Text = $"⚠️ Компания с малой капитализацией ({metric.MarketCap.Value} млрд. руб)"
                };
            }

            return new();
        }

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
                        Description = $"❗ P/E отрицательный",
                        Text = $"❗ P/E отрицательный ({metric.Pe.Value}). Компания понесла убытки в отчетном периоде. Сигнал финансовых трудностей, указывающий, что компания не окупается, а теряет акционерный капитал"
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
                        Description = $"✅ P/E низкое в секторе",
                        Text = $"✅ P/E низкое в секторе ({metric.Pe.Value}) - ниже, чем у 75% компаний сектора"
                    };
                
                else if (ratio > 0.5) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.LightGreen, 
                        Description = $"✅ P/E ниже среднего в секторе",
                        Text = $"✅ P/E ниже среднего в секторе ({metric.Pe.Value}) - ниже, чем у 50% компаний сектора"
                    };
                
                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Yellow, 
                        Description = $"⚠️ P/E выше среднего в секторе",
                        Text = $"⚠️ P/E выше среднего в секторе ({metric.Pe.Value}) - выше, чем у 75% компаний сектора"
                    };
                
                else 
                    return new() 
                    {
                        Ratio = 0.0,
                        Color = KnownColors.Red, 
                        Description = $"⚠️ P/E высокое в секторе",
                        Text = $"⚠️ P/E высокое в секторе ({metric.Pe.Value})"
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
                        Description = $"❗ P/BV отрицательный",
                        Text = $"❗ P/BV отрицательный ({metric.Pbv.Value}). Собственный капитал компании отрицателен. Обязательства компании превышают стоимость всех её активов. Негативный сигнал, бизнес работает за счет заемных средств и имеет долги, превышающие активы"
                    };

                if (metric.Pbv.Value < 1.0)
                    return new()
                    {
                        Ratio = 1.0,
                        Color = KnownColors.Green,
                        Description = $"✅ P/BV меньше единицы",
                        Text = $"✅ P/BV меньше единицы ({metric.Pbv.Value}). Стоимость компании меньше её собственного капитала"
                    };
               
                if (metric.Pbv.Value >= 1.0) 
                    return new() 
                    { 
                        Ratio = 0.0, 
                        Color = KnownColors.Red,
                        Description = $"⚠️ P/BV больше единицы",
                        Text = $"⚠️ P/BV больше единицы ({metric.Pbv.Value}). Стоимость компании превышает её балансовую стоимость"
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
            var prevPrevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 2).ToString());
            var prevMetric = await GetMetricAsync(ticker, (int.Parse(period) - 1).ToString());
            var metric = await GetMetricAsync(ticker, period);

            if (prevPrevMetric is null) return new();
            if (prevMetric is null) return new();
            if (metric is null) return new();

            if (prevPrevMetric.NetProfit.HasValue && prevMetric.NetProfit.HasValue && metric.NetProfit.HasValue)
            {
                double deltaPrc = Math.Abs((metric.NetProfit.Value - prevMetric.NetProfit.Value) / prevMetric.NetProfit.Value * 100.0).RoundTo(2);

                if (metric.NetProfit.Value <= 0.0) 
                    return new() 
                    { 
                        Color = KnownColors.Red, 
                        Description = "❗ Отриц. чистая прибыль",
                        Text = "❗ Отрицательная чистая прибыль. Компания получила убыток"
                    };

                if (prevPrevMetric.NetProfit.Value > 0.0 &&
                    prevMetric.NetProfit.Value > 0.0 &&
                    metric.NetProfit.Value > 0.0 &&
                    metric.NetProfit.Value > prevMetric.NetProfit.Value &&
                    prevMetric.NetProfit.Value > prevPrevMetric.NetProfit.Value)
                    return new()
                    {
                        Ratio = 1.0,
                        Color = KnownColors.Green,
                        Description = "✅ Рост чистой прибыли 2 года подряд",
                        Text = $"✅ Прибыль выросла на {deltaPrc} % по сравнению с предыдущим периодом. Рост прибыли 2 года подряд"
                    };

                if (prevMetric.NetProfit.Value > 0.0 &&
                    metric.NetProfit.Value > 0.0 &&
                    metric.NetProfit.Value > prevMetric.NetProfit.Value) 
                    return new() 
                    { 
                        Ratio = 0.75,
                        Color = KnownColors.LightGreen,
                        Description = "✅ Рост чистой прибыли",
                        Text = $"✅ Прибыль выросла на {deltaPrc} % по сравнению с предыдущим периодом"
                    };

                if (metric.NetProfit.Value < prevMetric.NetProfit.Value &&
                    prevMetric.NetProfit.Value < prevPrevMetric.NetProfit.Value)
                    return new()
                    {
                        Ratio = 0.25,
                        Color = KnownColors.LightRed,
                        Description = "❗ Падение чистой прибыли 2 года подряд",
                        Text = $"❗ Прибыль сократилась на {deltaPrc} % по сравнению с предыдущим периодом. Падение прибыли 2 года подряд"
                    };

                if (metric.NetProfit.Value < prevMetric.NetProfit.Value)
                    return new() 
                    { 
                        Ratio = 0.5, 
                        Color = KnownColors.Yellow, 
                        Description = "⚠️ Падение чистой прибыли",
                        Text = $"⚠️ Прибыль сократилась на {deltaPrc} % по сравнению с предыдущим периодом"
                    };
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
                double deltaPrc = Math.Abs((metric.Fcf.Value - prevMetric.Fcf.Value) / prevMetric.Fcf.Value * 100.0).RoundTo(2);

                if (metric.Fcf.Value <= 0.0)
                    return new() 
                    { 
                        Color = KnownColors.Red, 
                        Description = "❗ Отрицательный FCF",
                        Text = "❗ Отрицательный свободный денежный поток (FCF)"
                    };

                if (metric.Fcf.Value > 0.0 && metric.Fcf.Value > prevMetric.Fcf.Value)
                    return new() 
                    { 
                        Ratio = 1.0, 
                        Color = KnownColors.Green, 
                        Description = "✅ Рост FCF",
                        Text = $"✅ Свободный денежный поток (FCF) вырос на {deltaPrc} % по сравнению с предыдущим периодом"
                    };

                if (metric.Fcf.Value <= prevMetric.Fcf.Value)
                    return new() 
                    { 
                        Ratio = 0.75, 
                        Color = KnownColors.Yellow, 
                        Description = "⚠️ Падение FCF",
                        Text = $"⚠️ Свободный денежный поток (FCF) сократился на {deltaPrc} % по сравнению с предыдущим периодом"
                    };
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
                double deltaPrc = Math.Abs((metric.Eps.Value - prevMetric.Eps.Value) / prevMetric.Eps.Value * 100.0).RoundTo(2);

                if (metric.Eps.Value <= 0.0)
                    return new() 
                    { 
                        Color = KnownColors.Red, 
                        Description = "❗ Отрицательная EPS",
                        Text = "❗ Отрицательная прибыль на акцию (EPS)"
                    };

                if (metric.Eps.Value > 0.0 && metric.Eps.Value > prevMetric.Eps.Value)
                    return new() 
                    {
                        Ratio = 1.0, 
                        Color = KnownColors.Green, 
                        Description = "✅ Рост EPS",
                        Text = $"✅ Прибыль на акцию (EPS) выросла на {deltaPrc} % по сравнению с предыдущим периодом"
                    };

                if (metric.Eps.Value <= prevMetric.Eps.Value)
                    return new() 
                    { 
                        Ratio = 0.75, 
                        Color = KnownColors.Yellow, 
                        Description = "⚠️ Падение EPS",
                        Text = $"⚠️ Прибыль на акцию (EPS) сократилась на {deltaPrc} % по сравнению с предыдущим периодом"
                    };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioNetDebtAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);
            var metricPrev = await GetMetricAsync(ticker, (Convert.ToInt32(period) - 1).ToString());
            var metricPrevPrev = await GetMetricAsync(ticker, (Convert.ToInt32(period) - 2).ToString());

            if (metric is null) return new();

            if (metric.NetDebt.HasValue)
            {
                if (metric.NetDebt.Value <= 0.0) 
                    return new() 
                    { 
                        Ratio = 1.0,
                        Color = KnownColors.Green, 
                        Description = "✅ Отриц. долг",
                        Text = $"✅ У компании отрицательный долг ({metric.NetDebt.Value} млрд. руб.)"
                    };
            }

            if (metricPrev is null) return new();
            if (metricPrevPrev is null) return new();

            if (metric.NetDebt.HasValue && 
                metricPrev.NetDebt.HasValue && 
                metricPrevPrev.NetDebt.HasValue)
            {
                if (metric.NetDebt.Value < metricPrev.NetDebt.Value &&
                    metricPrev.NetDebt.Value < metricPrevPrev.NetDebt.Value)
                    return new()
                    {
                        Ratio = 0.75,
                        Color = KnownColors.Yellow,
                        Description = "⚠️ Долг сокращается 2 года подряд",
                        Text = $"⚠️ Долг сокращается 2 года подряд. Текущее значение {metric.NetDebt.Value} млрд. руб."
                    };

                if (metric.NetDebt.Value > metricPrev.NetDebt.Value &&
                    metricPrev.NetDebt.Value > metricPrevPrev.NetDebt.Value)
                    return new()
                    {
                        Ratio = 0.25,
                        Color = KnownColors.Red,
                        Description = "❗Долг растет 2 года подряд",
                        Text = $"❗ Долг растет 2 года подряд. Текущее значение {metric.NetDebt.Value} млрд. руб."
                    };
            }

            if (metric.NetDebt.HasValue)
                return new()
                {
                    Ratio = 0.5,
                    Color = KnownColors.Yellow,
                    Description = "⚠️ Долг без динамики",
                    Text = $"⚠️ Долг без динамики. Текущее значение {metric.NetDebt.Value} млрд. руб."
                };

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioRoaAsync(string ticker, string period)
        {
            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.Roa.HasValue)
            {
                if (metric.Roa.Value <= 0.0)
                    return new() 
                    { 
                        Color = KnownColors.Red, 
                        Description = "❗ ROA отрицательная",
                        Text = "❗ ROA отрицательная"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roa.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.Roa!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.Roa!.Value > 0.0).Count(x => x.Roa!.Value <= metric.Roa.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Green,
                        Description = $"✅ ROA высокая в секторе",
                        Text = $"✅ ROA высокая в секторе ({metric.Roa.Value} %) - выше, чем у 75% компаний сектора"
                    };
                
                else if (ratio > 0.5) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.LightGreen, 
                        Description = $"✅ ROA выше среднего в секторе",
                        Text = $"✅ ROA выше среднего в секторе ({metric.Roa.Value} %) - выше, чем у 50% компаний сектора"
                    };
                
                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ ROA ниже среднего в секторе",
                        Text = $"⚠️ ROA ниже среднего в секторе ({metric.Roa.Value} %) - ниже, чем у 75% компаний сектора"
                    };
                
                else 
                    return new() 
                    { 
                        Color = KnownColors.Red,
                        Description = $"⚠️ ROA низкая в секторе",
                        Text = $"⚠️ ROA низкая в секторе ({metric.Roa.Value} %)"
                    };
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
                    return new() 
                    { 
                        Color = KnownColors.Red, 
                        Description = "❗ ROE отрицательная",
                        Text = "❗ ROE отрицательная"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.Roe.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.Roe!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.Roe!.Value > 0.0).Count(x => x.Roe!.Value <= metric.Roe.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Green, 
                        Description = $"✅ ROE высокая в секторе",
                        Text = $"✅ ROE высокая в секторе ({metric.Roe.Value} %) - выше, чем у 75% компаний сектора"
                    };
                
                else if (ratio > 0.5) 
                    return new() 
                    { 
                        Ratio = ratio,
                        Color = KnownColors.LightGreen,
                        Description = $"✅ ROE выше среднего в секторе",
                        Text = $"✅ ROE выше среднего в секторе ({metric.Roe.Value} %) - выше, чем у 50% компаний сектора"
                    };
                
                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Yellow, 
                        Description = $"⚠️ ROE ниже среднего в секторе",
                        Text = $"⚠️ ROE ниже среднего в секторе ({metric.Roe.Value} %) - ниже, чем у 75% компаний сектора"
                    };
                
                else 
                    return new() 
                    {
                        Color = KnownColors.Red,
                        Description = $"⚠️ ROE низкая в секторе",
                        Text = $"⚠️ ROE низкая в секторе ({metric.Roe.Value} %)"
                    };
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
                    return new() 
                    {
                        Color = KnownColors.Red,
                        Description = "❗ EV/EBITDA отрицательное",
                        Text = "❗ EV/EBITDA отрицательное"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EvEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.EvEbitda!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.EvEbitda!.Value > 0.0).Count(x => x.EvEbitda!.Value >= metric.EvEbitda.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Green, 
                        Description = $"✅ EV/EBITDA низкое в секторе",
                        Text = $"✅ EV/EBITDA низкое в секторе ({metric.EvEbitda.Value}) - ниже, чем у 75% компаний сектора"
                    };
                
                else if (ratio > 0.5) 
                    return new() 
                    { 
                        Ratio = ratio,
                        Color = KnownColors.LightGreen,
                        Description = $"✅ EV/EBITDA ниже среднего в секторе",
                        Text = $"✅ EV/EBITDA ниже среднего в секторе ({metric.EvEbitda.Value}) - ниже, чем у 50% компаний сектора"
                    };
                
                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio,
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ EV/EBITDA выше среднего в секторе",
                        Text = $"⚠️ EV/EBITDA выше среднего в секторе ({metric.EvEbitda.Value}) - выше, чем у 75% компаний сектора"
                    };
                
                else 
                    return new() 
                    { 
                        Ratio = ratio,
                        Color = KnownColors.Red,
                        Description = $"⚠️ EV/EBITDA высокое в секторе",
                        Text = $"⚠️ EV/EBITDA высокое в секторе ({metric.EvEbitda.Value})"
                    };
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
                    return new() 
                    { 
                        Ratio = 1.0, 
                        Color = KnownColors.Green,
                        Description = "✅ ND/EBITDA отрицательное",
                        Text = "✅ ND/EBITDA отрицательное"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.NetDebtEbitda.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int predicatCount = sectorMetricValues.Count(x => x.NetDebtEbitda!.Value >= metric.NetDebtEbitda.Value);

                double ratio = Convert.ToDouble(predicatCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    { 
                        Ratio = ratio,
                        Color = KnownColors.Green,
                        Description = $"✅ ND/EBITDA низкое в секторе",
                        Text = $"✅ ND/EBITDA низкое в секторе ({metric.NetDebtEbitda.Value}) - ниже, чем у 75% компаний сектора"
                    };

                else if (ratio > 0.5) 
                    return new()
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.LightGreen,
                        Description = $"✅ ND/EBITDA ниже среднего в секторе",
                        Text = $"✅ ND/EBITDA ниже среднего в секторе ({metric.NetDebtEbitda.Value}) - ниже, чем у 50% компаний сектора"
                    };

                else if (ratio > 0.25) 
                    return new() 
                    { 
                        Ratio = ratio, 
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ ND/EBITDA выше среднего в секторе",
                        Text = $"⚠️ ND/EBITDA выше среднего в секторе ({metric.NetDebtEbitda.Value}) - выше, чем у 75% компаний сектора"
                    };

                else 
                    return new() 
                    {
                        Ratio = ratio, 
                        Color = KnownColors.Red,
                        Description = $"⚠️ ND/EBITDA высокое в секторе",
                        Text = $"⚠️ ND/EBITDA высокое в секторе ({metric.NetDebtEbitda.Value})"
                    };
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
                    return new() 
                    { 
                        Ratio = 0.0, 
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ По Debt Ratio высок. долг. нагр.",
                        Text = $"⚠️ По DebtRatio ({metric.DebtRatio.Value}) высокая долговая нагрузка. Обязательства компании больше 70 % от активов"
                    };
                
                else 
                    return new() 
                    { 
                        Ratio = 1.0,
                        Color = KnownColors.Green, 
                        Description = $"✅ По Debt Ratio умерен. долг. нагр.",
                        Text = $"✅ По DebtRatio ({metric.DebtRatio.Value}) умеренная долговая нагрузка. Обязательства компании меньше 70 % от активов"
                    };
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
                    return new() 
                    { 
                        Ratio = 0.0,
                        Color = KnownColors.Yellow, 
                        Description = $"⚠️ По Debt Equity высок. долг. нагр.",
                        Text = $"⚠️ По DebtEquity ({metric.DebtEquity.Value}) высокая долговая нагрузка. Обязательства компании в 2 и более раза больше собственного капитала"
                    };

                else
                    return new() 
                    { 
                        Ratio = 1.0, 
                        Color = KnownColors.Green,
                        Description = $"✅ По Debt Equity умерен. долг. нагр.",
                        Text = $"✅ По DebtEquity ({metric.DebtEquity.Value}) умеренная долговая нагрузка. Отношение обязательств компании к собственному капиталу менее 2"
                    };
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
                    return new() 
                    { 
                        Ratio = 0.0, 
                        Color = KnownColors.Red,
                        Description = "❗ EBITDA Margin отриц.",
                        Text = "❗ EBITDA Margin отрицательная"
                    };

                var sectorMetrics = await GetSectorMetricsAsync(ticker, period);
                var sectorMetricValues = sectorMetrics.Where(x => x.EbitdaRevenue.HasValue).ToList();

                int totalCount = sectorMetricValues.Count;
                int badCount = sectorMetricValues.Where(x => x.EbitdaRevenue!.Value <= 0.0).Count();
                int predicatCount = sectorMetricValues.Where(x => x.EbitdaRevenue!.Value > 0.0).Count(x => x.EbitdaRevenue!.Value <= metric.EbitdaRevenue.Value);

                double ratio = Convert.ToDouble(predicatCount + badCount) / Convert.ToDouble(totalCount);

                if (ratio > 0.75) 
                    return new() 
                    {
                        Ratio = ratio,
                        Color = KnownColors.Green,
                        Description = $"✅ EBITDA Margin высокая в секторе",
                        Text = $"✅ EBITDA Margin высокая в секторе ({metric.EbitdaRevenue.Value}) - выше, чем у 75% компаний сектора"
                    };

                else if (ratio > 0.5) 
                    return new() 
                    {
                        Ratio = ratio,
                        Color = KnownColors.LightGreen,
                        Description = $"✅ EBITDA Margin выше средн. в секторе",
                        Text = $"✅ EBITDA Margin выше среднего в секторе ({metric.EbitdaRevenue.Value}) - выше, чем у 50% компаний сектора"
                    };

                else if (ratio > 0.25) 
                    return new() 
                    {
                        Ratio = ratio,
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ EBITDA Margin ниже средн. в секторе",
                        Text = $"⚠️ EBITDA Margin ниже среднего в секторе ({metric.EbitdaRevenue.Value}) - ниже, чем у 75% компаний сектора"
                    };

                else 
                    return new() 
                    {
                        Ratio = ratio,
                        Color = KnownColors.Red,
                        Description = $"EBITDA Margin низкая в секторе",
                        Text = $"⚠️ EBITDA Margin низкая в секторе ({metric.EbitdaRevenue.Value})"
                    };
            }

            return new();
        }

        public async Task<FundamentalParameterRatio> GetRatioDividendYieldAsync(string ticker, string period)
        {
            var currentKeyRate = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).Where(x => x.Value.HasValue).Last().Value!.Value;

            double dividendYieldLimit = currentKeyRate * 2.0 / 3.0;

            var metric = await GetMetricAsync(ticker, period);

            if (metric is null) return new();

            if (metric.DividendYield.HasValue)
            {
                if (metric.DividendYield.Value == 0.0) 
                    return new()
                    { 
                        Ratio = 0.0,
                        Color = KnownColors.Red,
                        Description = "❗ Дивидендов нет",
                        Text = "❗ Дивидендов нет"
                    };

                if (metric.DividendYield.Value > dividendYieldLimit)
                    return new()
                    {
                        Ratio = 1.0,
                        Color = KnownColors.Green,
                        Description = $"✅ ДД больше 2/3 от ключевой ставки",
                        Text = $"✅ Дивидендная доходность ({metric.DividendYield.Value} %) - больше 2/3 от ключевой ставки ({currentKeyRate.RoundTo(2)} %)"
                    };

                if (metric.DividendYield.Value > 0.0) 
                    return new()
                    {
                        Ratio = 0.5,
                        Color = KnownColors.Yellow,
                        Description = $"⚠️ ДД менее 2/3 от ключевой ставки",
                        Text = $"⚠️ Дивидендная доходность ({metric.DividendYield.Value} %) - менее 2/3 от ключевой ставки ({currentKeyRate.RoundTo(2)} %)"
                    };
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
                    return new() 
                    { 
                        Color = KnownColors.Red,
                        Description = "Падение цены" 
                    };

                if (metric.DeltaMinMax.Value > 0.0) 
                    return new() 
                    {
                        Color = KnownColors.Green,
                        Description = "Рост цены"
                    };
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
