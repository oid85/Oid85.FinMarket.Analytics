using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class LifeSuperPortfolioService(
        ILifePortfolioRepository lifePortfolioRepository)
        : ILifeSuperPortfolioService
    {
        public async Task<LifeSuperPortfolioResponse> GetLifeSuperPortfolioAsync(LifeSuperPortfolioRequest request)
        {
            var lifePortfolios = await lifePortfolioRepository.GetAsync();
            
            var lifePortfolioItems = new List<LifePortfolioItem>();
            double totalSum = 0;

            foreach (var lifePortfolio in lifePortfolios)
            {
                if (lifePortfolio.Name == "Share")
                {
                    var lifePortfolioItem = new LifePortfolioItem();
                    lifePortfolioItems.Add(lifePortfolioItem);
                    continue;
                }

                if (lifePortfolio.Name == "Bond")
                {
                    var lifePortfolioItem = new LifePortfolioItem();
                    lifePortfolioItems.Add(lifePortfolioItem);
                    continue;
                }

                if (lifePortfolio.Name == "Momentum")
                {
                    var lifePortfolioItem = new LifePortfolioItem();
                    lifePortfolioItems.Add(lifePortfolioItem);
                    continue;
                }

                if (lifePortfolio.Name == "SevenEtf")
                {
                    var lifePortfolioItem = new LifePortfolioItem();
                    lifePortfolioItems.Add(lifePortfolioItem);
                    continue;
                }

                if (lifePortfolio.Name == "ThreeEtf")
                {
                    var lifePortfolioItem = new LifePortfolioItem();
                    lifePortfolioItems.Add(lifePortfolioItem);
                    continue;
                }
            }

            var orderedLifePortfolioItems = lifePortfolioItems.OrderByDescending(x => x.Weight).ToList();

            return new LifeSuperPortfolioResponse
            {
                TotalSum = totalSum,
                Portfolios = orderedLifePortfolioItems
            };
        }
    }
}
