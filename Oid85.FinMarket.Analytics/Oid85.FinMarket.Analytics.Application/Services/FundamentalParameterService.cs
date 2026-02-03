using Newtonsoft.Json.Linq;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class FundamentalParameterService(
        IInstrumentService instrumentService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IFundamentalParameterService
    {
        /// <inheritdoc />
        public async Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateAnalyticFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request)
        {
            var createOrUpdateFundamentalParameterRequest = new CreateOrUpdateFundamentalParameterRequest();

            if (request.Pe2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2019, Value = request.Pe2019.Value });
            if (request.Pe2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2020, Value = request.Pe2020.Value });
            if (request.Pe2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2021, Value = request.Pe2021.Value });
            if (request.Pe2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2022, Value = request.Pe2022.Value });
            if (request.Pe2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2023, Value = request.Pe2023.Value });
            if (request.Pe2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2024, Value = request.Pe2024.Value });
            if (request.Pe2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2025, Value = request.Pe2025.Value });
            if (request.Pe2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pe, Period = KnownFundamentalParameterPeriods._2026, Value = request.Pe2026.Value });

            if (request.Ebitda2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2019, Value = request.Ebitda2019.Value });
            if (request.Ebitda2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2020, Value = request.Ebitda2020.Value });
            if (request.Ebitda2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2021, Value = request.Ebitda2021.Value });
            if (request.Ebitda2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2022, Value = request.Ebitda2022.Value });
            if (request.Ebitda2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2023, Value = request.Ebitda2023.Value });
            if (request.Ebitda2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2024, Value = request.Ebitda2024.Value });
            if (request.Ebitda2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2025, Value = request.Ebitda2025.Value });
            if (request.Ebitda2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ebitda, Period = KnownFundamentalParameterPeriods._2026, Value = request.Ebitda2026.Value });

            if (request.Revenue2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2019, Value = request.Revenue2019.Value });
            if (request.Revenue2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2020, Value = request.Revenue2020.Value });
            if (request.Revenue2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2021, Value = request.Revenue2021.Value });
            if (request.Revenue2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2022, Value = request.Revenue2022.Value });
            if (request.Revenue2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2023, Value = request.Revenue2023.Value });
            if (request.Revenue2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2024, Value = request.Revenue2024.Value });
            if (request.Revenue2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2025, Value = request.Revenue2025.Value });
            if (request.Revenue2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2026, Value = request.Revenue2026.Value });

            if (request.NetProfit2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2019, Value = request.NetProfit2019.Value });
            if (request.NetProfit2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2020, Value = request.NetProfit2020.Value });
            if (request.NetProfit2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2021, Value = request.NetProfit2021.Value });
            if (request.NetProfit2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2022, Value = request.NetProfit2022.Value });
            if (request.NetProfit2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2023, Value = request.NetProfit2023.Value });
            if (request.NetProfit2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2024, Value = request.NetProfit2024.Value });
            if (request.NetProfit2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2025, Value = request.NetProfit2025.Value });
            if (request.NetProfit2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2026, Value = request.NetProfit2026.Value });

            if (request.Ev2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2019, Value = request.Ev2019.Value });
            if (request.Ev2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2020, Value = request.Ev2020.Value });
            if (request.Ev2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2021, Value = request.Ev2021.Value });
            if (request.Ev2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2022, Value = request.Ev2022.Value });
            if (request.Ev2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2023, Value = request.Ev2023.Value });
            if (request.Ev2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2024, Value = request.Ev2024.Value });
            if (request.Ev2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2025, Value = request.Ev2025.Value });
            if (request.Ev2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Ev, Period = KnownFundamentalParameterPeriods._2026, Value = request.Ev2026.Value });

            await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;            
            
            var instruments = (await instrumentService.GetStorageInstrumentAsync()).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();            
            
            var tickers = instruments.Select(x => x.Ticker).ToList();

            var lastCandleList2019 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2019, 12, 31)) })).Result.Candles;
            var lastCandleList2020 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2020, 12, 31)) })).Result.Candles;
            var lastCandleList2021 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2021, 12, 31)) })).Result.Candles;
            var lastCandleList2022 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2022, 12, 31)) })).Result.Candles;
            var lastCandleList2023 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2023, 12, 31)) })).Result.Candles;
            var lastCandleList2024 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2024, 12, 31)) })).Result.Candles;
            var lastCandleList2025 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2025, 12, 31)) })).Result.Candles;
            var lastCandleList2026 = (await finMarketStorageServiceApiClient.GetLastCandleAsync(new() { Tickers = tickers, Date = DateOnly.FromDateTime(new DateTime(2026, 12, 31)) })).Result.Candles;

            var priceDictionary2019 = tickers.Zip(lastCandleList2019, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2020 = tickers.Zip(lastCandleList2020, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2021 = tickers.Zip(lastCandleList2021, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2022 = tickers.Zip(lastCandleList2022, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2023 = tickers.Zip(lastCandleList2023, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2024 = tickers.Zip(lastCandleList2024, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2025 = tickers.Zip(lastCandleList2025, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);
            var priceDictionary2026 = tickers.Zip(lastCandleList2026, (k, v) => new { Key = k, Value = v?.Close }).ToDictionary(item => item.Key, item => item.Value);

            var response = new GetAnalyticFundamentalParameterListResponse();

            foreach (var instrument in instruments)
            {
                var fundamentalParameter = new GetAnalyticFundamentalParameterListItemResponse();

                fundamentalParameter.Ticker = instrument.Ticker;
                fundamentalParameter.Name = instrument.Name;

                fundamentalParameter.Price2019 = priceDictionary2019[instrument.Ticker];
                fundamentalParameter.Price2020 = priceDictionary2020[instrument.Ticker];
                fundamentalParameter.Price2021 = priceDictionary2021[instrument.Ticker];
                fundamentalParameter.Price2022 = priceDictionary2022[instrument.Ticker];
                fundamentalParameter.Price2023 = priceDictionary2023[instrument.Ticker];
                fundamentalParameter.Price2024 = priceDictionary2024[instrument.Ticker];
                fundamentalParameter.Price2025 = priceDictionary2025[instrument.Ticker];
                fundamentalParameter.Price2026 = priceDictionary2026[instrument.Ticker];

                fundamentalParameter.Pe2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Pe2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Pe2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Pe2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Pe2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Pe2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Pe2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Pe2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.Ebitda2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Ebitda2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Ebitda2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Ebitda2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Ebitda2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Ebitda2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Ebitda2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Ebitda2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.Revenue2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Revenue2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Revenue2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Revenue2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Revenue2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Revenue2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Revenue2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Revenue2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.NetProfit2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.NetProfit2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.NetProfit2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.NetProfit2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.NetProfit2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.NetProfit2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.NetProfit2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.NetProfit2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.Ev2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Ev2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Ev2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Ev2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Ev2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Ev2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Ev2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Ev2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.NetDebt2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.NetDebt2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.NetDebt2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.NetDebt2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.NetDebt2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.NetDebt2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.NetDebt2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.NetDebt2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.MarketCap2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.MarketCap2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.MarketCap2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.MarketCap2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.MarketCap2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.MarketCap2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.MarketCap2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.MarketCap2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.DividendYield2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.DividendYield2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.DividendYield2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.DividendYield2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.DividendYield2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.DividendYield2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.DividendYield2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.DividendYield2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.Roa2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Roa2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Roa2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Roa2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Roa2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Roa2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Roa2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Roa2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.Pbv2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2019);
                fundamentalParameter.Pbv2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2020);
                fundamentalParameter.Pbv2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2021);
                fundamentalParameter.Pbv2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2022);
                fundamentalParameter.Pbv2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2023);
                fundamentalParameter.Pbv2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2024);
                fundamentalParameter.Pbv2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2025);
                fundamentalParameter.Pbv2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2026);

                fundamentalParameter.EvEbitda2019 = GetEvEbitda(fundamentalParameter.Ev2019, fundamentalParameter.Ebitda2019);
                fundamentalParameter.EvEbitda2020 = GetEvEbitda(fundamentalParameter.Ev2020, fundamentalParameter.Ebitda2020);
                fundamentalParameter.EvEbitda2021 = GetEvEbitda(fundamentalParameter.Ev2021, fundamentalParameter.Ebitda2021);
                fundamentalParameter.EvEbitda2022 = GetEvEbitda(fundamentalParameter.Ev2022, fundamentalParameter.Ebitda2022);
                fundamentalParameter.EvEbitda2023 = GetEvEbitda(fundamentalParameter.Ev2023, fundamentalParameter.Ebitda2023);
                fundamentalParameter.EvEbitda2024 = GetEvEbitda(fundamentalParameter.Ev2024, fundamentalParameter.Ebitda2024);
                fundamentalParameter.EvEbitda2025 = GetEvEbitda(fundamentalParameter.Ev2025, fundamentalParameter.Ebitda2025);
                fundamentalParameter.EvEbitda2026 = GetEvEbitda(fundamentalParameter.Ev2026, fundamentalParameter.Ebitda2026);

                fundamentalParameter.NetDebtEbitda2019 = GetNetDebtEbitda(fundamentalParameter.NetDebt2019, fundamentalParameter.Ebitda2019);
                fundamentalParameter.NetDebtEbitda2020 = GetNetDebtEbitda(fundamentalParameter.NetDebt2020, fundamentalParameter.Ebitda2020);
                fundamentalParameter.NetDebtEbitda2021 = GetNetDebtEbitda(fundamentalParameter.NetDebt2021, fundamentalParameter.Ebitda2021);
                fundamentalParameter.NetDebtEbitda2022 = GetNetDebtEbitda(fundamentalParameter.NetDebt2022, fundamentalParameter.Ebitda2022);
                fundamentalParameter.NetDebtEbitda2023 = GetNetDebtEbitda(fundamentalParameter.NetDebt2023, fundamentalParameter.Ebitda2023);
                fundamentalParameter.NetDebtEbitda2024 = GetNetDebtEbitda(fundamentalParameter.NetDebt2024, fundamentalParameter.Ebitda2024);
                fundamentalParameter.NetDebtEbitda2025 = GetNetDebtEbitda(fundamentalParameter.NetDebt2025, fundamentalParameter.Ebitda2025);
                fundamentalParameter.NetDebtEbitda2026 = GetNetDebtEbitda(fundamentalParameter.NetDebt2026, fundamentalParameter.Ebitda2026);

                fundamentalParameter.EbitdaRevenue2019 = GetEbitdaRevenue(fundamentalParameter.Ebitda2019, fundamentalParameter.Revenue2019);
                fundamentalParameter.EbitdaRevenue2020 = GetEbitdaRevenue(fundamentalParameter.Ebitda2020, fundamentalParameter.Revenue2020);
                fundamentalParameter.EbitdaRevenue2021 = GetEbitdaRevenue(fundamentalParameter.Ebitda2021, fundamentalParameter.Revenue2021);
                fundamentalParameter.EbitdaRevenue2022 = GetEbitdaRevenue(fundamentalParameter.Ebitda2022, fundamentalParameter.Revenue2022);
                fundamentalParameter.EbitdaRevenue2023 = GetEbitdaRevenue(fundamentalParameter.Ebitda2023, fundamentalParameter.Revenue2023);
                fundamentalParameter.EbitdaRevenue2024 = GetEbitdaRevenue(fundamentalParameter.Ebitda2024, fundamentalParameter.Revenue2024);
                fundamentalParameter.EbitdaRevenue2025 = GetEbitdaRevenue(fundamentalParameter.Ebitda2025, fundamentalParameter.Revenue2025);
                fundamentalParameter.EbitdaRevenue2026 = GetEbitdaRevenue(fundamentalParameter.Ebitda2026, fundamentalParameter.Revenue2026);

                response.FundamentalParameters.Add(fundamentalParameter);                
            }

            return response;
        }

        private static double? GetFundamentalParameterValue(List<GetFundamentalParameterListItemResponse> fundamentalParameters, string ticker, string type, string period)
        {
            if (fundamentalParameters is null) return null;
            var value = fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period);
            return value?.Value;
        }

        private static double? GetEvEbitda(double? ev, double? ebitda)
        {
            if (ev is null) return null;
            if (ebitda is null) return null;
            if (ev == 0.0) return 0.0;
            if (ebitda == 0.0) return 0.0;

            return Math.Round(ev.Value / ebitda.Value, 2);
        }

        private static double? GetNetDebtEbitda(double? netDebt, double? ebitda)
        {
            if (netDebt is null) return null;
            if (ebitda is null) return null;
            if (netDebt == 0.0) return 0.0;
            if (ebitda == 0.0) return 0.0;

            return Math.Round(netDebt.Value / ebitda.Value, 2);
        }

        private static double? GetEbitdaRevenue(double? ebitda, double? revenue)
        {
            if (ebitda is null) return null;
            if (revenue is null) return null;
            if (ebitda == 0.0) return 0.0;
            if (revenue == 0.0) return 0.0;

            return Math.Round(ebitda.Value / revenue.Value, 2);
        }
    }
}
