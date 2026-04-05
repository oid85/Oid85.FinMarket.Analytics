using System.Diagnostics.Metrics;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class DataService(
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient)
        : IDataService
    {
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
            var data = new Dictionary<string, List<Candle>>();

            foreach (var ticker in tickers)
            {
                var candles = await GetCandleByTickerAsync(ticker);
                data.Add(ticker, candles);
            }

            return data;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<DateValue<double>>>> GetUltimateSmootherDataAsync(List<string> tickers)
        {
            var candleData = await GetCandleDataAsync(tickers);
            var ultimateSmootherData = GetUltimateSmootherData(candleData);

            return ultimateSmootherData;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<DateValue<double>>>> GetClosePriceDiagramDataAsync(List<string> tickers)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
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
            string period = DateTime.Now.Year.ToString();

            var candleData = await GetCandleDataAsync(tickers);

            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result
                .FundamentalParameters
                .Where(x => x.Type == KnownFundamentalParameterTypes.Dividend)
                .Where(x => x.Period == period)
                .ToList();

            var result = new Dictionary<string, Dividend>();

            foreach (var ticker in tickers)
            {
                if (!candleData.ContainsKey(ticker)) continue;

                var lastClosePrice = candleData[ticker].Last().Close;
                var dividendFundamentalParameter = fundamentalParameters.Find(x => x.Ticker == ticker);

                if (dividendFundamentalParameter is null) continue;

                double yield = dividendFundamentalParameter.Value / lastClosePrice * 100.0;
                var dividend = new Dividend { Ticker = ticker, Value = dividendFundamentalParameter.Value, Yield = yield };

                result.Add(ticker, dividend);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, double>> GetBenchmarkChangeAsync(List<string> tickers)
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

        private async Task<List<Candle>> GetCandleByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
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
    }
}
