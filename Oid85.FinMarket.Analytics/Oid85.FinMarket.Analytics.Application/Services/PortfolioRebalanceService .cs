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
        IPortfolioService portfolioService,
        IEtfPortfolioService etfPortfolioService,
        IInstrumentService instrumentService,
        IDataService dataService,
        IStorageApiClient storageApiClient)
        : IPortfolioRebalanceService
    {
        private readonly int _historyPeriod = 5;
        private readonly int _rebalancePeriod = 15;
        private readonly int _addMoneyPeriod = 15;
        private readonly double _startMoney = 5_000_000.0;
        private readonly double _addMoney = 25_000.0;

        public async Task<PortfolioRebalanceResponse> PortfolioRebalanceAsync(PortfolioRebalanceRequest request)
        {
            var portfolioSeries = await GetPortfolioSeriesAsync(
                (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient), 
                "Акции портфеля",
                KnownColors.Green);

            var portfolioEqualPartsSeries = await GetPortfolioSeriesAsync(
                (await portfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => 1.0),
                "Акции равными долями",
                KnownColors.Green);

            var etfPortfolioSeries = await GetPortfolioSeriesAsync(
                (await etfPortfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient),
                "ETF портфеля",
                KnownColors.Blue);

            var etfEqualPartsPortfolioSeries = await GetPortfolioSeriesAsync(
                (await etfPortfolioService.GetPortfolioPositionListAsync(new())).PortfolioPositions.ToDictionary(k => k.Ticker, v => 1.0),
                "ETF равными долями",
                KnownColors.Blue);

            var msftrSeries = await GetIndexSeriesAsync(
                KnownIndexTickers.MCFTR, 
                $"Индекс полн. дох. MCFTR",
                KnownColors.Orange);

            var moexSeries = await GetIndexSeriesAsync(
                KnownIndexTickers.IMOEX, 
                $"Индекс IMOEX",
                KnownColors.Orange);

            var response = new PortfolioRebalanceResponse 
            { 
                Series = 
                [
                    portfolioSeries,
                    portfolioEqualPartsSeries,
                    etfPortfolioSeries,
                    etfEqualPartsPortfolioSeries,
                    msftrSeries, 
                    moexSeries
                ] 
            };

            return response;
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
            double money = _startMoney;
            double totalSum = _startMoney;

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dates = DateUtils.GetDates(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _historyPeriod)),
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
                    if (i % _addMoneyPeriod == 0)
                    {
                        money += _addMoney;
                    }
                }

                void UpdateTotalSum()
                {
                    totalSum = costs.Values.Sum() + money;
                }

                void Rebalance()
                {
                    if (i % _rebalancePeriod == 0)
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
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * _historyPeriod)),
                DateOnly.FromDateTime(DateTime.Today));

            var price = analyseDataContext.GetPrice(indexTicker, dates[0]);
            var size = Math.Truncate(_startMoney / price!.Value);

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
