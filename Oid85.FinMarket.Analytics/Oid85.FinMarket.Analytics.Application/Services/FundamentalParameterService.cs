using System.Globalization;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class FundamentalParameterService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IInstrumentService instrumentService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient,
        IDataService dataService)
        : IFundamentalParameterService
    {
        /// <inheritdoc />
        public async Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateAnalyticFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request)
        {
            var createOrUpdateFundamentalParameterRequest = new CreateOrUpdateFundamentalParameterRequest();

            if (request.Value.Contains(';'))
            {
                var parts = request.Value.Split(';');

                for (int i = 0; i < parts.Length; i++)
                {
                    if (string.IsNullOrEmpty(parts[i].Trim())) continue;

                    createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                        new CreateOrUpdateFundamentalParameterItemRequest
                        {
                            Ticker = request.Ticker,
                            Type = request.Type,
                            Period = (int.Parse(request.Period) + i).ToString(),
                            Value = StringUtils.ToDouble(parts[i])
                        });
                }
            }

            else
            {
                createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                    new CreateOrUpdateFundamentalParameterItemRequest
                    {
                        Ticker = request.Ticker,
                        Type = request.Type,
                        Period = request.Period,
                        Value = StringUtils.ToDouble(request.Value)
                    });                
            }

            await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;
            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments.Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var tickers = instruments.Select(x => x.Ticker).ToList();
            var benchmarkChangeData = await dataService.GetBenchmarkChangeDataAsync(tickers);
            var scoreData = await dataService.GetFundamentalScoreDataAsync(tickers);

            var prices = new List<Dictionary<string, double?>>();

            foreach (var period in periods)
            {
                int year = int.Parse(period);
                var lastCandleList = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(year, 12, 31)) })).Result.Candles;
                var priceDictionary = tickers.Zip(lastCandleList, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
                prices.Add(priceDictionary);
            }

            var fundamentalParameterItems = new List<GetAnalyticFundamentalParameterListItemResponse>();

            foreach (var instrument in instruments)
            {
                var fundamentalParameterItem = new GetAnalyticFundamentalParameterListItemResponse
                {
                    Periods = periods,
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    IsSelected = instrument.IsSelected,
                    InPortfolio = instrument.InPortfolio,
                    BenchmarkChange = benchmarkChangeData.TryGetValue(instrument.Ticker, out double value) ? Math.Round(value, 2) : 0.0,
                    Moex = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Moex, string.Empty)
                };
                
                for (int i = 0; i < periods.Count; i++)
                {
                    fundamentalParameterItem.Price.Add(prices[i][instrument.Ticker].HasValue ? Math.Round(prices[i][instrument.Ticker].Value, 4) : null);
                    fundamentalParameterItem.Pe.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, periods[i]));
                    fundamentalParameterItem.Ebitda.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, periods[i]));
                    fundamentalParameterItem.Revenue.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, periods[i]));
                    fundamentalParameterItem.NetProfit.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, periods[i]));
                    fundamentalParameterItem.Ev.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, periods[i]));
                    fundamentalParameterItem.NetDebt.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, periods[i]));
                    fundamentalParameterItem.MarketCap.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, periods[i]));
                    fundamentalParameterItem.Dividend.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Dividend, periods[i]));
                    fundamentalParameterItem.Roa.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, periods[i]));
                    fundamentalParameterItem.Pbv.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, periods[i]));

                    fundamentalParameterItem.EvEbitda.Add(GetEvEbitda(fundamentalParameterItem.Ev[i], fundamentalParameterItem.Ebitda[i]));
                    fundamentalParameterItem.NetDebtEbitda.Add(GetNetDebtEbitda(fundamentalParameterItem.NetDebt[i], fundamentalParameterItem.Ebitda[i]));
                    fundamentalParameterItem.EbitdaRevenue.Add(GetEbitdaRevenue(fundamentalParameterItem.Ebitda[i], fundamentalParameterItem.Revenue[i]));
                    fundamentalParameterItem.DividendYield.Add(GetDividendYield(fundamentalParameterItem.Dividend[i], fundamentalParameterItem.Price[i]));
                    fundamentalParameterItem.DeltaMinMax.Add(await GetDeltaMinMaxAsync(instrument.Ticker, int.Parse(periods[i])));
                }

                fundamentalParameterItem.Score = scoreData[instrument.Ticker];

                fundamentalParameterItems.Add(fundamentalParameterItem);
            }

            var response = new GetAnalyticFundamentalParameterListResponse { FundamentalParameters = [.. fundamentalParameterItems.OrderByDescending(x => x.Score?.ScoreValue)] };

            int number = 1;

            foreach (var fundamentalParameterItem in response.FundamentalParameters)
                fundamentalParameterItem.Number = number++;

            return response;
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterBubbleDiagramResponse> GetAnalyticFundamentalParameterBubbleDiagramAsync(GetAnalyticFundamentalParameterBubbleDiagramRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;            

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();            

            var response = new GetAnalyticFundamentalParameterBubbleDiagramResponse();

            foreach (var instrument in instruments)
            {
                var ebitda = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, request.Year.ToString());
                var ev = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, request.Year.ToString());
                var netDebt = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, request.Year.ToString());
                var marketCap = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, request.Year.ToString());

                var evEbitda = GetEvEbitda(ev, ebitda);
                var netDebtEbitda = GetNetDebtEbitda(netDebt, ebitda);

                if (!evEbitda.HasValue) continue;
                if (!netDebtEbitda.HasValue) continue;
                if (!marketCap.HasValue) continue;

                response.Data.Add(
                    new GetAnalyticFundamentalParameterBubbleDiagramPointResponse
                    {
                        Ticker = instrument.Ticker,
                        EvEbitda = evEbitda.Value,
                        NetDebtEbitda = netDebtEbitda.Value,
                        MarketCap = marketCap.Value
                    });
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<GetFundamentalBySectorResponse> GetFundamentalBySectorAsync(GetFundamentalBySectorRequest request)
        {            
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? [])
                .Where(x => x.Type == KnownInstrumentTypes.Share).Where(x => x.Sector == request.Sector).OrderBy(x => x.Ticker).ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var priceData = await dataService.GetClosePriceDiagramDataAsync(tickers);

            List<string> periods = ["2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023", "2024", "2025"];

            var response = new GetFundamentalBySectorResponse();

            foreach (var instrument in instruments)
                response.PriceDiagram.Add(
                    new GetFundamentalBySectorItemResponse()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. priceData[instrument.Ticker].Select(x => new GetFundamentalBySectorDateValueResponse { Date = x.Date.ToString(), Value = x.Value })]
                    });

            foreach (var instrument in instruments)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, periods);

                response.RevenueDiagram.Add(
                    new GetFundamentalBySectorItemResponse()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new GetFundamentalBySectorDateValueResponse { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var instrument in instruments)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, periods);

                response.NetProfitDiagram.Add(
                    new GetFundamentalBySectorItemResponse()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new GetFundamentalBySectorDateValueResponse { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var instrument in instruments)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Dividend, periods);

                response.DividendDiagram.Add(
                    new GetFundamentalBySectorItemResponse()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new GetFundamentalBySectorDateValueResponse { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var instrument in instruments)
            {
                string year = "2024";

                var ebitda = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, year.ToString());
                var ev = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, year.ToString());
                var netDebt = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, year.ToString());
                var marketCap = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, year.ToString());

                var evEbitda = GetEvEbitda(ev, ebitda);
                var netDebtEbitda = GetNetDebtEbitda(netDebt, ebitda);

                if (!evEbitda.HasValue) continue;
                if (!netDebtEbitda.HasValue) continue;
                if (!marketCap.HasValue) continue;

                response.BubbleDiagram.Add(
                    new GetFundamentalBySectorBubbleDiagramPointResponse
                    {
                        Ticker = instrument.Ticker,
                        EvEbitda = evEbitda.Value,
                        NetDebtEbitda = netDebtEbitda.Value,
                        MarketCap = marketCap.Value
                    });
            }

            return response;
        }

        private static List<(string Period, double Value)> GetFundamentalParameterValues(List<GetFundamentalParameterListItemResponse> fundamentalParameters, string ticker, string type, List<string> periods)
        {
            if (fundamentalParameters is null)
                return [];

            List<(string Period, double Value)> result = [];

            foreach (var period in periods)
            {
                var fundamentalParameter = fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period);

                result.Add(fundamentalParameter is null ? (period, 0.0) : (period, fundamentalParameter.Value));
            }
            
            return result;
        }

        private static double? GetFundamentalParameterValue(List<GetFundamentalParameterListItemResponse> fundamentalParameters, string ticker, string type, string period)
        {
            if (fundamentalParameters is null)
                return null;

            var fundamentalParameter = fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period);

            return fundamentalParameter?.Value;
        }

        private static double? GetEvEbitda(double? ev, double? ebitda)
        {
            if (ev is null || ebitda is null)
                return null;

            if (ev == 0.0 || ebitda == 0.0)
                return 0.0;

            return Math.Round(ev.Value / ebitda.Value, 2);
        }

        private static double? GetNetDebtEbitda(double? netDebt, double? ebitda)
        {
            if (netDebt is null || ebitda is null)
                return null;

            if (netDebt == 0.0 || ebitda == 0.0)
                return 0.0;

            return Math.Round(netDebt.Value / ebitda.Value, 2);
        }

        private static double? GetEbitdaRevenue(double? ebitda, double? revenue)
        {
            if (revenue is null || ebitda is null)
                return null;

            if (revenue == 0.0 || ebitda == 0.0)
                return 0.0;

            return Math.Round(ebitda.Value / revenue.Value, 2);
        }

        private static double? GetDividendYield(double? dividend, double? price)
        {
            if (dividend is null || price is null)
                return null;

            if (dividend == 0.0 || price == 0.0)
                return 0.0;

            return Math.Round(dividend.Value / price.Value * 100.0, 2);
        }

        private async Task<double?> GetDeltaMinMaxAsync(string ticker, int year)
        {
            var response = await finMarketStorageServiceApiClient.GetCandleListAsync(
                new GetCandleListRequest
                {
                    Ticker = ticker,
                    From = DateOnly.FromDateTime(new DateTime(year, 1, 1)),
                    To = DateOnly.FromDateTime(new DateTime(year, 12, 31))
                });

            if (response?.Result?.Candles is null)
                return null;

            if (response?.Result?.Candles?.Count == 0)
                return null;

            var maxCandle = response?.Result?.Candles.MaxBy(x => x.Close);
            var minCandle = response?.Result?.Candles.MinBy(x => x.Close);

            if (maxCandle is null || minCandle is null)
                return null;

            double max = maxCandle.Close;
            double min = minCandle.Close;

            return maxCandle.Date < minCandle.Date
                ? -1 * Math.Abs(Math.Round((max - min) / max * 100.0, 2)) // Падение от максимума
                : Math.Abs(Math.Round((max - min) / min * 100.0, 2)); // Рост от минимума
        }
    }
}
