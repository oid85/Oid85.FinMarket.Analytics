using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioBacktestService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IPortfolioService portfolioService,
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
        private double _moneySum = 0.0;

        public async Task<PortfolioBacktestResponse> PortfolioBacktestAsync(PortfolioBacktestRequest request)
        {
            _historyPeriodInYears = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestHistoryPeriodInYears)) ?? "0");
            _periodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestPeriodInDays)) ?? "0");
            _addMoneyPeriodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestAddMoneyPeriodInDays)) ?? "0");
            _startMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestStartMoneySum)) ?? "0.0");
            _addMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.BacktestAddMoneySum)) ?? "0.0");

            var portfolioEquitySeries = await GetPortfolioSeriesAsync(
                (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient), 
                "Портфель", KnownColors.Green, false);

            var portfolioEquityBareSeries = await GetPortfolioSeriesAsync(
                (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "Портфель без ребалансировок и пополнений", KnownColors.Blue, true);

            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var response = new PortfolioBacktestResponse 
            { 
                Series = 
                [
                    portfolioEquitySeries,
                    portfolioEquityBareSeries,
                    msftrSeries
                ],
                Yield = GetAverageYearYieldPercent(portfolioEquitySeries),
                MaxDrawdown = GetMaxDrawdownPercent(portfolioEquitySeries),
                DividendSum = _dividendSum.RoundTo(2),
                MoneySum = _moneySum.RoundTo(2)
            };

            return response;

            double GetAverageYearYieldPercent(PortfolioRebalanceSeries series)
            {
                double first = series.Data.First().Value ?? 0.0;
                double last = series.Data.Last().Value ?? 0.0;

                if (last == 0.0) return 0.0;

                return ((last - first) / first * 100.0 / _historyPeriodInYears).RoundTo(2);
            }

            double GetMaxDrawdownPercent(PortfolioRebalanceSeries series)
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

                return drawdown.Min();
            }
        }

        private async Task<PortfolioRebalanceSeries> GetPortfolioSeriesAsync(
            Dictionary<string, double> weights, string portfolioName, string color, bool isBare)
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

            var dates = DateUtils.GetDates(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _historyPeriodInYears)),
                DateOnly.FromDateTime(DateTime.Today));

            var portfolioSeries = new PortfolioRebalanceSeries()
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
                        Value = totalSum.RoundTo(2)
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
                    if (isBare) return;

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
                    if (isBare) return;

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
                    if (i != 0 && isBare) return;

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

        private async Task<PortfolioRebalanceSeries> GetIndexSeriesAsync(
            string indexTicker, string portfolioName, string color)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dates = DateUtils.GetDates(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _historyPeriodInYears)),
                DateOnly.FromDateTime(DateTime.Today));

            var price = analyseDataContext.GetPrice(indexTicker, dates[0])!.Value;
            var size = Math.Truncate(_startMoneySum / price);

            var series = new PortfolioRebalanceSeries
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
                        Value = (size * analyseDataContext.GetPrice(indexTicker, dates[i])).RoundTo(2)
                    });

            return series;
        }
    }
}
