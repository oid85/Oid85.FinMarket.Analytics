using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
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
    public class FundamentalService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IInstrumentService instrumentService,
        IStorageApiClient storageApiClient,
        IDataService dataService,
        IFundamentalScoreService fundamentalScoreService,
        IAnalyseParameterFactory analyseParameterFactory)
        : IFundamentalService
    {
        /// <inheritdoc />
        public async Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request)
        {
            var createOrUpdateFundamentalParameterRequest = new CreateOrUpdateFundamentalParameterRequest();

            if (request.Value is not null)
            {
                if (request.Value.Contains('\t'))
                {
                    var parts = request.Value.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < parts.Length; i++)
                    {
                        string part = parts[i].Replace(" ", "").Replace("%", "").Trim();

                        if (string.IsNullOrEmpty(part)) continue;

                        createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                            new CreateOrUpdateFundamentalParameterItemRequest
                            {
                                Ticker = request.Ticker,
                                Type = request.Type,
                                Period = (int.Parse(request.Period!) + i).ToString(),
                                Value = StringUtils.ToDouble(part),
                                ExtData = request.ExtData ?? string.Empty
                            });
                    }

                    await storageApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                    return new CreateOrUpdateAnalyticFundamentalParameterResponse();
                }

                else if (request.Value.Contains(';'))
                {
                    var parts = request.Value.Split(';');

                    for (int i = 0; i < parts.Length; i++)
                    {
                        string part = parts[i].Replace(" ", "").Replace("%", "").Trim();

                        if (string.IsNullOrEmpty(part)) continue;

                        createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(
                            new CreateOrUpdateFundamentalParameterItemRequest
                            {
                                Ticker = request.Ticker,
                                Type = request.Type,
                                Period = (int.Parse(request.Period!) + i).ToString(),
                                Value = StringUtils.ToDouble(part),
                                ExtData = request.ExtData ?? string.Empty
                            });
                    }

                    await storageApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
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

                await storageApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
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

                await storageApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);
                return new CreateOrUpdateAnalyticFundamentalParameterResponse();
            }

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<DeleteAnalyticFundamentalParameterResponse> DeleteFundamentalParameterAsync(DeleteAnalyticFundamentalParameterRequest request)
        {
            await storageApiClient.DeleteFundamentalParameterAsync(
                new()
                {
                    FundamentalParameters = [new() { Ticker = request.Ticker, Type = request.Type, Period = request.Period }]
                });

            return new();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments.Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var analyticInstruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).ToList();
            bool showInPortfolio = (await parameterRepository.GetParameterValueAsync(KnownParameters.ShowInPortfolio)) == "true";

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var items = new List<AnalyticFundamentalParameterListItem>();

            foreach (var instrument in instruments)
            {
                var item = new AnalyticFundamentalParameterListItem
                {
                    Periods = periods,
                    Ticker = instrument.Ticker,
                    Sector = analyticInstruments.First(x => x.Ticker == instrument.Ticker)?.Sector ?? string.Empty,
                    Name = instrument.Name,
                    InPortfolio = instrument.InPortfolio && showInPortfolio,
                    BenchmarkChange = analyseDataContext.GetBenchmarkChange(instrument.Ticker),
                    Moex = analyseDataContext.GetMoexIndexShare(instrument.Ticker),
                    Concept = analyseDataContext.GetExtData(instrument.Ticker)?.Concept,
                    Report = analyseDataContext.GetReport(instrument.Ticker)
                };

                var metrics = analyseDataContext.GetFundamentalMetrics(instrument.Ticker);                

                for (int i = 0; i < metrics.Count; i++)
                {
                    item.Price.Add(metrics[i].Price);
                    item.NumberShares.Add(metrics[i].NumberShares);
                    item.Ebitda.Add(metrics[i].Ebitda);
                    item.OwnCapital.Add(metrics[i].OwnCapital);
                    item.MarketCap.Add(metrics[i].MarketCap);
                    item.Ev.Add(metrics[i].Ev);
                    item.Dividend.Add(metrics[i].Dividend);
                    item.Assets.Add(metrics[i].Assets);
                    item.Liabilities.Add(metrics[i].Liabilities);

                    item.Pe.Add(await analyseParameterFactory.CreatePeAsync(instrument.Ticker, periods[i]));
                    item.Pbv.Add(await analyseParameterFactory.CreatePbvAsync(instrument.Ticker, periods[i]));
                    item.Revenue.Add(await analyseParameterFactory.CreateRevenueAsync(instrument.Ticker, periods[i]));
                    item.NetProfit.Add(await analyseParameterFactory.CreateNetProfitAsync(instrument.Ticker, periods[i]));
                    item.Fcf.Add(await analyseParameterFactory.CreateFcfAsync(instrument.Ticker, periods[i]));
                    item.Eps.Add(await analyseParameterFactory.CreateEpsAsync(instrument.Ticker, periods[i]));
                    item.NetDebt.Add(await analyseParameterFactory.CreateNetDebtAsync(instrument.Ticker, periods[i]));
                    item.Roa.Add(await analyseParameterFactory.CreateRoaAsync(instrument.Ticker, periods[i]));
                    item.Roe.Add(await analyseParameterFactory.CreateRoeAsync(instrument.Ticker, periods[i]));
                    item.EvEbitda.Add(await analyseParameterFactory.CreateEvEbitdaAsync(instrument.Ticker, periods[i]));
                    item.NetDebtEbitda.Add(await analyseParameterFactory.CreateNetDebtEbitdaAsync(instrument.Ticker, periods[i]));
                    item.DebtRatio.Add(await analyseParameterFactory.CreateDebtRatioAsync(instrument.Ticker, periods[i]));
                    item.DebtEquity.Add(await analyseParameterFactory.CreateDebtEquityAsync(instrument.Ticker, periods[i]));
                    item.EbitdaRevenue.Add(await analyseParameterFactory.CreateEbitdaRevenueAsync(instrument.Ticker, periods[i]));
                    item.OwnCapitalNumberShares.Add(await analyseParameterFactory.CreateOwnCapitalNumberSharesAsync(instrument.Ticker, periods[i]));
                    item.DividendYield.Add(await analyseParameterFactory.CreateDividendYieldAsync(instrument.Ticker, periods[i]));
                    item.DeltaMinMax.Add(await analyseParameterFactory.CreateDeltaMinMaxAsync(instrument.Ticker, periods[i]));

                    item.FillData = analyseDataContext.GetFillFundamental(instrument.Ticker) ?? false;
                }

                item.Score = await fundamentalScoreService.GetFundamentalScoreAsync(instrument.Ticker);

                items.Add(item);
            }

            var response = new GetAnalyticFundamentalParameterListResponse { FundamentalParameters = [.. items.OrderByDescending(x => x.Score?.Score.Value)] };

            int number = 1; foreach (var item in response.FundamentalParameters) item.Number = number++;

            response.TotalCount = items.Count;
            response.NoFillDataCount = items.Count(x => !x.FillData);
            response.NoFillDataTickers = string.Join(", ", items.Where(x => !x.FillData).Select(x => x.Ticker).ToList());

            return response;
        }

        /// <inheritdoc />
        public async Task<GetFundamentalBySectorResponse> GetFundamentalBySectorAsync(GetFundamentalBySectorRequest request)
        {
            List<string> periods = [.. (await parameterRepository.GetParameterValueAsync(KnownParameters.Periods))!.Split(';')];
            var fundamentalParameters = (await storageApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;
            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).Where(x => x.Sector == request.Sector).OrderBy(x => x.Ticker).ToList();
            bool showInPortfolio = (await parameterRepository.GetParameterValueAsync(KnownParameters.ShowInPortfolio)) == "true";
            var fundamentalRatings = (await GetFundamentalRatingListAsync(new() { Sector = request.Sector })).Items;
            var tickers = fundamentalRatings.Select(x => x.Ticker).ToList();
            var priceData = await dataService.GetClosePriceDataAsync(tickers);

            var response = new GetFundamentalBySectorResponse { Sector = request.Sector };

            foreach (var ticker in tickers)
            {
                var priceDataMonth = priceData[ticker].Where(x => x.Date.Day == 1).OrderBy(x => x.Date).ToList();

                response.PriceDiagram.Add(
                    new()
                    {
                        Ticker = ticker,
                        Name = instruments.Find(x => x.Ticker == ticker)!.Name,
                        InPortfolio = instruments.Find(x => x.Ticker == ticker)!.InPortfolio && showInPortfolio,
                        Data = [.. priceDataMonth.Select(x => new FundamentalBySectorDateValue { Date = x.Date.ToString(), Value = x.Value })]
                    });
            }

            foreach (var ticker in tickers)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.NetDebt, periods);

                response.NetDebtDiagram.Add(
                    new ()
                    {
                        Ticker = ticker,
                        Name = instruments.Find(x => x.Ticker == ticker)!.Name,
                        InPortfolio = instruments.Find(x => x.Ticker == ticker)!.InPortfolio && showInPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new FundamentalBySectorDateValue { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var ticker in tickers)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.NetProfit, periods);

                response.NetProfitDiagram.Add(
                    new ()
                    {
                        Ticker = ticker,
                        Name = instruments.Find(x => x.Ticker == ticker)!.Name,
                        InPortfolio = instruments.Find(x => x.Ticker == ticker)!.InPortfolio && showInPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new FundamentalBySectorDateValue { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var ticker in tickers)
            {
                var fundamentalParameterValues = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.Dividend, periods);

                response.DividendDiagram.Add(
                    new ()
                    {
                        Ticker = ticker,
                        Name = instruments.Find(x => x.Ticker == ticker)!.Name,
                        InPortfolio = instruments.Find(x => x.Ticker == ticker)!.InPortfolio && showInPortfolio,
                        Data = [.. fundamentalParameterValues.Select(x => new FundamentalBySectorDateValue { Date = x.Period, Value = x.Value })]
                    });
            }

            foreach (var ticker in tickers)
            {
                var ebitda = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.Ebitda, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var ev = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.Ev, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var netDebt = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.NetDebt, periods).LastOrDefault(x => x.Value != 0.0).Value;
                var marketCap = GetFundamentalParameterValues(fundamentalParameters, ticker, KnownFundamentalParameterTypes.MarketCap, periods).LastOrDefault(x => x.Value != 0.0).Value;

                response.BubbleDiagram.Add(
                    new ()
                    {
                        Ticker = ticker,
                        EvEbitda = ev.Div(ebitda).RoundTo(2),
                        NetDebtEbitda = netDebt.Div(ebitda).RoundTo(2),
                        MarketCap = marketCap
                    });
            }

            response.FundamentalRatingItems = fundamentalRatings;

            return response;
        }

        /// <inheritdoc />
        public async Task<GetFundamentalByCompanyResponse> GetFundamentalByCompanyAsync(GetFundamentalByCompanyRequest request)
        {
            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            var tickers = instruments.Select(x => x.Ticker).ToList();
            var instrument = instruments.Find(x => x.Ticker == request.Ticker)!;

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dividend = analyseDataContext.GetDividend(instrument.Ticker);
            var fundamentalScore = await fundamentalScoreService.GetFundamentalScoreAsync(instrument.Ticker);
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

            var candles = analyseDataContext.GetCandles(instrument.Ticker).Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddYears(-1)));
            double maxPrice = candles.Select(x => x.Close).Max();
            double lastCandlePrice = candles.Last().Close;
            double fallingFromMax = (-1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0).RoundTo(2);
            
            bool showInPortfolio = (await parameterRepository.GetParameterValueAsync(KnownParameters.ShowInPortfolio)) == "true";

            var response = new GetFundamentalByCompanyResponse
            {
                Ticker = instrument.Ticker,
                InPortfolio = instrument.InPortfolio && showInPortfolio,
                Name = instrument.Name,
                Sector = instrument.Sector,
                Price = price,
                TrendState = trendState.Message,
                FallingFromMax = fallingFromMax,
                Dividend = dividend,
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
                response.NetDebtDiagramData.Add(new() { X = metric.Period, Y = metric.NetDebt });
                response.FcfDiagramData.Add(new() { X = metric.Period, Y = metric.Fcf });
                response.EpsDiagramData.Add(new() { X = metric.Period, Y = metric.Eps });
            }

            // Сравнительная диаграмма по мультипликаторам среди сектора
            var sectorInstruments = instruments.Where(x => x.Sector == instrument.Sector).ToList();
            List<Instrument> orderedSectorInstruments = [sectorInstruments.Find(x => x.Ticker == instrument.Ticker), .. sectorInstruments.Where(x => x.Ticker != instrument.Ticker).OrderBy(x => x.Ticker).ToList()];

            foreach (var sectorInstrument in orderedSectorInstruments)
            {
                var fundamentalMetric = analyseDataContext.GetFundamentalMetric(sectorInstrument.Ticker)!;

                response.PeSectorDiagramData.Add(new() { X = sectorInstrument.Ticker, Y = fundamentalMetric.Pe });
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

        /// <inheritdoc />
        public async Task<GetFundamentalRatingListResponse> GetFundamentalRatingListAsync(GetFundamentalRatingListRequest request)
        {
            bool showInPortfolio = (await parameterRepository.GetParameterValueAsync(KnownParameters.ShowInPortfolio)) == "true";

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? [])
                .Where(x => x.Type == KnownInstrumentTypes.Share)                 
                .ToList();

            if (request.Sector is not null)
                instruments = [.. instruments.Where(x => x.Sector == request.Sector)];

            var sectors = (await instrumentService.GetSectorListAsync(new ())).Sectors;
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var scores = new List<(string Ticker, FundamentalScore Score)>();

            foreach (var instrument in instruments)
            {
                var score = await fundamentalScoreService.GetFundamentalScoreAsync(instrument.Ticker);

                if (score is not null)
                    scores.Add((instrument.Ticker, score));
            };

            var items = new List<FundamentalRatingItem>();

            foreach (var sector in sectors)
            {
                var sectorTickers = instruments.Where(x => x.Sector == sector).Select(x => x.Ticker).ToList();
                var sectorScores = scores.Where(x => sectorTickers.Contains(x.Ticker)).ToList();

                foreach (var (ticker, score) in sectorScores)
                {
                    var instrument = instruments.Find(x => x.Ticker == ticker);

                    var candles = analyseDataContext.GetCandles(ticker).Where(x => x.Date > DateOnly.FromDateTime(DateTime.Today.AddYears(-1)));
                    double maxPrice = candles.Select(x => x.Close).Max();
                    double lastCandlePrice = candles.Last().Close;
                    double fallingFromMax = -1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0;

                    var ratingItem = new FundamentalRatingItem
                    {
                        Ticker = ticker,
                        Name = instrument?.Name ?? string.Empty,
                        Sector = instrument?.Sector ?? string.Empty,
                        InPortfolio = (instrument?.InPortfolio ?? false) && showInPortfolio,
                        Score = score,
                        Metric = analyseDataContext.GetFundamentalMetric(ticker),
                        FallingFromMax = fallingFromMax.RoundTo(2)
                    };

                    items.Add(ratingItem);
                }
            }

            List<FundamentalRatingItem> filteredItems = items;

            if (request.FilterType is not null)
            {
                if (string.IsNullOrEmpty(request.FilterType))
                    filteredItems = items;

                if (request.FilterType == "HighDividend")
                    filteredItems = [.. items.Where(x => IsHighDividend(x.Score))];

                if (request.FilterType == "LowDebt")
                    filteredItems = [.. items.Where(x => IsLowDebt(x.Score))];

                if (request.FilterType == "GrowingNetProfit")
                    filteredItems = [.. items.Where(x => IsGrowingNetProfit(x.Score))];
            }

            var response = new GetFundamentalRatingListResponse { Items = [.. filteredItems.OrderByDescending(x => x.Score?.Score.Value)] };

            int number = 1; foreach (var item in response.Items) item.Number = number++;

            response.TickerList = string.Join(", ", response.Items.Select(x => $"\"{x.Ticker}\"").ToList());

            return response;

            bool IsHighDividend(FundamentalScore? score)
            {
                if (score is null) return false;

                bool highScore = score.Score?.ColorFill == KnownColors.Green;
                highScore |= score.Score?.ColorFill == KnownColors.Yellow;

                bool dividendYieldCriteriaIsGood = score.DividendYield?.ColorFill == KnownColors.Green;
                dividendYieldCriteriaIsGood |= score.DividendYield?.ColorFill == KnownColors.LightGreen;

                bool dividendAristocratCriteriaIsGood = score.DividendAristocrat?.ColorFill == KnownColors.Green;
                dividendAristocratCriteriaIsGood |= score.DividendAristocrat?.ColorFill == KnownColors.LightGreen;

                return dividendYieldCriteriaIsGood && dividendAristocratCriteriaIsGood && highScore;
            }

            bool IsLowDebt(FundamentalScore? score)
            {
                if (score is null) return false;

                bool highScore = score.Score?.ColorFill == KnownColors.Green;
                highScore |= score.Score?.ColorFill == KnownColors.Yellow;

                bool netDebtCriteriaIsGood = score.NetDebt?.ColorFill == KnownColors.Green;
                netDebtCriteriaIsGood |= score.NetDebt?.ColorFill == KnownColors.LightGreen;
                netDebtCriteriaIsGood |= score.NetDebt?.ColorFill == KnownColors.Yellow;

                bool netDebtEbitdaCriteriaIsGood = score.NetDebtEbitda?.ColorFill == KnownColors.Green;
                netDebtEbitdaCriteriaIsGood |= score.NetDebtEbitda?.ColorFill == KnownColors.LightGreen;

                bool debtRatioCriteriaIsGood = score.DebtRatio?.ColorFill == KnownColors.Green;
                debtRatioCriteriaIsGood |= score.DebtRatio?.ColorFill == KnownColors.LightGreen;

                bool debtEquityCriteriaIsGood = score.DebtEquity?.ColorFill == KnownColors.Green;
                debtEquityCriteriaIsGood |= score.DebtEquity?.ColorFill == KnownColors.LightGreen;

                return netDebtCriteriaIsGood && netDebtEbitdaCriteriaIsGood && debtRatioCriteriaIsGood && debtEquityCriteriaIsGood && highScore;
            }

            bool IsGrowingNetProfit(FundamentalScore? score)
            {
                if (score is null) return false;

                bool highScore = score.Score?.ColorFill == KnownColors.Green;
                highScore |= score.Score?.ColorFill == KnownColors.Yellow;

                bool netProfitCriteriaIsGood = score.NetProfit?.ColorFill == KnownColors.Green;
                netProfitCriteriaIsGood |= score.NetProfit?.ColorFill == KnownColors.LightGreen;

                return netProfitCriteriaIsGood && highScore;
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
    }
}
