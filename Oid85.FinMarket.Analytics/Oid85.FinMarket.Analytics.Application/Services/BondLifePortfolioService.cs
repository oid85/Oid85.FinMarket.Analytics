using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BondLifePortfolioService(
        IBondpositionRepository positionRepository) 
        : IBondLifePortfolioService
    {
        public async Task<BondLifePortfolioPositionListResponse> GetPositionListAsync(BondLifePortfolioPositionListRequest request)
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

            List<BondLifePortfolioPositionListItem> orderedpositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];

            if (request.OrderField is not null)
            {
                if (request.OrderField == string.Empty)
                    orderedpositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "Percent")
                    orderedpositionItems = [.. positionItems.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "DeltaPercent")
                    orderedpositionItems = [.. positionItems.OrderByDescending(x => x.DeltaPercent)];

                else if (request.OrderField == "MonthDeltaPricePercent")
                    orderedpositionItems = [.. positionItems.OrderByDescending(x => x.MonthDeltaPricePercent)];
            }

            var response = new BondLifePortfolioPositionListResponse
            {
                TotalSum = totalSum,
                positionItems = orderedpositionItems
            };

            for (int i = 0; i < response.positionItems.Count; i++)
                response.positionItems[i].Number = i + 1;

            return response;
        }
    }
}
