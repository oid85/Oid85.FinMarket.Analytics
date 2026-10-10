using System.Timers;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class ShareLifePortfolioService(
        IStorageApiClient storageApiClient,
        IFundamentalScoreService fundamentalScoreService,
        IInstrumentService instrumentService,
        IShareLifePositionRepository positionRepository) 
        : IShareLifePortfolioService
    {
        public async Task<LifePortfolioResponse> GetPositionListAsync(LifePortfolioRequest request)
        {
            var keyRates = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();
            double currentKeyRate = keyRates.Last().Value ?? 0.0;

            var lifePositionData = (await positionRepository.GetAsync())
                .ToDictionary(k => k.Ticker, v => v);

            Dictionary<string, FundamentalScore?> fundamentalData = [];

            foreach (var ticker in lifePositionData.Keys)
                fundamentalData.Add(ticker, await fundamentalScoreService.GetFundamentalScoreAsync(ticker));

            var instrumentData = (await instrumentService.GetInstrumentListAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => lifePositionData.ContainsKey(x.Ticker))
                .ToDictionary(k => k.Ticker, v => v);

            var weightData = lifePositionData.ToDictionary(k => k.Key, v => GetWeight(v.Key));

            double totalSum = GetTotalSum();

            var positions = lifePositionData.Values
                .Select(x =>
                {
                    instrumentData.TryGetValue(x.Ticker, out var instrument);
                    lifePositionData.TryGetValue(x.Ticker, out var lifePosition);
                    fundamentalData.TryGetValue(x.Ticker, out var fundamental);

                    string name = instrument?.Name ?? string.Empty;
                    var weight = GetWeight(x.Ticker);
                    var yield = fundamental?.DividendYield?.Value ?? 0.0;
                    var lifeSize = lifePosition?.Size ?? 0;
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
                        Weight = weight,
                        Yield = yield,
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

            double GetWeight(string ticker) =>
                GetMarketCapCoefficient(ticker) + GetFundamentalScoreCoefficient(ticker) + GetDividendCoefficient(ticker);

            double GetMarketCapCoefficient(string ticker)
            {
                fundamentalData.TryGetValue(ticker, out var fundamental);

                return fundamental?.MarketCap?.Ratio switch
                {
                    0.5 => 1.0,
                    0.75 => 2.0,
                    1.0 => 3.0,
                    _ => 1.0
                };
            }

            double GetFundamentalScoreCoefficient(string ticker)
            {
                fundamentalData.TryGetValue(ticker, out var fundamental);
                return fundamental?.Score.Value ?? 1.0;
            }

            double GetDividendCoefficient(string ticker)
            {
                fundamentalData.TryGetValue(ticker, out var fundamental);

                double dividendCoefficient = 1.0;
                const double hiLimitCoefficient = 5.0;
                const double loLimitCoefficient = 3.0;
                double hiLimitYield = currentKeyRate;
                double loLimitYield = hiLimitYield / 3.0 * 2.0;

                var yield = fundamental?.DividendYield?.Value ?? 0.0;

                if (yield >= hiLimitYield) dividendCoefficient = hiLimitCoefficient;
                else if (yield <= loLimitYield) dividendCoefficient = 1.0;
                else dividendCoefficient = (yield - loLimitYield) * (hiLimitCoefficient - loLimitCoefficient) / (hiLimitYield - loLimitYield) + loLimitCoefficient;

                return dividendCoefficient;
            }

            int GetSize(string ticker)
            {
                instrumentData.TryGetValue(ticker, out var instrument);

                double tickerCost = GetCost(ticker);
                double price = instrument?.LastPrice ?? 0;
                int lot = instrument?.Lot ?? 1;

                return price == 0.0 ? 0 : Convert.ToInt32(Math.Truncate(tickerCost / price / lot) * lot);
            }

            double GetCost(string ticker)
            {
                lifePositionData.TryGetValue(ticker, out var lifePosition);
                weightData.TryGetValue(ticker, out var weight);

                double totalSum = GetTotalSum();
                double weightSum = weightData.Values.Sum();
                double share = weight / weightSum;

                return (totalSum * share).RoundTo(2);
            }

            double GetTotalSum() =>
                lifePositionData.Values.Sum(x =>
                {
                    instrumentData.TryGetValue(x.Ticker, out var instrument);
                    return (x.Size ?? 0) * (instrument?.LastPrice ?? 0);
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
