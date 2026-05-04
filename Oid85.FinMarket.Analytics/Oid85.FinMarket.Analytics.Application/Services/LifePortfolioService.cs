using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class LifePortfolioService(
        ILifePortfolioPositionRepository lifePortfolioPositionRepository,
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository) 
        : ILifePortfolioService
    {
        public async Task<EditLifePortfolioPositionResponse> EditLifePortfolioPositionAsync(EditLifePortfolioPositionRequest request)
        {
            await lifePortfolioPositionRepository.EditLifePortfolioPositionAsync(request.Ticker, request.Size);
            return new();
        }

        public async Task<ImportLifePortfolioPositionListResponse> ImportLifePortfolioPositionListAsync(ImportLifePortfolioPositionListRequest request)
        {
            string path = (await parameterRepository.GetParameterValueAsync(KnownParameters.SnowballHoldingsFilePath))!;
            var lines = await File.ReadAllLinesAsync(path);

            // Удалим старые позиции
            await lifePortfolioPositionRepository.DeleteAllLifePortfolioPositionAsync();

            // Сбросим флаг InPortfolio
            var tickers = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Select(x => x.Ticker).ToList();
            
            foreach (var ticker in tickers)
            {
                var instrument = await instrumentRepository.GetInstrumentByTickerAsync(ticker);
                instrument!.InPortfolio = false;
                await instrumentRepository.EditInstrumentAsync(instrument);
            }

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

                // Установим флаг InPortfolio
                var instrument = await instrumentRepository.GetInstrumentByTickerAsync(lifePosition.Ticker);
                
                if (instrument is not null)
                {
                    instrument!.InPortfolio = true;
                    await instrumentRepository.EditInstrumentAsync(instrument);
                }
            }

            return new();
        }
    }
}
