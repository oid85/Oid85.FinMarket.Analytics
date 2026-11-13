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
    public class BondLifePortfolioService(
        ITraderTinkoffEldApiClient traderTinkoffApiClient,
        IInstrumentService instrumentService,
        IBondAnalyseService bondAnalyseService,
        IBondLifePositionRepository positionRepository,
        IParameterRepository parameterRepository) 
        : IBondLifePortfolioService
    {
        public async Task<LifePortfolioResponse> GetPositionListAsync(LifePortfolioRequest request)
        {
            var portfolioResponse = await traderTinkoffApiClient.GetPortfolioInfoAsync(
                new TinkoffPortfolioInfoRequest { AccountName = "Bonds" });

            var portfolioPositionData = portfolioResponse.Result.Positions
                .ToDictionary(k => k.Ticker, v => v);

            var lifePositionData = (await positionRepository.GetAsync())
                .ToDictionary(k => k.Ticker, v => v);

            var instrumentData = (await instrumentService.GetInstrumentListAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .Where(x => lifePositionData.ContainsKey(x.Ticker))
                .ToDictionary(k => k.Ticker, v => v);
            
            var bondAnalyseData = (await bondAnalyseService.GetBondAnalyseAsync(new()))
                .Items
                .Where(x => lifePositionData.ContainsKey(x.Ticker))                
                .ToDictionary(k => k.Ticker, v => v);

            var weightData = lifePositionData.ToDictionary(k => k.Key, v => GetWeight(v.Key));

            double totalSum = Convert.ToDouble(portfolioResponse.Result.TotalSum);

            var positions = portfolioPositionData.Values
                .Select(x =>
                {
                    bondAnalyseData.TryGetValue(x.Ticker, out var bondAnalyse);
                    instrumentData.TryGetValue(x.Ticker, out var instrument);                    
                    portfolioPositionData.TryGetValue(x.Ticker, out var portfolioPosition);

                    string name = portfolioPosition?.Name ?? string.Empty;                    
                    var rating = bondAnalyse?.Rating ?? string.Empty;
                    var weight = GetWeight(x.Ticker);
                    var lifeSize = portfolioPosition?.Size ?? 0;
                    var price = portfolioPosition?.CurrentPrice ?? 0;
                    var cost = GetCost(x.Ticker);
                    var size = GetSize(x.Ticker);
                    var percent = (cost / totalSum * 100.0).RoundTo(2);
                    var (delta, deltaText) = GetDelta(size, lifeSize);
                    var (deltaPercent, deltaPercentText) = GetDeltaPercent(size, lifeSize);
                    string recommendation = GetRecommendation(deltaPercent);
                    string colorFill = GetColor(deltaPercent);

                    return new LifePositionItem
                    {
                        Ticker = x.Ticker,
                        Name = name,                        
                        Rating = rating,
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
                        ColorFill = colorFill
                    };
                })
                .ToList();
            
            return new LifePortfolioResponse
            {
                TotalSum = totalSum,
                PortfolioPositions = GetOrderedPositions(positions, request.OrderField)
            };

            double GetWeight(string ticker) => 1.0;

            int GetSize(string ticker)
            {
                instrumentData.TryGetValue(ticker, out var instrument);
                portfolioPositionData.TryGetValue(ticker, out var portfolioPosition);

                double tickerCost = GetCost(ticker);
                double price = Convert.ToDouble(portfolioPosition?.CurrentPrice ?? 0) + instrument?.Nkd ?? 0;

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
            List<LifePositionItem> positions, string? orderField)
        {
            List<LifePositionItem> orderedPositions = orderField switch
            {
                "Percent" => [.. positions.OrderByDescending(x => x.Percent)],
                "Yield" => [.. positions.OrderByDescending(x => x.Yield)],
                "DeltaPercent" => [.. positions.OrderByDescending(x => x.DeltaPercent)],
                _ => [.. positions.OrderByDescending(x => x.Name)]
            };

            for (int i = 0; i < orderedPositions.Count; i++)
                orderedPositions[i].Number = i + 1;

            return orderedPositions;
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

            return new ();
        }
    }
}
