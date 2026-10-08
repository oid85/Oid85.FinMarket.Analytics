using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class BondLifePortfolioService(
        IInstrumentService instrumentService,
        IBondAnalyseService bondAnalyseService,
        IBondLifePositionRepository positionRepository) 
        : IBondLifePortfolioService
    {
        public async Task<LifePortfolioResponse> GetPositionListAsync(LifePortfolioRequest request)
        {
            var lifePositionData = (await positionRepository.GetAsync())
                .ToDictionary(k => k.Ticker, v => v);

            var instrumentData = (await instrumentService.GetInstrumentListAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .ToDictionary(k => k.Ticker, v => v);
            
            var bondAnalyseData = (await bondAnalyseService.GetBondAnalyseAsync(new()))
                .Items
                .ToDictionary(k => k.Ticker, v => v);

            double totalSum = GetTotalSum();            

            var positions = lifePositionData.Values
                .Select(x =>
                {
                    bondAnalyseData.TryGetValue(x.Ticker, out var bondAnalyse);
                    instrumentData.TryGetValue(x.Ticker, out var instrument);

                    string name = instrument?.Name ?? string.Empty;
                    var yield = bondAnalyse?.Yield ?? 0.0;
                    var rating = bondAnalyse?.Rating ?? string.Empty;
                    var weight = GetWeight(x.Ticker);
                    var lifeSize = lifePositionData[x.Ticker]?.Size ?? 0;
                    var price = instrument?.LastPrice ?? 0;
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
                        Yield = yield,
                        Rating = rating,
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
            
            return new LifePortfolioResponse
            {
                TotalSum = totalSum,
                PortfolioPositions = GetOrderedPositions(positions, request.OrderField)
            };

            double GetWeight(string ticker)
            {
                lifePositionData.TryGetValue(ticker, out var lifePosition);
                return lifePosition?.Weight ?? 0;
            }

            int GetSize(string ticker)
            {
                instrumentData.TryGetValue(ticker, out var instrument);

                double tickerCost = GetCost(ticker);
                double price = instrument?.LastPrice ?? 0 + instrument?.Nkd ?? 0;

                return price == 0.0 ? 0 : Convert.ToInt32(Math.Truncate(tickerCost / price));
            }

            double GetCost(string ticker)
            {
                double totalSum = GetTotalSum();
                double weightSum = lifePositionData.Values.Sum(x => GetWeight(x.Ticker));

                lifePositionData.TryGetValue(ticker, out var lifePosition);

                double share = (lifePosition?.Weight ?? 0) / weightSum;

                return (totalSum * share).RoundTo(2);
            }

            double GetTotalSum() =>
                lifePositionData.Values.Sum(x =>
                {
                    instrumentData.TryGetValue(x.Ticker, out var instrument);
                    return (x.Size ?? 0) * (instrument?.LastPrice ?? 0 + instrument?.Nkd ?? 0);
                });
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
