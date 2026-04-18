namespace Oid85.FinMarket.Analytics.Core.Models
{
    /// <summary>
    /// Контекст данных
    /// </summary>
    public class AnalyseDataContext
    {
        /// <summary>
        /// Свечи
        /// </summary>
        public Dictionary<string, List<Candle>> CandleData { get; set; } = [];

        /// <summary>
        /// Сглаженные цены
        /// </summary>
        public Dictionary<string, List<DateValue<double>>> UltimateSmootherData { get; set; } = [];

        /// <summary>
        /// Цены закрытия
        /// </summary>
        public Dictionary<string, List<DateValue<double>>> ClosePriceData { get; set; } = [];

        /// <summary>
        /// Дивиденды (последний в БД)
        /// </summary>
        public Dictionary<string, Dividend> DividendData { get; set; } = [];

        /// <summary>
        /// Изменение относительно индекса полной доходности MCFTR
        /// </summary>
        public Dictionary<string, double> BenchmarkChangeData { get; set; } = [];

        /// <summary>
        /// Рейтинг на основе данных фундаментального анализа
        /// </summary>
        public Dictionary<string, FundamentalScore> FundamentalScoreData { get; set; } = [];

        /// <summary>
        /// Фундаментальные данные
        /// </summary>
        public Dictionary<string, List<FundamentalMetric>> FundamentalMetricData { get; set; } = [];

        /// <summary>
        /// Консенсус прогнозы
        /// </summary>
        public Dictionary<string, Forecast> ConsensusForecastData { get; set; } = [];

        /// <summary>
        /// Консенсус прогнозы (NataliaBaffetovna)
        /// </summary>
        public Dictionary<string, Forecast> NataliaBaffetovnaForecastData { get; set; } = [];

        /// <summary>
        /// Консенсус прогнозы (FinanceMarker)
        /// </summary>
        public Dictionary<string, Forecast> FinanceMarkerForecastData { get; set; } = [];

        /// <summary>
        /// Консенсус прогнозы (VladProDengi)
        /// </summary>
        public Dictionary<string, Forecast> VladProDengiForecastData { get; set; } = [];

        /// <summary>
        /// Купоны (для облигаций)
        /// </summary>
        public Dictionary<string, List<BondCoupon>> BondCouponData { get; set; } = [];

        /// <summary>
        /// Доп. данные фундаментального анализа
        /// </summary>
        public Dictionary<string, (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)> ExtData { get; set; } = [];

        /// <summary>
        /// Получить последнюю цену инструмента по тикеру
        /// </summary>
        public double? GetPrice(string ticker)
        {
            if (!CandleData.TryGetValue(ticker, out List<Candle>? candles)) return null;
            if (candles is null) return null;
            if (candles is []) return null;

            return Math.Round(candles[^1].Close, 4);
        }

        /// <summary>
        /// Получить последние актуальные фундаментальные данные по тикеру
        /// </summary>
        public FundamentalMetric? GetFundamentalMetric(string ticker)
        {
            if (!FundamentalMetricData.TryGetValue(ticker, out List<FundamentalMetric>? metrics)) return null;
            if (metrics is null) return null;
            if (metrics is []) return null;

            var metric = new FundamentalMetric
            {
                Period = metrics[^1].Period,
                Pe = metrics.FindLast(x => x.Pe.HasValue)?.Pe,
                Pbv = metrics.FindLast(x => x.Pbv.HasValue)?.Pbv,
                Roa = metrics.FindLast(x => x.Roa.HasValue)?.Roa,
                EvEbitda = metrics.FindLast(x => x.EvEbitda.HasValue)?.EvEbitda,
                NetDebtEbitda = metrics.FindLast(x => x.NetDebtEbitda.HasValue)?.NetDebtEbitda,
                Dividend = metrics.FindLast(x => x.Dividend.HasValue)?.Dividend,
                NetProfit = metrics.FindLast(x => x.NetProfit.HasValue)?.NetProfit
            };

            return metric;
        }

        /// <summary>
        /// Получить свечи по тикеру
        /// </summary>
        public List<Candle> GetCandles(string ticker) => !CandleData.TryGetValue(ticker, out var result) ? [] : result;

        /// <summary>
        /// Получить сглаженные цены по тикеру
        /// </summary>
        public List<DateValue<double>> GetUltimateSmoothers(string ticker) => !UltimateSmootherData.TryGetValue(ticker, out var result) ? [] : result;

        /// <summary>
        /// Получить цены по тикеру
        /// </summary>
        public List<DateValue<double>> GetClosePrices(string ticker) => !ClosePriceData.TryGetValue(ticker, out var result) ? [] : result;

        /// <summary>
        /// Получить дивиденды по тикеру
        /// </summary>
        public Dividend? GetDividend(string ticker) => !DividendData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить изменение относительно индекса полной доходности MCFTR по тикеру
        /// </summary>
        public double? GetBenchmarkChange(string ticker) => !BenchmarkChangeData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить рейтинг на основе данных фундаментального анализа по тикеру
        /// </summary>
        public FundamentalScore? GetFundamentalScore(string ticker) => !FundamentalScoreData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить фундаментальные данные по тикеру
        /// </summary>
        public List<FundamentalMetric> GetFundamentalMetrics(string ticker) => !FundamentalMetricData.TryGetValue(ticker, out var result) ? [] : result;

        /// <summary>
        /// Получить консенсус прогноз по тикеру
        /// </summary>
        public Forecast? GetConsensusForecast(string ticker) => !ConsensusForecastData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить консенсус прогноз по тикеру (NataliaBaffetovna)
        /// </summary>
        public Forecast? GetNataliaBaffetovnaForecast(string ticker) => !NataliaBaffetovnaForecastData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить консенсус прогноз по тикеру (FinanceMarker)
        /// </summary>
        public Forecast? GetFinanceMarkerForecast(string ticker) => !FinanceMarkerForecastData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить консенсус прогноз по тикеру (VladProDengi)
        /// </summary>
        public Forecast? GetVladProDengiForecast(string ticker) => !VladProDengiForecastData.TryGetValue(ticker, out var result) ? null : result;

        /// <summary>
        /// Получить купоны по тикеру (для облигаций)
        /// </summary>
        public List<BondCoupon> GetBondCoupons(string ticker) => !BondCouponData.TryGetValue(ticker, out var result) ? [] : result;

        /// <summary>
        /// Получить доп. данные фундаментального анализа по тикеру
        /// </summary>
        public (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)? GetExtData(string ticker) => !ExtData.TryGetValue(ticker, out var result) ? null : result;
    }
}
