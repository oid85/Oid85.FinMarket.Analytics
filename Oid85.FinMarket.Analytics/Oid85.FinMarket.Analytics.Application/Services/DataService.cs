using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class DataService(
        IParameterRepository parameterRepository,
        IInstrumentRepository instrumentRepository,
        IStorageApiClient storageApiClient)
        : IDataService
    {
        private Dictionary<string, List<Candle>>? _candleData = null;
        private List<FundamentalParameterListItem>? _fundamentalParameters = null;
        private AnalyseDataContext? _analyseDataContext = null;

        /// <inheritdoc />
        public async Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers)
        {
            if (_candleData is not null) return _candleData;

            _candleData = [];

            foreach (var ticker in tickers)
            {
                var candles = await GetCandlesByTickerAsync(ticker);
                _candleData.Add(ticker, candles);
            }

            return _candleData;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<DateValue<double>>>> GetUltimateSmootherDataAsync(List<string> tickers)
        {
            var candleData = await GetCandleDataAsync(tickers);
            var ultimateSmootherData = GetUltimateSmootherData(candleData);

            return ultimateSmootherData;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<DateValue<double>>>> GetClosePriceDataAsync(List<string> tickers)
        {
            var years = int.Parse((await parameterRepository.GetParameterValueAsync(KnownParameters.DiagramPeriodLongYears)) ?? "5");
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * years));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var candleData = await GetCandleDataAsync(tickers);

            var data = new Dictionary<string, List<DateValue<double>>>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var candles = candleData[ticker].Where(x => x.Date >= from && x.Date <= to);
                var dateValues = candles.Select(x => new DateValue<double> { Date = x.Date, Value = x.Close }).ToList();

                data.Add(ticker, dateValues);
            }

            return data;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, double>> GetBenchmarkChangeDataAsync(List<string> tickers)
        {
            const int lastDaysCount = 90; // Считаем изменение к бенчмарку за последние 90 дней
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * lastDaysCount));
            var today = DateOnly.FromDateTime(DateTime.Today);
            var ultimateSmootherData = await GetUltimateSmootherDataAsync(tickers);

            var benchmarkIncrement = GetIncrement(KnownIndexTickers.MCFTR);

            var result = new Dictionary<string, double>();

            foreach (var ticker in tickers)
                result.Add(ticker, Math.Round(GetIncrement(ticker) - benchmarkIncrement, 2));

            return result;

            // Изменение меджу первым и последним значением в процентах (приращение)
            double GetIncrement(string ticker)
            {
                if (ultimateSmootherData.TryGetValue(ticker, out List<DateValue<double>>? dateValues))
                {
                    var filteredDateValues = dateValues.Where(x => x.Date >= startDate && x.Date <= today).ToList();

                    if (filteredDateValues.Count == 0)
                        return 0.0;

                    var result = (filteredDateValues.Last().Value - filteredDateValues.First().Value) / filteredDateValues.First().Value * 100.0;

                    return Math.Round(result, 2);
                }

                return 0.0;
            }
        }

        public async Task<Dictionary<string, double?>> GetMoexIndexShareDataAsync(List<string> tickers)
        {
            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var result = new Dictionary<string, double?>();

            foreach (var ticker in tickers)
            {
                var moexIndexShare = fundamentalParameterList.Find(x => x.Ticker == ticker && x.Type == KnownFundamentalParameterTypes.Moex)?.Value;
                result.Add(ticker, moexIndexShare);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<FundamentalMetric>>> GetFundamentalMetricDataAsync(List<string> tickers)
        {
            var candleData = await GetCandleDataAsync(tickers);

            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];

            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var result = new Dictionary<string, List<FundamentalMetric>>();

            foreach (var ticker in tickers)
            {
                var fundamentalParametersByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                var candles = !candleData.TryGetValue(ticker, out var tickerCandles) ? [] : tickerCandles;

                var metrics = new List<FundamentalMetric>();

                foreach (var period in periods)
                {
                    var periodCandles = candles?.Where(x => x.Date.Year == int.Parse(period))?.ToList() ?? [];

                    var price = periodCandles?.LastOrDefault()?.Close.RoundTo(4);

                    var fundamentalParametersByPeriod = fundamentalParametersByTicker.Where(x => x.Period == period).ToList();

                    var numberShares = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NumberShares)?.Value.RoundTo(2);
                    var pe = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Pe)?.Value.RoundTo(2);
                    var pbv = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Pbv)?.Value.RoundTo(2);
                    var roa = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Roa)?.Value.RoundTo(2);
                    var roe = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Roe)?.Value.RoundTo(2);
                    var fcf = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Fcf)?.Value.RoundTo(2);
                    var eps = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Eps)?.Value.RoundTo(2);
                    var dividend = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Dividend)?.Value.RoundTo(4);
                    var netProfit = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NetProfit)?.Value.RoundTo(2);
                    var revenue = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Revenue)?.Value.RoundTo(2);
                    var marketCap = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.MarketCap)?.Value.RoundTo(2);
                    var ev = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Ev)?.Value.RoundTo(2);
                    var ebitda = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Ebitda)?.Value.RoundTo(2);
                    var ownCapital = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.OwnCapital)?.Value.RoundTo(2);
                    var netDebt = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NetDebt)?.Value.RoundTo(2);
                    var assets = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Assets)?.Value.RoundTo(2);
                    var liabilities = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Liabilities)?.Value.RoundTo(2);

                    var evEbitda = ev.Div(ebitda).RoundTo(2);
                    var ebitdaRevenue = ebitda.Div(revenue).RoundTo(2);
                    var ownCapitalnumberShares = ownCapital.Mult(1_000).Div(numberShares).RoundTo(2);
                    var debtRatio = liabilities.Div(assets).RoundTo(2);
                    var debtEquity = liabilities.Div(ownCapital).RoundTo(2);
                    var netDebtEbitda = netDebt.Div(ebitda).RoundTo(2);
                    var dividendYield = dividend.Div(price).Mult(100.0).RoundTo(2);

                    var deltaMinMax = GetDeltaMinMax(periodCandles);

                    metrics.Add(
                        new()
                        {
                            Ticker = ticker,
                            Price = price,
                            Period = period,
                            NumberShares = numberShares,
                            Pe = pe,
                            Pbv = pbv,
                            Roa = roa,
                            Roe = roe,
                            Ev = ev,
                            MarketCap = marketCap,
                            Ebitda = ebitda,
                            OwnCapital = ownCapital,
                            NetDebt = netDebt,
                            Assets = assets,
                            Liabilities = liabilities,
                            Revenue = revenue,
                            Fcf = fcf,
                            Eps = eps,
                            Dividend = dividend,
                            DividendYield = dividendYield,
                            NetProfit = netProfit,
                            EvEbitda = evEbitda,
                            NetDebtEbitda = netDebtEbitda,
                            DebtRatio = debtRatio,
                            DebtEquity = debtEquity,
                            EbitdaRevenue = ebitdaRevenue,
                            OwnCapitalNumberShares = ownCapitalnumberShares,
                            DeltaMinMax = deltaMinMax
                        });

                    static double? GetDeltaMinMax(List<Candle>? candles)
                    {
                        if (candles is null) return null;

                        var maxCandle = candles.MaxBy(x => x.Close);
                        var minCandle = candles.MinBy(x => x.Close);

                        if (maxCandle is null || minCandle is null) return null;

                        double max = maxCandle.Close;
                        double min = minCandle.Close;

                        return maxCandle.Date < minCandle.Date
                            ? -1 * Math.Abs(((max - min) / max * 100.0).RoundTo(2)) // Падение от максимума
                            : Math.Abs(((max - min) / min * 100.0).RoundTo(2));     // Рост от минимума
                    }
                }

                result.Add(ticker, metrics);
            }

            return result;
        }

        public async Task<Dictionary<string, bool>> GetFillFundamentalDataAsync(List<string> tickers)
        {
            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            int year = DateTime.Today.Year; 
            int month = DateTime.Today.Month;
            int kv = 0;

            if (month == 1 || month == 3 || month == 3) { kv = 0; year--; }
            if (month == 4 || month == 5 || month == 6) kv = 1;
            if (month == 7 || month == 8 || month == 9) kv = 2;
            if (month == 10 || month == 11 || month == 12) kv = 3;

            var reportData = (year + kv / 10.0).RoundTo(1);

            var result = new Dictionary<string, bool>();

            foreach (var ticker in tickers)
            {
                var fundamentalParametersByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                var report = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.Report)?.Value;

                if (!report.HasValue)
                    result.Add(ticker, false);

                else
                    result.Add(ticker, report.Value == reportData);
            }

            return result;
        }

        public async Task<Dictionary<string, string>> GetReportDataAsync(List<string> tickers)
        {
            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var result = new Dictionary<string, string>();

            foreach (var ticker in tickers)
            {
                var fundamentalParametersByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                string report = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.Report)?.Value.ToString() ?? string.Empty;

                result.Add(ticker, report);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)>> GetExtDataAsync(List<string> tickers)
        {
            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var result = new Dictionary<string, (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)>();

            foreach (var ticker in tickers)
            {
                var fundamentalParametersByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                string? dividendPolyticInfo = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.DividendPolyticInfo)?.ExtData;
                string? growthDriverInfo = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.GrowthDriverInfo)?.ExtData;
                string? riskInfo = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.RiskInfo)?.ExtData;
                string? concept = fundamentalParametersByTicker.Find(x => x.Type == KnownFundamentalParameterTypes.Concept)?.ExtData;

                result.Add(ticker, (dividendPolyticInfo, growthDriverInfo, riskInfo, concept));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<AnalyseDataContext> GetAnalyseDataContextAsync()
        {
            if (_analyseDataContext is not null) return _analyseDataContext;

            var instruments = (await instrumentRepository.GetInstrumentsAsync())!
                .Where(x => 
                    x.Type == KnownInstrumentTypes.Share || 
                    x.Type == KnownInstrumentTypes.Index || 
                    x.Type == KnownInstrumentTypes.Etf)
                .ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            _analyseDataContext = new AnalyseDataContext
            {
                CandleData = await GetCandleDataAsync(tickers),
                UltimateSmootherData = await GetUltimateSmootherDataAsync(tickers),
                ClosePriceData = await GetClosePriceDataAsync(tickers),
                BenchmarkChangeData = await GetBenchmarkChangeDataAsync(tickers),
                MoexIndexShareData = await GetMoexIndexShareDataAsync(tickers),
                FundamentalMetricData = await GetFundamentalMetricDataAsync(tickers),
                FillFundamentalData = await GetFillFundamentalDataAsync(tickers),
                ReportData = await GetReportDataAsync(tickers),
                ExtData = await GetExtDataAsync(tickers)
            };

            return _analyseDataContext;
        }

        private async Task<List<Candle>> GetCandlesByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var response = await storageApiClient.GetCandleListAsync(
                new()
                {
                    From = from,
                    To = to,
                    Ticker = ticker
                });

            return response.Result.Candles
                .Select(x =>
                new Candle
                {
                    Open = x.Open,
                    Close = x.Close,
                    Low = x.Low,
                    High = x.High,
                    Volume = x.Volume,
                    Date = x.Date
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        private static List<DateValue<double>> GetUltimateSmootherValues(List<Candle> candles)
        {
            int period = 50;

            // Ultimate Smoother function based on John Ehlers' formula
            double coeff = Math.Sqrt(2.0);
            double step = 2.0 * Math.PI / period;
            double a1 = Math.Exp(-1.0 * coeff * Math.PI / period);
            double b1 = 2.0 * a1 * Math.Cos(coeff * step / period);
            double c2 = b1;
            double c3 = -1.0 * a1 * a1;
            double c1 = (1 + c2 - c3) / 4.0;

            var result = new List<DateValue<double>>();

            for (int i = 0; i < candles.Count; i++)
                if (i < 3)
                    result.Add(
                        new DateValue<double>
                        {
                            Date = candles[i].Date,
                            Value = candles[i].Close
                        });

                else
                    result.Add(
                        new DateValue<double>
                        {
                            Date = candles[i].Date,
                            Value = (1 - c1) * candles[i].Close +
                                    (2 * c1 - c2) * candles[i - 1].Close -
                                    (c1 + c3) * candles[i - 2].Close +
                                    c2 * result[i - 1].Value +
                                    c3 * result[i - 2].Value
                        });

            return result;
        }

        private static Dictionary<string, List<DateValue<double>>> GetUltimateSmootherData(Dictionary<string, List<Candle>> candleData)
        {
            var data = new Dictionary<string, List<DateValue<double>>>();

            foreach (var pair in candleData)
            {
                string ticker = pair.Key;

                if (!candleData.ContainsKey(ticker)) continue;

                var candles = candleData[ticker];
                var closePrices = candles.Select(x => x.Close).ToList();
                var ultimateSmootherValues = GetUltimateSmootherValues(candles);

                data.Add(ticker, ultimateSmootherValues);
            }

            return data;
        }

        private async Task<List<FundamentalParameterListItem>> GetFundamentalParameterListAsync()
        {
            if (_fundamentalParameters is not null) return _fundamentalParameters;

            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';'), string.Empty];

            _fundamentalParameters = (await storageApiClient.GetFundamentalParameterListAsync(new() { Periods = periods })).Result.FundamentalParameters;

            return _fundamentalParameters;
        }
    }
}
