using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Storage.Core.Requests;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class MacroParameterService(
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IMacroParameterService
    {
        public async Task<CreateOrUpdateAnalyticMacroParameterResponse> CreateOrUpdateAnalyticMacroParameterAsync(CreateOrUpdateAnalyticMacroParameterRequest request)
        {
            await finMarketStorageServiceApiClient.CreateOrUpdateConsumerPriceIndexChangeAsync(
                new CreateOrUpdateConsumerPriceIndexChangeRequest { Date = request.Date, Value = request.ConsumerPriceIndexChange });

            await finMarketStorageServiceApiClient.CreateOrUpdateMonetaryAggregateAsync(
                new CreateOrUpdateMonetaryAggregateRequest { Date = request.Date, M0 = request.M0, M1 = request.M1, M2 = request.M2, M2X = request.M2X });

            await finMarketStorageServiceApiClient.CreateOrUpdateKeyRateAsync(
                new CreateOrUpdateKeyRateRequest { Date = request.Date, Value = request.KeyRate });

            return new CreateOrUpdateAnalyticMacroParameterResponse();
        }

        public async Task<GetAnalyticMacroParameterListResponse> GetAnalyticMacroParameterListAsync(GetAnalyticMacroParameterListRequest request)
        {
            var consumerPriceIndexChanges = (await finMarketStorageServiceApiClient.GetConsumerPriceIndexChangeListAsync(new())).Result.ConsumerPriceIndexChanges.OrderBy(x => x.Date).ToList();
            var monetaryAggregates = (await finMarketStorageServiceApiClient.GetMonetaryAggregateListAsync(new())).Result.MonetaryAggregates.OrderBy(x => x.Date).ToList();
            var keyRates = (await finMarketStorageServiceApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var moexIndexCandles = (await finMarketStorageServiceApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.IMOEX, From = from, To = to })).Result.Candles;
            var dates = DateUtils.GetMonthDates(from, to);
            
            var macroParameterItems = new List<GetAnalyticMacroParameterItemListResponse>();

            foreach (var date in dates)
            {
                var macroParameterItem = new GetAnalyticMacroParameterItemListResponse
                {
                    Date = date
                };

                var consumerPriceIndexChange = consumerPriceIndexChanges.FirstOrDefault(x => x.Date == date);
                var monetaryAggregate = monetaryAggregates.FirstOrDefault(x => x.Date == date);
                var keyRate = keyRates.LastOrDefault(x => x.Date <= date);
                var moexIndexCandle = moexIndexCandles.FirstOrDefault(x => x.Date >= date);

                if (consumerPriceIndexChange is not null)
                    macroParameterItem.ConsumerPriceIndexChange = consumerPriceIndexChange.Value;

                if (monetaryAggregate is not null)
                {
                    macroParameterItem.M0 = monetaryAggregate.M0;
                    macroParameterItem.M1 = monetaryAggregate.M1;
                    macroParameterItem.M2 = monetaryAggregate.M2;
                    macroParameterItem.M2X = monetaryAggregate.M2X;
                }

                if (keyRate is not null)
                    macroParameterItem.KeyRate = keyRate.Value;

                if (moexIndexCandle is not null)
                    macroParameterItem.IMOEX = moexIndexCandle.Close;

                macroParameterItems.Add(macroParameterItem);
            }

            var response = new GetAnalyticMacroParameterListResponse
            {
                MacroParameters = [.. macroParameterItems.OrderByDescending(x => x.Date)]
            };

            return response;
        }
    }
}
