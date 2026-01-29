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
            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;
            
            var instruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();
            
            var response = new GetAnalyticFundamentalParameterListResponse();

            foreach (var instrument in instruments)
                response.FundamentalParameters.Add(
                    new GetAnalyticFundamentalParameterListItemResponse
                    {
                        Ticker = instrument.Ticker,
                        Pe = new GetAnalyticFundamentalParameterListItemValueResponse
                        {
                            _2019 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2019),
                            _2020 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2020),
                            _2021 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2021),
                            _2022 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2022),
                            _2023 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2023),
                            _2024 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2024),
                            _2025 = GetFundanemtalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2025)
                        }
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
