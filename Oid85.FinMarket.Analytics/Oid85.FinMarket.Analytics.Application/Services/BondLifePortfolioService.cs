using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BondLifePortfolioService(
        IBondLifePortfolioPositionRepository lifePortfolioPositionRepository) 
        : IBondLifePortfolioService
    {
        public async Task<BondLifePortfolioPositionListResponse> GetBondLifePortfolioPositionListAsync(BondLifePortfolioPositionListRequest request)
        {
            var positions = await lifePortfolioPositionRepository.GetAsync();

            var portfolioPositions = new List<BondLifePortfolioPositionListItem>();

            foreach (var position in positions)
            {
                portfolioPositions.Add(
                    new ()
                    {
                        Ticker = position.Ticker,
                        Name = position.Name,
                        Size = position.Size ?? 0,
                        Price = position.Price ?? 0
                    });
            }

            double totalSum = portfolioPositions.Sum(x => x.Size * x.Price);

            List<BondLifePortfolioPositionListItem> orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

            if (request.OrderField is not null)
            {
                if (request.OrderField == string.Empty)
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "Percent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "DeltaPercent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.DeltaPercent)];

                else if (request.OrderField == "MonthDeltaPricePercent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.MonthDeltaPricePercent)];
            }

            var response = new BondLifePortfolioPositionListResponse
            {
                TotalSum = totalSum,
                PortfolioPositions = orderedPortfolioPositions
            };

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            return response;
        }
    }
}
