using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioBacktestService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IPortfolioService portfolioService,
        IBondAnalyseService bondAnalyseService,
        IFundamentalService fundamentalService,
        IInstrumentService instrumentService,
        IDataService dataService,
        IStorageApiClient storageApiClient)
        : IPortfolioBacktestService
    {
        private int _historyPeriodInYears;
        private int _periodInDays;
        private int _addMoneyPeriodInDays;
        private double _startMoneySum;
        private double _addMoneySum;

        private double _dividendSum = 0.0;
        private double _couponSum = 0.0;
        private double _moneySum = 0.0;

        private DateOnly _startDate;
        private DateOnly _endDate;

        public async Task<PortfolioBacktestResponse> PortfolioBacktestAsync(PortfolioBacktestRequest request)
        {
            _historyPeriodInYears = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestHistoryPeriodInYears)) ?? "0");
            _periodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestPeriodInDays)) ?? "0");
            _addMoneyPeriodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestAddMoneyPeriodInDays)) ?? "0");
            _startMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestStartMoneySum)) ?? "0.0");
            _addMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestAddMoneySum)) ?? "0.0");

            _startDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _historyPeriodInYears));            
            _endDate = DateOnly.FromDateTime(DateTime.Today);

            if (request.PortfolioName is null)
                return await LifePortfolioBacktestAsync();

            if (request.PortfolioName == string.Empty)
                return await LifePortfolioBacktestAsync();

            if (request.PortfolioName == "LifePortfolio")
                return await LifePortfolioBacktestAsync();

            if (request.PortfolioName == "HighDividend")
                return await HighDividendFundamentalScorePortfolioBacktestAsync();

            if (request.PortfolioName == "LowDebt")
                return await LowDebtFundamentalScorePortfolioBacktestAsync();

            if (request.PortfolioName == "GrowingNetProfit")
                return await GrowingNetProfitFundamentalScorePortfolioBacktestAsync();

            if (request.PortfolioName == "Bond")
                return await BondPortfolioForwardtestAsync();

            return await LifePortfolioBacktestAsync();
        }

        private async Task<PortfolioBacktestResponse> BondPortfolioForwardtestAsync()
        {
            _startDate = DateOnly.FromDateTime(DateTime.Today);
            _endDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

            var keyRate = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).Last().Value;

            var bondAnalyseItems = (await bondAnalyseService.GetBondAnalyseAsync(new()))
                .Items
                .Where(x => x.IsFloatingCoupon != "да")
                .Where(x => x.Yield >= keyRate * 1.2)
                .OrderByDescending(x => x.Yield)
                .ToList();

            var instruments = await instrumentService.GetInstrumentListAsync();
            var tickers = bondAnalyseItems.Select(x => x.Ticker).ToList();            
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();
            var dates = DateUtils.GetDates(_startDate, _endDate);
            var sizes = tickers.ToDictionary(k => k, v => 0);
            var costs = tickers.ToDictionary(k => k, v => 0.0);
            var prices = bondAnalyseItems.ToDictionary(k => k.Ticker, v => v.Price);
            var weights = tickers.ToDictionary(k => k, v => 1.0);
            var lots = tickers.ToDictionary(k => k, v => 1);
            double money = _startMoneySum;
            double totalSum = _startMoneySum;
            var bondCoupons = await dataService.GetBondCouponsAsync(tickers);

            var bondSeries = new PortfolioBacktestSeries()
            {
                Name = "Облигации",
                Color = KnownColors.Green,
                ColorFill = KnownColors.Green
            };

            var monthCouponSeries = new PortfolioBacktestSeries()
            {
                Name = "Месячный купон (x100)",
                Color = KnownColors.DarkGreen,
                ColorFill = KnownColors.DarkGreen
            };

            for (int i = 0; i < dates.Count; i++)
            {
                AddCoupons();
                AddMoney();
                UpdateCosts();
                UpdateTotalSum();
                Rebalance();

                bondSeries.Data.Add(
                    new()
                    {
                        Date = dates[i],
                        Value = (totalSum / 1_000.0).RoundTo(2)
                    });

                monthCouponSeries.Data.Add(
                    new()
                    {
                        Date = dates[i],
                        Value = (_couponSum / 1_00.0 / ((i + 1) / 30.0)).RoundTo(2)
                    });

                void UpdateCosts()
                {
                    foreach (var ticker in tickers)
                        costs[ticker] = prices[ticker] * sizes[ticker];
                }

                void UpdateSizes()
                {
                    double baseUnit = totalSum / weights.Values.Sum();

                    foreach (var ticker in tickers)
                    {
                        if (prices[ticker] == 0.0)
                        {
                            costs[ticker] = 0.0;
                            continue;
                        }

                        double tickerCost = baseUnit * weights[ticker];
                        double tickerSize = tickerCost / prices[ticker];
                        tickerSize /= lots[ticker];
                        tickerSize = Math.Truncate(tickerSize);
                        tickerSize *= lots[ticker];
                        sizes[ticker] = Convert.ToInt32(tickerSize);
                    }
                }

                void AddCoupons()
                {
                    foreach (var ticker in tickers)
                    {
                        var coupons = bondCoupons[ticker];
                        var coupon = coupons.Find(x => x.Ticker == ticker && x.CouponDate == dates[i]);

                        if (coupon is not null)
                        {
                            double couponPay = sizes[ticker] * coupon.PayOneBond;
                            money += couponPay;
                            _couponSum += couponPay;
                        }
                    }
                }

                void AddMoney()
                {
                    if (i % _addMoneyPeriodInDays == 0)
                    {
                        money += _addMoneySum;
                        _moneySum += _addMoneySum;
                    }
                }

                void UpdateTotalSum()
                {
                    totalSum = costs.Values.Sum() + money;
                }

                void Rebalance()
                {
                    UpdateSizes();
                    UpdateCosts();
                    money = totalSum - costs.Values.Sum();
                }
            }

            var response = new PortfolioBacktestResponse
            {
                Series =
                [
                    bondSeries,
                    monthCouponSeries
                ],
                Yield = GetAverageYearYieldPercent(bondSeries),
                CouponSum = _couponSum.RoundTo(2),
                MoneySum = _moneySum.RoundTo(2)
            };

            response.PortfolioPositions = bondAnalyseItems
                .Select(x =>
                new PortfolioPositionItem
                {
                    Ticker = x.Ticker,
                    Sector = instruments.Find(xx => xx.Ticker == x.Ticker)?.Sector ?? string.Empty,
                    Name = x.Name,
                    CurrentDividendYield = x.Yield
                })
                .ToList();

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Percent = (100.0 / (response.PortfolioPositions.Sum(x => x.ResultCoefficient))).RoundTo(2);

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            return response;
        }

        private async Task<PortfolioBacktestResponse> LifePortfolioBacktestAsync()
        {
            var positions = (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions;

            var portfolioEquitySeries = await GetPortfolioSeriesAsync(
                positions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "Портфель", KnownColors.Green, true);

            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var drawdownValues = GetDrawdownValues(portfolioEquitySeries);

            var response = new PortfolioBacktestResponse
            {
                Series =
                [
                    portfolioEquitySeries,
                    msftrSeries
                ],
                Yield = GetAverageYearYieldPercent(portfolioEquitySeries),
                MaxDrawdown = drawdownValues.Min(),
                CurrentDrawdown = drawdownValues.Last(),
                DividendSum = _dividendSum.RoundTo(2),
                MoneySum = _moneySum.RoundTo(2)
            };

            response.PortfolioPositions = positions
                .Select(x =>
                new PortfolioPositionItem
                {
                    Ticker = x.Ticker,
                    Sector = x.Sector,
                    Name = x.Name,
                    FundamentalScoreCoefficient = x.FundamentalScoreCoefficient,
                    DividendCoefficient = x.DividendCoefficient,
                    MarketCapCoefficient = x.MarketCapCoefficient,
                    ResultCoefficient = x.ResultCoefficient,
                    Percent = x.Percent,
                    CurrentDividendYield = x.CurrentDividendYield
                })
                .ToList();

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            return response;
        }
        
        private async Task<PortfolioBacktestResponse> HighDividendFundamentalScorePortfolioBacktestAsync()
        {
            var response = new PortfolioBacktestResponse();

            var fundamentalRatingListItems = (await fundamentalService.GetFundamentalRatingListAsync(new() { FilterType = "HighDividend" })).Items;

            var portfolioPositions = new List<PortfolioPositionItem>();

            foreach (var fundamentalRatingListItem in fundamentalRatingListItems)
            {
                var fundamentalScoreCoefficient = fundamentalRatingListItem.Score?.Score.Value.RoundTo(2) ?? 1.0;
                var dividendCoefficient = await GetDividendCoefficient(fundamentalRatingListItem.Metric?.DividendYield);
                var marketCapCoefficient = fundamentalRatingListItem.Score?.MarketCap?.Ratio ?? 1.0;

                portfolioPositions.Add(
                    new PortfolioPositionItem
                    {
                        Ticker = fundamentalRatingListItem.Ticker,
                        Sector = fundamentalRatingListItem.Sector,
                        Name = fundamentalRatingListItem.Name,
                        FundamentalScoreCoefficient = fundamentalScoreCoefficient,
                        DividendCoefficient = dividendCoefficient,
                        MarketCapCoefficient = marketCapCoefficient,
                        ResultCoefficient = fundamentalScoreCoefficient * dividendCoefficient * marketCapCoefficient,
                        CurrentDividendYield = fundamentalRatingListItem.Metric?.DividendYield ?? 0.0
                    });
            }

            double sumResultCoefficient = portfolioPositions.Sum(x => x.ResultCoefficient);
            for (int i = 0; i < portfolioPositions.Count; i++)
                portfolioPositions[i].Percent = (portfolioPositions[i].ResultCoefficient / sumResultCoefficient * 100.0).RoundTo(2);

            response.PortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            var portfolioEquitySeries = await GetPortfolioSeriesAsync(
                response.PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "ТОП дивидендных фунд. рейт.", KnownColors.Green, true);

            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var drawdownValues = GetDrawdownValues(portfolioEquitySeries);

            response.Series = [portfolioEquitySeries, msftrSeries];
            response.Yield = GetAverageYearYieldPercent(portfolioEquitySeries);
            response.MaxDrawdown = drawdownValues.Min();
            response.CurrentDrawdown = drawdownValues.Last();
            response.DividendSum = _dividendSum.RoundTo(2);
            response.MoneySum = _moneySum.RoundTo(2);

            return response;
        }

        private async Task<PortfolioBacktestResponse> LowDebtFundamentalScorePortfolioBacktestAsync()
        {
            var response = new PortfolioBacktestResponse();

            var fundamentalRatingListItems = (await fundamentalService.GetFundamentalRatingListAsync(new() { FilterType = "LowDebt" })).Items;

            var portfolioPositions = new List<PortfolioPositionItem>();

            foreach (var fundamentalRatingListItem in fundamentalRatingListItems)
            {
                var fundamentalScoreCoefficient = fundamentalRatingListItem.Score?.Score.Value.RoundTo(2) ?? 1.0;
                var dividendCoefficient = await GetDividendCoefficient(fundamentalRatingListItem.Metric?.DividendYield);
                var marketCapCoefficient = fundamentalRatingListItem.Score?.MarketCap?.Ratio ?? 1.0;

                portfolioPositions.Add(
                    new PortfolioPositionItem
                    {
                        Ticker = fundamentalRatingListItem.Ticker,
                        Sector = fundamentalRatingListItem.Sector,
                        Name = fundamentalRatingListItem.Name,
                        FundamentalScoreCoefficient = fundamentalScoreCoefficient,
                        DividendCoefficient = dividendCoefficient,
                        MarketCapCoefficient = marketCapCoefficient,
                        ResultCoefficient = fundamentalScoreCoefficient * dividendCoefficient * marketCapCoefficient,
                        CurrentDividendYield = fundamentalRatingListItem.Metric?.DividendYield ?? 0.0
                    });
            }

            double sumResultCoefficient = portfolioPositions.Sum(x => x.ResultCoefficient);
            for (int i = 0; i < portfolioPositions.Count; i++)
                portfolioPositions[i].Percent = (portfolioPositions[i].ResultCoefficient / sumResultCoefficient * 100.0).RoundTo(2);

            response.PortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            var portfolioEquitySeries = await GetPortfolioSeriesAsync(
                response.PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "ТОП с низким долгом фунд. рейт.", KnownColors.Green, true);

            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var drawdownValues = GetDrawdownValues(portfolioEquitySeries);

            response.Series = [portfolioEquitySeries, msftrSeries];
            response.Yield = GetAverageYearYieldPercent(portfolioEquitySeries);
            response.MaxDrawdown = drawdownValues.Min();
            response.CurrentDrawdown = drawdownValues.Last();
            response.DividendSum = _dividendSum.RoundTo(2);
            response.MoneySum = _moneySum.RoundTo(2);

            return response;
        }

        private async Task<PortfolioBacktestResponse> GrowingNetProfitFundamentalScorePortfolioBacktestAsync()
        {
            var response = new PortfolioBacktestResponse();

            var fundamentalRatingListItems = (await fundamentalService.GetFundamentalRatingListAsync(new() { FilterType = "GrowingNetProfit" })).Items;

            var portfolioPositions = new List<PortfolioPositionItem>();

            foreach (var fundamentalRatingListItem in fundamentalRatingListItems)
            {
                var fundamentalScoreCoefficient = fundamentalRatingListItem.Score?.Score.Value.RoundTo(2) ?? 1.0;
                var dividendCoefficient = await GetDividendCoefficient(fundamentalRatingListItem.Metric?.DividendYield);
                var marketCapCoefficient = fundamentalRatingListItem.Score?.MarketCap?.Ratio ?? 1.0;

                portfolioPositions.Add(
                    new PortfolioPositionItem
                    {
                        Ticker = fundamentalRatingListItem.Ticker,
                        Sector = fundamentalRatingListItem.Sector,
                        Name = fundamentalRatingListItem.Name,
                        FundamentalScoreCoefficient = fundamentalScoreCoefficient,
                        DividendCoefficient = dividendCoefficient,
                        MarketCapCoefficient = marketCapCoefficient,
                        ResultCoefficient = fundamentalScoreCoefficient * dividendCoefficient * marketCapCoefficient,
                        CurrentDividendYield = fundamentalRatingListItem.Metric?.DividendYield ?? 0.0
                    });
            }

            double sumResultCoefficient = portfolioPositions.Sum(x => x.ResultCoefficient);
            for (int i = 0; i < portfolioPositions.Count; i++)
                portfolioPositions[i].Percent = (portfolioPositions[i].ResultCoefficient / sumResultCoefficient * 100.0).RoundTo(2);

            response.PortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            var portfolioEquitySeries = await GetPortfolioSeriesAsync(
                response.PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "ТОП с растущей ЧП фунд. рейт.", KnownColors.Green, true);

            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var drawdownValues = GetDrawdownValues(portfolioEquitySeries);

            response.Series = [ portfolioEquitySeries, msftrSeries ];
            response.Yield = GetAverageYearYieldPercent(portfolioEquitySeries);
            response.MaxDrawdown = drawdownValues.Min();
            response.CurrentDrawdown = drawdownValues.Last();
            response.DividendSum = _dividendSum.RoundTo(2);
            response.MoneySum = _moneySum.RoundTo(2);

            return response;
        }

        private async Task<PortfolioBacktestSeries> GetPortfolioSeriesAsync(
            Dictionary<string, double> weights, string portfolioName, string color, bool withServe)
        {
            var tickers = weights.Keys.ToList();
            var dividends = (await storageApiClient.GetDividendListAsync(new())).Result.Dividends.Where(x => tickers.Contains(x.Ticker)).ToList();
            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => tickers.Contains(x.Ticker)).Where(x => x.InPortfolio).ToList();
            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync()).Where(x => tickers.Contains(x.Ticker)).ToList();
            var sizes = tickers.ToDictionary(k => k, v => 0);
            var costs = tickers.ToDictionary(k => k, v => 0.0);
            var prices = tickers.ToDictionary(k => k, v => 0.0);
            var lots = storageInstruments.ToDictionary(k => k.Ticker, v => v.Lot ?? 1);
            double money = _startMoneySum;
            double totalSum = _startMoneySum;

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dates = DateUtils.GetDates(_startDate, _endDate);

            var portfolioSeries = new PortfolioBacktestSeries()
            {
                Name = $"{portfolioName}",
                Color = color,
                ColorFill = color
            };

            for (int i = 0; i < dates.Count; i++)
            {
                AddDividends();
                AddMoney();                
                UpdatePrices();
                UpdateCosts();
                UpdateTotalSum();
                Rebalance();

                portfolioSeries.Data.Add(
                    new()
                    {
                        Date = dates[i],
                        Value = (totalSum / 1_000.0).RoundTo(2)
                    });

                void UpdatePrices()
                {
                    foreach (var ticker in tickers)
                        prices[ticker] = analyseDataContext.GetPrice(ticker, dates[i]) ?? 0.0;
                }

                void UpdateCosts()
                {
                    foreach (var ticker in tickers)
                        costs[ticker] = prices[ticker] * sizes[ticker];
                }

                void UpdateSizes()
                {
                    double baseUnit = totalSum / weights.Values.Sum();

                    foreach (var ticker in tickers)
                    {
                        if (prices[ticker] == 0.0)
                        {
                            costs[ticker] = 0.0;
                            continue;
                        }

                        double tickerCost = baseUnit * weights[ticker];
                        double tickerSize = tickerCost / prices[ticker];
                        tickerSize /= lots[ticker];
                        tickerSize = Math.Truncate(tickerSize);
                        tickerSize *= lots[ticker];
                        sizes[ticker] = Convert.ToInt32(tickerSize);
                    }
                }

                void AddDividends()
                {
                    if (!withServe) return;

                    foreach (var ticker in tickers)
                    {
                        var dividend = dividends.Find(x => x.Ticker == ticker && x.Date == dates[i]);

                        if (dividend is not null)
                        {
                            double dividendPay = sizes[ticker] * dividend.Value;
                            money += dividendPay;
                            _dividendSum += dividendPay;
                        }
                    }
                }

                void AddMoney()
                {
                    if (!withServe) return;

                    if (i % _addMoneyPeriodInDays == 0)
                    {
                        money += _addMoneySum;
                        _moneySum += _addMoneySum;
                    }
                }

                void UpdateTotalSum()
                {
                    totalSum = costs.Values.Sum() + money;
                }

                void Rebalance()
                {
                    if (i != 0 && !withServe) return;

                    if (i % _periodInDays == 0)
                    {
                        UpdateSizes();
                        UpdateCosts();
                        money = totalSum - costs.Values.Sum();
                    }
                }
            }

            return portfolioSeries;
        }

        private async Task<PortfolioBacktestSeries> GetIndexSeriesAsync(
            string indexTicker, string portfolioName, string color)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dates = DateUtils.GetDates(_startDate, _endDate);

            var price = analyseDataContext.GetPrice(indexTicker, dates[0])!.Value;
            var size = Math.Truncate(_startMoneySum / price);

            var series = new PortfolioBacktestSeries
            {
                Name = $"{portfolioName}",
                Color = color,
                ColorFill = color
            };

            for (int i = 0; i < dates.Count; i++)
                series.Data.Add(
                    new()
                    {
                        Date = dates[i],
                        Value = ((size * analyseDataContext.GetPrice(indexTicker, dates[i])) / 1_000.0).RoundTo(2)
                    });

            return series;
        }

        private double GetAverageYearYieldPercent(PortfolioBacktestSeries series)
        {
            double first = series.Data.First().Value ?? 0.0;
            double last = series.Data.Last().Value ?? 0.0;

            if (last == 0.0) return 0.0;

            var years = (_endDate.ToDateTime(TimeOnly.MinValue) - _startDate.ToDateTime(TimeOnly.MaxValue)).TotalDays / 365.0;

            return ((last - first) / first * 100.0 / years).RoundTo(2);
        }

        private static List<double> GetDrawdownValues(PortfolioBacktestSeries series)
        {
            List<double> equity = [.. series.Data.Select(x => x.Value ?? 0.0)];
            List<double> drawdown = [];

            for (int i = 0; i < equity.Count; i++)
            {
                if (i == 0)
                    drawdown.Add(0.0);

                else
                {
                    var maxEquity = equity.Take(i).Max();
                    drawdown.Add(equity[i] >= maxEquity ? 0.0 : ((equity[i] - maxEquity) / maxEquity * 100.0).RoundTo(2));
                }
            }

            return drawdown;
        }

        private async Task<double> GetDividendCoefficient(double? dividendYield)
        {
            if (!dividendYield.HasValue)
                return 1.0;

            var keyRates = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();
            double currentKeyRate = keyRates.Last().Value ?? 0.0;
            
            double hiLimitCoefficient = 3.0;
            double loLimitCoefficient = 2.0;
            double hiLimitYield = currentKeyRate;
            double loLimitYield = hiLimitYield / 3.0 * 2.0;

            double dividendCoefficient = 1.0;

            if (dividendYield.Value >= hiLimitYield) dividendCoefficient = hiLimitCoefficient;
            else if (dividendYield.Value <= loLimitYield) dividendCoefficient = 1.0;
            else dividendCoefficient = (dividendYield.Value - loLimitYield) * (hiLimitCoefficient - loLimitCoefficient) / (hiLimitYield - loLimitYield) + loLimitCoefficient;

            return dividendCoefficient;
        }
    }
}
