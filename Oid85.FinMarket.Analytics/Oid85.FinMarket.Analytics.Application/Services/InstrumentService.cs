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
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient,
        IDataService dataService)
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
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);

            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * request.LastDaysCount));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var benchmarkIncrement = GetIncrement(KnownBenchmarkTickers.MCFTR);

            var items = new List<GetAnalyticInstrumentListItemResponse>();

            foreach (var instrument in instruments)
            {
                var item = new GetAnalyticInstrumentListItemResponse();

                item.Id = instrument.Id;
                item.Ticker = instrument.Ticker;
                item.Name = instrument.Name;
                item.IsSelected = instrument.IsSelected;
                item.Type = instrument.Type;
                item.BenchmarkChange = Math.Round(GetIncrement(instrument.Ticker) - benchmarkIncrement, 2);

                items.Add(item);
            }

            var response = new GetAnalyticInstrumentListResponse()
            {
                Instruments = [.. items.OrderByDescending(x => x.BenchmarkChange)]
            };

            return response;

            // Изменение меджу первым и последним значением в процентах (приращение)
            double GetIncrement(string ticker)
            {
                if (ultimateSmootherData.TryGetValue(ticker, out List<DateValue<double>>? dateValues))
                {
                    var filteredDateValues = dateValues.Where(x => x.Date >= startDate && x.Date <= today).ToList();

                    if (filteredDateValues.Count == 0)
                        return 0.0;

                    var result = (filteredDateValues.Last().Value - filteredDateValues.First().Value) / filteredDateValues.First().Value * 100.0;

                    return Math.Round(result, 2);
                }

                return 0.0;
            }
        }

        /// <inheritdoc />
        public async Task<SelectInstrumentResponse> SelectInstrumentAsync(SelectInstrumentRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByIdAsync(request.Id);

            instrument!.IsSelected = !instrument!.IsSelected;

            await instrumentRepository.EditInstrumentAsync(instrument);

            return new SelectInstrumentResponse() { Id = request.Id };
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
                    Type = x.Type
                })
                .ToList();
        }
    }
}
