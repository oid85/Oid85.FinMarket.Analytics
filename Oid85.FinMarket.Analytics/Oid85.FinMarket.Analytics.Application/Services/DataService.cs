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
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient)
        : IDataService
    {
        private Dictionary<string, List<Candle>>? _candleData = null;
        private List<FundamentalParameterListItem>? _fundamentalParameters = null;
        private List<ForecastListItem>? _forecasts = null;
        private AnalyseDataContext? _analyseDataContext = null;

        /// <inheritdoc />
        public async Task<Dictionary<string, List<BondCoupon>>> GetBondCouponsAsync(List<string> tickers)
        {
            var couponDictionary = new Dictionary<string, List<BondCoupon>>();

            foreach (var ticker in tickers)
            {
                var couponsTwoYear = (await finMarketStorageServiceApiClient.GetBondCouponListAsync(
                    new GetBondCouponListRequest
                    {
                        Ticker = ticker,
                        From = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                        To = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
                    })).Result.BondCoupons;

                for (int i = 1; i < couponsTwoYear.Count; i++)
                    if (couponsTwoYear[i].PayOneBond == 0) couponsTwoYear[i].PayOneBond = couponsTwoYear[i - 1].PayOneBond;

                var coupons = couponsTwoYear
                    .Where(x => 
                        x.CouponDate >= DateOnly.FromDateTime(DateTime.Today) && 
                        x.CouponDate <= DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
                    .Select(x => new BondCoupon 
                    {
                        Ticker = x.Ticker,
                        CouponNumber = x.CouponNumber,
                        CouponPeriod = x.CouponPeriod,
                        CouponDate = x.CouponDate,
                        PayOneBond = x.PayOneBond
                    })
                    .ToList();

                couponDictionary.Add(ticker, coupons);
            }

            return couponDictionary;
        }

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
            var years = int.Parse((await parameterRepository.GetParameterValueAsync(KnownParameters.DiagramPeriodLongYears)) ?? "3");
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * years));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var candleData = await GetCandleDataAsync(tickers);

            var data = new Dictionary<string, List<DateValue<double>>>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var candles = candleData[ticker].Where(x => x.Date >= from && x.Date <= to);
                var dateValues = candles.Select(x => new DateValue<double> { Date = x.Date, Value = x.Close}).ToList();

                data.Add(ticker, dateValues);
            }

            return data;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Dividend>> GetDividendDataAsync(List<string> tickers)
        {
            var candleData = await GetCandleDataAsync(tickers);

            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var fundamentalParameters = fundamentalParameterList.Where(x => x.Type == KnownFundamentalParameterTypes.Dividend).ToList();

            var result = new Dictionary<string, Dividend>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var lastClosePrice = candleData[ticker].Last().Close;

                var dividendFundamentalParameter = fundamentalParameters.Where(x => x.Ticker == ticker).OrderBy(x => x.Period).LastOrDefault();

                if (dividendFundamentalParameter is null) continue;

                double yield = dividendFundamentalParameter.Value / lastClosePrice * 100.0;

                var dividend = new Dividend { Ticker = ticker, Value = dividendFundamentalParameter.Value, Yield = Math.Round(yield, 2) };

                result.Add(ticker, dividend);
            }

            return result;
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

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetConsensusForecastDataAsync()
        {
            var forecasts = await GetConsensusForecastListAsync();
            var tickers = forecasts.Select(x => x.Ticker).ToList();            
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var forecast in forecasts)
            {
                if (!candleData.ContainsKey(forecast.Ticker)) continue;
                
                var price = candleData[forecast.Ticker].Last().Close;

                result.Add(
                    forecast.Ticker,
                    new ()
                    {
                        Ticker = forecast.Ticker,
                        ConsensusPrice = forecast.ConsensusPrice,
                        CurrentPrice = price,
                        UpsidePrc = Math.Round((forecast.ConsensusPrice - price) / price * 100.0, 2),
                        MaxTarget = forecast.MaxTarget,
                        MinTarget = forecast.MinTarget,
                        Recommendation = forecast.RecommendationString
                    });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetNataliaBaffetovnaForecastDataAsync(List<string> tickers)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameterList = await GetFundamentalParameterListAsync();
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var price = candleData[ticker].Last().Close;                

                var fundamentalParameterListByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();
                var consensusPrice = periods.Select(x => fundamentalParameterListByTicker.Find(fp => fp.Period == x && fp.Type == KnownFundamentalParameterTypes.NataliaBaffetovnaForecast)?.Value).LastOrDefault();

                if (!consensusPrice.HasValue) continue;

                result.Add(ticker, new () 
                { 
                    Ticker = ticker, 
                    ConsensusPrice = consensusPrice,
                    CurrentPrice = price,
                    UpsidePrc = Math.Round((consensusPrice.Value - price) / price * 100.0, 2)
                });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetFinanceMarkerForecastDataAsync(List<string> tickers)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameterList = await GetFundamentalParameterListAsync();
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var price = candleData[ticker].Last().Close;

                var fundamentalParameterListByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();
                var consensusPrice = periods.Select(x => fundamentalParameterListByTicker.Find(fp => fp.Period == x && fp.Type == KnownFundamentalParameterTypes.FinanceMarkerForecast)?.Value).LastOrDefault();

                if (!consensusPrice.HasValue) continue;

                result.Add(ticker, new()
                {
                    Ticker = ticker,
                    ConsensusPrice = consensusPrice,
                    CurrentPrice = price,
                    UpsidePrc = Math.Round((consensusPrice.Value - price) / price * 100.0, 2)
                });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetVladProDengiForecastDataAsync(List<string> tickers)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameterList = await GetFundamentalParameterListAsync();
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var price = candleData[ticker].Last().Close;

                var fundamentalParameterListByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();
                var consensusPrice = periods.Select(x => fundamentalParameterListByTicker.Find(fp => fp.Period == x && fp.Type == KnownFundamentalParameterTypes.VladProDengiForecast)?.Value).LastOrDefault();

                if (!consensusPrice.HasValue) continue;

                result.Add(ticker, new()
                {
                    Ticker = ticker,
                    ConsensusPrice = consensusPrice,
                    CurrentPrice = price,
                    UpsidePrc = Math.Round((consensusPrice.Value - price) / price * 100.0, 2)
                });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetMozgovikForecastDataAsync(List<string> tickers)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameterList = await GetFundamentalParameterListAsync();
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var price = candleData[ticker].Last().Close;

                var fundamentalParameterListByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();
                var consensusPrice = periods.Select(x => fundamentalParameterListByTicker.Find(fp => fp.Period == x && fp.Type == KnownFundamentalParameterTypes.MozgovikForecast)?.Value).LastOrDefault();

                if (!consensusPrice.HasValue) continue;

                result.Add(ticker, new()
                {
                    Ticker = ticker,
                    ConsensusPrice = consensusPrice,
                    CurrentPrice = price,
                    UpsidePrc = Math.Round((consensusPrice.Value - price) / price * 100.0, 2)
                });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Forecast>> GetPredictNetProfitForecastDataAsync(List<string> tickers)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;

            var fundamentalParameterList = await GetFundamentalParameterListAsync();
            var candleData = await GetCandleDataAsync(tickers);

            var result = new Dictionary<string, Forecast>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var price = candleData[ticker].Last().Close;

                var fundamentalParameterListByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                double? predictPe = fundamentalParameterListByTicker.Find(x => x.Period == predictYear && x.Type == KnownFundamentalParameterTypes.Pe)?.Value;
                double? predictNetProfit = fundamentalParameterListByTicker.Find(x => x.Period == predictYear && x.Type == KnownFundamentalParameterTypes.NetProfit)?.Value;
                double? predictNumberShares = fundamentalParameterListByTicker.Find(x => x.Period == predictYear && x.Type == KnownFundamentalParameterTypes.NumberShares)?.Value;

                predictNetProfit = predictNetProfit.Mult(1_000_000_000);
                predictNumberShares = predictNumberShares.Mult(1_000_000);

                double? predictNetProfitConsensusPrice = predictNetProfit.Mult(predictPe).Div(predictNumberShares);

                double? consensusPrice = predictNetProfit.Mult(predictPe).Div(predictNumberShares);

                if (!consensusPrice.HasValue) continue;

                result.Add(ticker, new()
                {
                    Ticker = ticker,
                    ConsensusPrice = consensusPrice,
                    CurrentPrice = price,
                    UpsidePrc = Math.Round((consensusPrice.Value - price) / price * 100.0, 2)
                });
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

                    var price = periodCandles?.LastOrDefault()?.Close;

                    var fundamentalParametersByPeriod = fundamentalParametersByTicker.Where(x => x.Period == period).ToList();

                    var numberShares = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NumberShares)?.Value.RoundTo(2);
                    var pe = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Pe)?.Value.RoundTo(2);
                    var pbv = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Pbv)?.Value.RoundTo(2);
                    var roa = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Roa)?.Value.RoundTo(2);
                    var roe = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Roe)?.Value.RoundTo(2);
                    var fcf = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Fcf)?.Value.RoundTo(2);
                    var eps = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Eps)?.Value.RoundTo(2);
                    var dividend = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Dividend)?.Value.RoundTo(2);
                    var netProfit = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NetProfit)?.Value.RoundTo(2);
                    var revenue = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Revenue)?.Value.RoundTo(2);
                    var marketCap = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.MarketCap)?.Value.RoundTo(2);
                    var ev = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Ev)?.Value.RoundTo(2);
                    var ebitda = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.Ebitda)?.Value.RoundTo(2);
                    var ownCapital = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.OwnCapital)?.Value.RoundTo(2);
                    var netDebt = fundamentalParametersByPeriod.Find(x => x.Type == KnownFundamentalParameterTypes.NetDebt)?.Value.RoundTo(2);

                    var evEbitda = ev.Div(ebitda).RoundTo(2);
                    var ebitdaRevenue = ebitda.Div(revenue).RoundTo(2);
                    var netDebtEbitda = netDebt.Div(ebitda).RoundTo(2);
                    var dividendYield = dividend.Div(price).Mult(100.0).RoundTo(2);

                    var deltaMinMax = GetDeltaMinMax(periodCandles);

                    metrics.Add(
                        new ()
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
                            Revenue = revenue,
                            Fcf = fcf,
                            Eps = eps,
                            Dividend = dividend,
                            DividendYield = dividendYield,
                            NetProfit = netProfit,
                            EvEbitda = evEbitda,
                            NetDebtEbitda = netDebtEbitda,
                            EbitdaRevenue = ebitdaRevenue,
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
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;

            var fundamentalParameterList = await GetFundamentalParameterListAsync();

            var result = new Dictionary<string, bool>();

            foreach (var ticker in tickers)
            {
                var fundamentalParametersByTicker = fundamentalParameterList.Where(x => x.Ticker == ticker).ToList();

                string lastPeriod = (int.Parse(predictYear) - 1).ToString();

                bool fillFundamental = fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.NumberShares) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.MarketCap) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.OwnCapital) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Dividend) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Pe) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Pbv) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Roa) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Roe) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Revenue) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.NetProfit) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Eps) is not null;
                fillFundamental &= fundamentalParametersByTicker.Find(x => x.Period == lastPeriod && x.Type == KnownFundamentalParameterTypes.Fcf) is not null;
                
                result.Add(ticker, fillFundamental);
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

            var instruments = (await instrumentRepository.GetInstrumentsAsync())!.Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var tickers = instruments.Select(x => x.Ticker).ToList();

            _analyseDataContext = new AnalyseDataContext
            {
                CandleData = await GetCandleDataAsync(tickers),
                UltimateSmootherData = await GetUltimateSmootherDataAsync(tickers),
                ClosePriceData = await GetClosePriceDataAsync(tickers),
                DividendData = await GetDividendDataAsync(tickers),
                BenchmarkChangeData = await GetBenchmarkChangeDataAsync(tickers),
                FundamentalMetricData = await GetFundamentalMetricDataAsync(tickers),
                ConsensusForecastData = await GetConsensusForecastDataAsync(),
                NataliaBaffetovnaForecastData = await GetNataliaBaffetovnaForecastDataAsync(tickers),
                FinanceMarkerForecastData = await GetFinanceMarkerForecastDataAsync(tickers),
                VladProDengiForecastData = await GetVladProDengiForecastDataAsync(tickers),
                MozgovikForecastData = await GetMozgovikForecastDataAsync(tickers),
                PredictNetProfitForecastData = await GetPredictNetProfitForecastDataAsync(tickers),
                BondCouponData = await GetBondCouponsAsync(tickers),
                FillFundamentalData = await GetFillFundamentalDataAsync(tickers),
                ExtData = await GetExtDataAsync(tickers)
            };

            return _analyseDataContext;
        }

        private async Task<List<Candle>> GetCandlesByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var response = await finMarketStorageServiceApiClient.GetCandleListAsync(
                new ()
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

            _fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new() { Periods = periods })).Result.FundamentalParameters;

            return _fundamentalParameters;
        }

        private async Task<List<ForecastListItem>> GetConsensusForecastListAsync()
        {
            if (_forecasts is not null) return _forecasts;

            _forecasts = (await finMarketStorageServiceApiClient.GetForecastListAsync(new())).Result.Forecasts;

            return _forecasts;
        }
    }
}
