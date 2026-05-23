using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioRebalanceService(
        IInstrumentRepository instrumentRepository,
        IPortfolioService portfolioService,
        IInstrumentService instrumentService,
        IDataService dataService,
        IStorageApiClient storageApiClient)
        : IPortfolioRebalanceService
    {
        public async Task<PortfolioRebalanceResponse> PortfolioRebalanceAsync(PortfolioRebalanceRequest request)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var dividends = (await storageApiClient.GetDividendListAsync(new())).Result.Dividends;

            var positions = (await portfolioService.GetPortfolioPositionListAsync(new GetPortfolioPositionListRequest())).PortfolioPositions;
            
            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? [])
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .ToList();

            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .ToList();

            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-3));
            var to = DateOnly.FromDateTime(DateTime.Today);

            List<DateOnly> dates = DateUtils.GetDates(from, to);
            List<DateOnly> rebalanceDates = [dates[0], ..dates.Where(x => x.Day == 1)];

            double startMoney = 5_000_000.0;
            double money = startMoney;

            var tickers = positions.Select(x => x.Ticker).ToList();
            var weights = positions.ToDictionary(k => k.Ticker, v => v.ResultCoefficient);
            var sizes = positions.ToDictionary(k => k.Ticker, v => 0);
            var costs = positions.ToDictionary(k => k.Ticker, v => 0.0);
            var prices = positions.ToDictionary(k => k.Ticker, v => 0.0);
            var lots = storageInstruments.ToDictionary(k => k.Ticker, v => v.Lot ?? 1);

            var equitySeriesItem = new PortfolioRebalanceSeries()
            {
                Name = "Портфель",
                Color = KnownColors.Green
            };

            int addMoneyPeriod = 30;
            int rebalancePeriod = 15;

            for (int i = 0; i < dates.Count; i++)
            {
                DateOnly date = dates[i];

                UpdateDividends(date);
                
                double currentTotalSum = costs.Values.Sum() + money;
                
                UpdatePrices(date);
                UpdateCosts();

                if (i % addMoneyPeriod == 0)
                {
                    money += 30_000.0;
                    currentTotalSum = costs.Values.Sum() + money;
                }

                if (i % rebalancePeriod == 0)
                {                    
                    UpdateSizes(currentTotalSum, currentTotalSum / weights.Values.Sum());
                    UpdateCosts();
                    money = currentTotalSum - costs.Values.Sum();
                }
                                
                equitySeriesItem.Data.Add(
                    new ()
                    {
                        Date = date,
                        Value = currentTotalSum.RoundTo(2)
                    });
            }

            var msftrItem = new PortfolioRebalanceSeries()
            {
                Name = "Индекс полн. дох. MCFTR",
                Color = KnownColors.Blue
            };

            var msftrCandles = analyseDataContext.GetCandles(KnownIndexTickers.MCFTR);

            var msftrPrice = analyseDataContext.GetPrice(KnownIndexTickers.MCFTR, dates[0]);
            var msftrSize = Math.Truncate(startMoney / msftrPrice.Value);

            for (int i = 0; i < dates.Count; i++)
            {
                DateOnly date = dates[i];

                msftrItem.Data.Add(
                    new()
                    {
                        Date = date,
                        Value = (msftrSize * analyseDataContext.GetPrice(KnownIndexTickers.MCFTR, date)).RoundTo(2)
                    });
            }

            var response = new PortfolioRebalanceResponse { Series = [ equitySeriesItem, msftrItem] };

            return response;

            void UpdatePrices(DateOnly date)
            {
                foreach (var ticker in tickers) 
                    prices[ticker] = analyseDataContext.GetPrice(ticker, date) ?? 0.0;
            }

            void UpdateCosts()
            {
                foreach (var ticker in tickers) 
                    costs[ticker] = prices[ticker] * sizes[ticker];
            }

            void UpdateSizes(double currentTotalSum, double baseUnit)
            {
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

            void UpdateDividends(DateOnly date)
            {
                var dividendsByDate = dividends.Where(x => x.Date == date).ToList() ?? [];

                foreach (var ticker in tickers)
                {
                    var dividend = dividendsByDate.Find(x => x.Ticker == ticker);

                    if (dividend is not null)
                        money += sizes[ticker] * dividend.Value;
                }
            }
        }
    }
}
