using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class LifePortfolioService(
        ILifePortfolioPositionRepository lifePortfolioPositionRepository) 
        : ILifePortfolioService
    {
        public async Task<EditLifePortfolioPositionResponse> EditLifePortfolioPositionAsync(EditLifePortfolioPositionRequest request)
        {
            await lifePortfolioPositionRepository.EditLifePortfolioPositionAsync(request.Ticker, request.Size);
            return new();
        }

        public async Task<ImportLifePortfolioPositionListResponse> ImportLifePortfolioPositionListAsync(ImportLifePortfolioPositionListRequest request)
        {
            string path = @"c:\Users\79131\Downloads\Snowball Holdings.csv";
            var lines = await File.ReadAllLinesAsync(path);

            await lifePortfolioPositionRepository.DeleteAllLifePortfolioPositionAsync();

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');

                var lifePosition = new LifePortfolioPosition
                {
                    Ticker = parts[0].Replace("\"", ""),
                    Name = parts[1].Replace("\"", ""),
                    Size = int.Parse(parts[3].Replace("\"", "")),
                    IsDeleted = false
                };

                await lifePortfolioPositionRepository.AddLifePortfolioPositionAsync(lifePosition);
            }

            return new();
        }
    }
}
