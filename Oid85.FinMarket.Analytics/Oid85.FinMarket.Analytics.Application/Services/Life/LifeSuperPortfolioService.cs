using System.Drawing;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
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

            var bondLifePortfolio = await bondLifePortfolioService.GetPositionListAsync(new ());
            var shareLifePortfolio = await shareLifePortfolioService.GetPositionListAsync(new ());
            var sevenEtfLifePortfolio = await sevenEtfLifePortfolioService.GetPositionListAsync(new ());
            var threeEtfLifePortfolio = await threeEtfLifePortfolioService.GetPositionListAsync(new ());
            var momentumLifePortfolio = await traderFinamApiClient.GetPortfolioInfoAsync(new());

            double totalSum =
                (bondLifePortfolio.TotalSum +
                shareLifePortfolio.TotalSum +
                sevenEtfLifePortfolio.TotalSum +
                threeEtfLifePortfolio.TotalSum +
                Convert.ToDouble(momentumLifePortfolio.Result.TotalSum))
                .RoundTo(2);

            double weightSum = lifePortfolioData.Values.Sum(x => x.Weight ?? 0);

            lifePortfolioItemData["Bond"].LifeCost = bondLifePortfolio.TotalSum.RoundTo(2);
            lifePortfolioItemData["Share"].LifeCost = shareLifePortfolio.TotalSum.RoundTo(2);
            lifePortfolioItemData["SevenEtf"].LifeCost = sevenEtfLifePortfolio.TotalSum.RoundTo(2);
            lifePortfolioItemData["ThreeEtf"].LifeCost = threeEtfLifePortfolio.TotalSum.RoundTo(2);
            lifePortfolioItemData["Momentum"].LifeCost = Convert.ToDouble(momentumLifePortfolio.Result.TotalSum).RoundTo(2);

            foreach (var (_, portfolio) in lifePortfolioItemData)
            {
                portfolio.Percent = (portfolio.Weight / weightSum * 100.0).RoundTo(2);                
                portfolio.Cost = (portfolio.Weight / weightSum * totalSum).RoundTo(2);
                               
                double delta = portfolio.Cost - portfolio.LifeCost;
                double deltaPercent = (delta / portfolio.LifeCost * 100.0).RoundTo(2);

                const double deltaLimit = 10.0;

                string recommendation = deltaPercent switch
                {
                    > deltaLimit => "нарастить",
                    < -1 * deltaLimit => "сократить",
                    _ => string.Empty
                };

                string deltaPercentText = deltaPercent switch
                {
                    > 0.0 => $"(-) меньше расч. на {Math.Abs(deltaPercent)} %",
                    < 0.0 => $"(+) больше расч. на {Math.Abs(deltaPercent)} %",
                    _ => string.Empty
                };

                string colorFill = deltaPercent switch
                {
                    > deltaLimit => KnownColors.LightYellow,
                    < -1 * deltaLimit => KnownColors.LightYellow,
                    _ => KnownColors.LightGreen
                };

                portfolio.Recommendation = recommendation;
                portfolio.DeltaPercent = deltaPercent;
                portfolio.DeltaPercentText = deltaPercentText;
                portfolio.ColorFill = colorFill;
            }

            var orderedLifePortfolioItems = lifePortfolioItemData.Values.OrderByDescending(x => x.LifeCost).ToList();

            for (int i = 0; i < orderedLifePortfolioItems.Count; i++)
                orderedLifePortfolioItems[i].Number = i + 1;

            return new LifeSuperPortfolioResponse
            {
                TotalSum = totalSum,
                Portfolios = orderedLifePortfolioItems
            };
        }
    }
}
