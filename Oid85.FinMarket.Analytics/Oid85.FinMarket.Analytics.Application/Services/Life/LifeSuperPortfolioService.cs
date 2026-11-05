using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class LifeSuperPortfolioService(
        ITraderFinamApiClient traderFinamApiClient,
        IShareLifePortfolioService shareLifePortfolioService,
        IBondLifePortfolioService bondLifePortfolioService,
        ISevenEtfLifePortfolioService sevenEtfLifePortfolioService,
        IThreeEtfLifePortfolioService threeEtfLifePortfolioService,
        ILifePortfolioRepository lifePortfolioRepository)
        : ILifeSuperPortfolioService
    {
        public async Task<LifeSuperPortfolioResponse> GetLifeSuperPortfolioAsync(LifeSuperPortfolioRequest request)
        {
            var lifePortfolioData = (await lifePortfolioRepository.GetAsync()).ToDictionary(k => k.Name, v => v);
            
            var lifePortfolioItemData = lifePortfolioData.ToDictionary(
                k => k.Key,
                v => new LifePortfolioItem
                {
                    Name = v.Key,
                    Description = v.Value.Description,
                    Weight = v.Value.Weight ?? 0.0
                });

            lifePortfolioItemData["Share"].Number = 1;
            lifePortfolioItemData["Bond"].Number = 2;
            lifePortfolioItemData["Momentum"].Number = 3;
            lifePortfolioItemData["SevenEtf"].Number = 4;
            lifePortfolioItemData["ThreeEtf"].Number = 5;
            lifePortfolioItemData["FollowTrend"].Number = 6;

            var orderedLifePortfolioItems = lifePortfolioItemData.Values.OrderByDescending(x => x.LifeCost).ToList();

            return new LifeSuperPortfolioResponse
            {
                TotalSum = 0.0,
                Portfolios = orderedLifePortfolioItems
            };
        }
    }
}
