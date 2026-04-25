using System.Linq;
using Oid85.FinMarket.Analytics.Application.Helpers;
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
            
            if (request.Value is not null)
            {
                if (request.Value.Contains('\t'))
                {
                    var parts = request.Value.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (string.IsNullOrEmpty(parts[i].Trim())) continue;

                        createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                            new CreateOrUpdateFundamentalParameterItemRequest
                            {
                                Ticker = request.Ticker,
                                Type = request.Type,
                                Period = (int.Parse(request.Period!) + i).ToString(),
                                Value = StringUtils.ToDouble(parts[i]),
                                ExtData = request.ExtData ?? string.Empty
                            });
                    }

                    await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                    return new CreateOrUpdateAnalyticFundamentalParameterResponse();
                }

                else if (request.Value.Contains(';'))
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
                                Period = (int.Parse(request.Period!) + i).ToString(),
                                Value = StringUtils.ToDouble(parts[i]),
                                ExtData = request.ExtData ?? string.Empty
                            });
                    }

                    await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                    return new CreateOrUpdateAnalyticFundamentalParameterResponse();
                }

                else
                    createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                        new CreateOrUpdateFundamentalParameterItemRequest
                        {
                            Ticker = request.Ticker,
                            Type = request.Type,
                            Period = request.Period,
                            Value = StringUtils.ToDouble(request.Value),
                            ExtData = request.ExtData ?? string.Empty
                        });

                await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                return new CreateOrUpdateAnalyticFundamentalParameterResponse();
            }

            if (request.ExtData is not null)
            {
                createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                    new CreateOrUpdateFundamentalParameterItemRequest
                    {
                        Ticker = request.Ticker,
                        Type = request.Type,
                        Period = request.Period,
                        Value = StringUtils.ToDouble(request.Value),
                        ExtData = request.ExtData ?? string.Empty
                    });

                await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                return new CreateOrUpdateAnalyticFundamentalParameterResponse();
            }

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<DeleteAnalyticFundamentalParameterResponse> DeleteAnalyticFundamentalParameterAsync(DeleteAnalyticFundamentalParameterRequest request)
        {
            await finMarketStorageServiceApiClient.DeleteFundamentalParameterAsync(
                new ()
                {
                    FundamentalParameters = [ new () { Ticker = request.Ticker, Type = request.Type, Period = request.Period } ]
                });

            return new();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new() { Periods = periods })).Result.FundamentalParameters;
            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments.Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var tickers = instruments.Select(x => x.Ticker).ToList();
            var benchmarkChangeData = await dataService.GetBenchmarkChangeDataAsync(tickers);
            var scoreData = await dataService.GetFundamentalScoreDataAsync(tickers);
            var extData = await dataService.GetExtDataAsync(tickers);

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
                    BenchmarkChange = benchmarkChangeData.TryGetValue(instrument.Ticker, out double value) ? value.RoundTo(2) : 0.0,
                    Concept = (extData.TryGetValue(instrument.Ticker, out var extDataItem) ? extDataItem.Concept : null) ?? "..."
                };
                
                for (int i = 0; i < periods.Count; i++)
                {
                    fundamentalParameterItem.Price.Add(prices[i][instrument.Ticker].HasValue ? prices[i][instrument.Ticker].Value.RoundTo(4) : null);
                    fundamentalParameterItem.NumberShares.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NumberShares, periods[i]).RoundTo(2));
                    fundamentalParameterItem.Pe.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, periods[i]).RoundTo(2));
                    fundamentalParameterItem.Ebitda.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, periods[i]).RoundTo(1));
                    fundamentalParameterItem.Revenue.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, periods[i]).RoundTo(1));
                    fundamentalParameterItem.NetProfit.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, periods[i]).RoundTo(1));
                    fundamentalParameterItem.Fcf.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Fcf, periods[i]).RoundTo(1));
                    fundamentalParameterItem.Eps.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Eps, periods[i]).RoundTo(2));
                    fundamentalParameterItem.Ev.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, periods[i]).RoundTo(1));
                    fundamentalParameterItem.NetDebt.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, periods[i]).RoundTo(1));
                    fundamentalParameterItem.MarketCap.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, periods[i]).RoundTo(1));
                    fundamentalParameterItem.Dividend.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Dividend, periods[i]).RoundTo(5));
                    fundamentalParameterItem.Roa.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, periods[i]).RoundTo(2));
                    fundamentalParameterItem.Pbv.Add(GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, periods[i]).RoundTo(2));

                    fundamentalParameterItem.EvEbitda.Add(fundamentalParameterItem.Ev[i].Div(fundamentalParameterItem.Ebitda[i]).RoundTo(2));
                    fundamentalParameterItem.NetDebtEbitda.Add(fundamentalParameterItem.NetDebt[i].Div(fundamentalParameterItem.Ebitda[i]).RoundTo(2));
                    fundamentalParameterItem.EbitdaRevenue.Add(fundamentalParameterItem.Ebitda[i].Div(fundamentalParameterItem.Revenue[i]).RoundTo(2));
                    fundamentalParameterItem.DividendYield.Add(fundamentalParameterItem.Dividend[i].Div(fundamentalParameterItem.Price[i]).Mult(100.0).RoundTo(2));                    
                    fundamentalParameterItem.DeltaMinMax.Add(await GetDeltaMinMaxAsync(instrument.Ticker, int.Parse(periods[i])));

                    string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;

                    if (periods[i] == (int.Parse(predictYear) - 1).ToString())
                    {
                        if (fundamentalParameterItem.NumberShares.Last().HasValue &&
                            fundamentalParameterItem.Pe.Last().HasValue &&
                            fundamentalParameterItem.Pbv.Last().HasValue &&
                            fundamentalParameterItem.Roa.Last().HasValue &&
                            fundamentalParameterItem.MarketCap.Last().HasValue &&
                            fundamentalParameterItem.Revenue.Last().HasValue &&
                            fundamentalParameterItem.NetProfit.Last().HasValue &&
                            fundamentalParameterItem.Eps.Last().HasValue &&
                            fundamentalParameterItem.Fcf.Last().HasValue &&
                            fundamentalParameterItem.Dividend.Last().HasValue)
                        {
                            fundamentalParameterItem.FillData = true;
                        }
                    }
                }

                fundamentalParameterItem.Score = scoreData.TryGetValue(instrument.Ticker, out FundamentalScore? score) ? score : null;

                fundamentalParameterItems.Add(fundamentalParameterItem);
            }

            var response = new GetAnalyticFundamentalParameterListResponse { FundamentalParameters = [.. fundamentalParameterItems.OrderByDescending(x => x.Score?.ScoreValue)] };

            int number = 1;

            foreach (var fundamentalParameterItem in response.FundamentalParameters) 
                fundamentalParameterItem.Number = number++;

            response.TotalCount = fundamentalParameterItems.Count;
            response.NoFillDataCount = fundamentalParameterItems.Count(x => !x.FillData);
            response.NoFillDataTickers = string.Join(", ", fundamentalParameterItems.Where(x => !x.FillData).Select(x => x.Ticker).ToList());

            return response;
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterBubbleDiagramResponse> GetAnalyticFundamentalParameterBubbleDiagramAsync(GetAnalyticFundamentalParameterBubbleDiagramRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];

            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;            

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();            

            var response = new GetAnalyticFundamentalParameterBubbleDiagramResponse();

            foreach (var instrument in instruments)
            {
                var ebitda = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var ev = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var netDebt = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var marketCap = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, periods).LastOrDefault(x => x.Value != 0.0).Value;

                response.Data.Add(
                    new GetAnalyticFundamentalParameterBubbleDiagramPointResponse
                    {
                        Ticker = instrument.Ticker,
                        EvEbitda = ev.Div(ebitda).RoundTo(2),
                        NetDebtEbitda = netDebt.Div(ebitda).RoundTo(2),
                        MarketCap = marketCap
                    });
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<GetFundamentalBySectorResponse> GetFundamentalBySectorAsync(GetFundamentalBySectorRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];

            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).Where(x => x.Sector == request.Sector).OrderBy(x => x.Ticker).ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var priceData = await dataService.GetClosePriceDataAsync(tickers);
           
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
                var ebitda = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var ev = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var netDebt = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var marketCap = GetFundamentalParameterValues(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, periods).LastOrDefault(x => x.Value != 0.0).Value;

                response.BubbleDiagram.Add(
                    new GetFundamentalBySectorBubbleDiagramPointResponse
                    {
                        Ticker = instrument.Ticker,
                        EvEbitda = ev.Div(ebitda).RoundTo(2),
                        NetDebtEbitda = netDebt.Div(ebitda).RoundTo(2),
                        MarketCap = marketCap
                    });
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<GetFundamentalByCompanyResponse> GetFundamentalByCompanyAsync(GetFundamentalByCompanyRequest request)
        {
            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var tickers = instruments.Select(x => x.Ticker).ToList();
            var instrument = instruments.Find(x => x.Ticker == request.Ticker)!;

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync(tickers);

            var dividend = analyseDataContext.GetDividend(instrument.Ticker);
            var consensusForecast = analyseDataContext.GetConsensusForecast(instrument.Ticker);
            var nataliaBaffetovnaForecast = analyseDataContext.GetNataliaBaffetovnaForecast(instrument.Ticker);
            var financeMarkerForecast = analyseDataContext.GetFinanceMarkerForecast(instrument.Ticker);
            var vladProDengiForecast = analyseDataContext.GetVladProDengiForecast(instrument.Ticker);
            var predictNetProfitForecast = analyseDataContext.GetPredictNetProfitForecast(instrument.Ticker);
            var fundamentalScore = analyseDataContext.GetFundamentalScore(instrument.Ticker);
            var benchmarkChange = analyseDataContext.GetBenchmarkChange(instrument.Ticker);
            var companyFundamentalMetric = analyseDataContext.GetFundamentalMetric(instrument.Ticker);
            var companyFundamentalMetrics = analyseDataContext.GetFundamentalMetrics(instrument.Ticker);
            var prices = analyseDataContext.GetClosePrices(instrument.Ticker);
            var price = analyseDataContext.GetPrice(instrument.Ticker);
            var ultimateSmoothers = analyseDataContext.GetUltimateSmoothers(instrument.Ticker);
            var dividendPolyticInfo = analyseDataContext.GetExtData(instrument.Ticker)?.DividendPolyticInfo;
            var growthDriverInfo = analyseDataContext.GetExtData(instrument.Ticker)?.GrowthDriverInfo;
            var riskInfo = analyseDataContext.GetExtData(instrument.Ticker)?.RiskInfo;
            var concept = analyseDataContext.GetExtData(instrument.Ticker)?.Concept;
            var trendState = TrendStateHelper.GetTrendState(ultimateSmoothers);

            var response = new GetFundamentalByCompanyResponse
            {
                Ticker = instrument.Ticker,
                InPortfolio = instrument.InPortfolio,
                Name = instrument.Name,
                Sector = instrument.Sector,
                Price = price,
                TrendState = trendState.Message,
                Dividend = dividend,
                ConsensusForecast = consensusForecast,
                NataliaBaffetovnaForecast = nataliaBaffetovnaForecast,
                FinanceMarkerForecast = financeMarkerForecast,
                VladProDengiForecast = vladProDengiForecast,
                PredictNetProfitForecast = predictNetProfitForecast,
                FundamentalScore = fundamentalScore,
                BenchmarkChange = benchmarkChange,
                CompanyFundamentalMetric = companyFundamentalMetric,
                PriceDiagramData = GetPriceDiagramData(),
                DividendPolyticInfo = dividendPolyticInfo,
                GrowthDriverInfo = growthDriverInfo,
                RiskInfo = riskInfo,
                Concept = concept
            };

            // Диаграммы динамики показателей компании
            foreach (var metric in companyFundamentalMetrics)
            {
                response.NetProfitDiagramData.Add(new() { X = metric.Period, Y = metric.NetProfit });
                response.DividendDiagramData.Add(new() { X = metric.Period, Y = metric.Dividend });
                response.PeDiagramData.Add(new() { X = metric.Period, Y = metric.Pe });
                response.PbvDiagramData.Add(new() { X = metric.Period, Y = metric.Pbv });
                response.EvEbitdaDiagramData.Add(new() { X = metric.Period, Y = metric.EvEbitda });
                response.NetDebtEbitdaDiagramData.Add(new() { X = metric.Period, Y = metric.NetDebtEbitda });
                response.FcfDiagramData.Add(new() { X = metric.Period, Y = metric.Fcf });
                response.EpsDiagramData.Add(new() { X = metric.Period, Y = metric.Eps });
            }  
                
            // Сравнительная диаграмма по мультипликаторам среди сектора
            var sectorInstruments = instruments.Where(x => x.Sector == instrument.Sector).ToList();
            List<Instrument> orderedSectorInstruments = [sectorInstruments.Find(x => x.Ticker == instrument.Ticker), .. sectorInstruments.Where(x => x.Ticker != instrument.Ticker).OrderBy(x => x.Ticker).ToList()];

            foreach (var sectorInstrument in orderedSectorInstruments)
            {
                var fundamentalMetric = analyseDataContext.GetFundamentalMetric(sectorInstrument.Ticker)!;

                response.PeSectorDiagramData.Add(new () { X = sectorInstrument.Ticker, Y = fundamentalMetric.Pe });
                response.PbvSectorDiagramData.Add(new() { X = sectorInstrument.Ticker, Y = fundamentalMetric.Pbv });
                response.EvEbitdaSectorDiagramData.Add(new() { X = sectorInstrument.Ticker, Y = fundamentalMetric.EvEbitda });
                response.NetDebtEbitdaSectorDiagramData.Add(new() { X = sectorInstrument.Ticker, Y = fundamentalMetric.NetDebtEbitda });
            }

            return response;

            List<PriceDiagramDataPoint> GetPriceDiagramData()
            {
                var priceDiagramData = new List<PriceDiagramDataPoint>();

                for (int i = 1; i < prices.Count; i++)
                {
                    var date = prices[i].Date;
                    var price = prices[i].Value;
                    var us = ultimateSmoothers.Find(x => x.Date == date)?.Value;                    

                    var point = new PriceDiagramDataPoint
                    {
                        Date = date,
                        PriceValue = price,
                        UltimateSmootherValue = us
                    };

                    priceDiagramData.Add(point);
                }

                return priceDiagramData;
            }
        }

        private static List<(string Period, double Value)> GetFundamentalParameterValues(List<FundamentalParameterListItem> fundamentalParameters, string ticker, string type, List<string> periods)
        {
            if (fundamentalParameters is null) return [];

            List<(string Period, double Value)> result = [];

            foreach (var period in periods)
            {
                var fundamentalParameter = fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period);

                result.Add(fundamentalParameter is null ? (period, 0.0) : (period, fundamentalParameter.Value));
            }
            
            return result;
        }

        private static double? GetFundamentalParameterValue(List<FundamentalParameterListItem> fundamentalParameters, string ticker, string type, string period)
        {
            if (fundamentalParameters is null) 
                return null;

            return fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period)?.Value;
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
                ? -1 * Math.Abs(((max - min) / max * 100.0).RoundTo(2)) // Падение от максимума
                : Math.Abs(((max - min) / min * 100.0).RoundTo(2));     // Рост от минимума
        }
    }
}
