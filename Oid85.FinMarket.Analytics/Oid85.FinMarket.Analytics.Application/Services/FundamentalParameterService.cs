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

            if (request.Revenue2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2019, Value = request.Revenue2019.Value });
            if (request.Revenue2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2020, Value = request.Revenue2020.Value });
            if (request.Revenue2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2021, Value = request.Revenue2021.Value });
            if (request.Revenue2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2022, Value = request.Revenue2022.Value });
            if (request.Revenue2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2023, Value = request.Revenue2023.Value });
            if (request.Revenue2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2024, Value = request.Revenue2024.Value });
            if (request.Revenue2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Revenue, Period = KnownFundamentalParameterPeriods._2025, Value = request.Revenue2025.Value });

            if (request.NetProfit2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2019, Value = request.NetProfit2019.Value });
            if (request.NetProfit2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2020, Value = request.NetProfit2020.Value });
            if (request.NetProfit2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2021, Value = request.NetProfit2021.Value });
            if (request.NetProfit2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2022, Value = request.NetProfit2022.Value });
            if (request.NetProfit2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2023, Value = request.NetProfit2023.Value });
            if (request.NetProfit2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2024, Value = request.NetProfit2024.Value });
            if (request.NetProfit2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetProfit, Period = KnownFundamentalParameterPeriods._2025, Value = request.NetProfit2025.Value });

            await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;
            
            var instruments = (await instrumentService.GetStorageInstrumentAsync()).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            
            var response = new GetAnalyticFundamentalParameterListResponse();

            foreach (var instrument in instruments)
                response.FundamentalParameters.Add(
                    new GetAnalyticFundamentalParameterListItemResponse
                    {
                        Ticker = instrument.Ticker,

                        Pe2019 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2019),
                        Pe2020 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2020),
                        Pe2021 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2021),
                        Pe2022 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2022),
                        Pe2023 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2023),
                        Pe2024 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2024),
                        Pe2025 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2025),

                        Revenue2019 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2019),
                        Revenue2020 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2020),
                        Revenue2021 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2021),
                        Revenue2022 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2022),
                        Revenue2023 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2023),
                        Revenue2024 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2024),
                        Revenue2025 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2025),

                        NetProfit2019 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2019),
                        NetProfit2020 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2020),
                        NetProfit2021 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2021),
                        NetProfit2022 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2022),
                        NetProfit2023 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2023),
                        NetProfit2024 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2024),
                        NetProfit2025 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2025)
                    });

            return response;
        }

        private static double? GetFundanemtalParameterValue(List<GetFundamentalParameterListItemResponse> fundamentalParameters, string ticker, string type, string period)
        {
            if (fundamentalParameters is null)
                return null;

            var value = fundamentalParameters.Find(x => x.Ticker == ticker && x.Type == type && x.Period == period);

            return value?.Value;
        }
    }
}
