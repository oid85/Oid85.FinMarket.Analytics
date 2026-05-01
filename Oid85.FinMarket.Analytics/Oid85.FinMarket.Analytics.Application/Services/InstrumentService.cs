using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
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
            var instruments = (await instrumentRepository.GetInstrumentsAsync()) ?? [];

            var items = new List<GetAnalyticInstrumentListItemResponse>();

            foreach (var instrument in instruments)
                items.Add(new GetAnalyticInstrumentListItemResponse
                {
                    Id = instrument.Id,
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    IsSelected = instrument.IsSelected,
                    InPortfolio = instrument.InPortfolio,
                    ManualCoefficient = instrument.ManualCoefficient,
                    Type = instrument.Type
                });

            var response = new GetAnalyticInstrumentListResponse()
            {
                Instruments = [.. items.OrderBy(x => x.Ticker)]
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<SelectInstrumentResponse> SelectInstrumentAsync(SelectInstrumentRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);

            instrument!.IsSelected = !instrument!.IsSelected;

            await instrumentRepository.EditInstrumentAsync(instrument);

            return new SelectInstrumentResponse() { Id = instrument.Id };
        }

        /// <inheritdoc />
        public async Task<PortfolioInstrumentResponse> PortfolioInstrumentAsync(PortfolioInstrumentRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);

            instrument!.InPortfolio = !instrument!.InPortfolio;

            await instrumentRepository.EditInstrumentAsync(instrument);

            return new PortfolioInstrumentResponse() { Id = instrument.Id };
        }

        public async Task<List<Instrument>> GetStorageInstrumentAsync()
        {
            var response = await finMarketStorageServiceApiClient.GetInstrumentListAsync(new());
            return response.Result.Instruments
                .Select(x =>
                new Instrument
                {
                    Ticker = x.Ticker,
                    Name = x.Name,
                    Type = x.Type,
                    Nkd = x.Nkd,
                    LastPrice = x.LastPrice,
                    MaturityDate = x.MaturityDate,
                    Nominal = x.Nominal,
                    Currency = x.Currency,
                    Lot = x.Lot
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task SyncInstrumentListAsync()
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
                            ManualCoefficient = 1
                        });

            // Удаляем неактуальные
            foreach (var analyticInstrument in analyticInstruments)
                if (!storageInstruments.Select(x => x.Ticker).Contains(analyticInstrument.Ticker))
                    await instrumentRepository.DeleteByTickerAsync(analyticInstrument.Ticker);
        }

        /// <inheritdoc />
        public async Task<GetSectorListResponse> GetSectorListAsync(GetSectorListRequest request)
        {
            var analyticInstruments = (await instrumentRepository.GetInstrumentsAsync()) ?? [];

            var sectors = analyticInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.Sector != "-")
                .Where(x => !string.IsNullOrEmpty(x.Sector))
                .Select(x => x.Sector)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return new GetSectorListResponse { Sectors = sectors };
        }
    }
}
