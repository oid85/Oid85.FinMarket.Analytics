using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class SevenEtfLifePortfolioService(
        ITraderTinkoffOidApiClient traderTinkoffApiClient,
        IInstrumentService instrumentService,
        IDataService dataService,
        ISevenEtfLifePositionRepository positionRepository,
        IParameterRepository parameterRepository) 
        : ISevenEtfLifePortfolioService
    {
        public async Task<LifePortfolioResponse> GetPositionListAsync(LifePortfolioRequest request)
        {
            List<string> shareTickers = ["TITR", "TDIV", "TMOS", "TRND"];
            List<string> bondTickers = ["TOFZ", "TBRU"];
            List<string> goldTickers = ["TGLD", "GOLD"];
            List<string> currencyTickers = ["TCNY", "TLCB"];
            List<string> moneyTickers = ["TMON", "TPAY"];

            var portfolioResponse = await traderTinkoffApiClient.GetPortfolioInfoAsync(
                new TinkoffPortfolioInfoRequest { AccountName = "ETF" });

            var portfolioPositionData = portfolioResponse.Result.Positions
                .ToDictionary(k => k.Ticker, v => v);

            var lifePositionData = (await positionRepository.GetAsync())
                .ToDictionary(k => k.Ticker, v => v);

            var tickers = lifePositionData.Keys.ToList();

            var candleData = await dataService.GetCandleDataAsync(tickers);

            var instrumentData = (await instrumentService.GetInstrumentListAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Etf)
                .Where(x => lifePositionData.ContainsKey(x.Ticker))
                .ToDictionary(k => k.Ticker, v => v);

            var weightData = portfolioPositionData.ToDictionary(k => k.Key, v => GetWeight(v.Key));

            double totalSum = Convert.ToDouble(portfolioResponse.Result.TotalSum);
            double money = Convert.ToDouble(portfolioResponse.Result.Money);

            var positions = portfolioPositionData.Values
                .Select(x =>
                {
                    instrumentData.TryGetValue(x.Ticker, out var instrument);
                    lifePositionData.TryGetValue(x.Ticker, out var lifePosition);
                    portfolioPositionData.TryGetValue(x.Ticker, out var portfolioPosition);
                    weightData.TryGetValue(x.Ticker, out var weight);

                    string name = portfolioPosition?.Name ?? string.Empty;                    
                    var lifeSize = portfolioPosition?.Size ?? 0;
                    var price = portfolioPosition?.CurrentPrice ?? 0;
                    var cost = GetCost(x.Ticker);
                    var size = GetSize(x.Ticker);
                    var percent = (cost / totalSum * 100.0).RoundTo(2);
                    var (delta, deltaText) = GetDelta(size, lifeSize);
                    var (deltaPercent, deltaPercentText) = GetDeltaPercent(size, lifeSize);
                    var monthDeltaPercent = GetMonthDeltaPercent(x.Ticker).RoundTo(2);
                    string recommendation = GetRecommendation(deltaPercent);
                    string colorFill = GetColor(deltaPercent);

                    return new LifePositionItem
                    {
                        Ticker = x.Ticker,
                        Name = name,
                        Weight = weight,
                        Cost = cost,
                        Percent = percent,
                        Size = size,
                        LifeSize = lifeSize,
                        Price = Convert.ToDouble(price),
                        Delta = delta,
                        DeltaPercent = deltaPercent,
                        DeltaText = deltaText,
                        DeltaPercentText = deltaPercentText,
                        Recommendation = recommendation,
                        MonthDeltaPercent = monthDeltaPercent,
                        ColorFill = colorFill
                    };
                })
                .ToList();

            var classActiveItems = new List<ClassActiveItem>
            {
                new() { Name = $"Акции ({string.Join(',', shareTickers.OrderBy(x => x))})", Percent = GetClassActivePercent(shareTickers) },
                new() { Name = $"Облигации ({string.Join(',', bondTickers.OrderBy(x => x))})", Percent = GetClassActivePercent(bondTickers) },
                new() { Name = $"Золото ({string.Join(',', goldTickers.OrderBy(x => x))})", Percent = GetClassActivePercent(goldTickers) },
                new() { Name = $"Валюта ({string.Join(',', currencyTickers.OrderBy(x => x))})", Percent = GetClassActivePercent(currencyTickers) },
                new() { Name = $"Ликвидность ({string.Join(',', moneyTickers.OrderBy(x => x))})", Percent = GetClassActivePercent(moneyTickers) }
            };

            return new LifePortfolioResponse
            {
                TotalSum = totalSum,
                Money = money,
                PortfolioPositions = GetOrderedPositions(positions),
                ClassActiveItems = GetOrderedClassActives(classActiveItems)
            };

            double GetWeight(string ticker)
            {
                double moneyWeight = 2.0;
                double shareWeight = 2.0;
                double bondWeight = 2.0;
                double goldWeight = 1.0;
                double currencyWeight = 1.0;
                
                if (shareTickers.Contains(ticker)) return shareWeight / shareTickers.Count;
                if (bondTickers.Contains(ticker)) return bondWeight / bondTickers.Count;                
                if (currencyTickers.Contains(ticker)) return currencyWeight / currencyTickers.Count;
                if (moneyTickers.Contains(ticker)) return moneyWeight / moneyTickers.Count;

                if (goldTickers.Contains(ticker))
                {
                    if (ticker == "TGLD") return goldWeight * 0.25;
                    if (ticker == "GOLD") return goldWeight * 0.75;
                }

                return 0.0;
            }

            double GetClassActivePercent(List<string> tickers)
            {
                double weightSum = weightData.Values.Sum();

                double weight = weightData
                    .Where(x => tickers.Contains(x.Key))
                    .Sum(x => x.Value);

                double percent = (weight / weightSum * 100.0).RoundTo(2);

                return percent;
            }

            double GetMonthDeltaPercent(string ticker)
            {
                candleData.TryGetValue(ticker, out var candles);

                if (candles is null) return 0.0;
                if (candles is []) return 0.0;

                var firstCandle = candles.FindLast(x => x.Date <= DateOnly.FromDateTime(DateTime.Today));
                var lastCandle = candles.FindLast(x => x.Date <= DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)));

                if (firstCandle is null) return 0.0;
                if (lastCandle is null) return 0.0;

                return (lastCandle.Close - firstCandle.Close) / firstCandle.Close * 100.0;
            }

            int GetSize(string ticker)
            {
                instrumentData.TryGetValue(ticker, out var instrument);
                portfolioPositionData.TryGetValue(ticker, out var portfolioPosition);

                double tickerCost = GetCost(ticker);
                double price = Convert.ToDouble(portfolioPosition?.CurrentPrice ?? 0);

                return price == 0.0 ? 0 : Convert.ToInt32(Math.Truncate(tickerCost / price));
            }

            double GetCost(string ticker)
            {                
                weightData.TryGetValue(ticker, out var weight);

                double weightSum = weightData.Values.Sum();
                double share = weight / weightSum;

                return (totalSum * share).RoundTo(2);
            }
        }

        private static (int delta, string deltaText) GetDelta(int size, int lifeSize)
        {
            int delta = size - lifeSize;

            string deltaText = delta switch
            {
                > 0 => $"купить {Math.Abs(delta)} шт.",
                < 0 => $"продать {Math.Abs(delta)} шт.",
                _ => string.Empty
            };

            return (delta, deltaText);
        }

        private static (double deltaPercent, string deltaPercentText) GetDeltaPercent(int size, int lifeSize)
        {
            int delta = size - lifeSize;

            double deltaPercent = (Convert.ToDouble(delta) / Convert.ToDouble(lifeSize) * 100.0).RoundTo(2);

            string deltaPercentText = deltaPercent switch
            {
                > 0.0 => $"(-) меньше расч. на {Math.Abs(deltaPercent)} %",
                < 0.0 => $"(+) больше расч. на {Math.Abs(deltaPercent)} %",
                _ => string.Empty
            };

            return (deltaPercent, deltaPercentText);
        }

        private static string GetRecommendation(double deltaPercent)
        {
            const double deltaLimit = 10.0;

            return deltaPercent switch
            {
                > deltaLimit => $"докупить",
                < -1 * deltaLimit => $"сократить",
                _ => string.Empty
            };
        }

        private static string GetColor(double deltaPercent)
        {
            const double deltaLimit = 10.0;

            return deltaPercent switch
            {
                > deltaLimit => KnownColors.LightYellow,
                < -1 * deltaLimit => KnownColors.LightYellow,
                _ => KnownColors.LightGreen
            };
        }

        private static List<LifePositionItem> GetOrderedPositions(
            List<LifePositionItem> positions)
        {
            List<LifePositionItem> orderedPositions = [.. positions.OrderByDescending(x => x.DeltaPercent)];

            for (int i = 0; i < orderedPositions.Count; i++)
                orderedPositions[i].Number = i + 1;

            return orderedPositions;
        }

        private static List<ClassActiveItem> GetOrderedClassActives(
            List<ClassActiveItem> items)
        {
            List<ClassActiveItem> orderedItems = [.. items.OrderByDescending(x => x.Percent)];

            for (int i = 0; i < orderedItems.Count; i++)
                orderedItems[i].Number = i + 1;

            return orderedItems;
        }

        public async Task<LifePortfolioImportResponse> ImportPositionListAsync(LifePortfolioImportRequest request)
        {
            var path = await parameterRepository.GetParameterValueAsync("SnowballHoldingsFilePath");

            var models =
                (await File.ReadAllLinesAsync(path!))
                .Skip(1)
                .Select(x => x.Split(','))
                .Select(x => new LifePosition
                {
                    Ticker = x[0].Replace("\"", ""),
                    Name = x[1].Replace("\"", ""),
                    Size = int.Parse(x[3].Replace("\"", "")),
                    Weight = 1.0
                })
                .ToList();

            await positionRepository.DeleteAsync();
            await positionRepository.AddAsync(models);

            return new();
        }
    }
}
