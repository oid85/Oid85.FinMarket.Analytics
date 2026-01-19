using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class InstrumentService(
        IInstrumentRepository instrumentRepository,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IInstrumentService
    {
        /// <inheritdoc />
        public async Task<GetAnalyticInstrumentListResponse> GetAnalyticInstrumentListAsync(GetAnalyticInstrumentListRequest request)
        {
            var analyticInstruments = (await instrumentRepository.GetInstrumentsAsync()) ?? [];
            var storageInstruments = (await GetStorageInstrumentAsync()) ?? [];

            // Добавляем новые
            foreach (var storageInstrument in storageInstruments)
                if (!analyticInstruments.Select(x => x.Ticker).Contains(storageInstrument.Ticker))
                    await instrumentRepository.AddAsync(
                        new Instrument
                        {
                            Ticker = storageInstrument.Ticker,
                            Name = storageInstrument.Name,
                            Type = storageInstrument.Type,
                            IsSelected = true
                        });
  
            // Удаляем неактуальные
            foreach (var analyticInstrument in analyticInstruments)
                if (!storageInstruments.Select(x => x.Ticker).Contains(analyticInstrument.Ticker))
                    await instrumentRepository.DeleteByTickerAsync(analyticInstrument.Ticker);

            var instruments = (await instrumentRepository.GetInstrumentsAsync()) ?? [];

            var response = new GetAnalyticInstrumentListResponse()
            {
                Instruments = instruments
                .Select(x => new GetAnalyticInstrumentListItemResponse                
                {
                    Id = x.Id,
                    Ticker = x.Ticker,
                    Name = x.Name,
                    Type = x.Type,
                    IsSelected = x.IsSelected
                })
                .OrderBy(x => x.Ticker)
                .ToList()
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<SelectInstrumentResponse> SelectInstrumentAsync(SelectInstrumentRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByIdAsync(request.Id);

            instrument!.IsSelected = !instrument!.IsSelected;

            await instrumentRepository.EditInstrumentAsync(instrument);

            return new SelectInstrumentResponse() { Id = request.Id };
        }

        private async Task<List<Instrument>> GetStorageInstrumentAsync()
        {
            var response = await finMarketStorageServiceApiClient.GetInstrumentListAsync(new());
            return response.Result.Instruments
                .Select(x =>
                new Instrument
                {
                    Ticker = x.Ticker,
                    Name = x.Name,
                    Type = x.Type
                })
                .ToList();
        }
    }
}
