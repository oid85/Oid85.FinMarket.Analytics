using System.Reflection.Metadata;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class TrendService(
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : ITrendService
    {
        /// <inheritdoc />
        public async Task<GetTrendDynamicResponse> GetTrendDynamicAsync(GetTrendDynamicRequest request)
        {
            var monthAgo = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = await GetInstrumentAsync();
            var candleData = await GetCandleData(instruments.Select(x => x.Ticker).ToList());
            var ultimateSmootherData = GetUltimateSmootherData(candleData);

            var response = new GetTrendDynamicResponse();

            response.Dates = DateUtils.GetDates(monthAgo, today);
            response.Indexes = GetTrendDynamicData(monthAgo, today, instruments.Where(x => x.Type == KnownInstrumentTypes.Index).Select(x => x.Ticker).ToList(), ultimateSmootherData, candleData);
            response.Shares = GetTrendDynamicData(monthAgo, today, instruments.Where(x => x.Type == KnownInstrumentTypes.Share).Select(x => x.Ticker).ToList(), ultimateSmootherData, candleData);
            response.Futures = GetTrendDynamicData(monthAgo, today, instruments.Where(x => x.Type == KnownInstrumentTypes.Future).Select(x => x.Ticker).ToList(), ultimateSmootherData, candleData);
            response.Bonds = GetTrendDynamicData(monthAgo, today, instruments.Where(x => x.Type == KnownInstrumentTypes.Bond).Select(x => x.Ticker).ToList(), ultimateSmootherData, candleData);

            return response;
        }

        private static List<TrendDynamicData> GetTrendDynamicData(
            DateOnly from, 
            DateOnly to, 
            List<string> tickers, 
            Dictionary<string, List<DateValue<double>>> ultimateSmootherData,
            Dictionary<string, List<Candle>> candleData)
        {
            var data = new List<TrendDynamicData>();

            foreach (var ticker in tickers)
            {
                var dataItem = new TrendDynamicData() { Ticker = ticker, Trend = [], Delta = [] };
                var ultimateSmootherValues = ultimateSmootherData[ticker].Where(x => x.Date >= from && x.Date <= to).ToList();
                var candles = candleData[ticker].Where(x => x.Date >= from && x.Date <= to).ToList();

                dataItem.Trend.Add(0);
                dataItem.Delta.Add(0.0);


                // Trend
                for (int i = 1; i < ultimateSmootherValues.Count; i++)
                    if (ultimateSmootherValues[i].Value > ultimateSmootherValues[i - 1].Value)
                        dataItem.Trend.Add(1);

                    else if (ultimateSmootherValues[i].Value < ultimateSmootherValues[i - 1].Value)
                        dataItem.Trend.Add(-1);

                    else
                        dataItem.Trend.Add(0);                

                // Delta
                for (int i = 1; i < candles.Count; i++)
                    dataItem.Delta.Add(Math.Round((candles[i].Close - candles[i - 1].Close) / candles[i - 1].Close * 100.0, 2));

                data.Add(dataItem);
            }

            return data;
        }

        private async Task<Dictionary<string, List<Candle>>> GetCandleData(List<string> tickers)
        {
            var data = new Dictionary<string, List<Candle>>();

            foreach (var ticker in tickers)
            {
                var candles = await GetCandleByTickerAsync(ticker);
                data.Add(ticker, candles);
            }

            return data;
        }

        private static Dictionary<string, List<DateValue<double>>> GetUltimateSmootherData(Dictionary<string, List<Candle>> candleData)
        {
            var data = new Dictionary<string, List<DateValue<double>>>();

            foreach (var pair in candleData)
            {
                string ticker = pair.Key;
                var candles = candleData[ticker];
                var closePrices = candles.Select(x => x.Close).ToList();
                var ultimateSmootherValues = CalculateUltimateSmoother(candles);

                data.Add(ticker, ultimateSmootherValues);
            }

            return data;
        }

        private async Task<List<Instrument>> GetInstrumentAsync()
        {
            var response = await finMarketStorageServiceApiClient.GetInstrumentListAsync(new());
            return response.Result.Instruments
                .Select(x => 
                new Instrument
                {
                    Ticker = x.Ticker,
                    Name = x.Name,
                    Type = x.Type
                })
                .ToList();
        }

        private async Task<List<Candle>> GetCandleByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var response = await finMarketStorageServiceApiClient.GetCandleListAsync(
                new GetCandleListRequest
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

        public static List<DateValue<double>> CalculateUltimateSmoother(List<Candle> candles)
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
    }
}
