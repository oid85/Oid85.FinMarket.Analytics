using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BondLifePortfolioService(
        IBondLifePositionRepository positionRepository) 
        : IBondLifePortfolioService
    {
        public async Task<BondLifePortfolioResponse> GetPositionListAsync(BondLifePortfolioRequest request)
        {
            var positions = await positionRepository.GetAsync();

            var positionItems = new List<BondLifePortfolioPositionListItem>();

            foreach (var position in positions)
            {
                positionItems.Add(
                    new ()
                    {
                        Ticker = position.Ticker,
                        Name = position.Name,
                        Size = position.Size ?? 0,
                        Price = position.Price ?? 0
                    });
            }

            double totalSum = positionItems.Sum(x => x.Size * x.Price);

            List<BondLifePortfolioPositionListItem> orderedPositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];

            if (request.OrderField is not null)
            {                
                if (request.OrderField == "Percent") orderedPositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];
                else if (request.OrderField == "DeltaPercent") orderedPositionItems = [.. positionItems.OrderByDescending(x => x.DeltaPercent)];
                else if (request.OrderField == "MonthDeltaPricePercent") orderedPositionItems = [.. positionItems.OrderByDescending(x => x.MonthDeltaPricePercent)];
                else orderedPositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];
            }

            var response = new BondLifePortfolioResponse
            {
                TotalSum = totalSum,
                PortfolioPositions = orderedPositionItems
            };

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            return response;
        }
    }
}
