using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioRebalanceService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IPortfolioService portfolioService,
        IInstrumentService instrumentService,
        IDataService dataService,
        IStorageApiClient storageApiClient)
        : IPortfolioRebalanceService
    {
        private int _rebalanceHistoryPeriodInYears;
        private int _rebalancePeriodInDays;
        private int _rebalanceAddMoneyPeriodInDays;
        private double _rebalanceStartMoneySum;
        private double _rebalanceAddMoneySum;

        public async Task<PortfolioRebalanceResponse> PortfolioRebalanceAsync(PortfolioRebalanceRequest request)
        {
            _rebalanceHistoryPeriodInYears = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.RebalanceHistoryPeriodInYears)) ?? "0");
            _rebalancePeriodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.RebalancePeriodInDays)) ?? "0");
            _rebalanceAddMoneyPeriodInDays = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.RebalanceAddMoneyPeriodInDays)) ?? "0");
            _rebalanceStartMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.RebalanceStartMoneySum)) ?? "0.0");
            _rebalanceAddMoneySum = Convert.ToDouble((await parameterRepository.GetParameterValueAsync(KnownParameters.RebalanceAddMoneySum)) ?? "0.0");

            var portfolioSeries = await GetPortfolioSeriesAsync(
                (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient), 
                "Портфель", KnownColors.Green);                        
            
            var msftrSeries = await GetIndexSeriesAsync(KnownIndexTickers.MCFTR, $"Индекс полн. дох. MCFTR", KnownColors.Orange);

            var response = new PortfolioRebalanceResponse 
            { 
                Series = 
                [
                    portfolioSeries,          
                    msftrSeries
                ],
                Yield = GetYield(portfolioSeries)
            };

            return response;

            double GetYield(PortfolioRebalanceSeries portfolioSeries)
            {
                double first = portfolioSeries.Data.First().Value ?? 0.0;
                double last = portfolioSeries.Data.Last().Value ?? 0.0;

                if (last == 0.0)
                    return 0.0;

                return ((last - first) / first * 100.0 / _rebalanceHistoryPeriodInYears).RoundTo(2);
            }
        }

        private async Task<PortfolioRebalanceSeries> GetPortfolioSeriesAsync(
            Dictionary<string, double> weights, string portfolioName, string color)
        {
            var tickers = weights.Keys.ToList();
            var dividends = (await storageApiClient.GetDividendListAsync(new())).Result.Dividends.Where(x => tickers.Contains(x.Ticker)).ToList();
            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => tickers.Contains(x.Ticker)).Where(x => x.InPortfolio).ToList();
            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync()).Where(x => tickers.Contains(x.Ticker)).ToList();
            var sizes = tickers.ToDictionary(k => k, v => 0);
            var costs = tickers.ToDictionary(k => k, v => 0.0);
            var prices = tickers.ToDictionary(k => k, v => 0.0);
            var lots = storageInstruments.ToDictionary(k => k.Ticker, v => v.Lot ?? 1);
            double money = _rebalanceStartMoneySum;
            double totalSum = _rebalanceStartMoneySum;

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dates = DateUtils.GetDates(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _rebalanceHistoryPeriodInYears)),
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
                    foreach (var ticker in tickers)
                    {
                        var dividend = dividends.Find(x => x.Ticker == ticker && x.Date == dates[i]);

                        if (dividend is not null)
                            money += sizes[ticker] * dividend.Value;
                    }
                }

                void AddMoney()
                {
                    if (i % _rebalanceAddMoneyPeriodInDays == 0)
                    {
                        money += _rebalanceAddMoneySum;
                    }
                }

                void UpdateTotalSum()
                {
                    totalSum = costs.Values.Sum() + money;
                }

                void Rebalance()
                {
                    if (i % _rebalancePeriodInDays == 0)
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
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _rebalanceHistoryPeriodInYears)),
                DateOnly.FromDateTime(DateTime.Today));

            var price = analyseDataContext.GetPrice(indexTicker, dates[0])!.Value;
            var size = Math.Truncate(_rebalanceStartMoneySum / price);

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
