using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Models.Life;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class BondLifePortfolioService(
        IInstrumentService instrumentService,
        IBondAnalyseService bondAnalyseService,
        IBondLifePositionRepository positionRepository) 
        : IBondLifePortfolioService
    {
        public async Task<BondLifePortfolioResponse> GetPositionListAsync(BondLifePortfolioRequest request)
        {
            var lifePositionData = (await positionRepository.GetAsync())
                .ToDictionary(k => k.Ticker, v => v);

            var instrumentData = (await instrumentService.GetInstrumentListAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .ToDictionary(k => k.Ticker, v => v);
            
            var bondAnalyseData = (await bondAnalyseService.GetBondAnalyseAsync(new()))
                .Items
                .ToDictionary(k => k.Ticker, v => v);

            double totalSum = GetTotalSum(lifePositionData, instrumentData);            

            var positions = lifePositionData.Values
                .Select(x =>
                {
                    string ticker = x.Ticker;
                    string name = instrumentData[x.Ticker].Name ?? string.Empty;
                    var yield = bondAnalyseData.ContainsKey(ticker) ? bondAnalyseData[ticker]?.Yield ?? 0.0 : 0.0;
                    var rating = bondAnalyseData.ContainsKey(ticker) ? bondAnalyseData[ticker]?.Rating ?? string.Empty : string.Empty;
                    var weight = lifePositionData[ticker]?.Weight ?? 0;
                    var lifeSize = lifePositionData[x.Ticker]?.Size ?? 0;
                    var price = instrumentData[x.Ticker]?.LastPrice ?? 0;
                    var cost = GetCost(lifePositionData, instrumentData, x.Ticker);
                    var size = GetSize(lifePositionData, instrumentData, x.Ticker);
                    var percent = (cost / totalSum * 100.0).RoundTo(2);

                    int delta = size - lifeSize;

                    string deltaText = delta switch
                    {
                        > 0 => $"Купить {Math.Abs(delta)} шт.",
                        < 0 => $"Продать {Math.Abs(delta)} шт.",
                        _ => string.Empty
                    };

                    double deltaPercent = (Convert.ToDouble(delta) / Convert.ToDouble(lifeSize) * 100.0).RoundTo(2);
                    
                    string deltaPercentText = deltaPercent switch
                    {
                        > 0.0 => $"Больше расч. на {Math.Abs(deltaPercent)} %",
                        < 0.0 => $"Меньше расч. на {Math.Abs(deltaPercent)} %",
                        _ => string.Empty
                    };

                    const double deltaLimit = 10.0;

                    string recommendation = deltaPercent switch
                    {
                        > deltaLimit => $"Сократить",
                        < -1 * deltaLimit => $"Докупить",
                        _ => string.Empty
                    };

                    string colorFill = deltaPercent switch
                    {
                        > deltaLimit => KnownColors.LightYellow,
                        < -1 * deltaLimit => KnownColors.LightYellow,
                        _ => KnownColors.LightGreen
                    };

                    return new BondLifePositionListItem
                    {
                        Ticker = ticker,
                        Name = name,
                        Yield = yield,
                        // Rating = rating,
                        Weight = weight,
                        Cost = cost,
                        Percent = percent,
                        Size = size,
                        LifeSize = lifeSize,
                        Price = price,
                        Delta = delta,
                        DeltaText = deltaText,
                        DeltaPercentText = deltaPercentText,
                        Recommendation = recommendation,
                        ColorFill = colorFill
                    };
                })
                .ToList();
            
            var response = new BondLifePortfolioResponse
            {
                TotalSum = totalSum,
                PortfolioPositions = GetOrderedPositions(positions, request.OrderField)
            };

            return response;
        }

        private static int GetSize(
            Dictionary<string, BondLifePosition> lifePositionData, 
            Dictionary<string, Instrument> instrumentData,
            string ticker)
        {
            double tickerCost = GetCost(lifePositionData, instrumentData, ticker);
            double price = instrumentData[ticker].LastPrice ?? 0 + instrumentData[ticker].Nkd ?? 0;

            return price == 0.0 ? 0 : Convert.ToInt32(Math.Truncate(tickerCost / price));
        }

        private static double GetCost(
            Dictionary<string, BondLifePosition> lifePositionData,
            Dictionary<string, Instrument> instrumentData,
            string ticker)
        {
            double totalSum = GetTotalSum(lifePositionData, instrumentData);
            double weightSum = lifePositionData.Values.Sum(x => x.Weight ?? 0);
            double share = (lifePositionData[ticker].Weight ?? 0) / weightSum;

            return (totalSum * share).RoundTo(2);
        }

        private static double GetTotalSum(Dictionary<string, BondLifePosition> lifePositionData, Dictionary<string, Instrument> instrumentData) =>
            lifePositionData.Values.Sum(x => (x.Size ?? 0) * (instrumentData[x.Ticker]?.LastPrice ?? 0 + instrumentData[x.Ticker]?.Nkd ?? 0));

        private static List<BondLifePositionListItem> GetOrderedPositions(
            List<BondLifePositionListItem> positions, string? orderField)
        {
            List<BondLifePositionListItem> orderedPositions = orderField switch
            {
                null => [.. positions.OrderByDescending(x => x.Percent)],
                "" => [.. positions.OrderByDescending(x => x.Percent)],
                "Percent" => [.. positions.OrderByDescending(x => x.Percent)],
                "DeltaPercent" => [.. positions.OrderByDescending(x => x.DeltaPercent)],
                _ => [.. positions.OrderByDescending(x => x.Percent)]
            };

            for (int i = 0; i < orderedPositions.Count; i++)
                orderedPositions[i].Number = i + 1;

            return orderedPositions;
        }
    }
}
