using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class FundamentalParameterService(
        IInstrumentService instrumentService,
        IInstrumentRepository instrumentRepository,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IFundamentalParameterService
    {
        /// <inheritdoc />
        public async Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateAnalyticFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request)
        {
            var createOrUpdateFundamentalParameterRequest = new CreateOrUpdateFundamentalParameterRequest();

            if (request.Moex.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Moex, Period = string.Empty, Value = request.Moex.Value });

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

            if (request.NetDebt2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2019, Value = request.NetDebt2019.Value });
            if (request.NetDebt2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2020, Value = request.NetDebt2020.Value });
            if (request.NetDebt2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2021, Value = request.NetDebt2021.Value });
            if (request.NetDebt2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2022, Value = request.NetDebt2022.Value });
            if (request.NetDebt2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2023, Value = request.NetDebt2023.Value });
            if (request.NetDebt2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2024, Value = request.NetDebt2024.Value });
            if (request.NetDebt2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2025, Value = request.NetDebt2025.Value });
            if (request.NetDebt2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.NetDebt, Period = KnownFundamentalParameterPeriods._2026, Value = request.NetDebt2026.Value });

            if (request.MarketCap2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2019, Value = request.MarketCap2019.Value });
            if (request.MarketCap2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2020, Value = request.MarketCap2020.Value });
            if (request.MarketCap2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2021, Value = request.MarketCap2021.Value });
            if (request.MarketCap2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2022, Value = request.MarketCap2022.Value });
            if (request.MarketCap2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2023, Value = request.MarketCap2023.Value });
            if (request.MarketCap2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2024, Value = request.MarketCap2024.Value });
            if (request.MarketCap2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2025, Value = request.MarketCap2025.Value });
            if (request.MarketCap2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.MarketCap, Period = KnownFundamentalParameterPeriods._2026, Value = request.MarketCap2026.Value });

            if (request.DividendYield2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2019, Value = request.DividendYield2019.Value });
            if (request.DividendYield2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2020, Value = request.DividendYield2020.Value });
            if (request.DividendYield2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2021, Value = request.DividendYield2021.Value });
            if (request.DividendYield2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2022, Value = request.DividendYield2022.Value });
            if (request.DividendYield2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2023, Value = request.DividendYield2023.Value });
            if (request.DividendYield2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2024, Value = request.DividendYield2024.Value });
            if (request.DividendYield2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2025, Value = request.DividendYield2025.Value });
            if (request.DividendYield2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.DividendYield, Period = KnownFundamentalParameterPeriods._2026, Value = request.DividendYield2026.Value });

            if (request.Roa2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2019, Value = request.Roa2019.Value });
            if (request.Roa2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2020, Value = request.Roa2020.Value });
            if (request.Roa2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2021, Value = request.Roa2021.Value });
            if (request.Roa2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2022, Value = request.Roa2022.Value });
            if (request.Roa2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2023, Value = request.Roa2023.Value });
            if (request.Roa2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2024, Value = request.Roa2024.Value });
            if (request.Roa2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2025, Value = request.Roa2025.Value });
            if (request.Roa2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Roa, Period = KnownFundamentalParameterPeriods._2026, Value = request.Roa2026.Value });

            if (request.Pbv2019.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2019, Value = request.Pbv2019.Value });
            if (request.Pbv2020.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2020, Value = request.Pbv2020.Value });
            if (request.Pbv2021.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2021, Value = request.Pbv2021.Value });
            if (request.Pbv2022.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2022, Value = request.Pbv2022.Value });
            if (request.Pbv2023.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2023, Value = request.Pbv2023.Value });
            if (request.Pbv2024.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2024, Value = request.Pbv2024.Value });
            if (request.Pbv2025.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2025, Value = request.Pbv2025.Value });
            if (request.Pbv2026.HasValue) createOrUpdateFundamentalParameterRequest.FundamentalParameters.Add(new CreateOrUpdateFundamentalParameterItemRequest { Ticker = request.Ticker, Type = KnownFundamentalParameterTypes.Pbv, Period = KnownFundamentalParameterPeriods._2026, Value = request.Pbv2026.Value });

            await finMarketStorageServiceApiClient.CreateOrUpdateFundamentalParameterAsync(createOrUpdateFundamentalParameterRequest);

            return new CreateOrUpdateAnalyticFundamentalParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;
            
            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();            
            
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

            var fundamentalParameterItems = new List<GetAnalyticFundamentalParameterListItemResponse>();            

            foreach (var instrument in instruments)
            {
                var fundamentalParameterItem = new GetAnalyticFundamentalParameterListItemResponse();
                
                fundamentalParameterItem.Ticker = instrument.Ticker;
                fundamentalParameterItem.Name = instrument.Name;
                fundamentalParameterItem.IsSelected = instrument.IsSelected;
                fundamentalParameterItem.InPortfolio = instrument.InPortfolio;

                fundamentalParameterItem.Moex = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Moex, string.Empty);

                fundamentalParameterItem.Price2019 = priceDictionary2019[instrument.Ticker];
                fundamentalParameterItem.Price2020 = priceDictionary2020[instrument.Ticker];
                fundamentalParameterItem.Price2021 = priceDictionary2021[instrument.Ticker];
                fundamentalParameterItem.Price2022 = priceDictionary2022[instrument.Ticker];
                fundamentalParameterItem.Price2023 = priceDictionary2023[instrument.Ticker];
                fundamentalParameterItem.Price2024 = priceDictionary2024[instrument.Ticker];
                fundamentalParameterItem.Price2025 = priceDictionary2025[instrument.Ticker];
                fundamentalParameterItem.Price2026 = priceDictionary2026[instrument.Ticker];                

                fundamentalParameterItem.Pe2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Pe2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Pe2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Pe2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Pe2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Pe2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Pe2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Pe2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pe, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.Ebitda2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Ebitda2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Ebitda2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Ebitda2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Ebitda2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Ebitda2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Ebitda2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Ebitda2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.Revenue2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Revenue2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Revenue2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Revenue2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Revenue2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Revenue2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Revenue2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Revenue2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Revenue, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.NetProfit2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.NetProfit2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.NetProfit2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.NetProfit2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.NetProfit2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.NetProfit2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.NetProfit2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.NetProfit2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetProfit, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.Ev2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Ev2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Ev2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Ev2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Ev2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Ev2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Ev2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Ev2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.NetDebt2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.NetDebt2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.NetDebt2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.NetDebt2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.NetDebt2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.NetDebt2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.NetDebt2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.NetDebt2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.MarketCap2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.MarketCap2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.MarketCap2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.MarketCap2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.MarketCap2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.MarketCap2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.MarketCap2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.MarketCap2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.DividendYield2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.DividendYield2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.DividendYield2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.DividendYield2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.DividendYield2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.DividendYield2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.DividendYield2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.DividendYield2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.DividendYield, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.Roa2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Roa2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Roa2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Roa2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Roa2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Roa2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Roa2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Roa2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Roa, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.Pbv2019 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2019);
                fundamentalParameterItem.Pbv2020 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2020);
                fundamentalParameterItem.Pbv2021 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2021);
                fundamentalParameterItem.Pbv2022 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2022);
                fundamentalParameterItem.Pbv2023 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2023);
                fundamentalParameterItem.Pbv2024 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2024);
                fundamentalParameterItem.Pbv2025 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2025);
                fundamentalParameterItem.Pbv2026 = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Pbv, KnownFundamentalParameterPeriods._2026);

                fundamentalParameterItem.EvEbitda2019 = GetEvEbitda(fundamentalParameterItem.Ev2019, fundamentalParameterItem.Ebitda2019);
                fundamentalParameterItem.EvEbitda2020 = GetEvEbitda(fundamentalParameterItem.Ev2020, fundamentalParameterItem.Ebitda2020);
                fundamentalParameterItem.EvEbitda2021 = GetEvEbitda(fundamentalParameterItem.Ev2021, fundamentalParameterItem.Ebitda2021);
                fundamentalParameterItem.EvEbitda2022 = GetEvEbitda(fundamentalParameterItem.Ev2022, fundamentalParameterItem.Ebitda2022);
                fundamentalParameterItem.EvEbitda2023 = GetEvEbitda(fundamentalParameterItem.Ev2023, fundamentalParameterItem.Ebitda2023);
                fundamentalParameterItem.EvEbitda2024 = GetEvEbitda(fundamentalParameterItem.Ev2024, fundamentalParameterItem.Ebitda2024);
                fundamentalParameterItem.EvEbitda2025 = GetEvEbitda(fundamentalParameterItem.Ev2025, fundamentalParameterItem.Ebitda2025);
                fundamentalParameterItem.EvEbitda2026 = GetEvEbitda(fundamentalParameterItem.Ev2026, fundamentalParameterItem.Ebitda2026);

                fundamentalParameterItem.NetDebtEbitda2019 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2019, fundamentalParameterItem.Ebitda2019);
                fundamentalParameterItem.NetDebtEbitda2020 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2020, fundamentalParameterItem.Ebitda2020);
                fundamentalParameterItem.NetDebtEbitda2021 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2021, fundamentalParameterItem.Ebitda2021);
                fundamentalParameterItem.NetDebtEbitda2022 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2022, fundamentalParameterItem.Ebitda2022);
                fundamentalParameterItem.NetDebtEbitda2023 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2023, fundamentalParameterItem.Ebitda2023);
                fundamentalParameterItem.NetDebtEbitda2024 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2024, fundamentalParameterItem.Ebitda2024);
                fundamentalParameterItem.NetDebtEbitda2025 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2025, fundamentalParameterItem.Ebitda2025);
                fundamentalParameterItem.NetDebtEbitda2026 = GetNetDebtEbitda(fundamentalParameterItem.NetDebt2026, fundamentalParameterItem.Ebitda2026);

                fundamentalParameterItem.EbitdaRevenue2019 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2019, fundamentalParameterItem.Revenue2019);
                fundamentalParameterItem.EbitdaRevenue2020 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2020, fundamentalParameterItem.Revenue2020);
                fundamentalParameterItem.EbitdaRevenue2021 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2021, fundamentalParameterItem.Revenue2021);
                fundamentalParameterItem.EbitdaRevenue2022 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2022, fundamentalParameterItem.Revenue2022);
                fundamentalParameterItem.EbitdaRevenue2023 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2023, fundamentalParameterItem.Revenue2023);
                fundamentalParameterItem.EbitdaRevenue2024 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2024, fundamentalParameterItem.Revenue2024);
                fundamentalParameterItem.EbitdaRevenue2025 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2025, fundamentalParameterItem.Revenue2025);
                fundamentalParameterItem.EbitdaRevenue2026 = GetEbitdaRevenue(fundamentalParameterItem.Ebitda2026, fundamentalParameterItem.Revenue2026);

                fundamentalParameterItem.DeltaMinMax2019 = await GetDeltaMinMaxAsync(instrument.Ticker, 2019);
                fundamentalParameterItem.DeltaMinMax2020 = await GetDeltaMinMaxAsync(instrument.Ticker, 2020);
                fundamentalParameterItem.DeltaMinMax2021 = await GetDeltaMinMaxAsync(instrument.Ticker, 2021);
                fundamentalParameterItem.DeltaMinMax2022 = await GetDeltaMinMaxAsync(instrument.Ticker, 2022);
                fundamentalParameterItem.DeltaMinMax2023 = await GetDeltaMinMaxAsync(instrument.Ticker, 2023);
                fundamentalParameterItem.DeltaMinMax2024 = await GetDeltaMinMaxAsync(instrument.Ticker, 2024);
                fundamentalParameterItem.DeltaMinMax2025 = await GetDeltaMinMaxAsync(instrument.Ticker, 2025);
                fundamentalParameterItem.DeltaMinMax2026 = await GetDeltaMinMaxAsync(instrument.Ticker, 2026);

                fundamentalParameterItem.Score = GetScore(fundamentalParameterItem);

                fundamentalParameterItems.Add(fundamentalParameterItem);                
            }

            var response = new GetAnalyticFundamentalParameterListResponse
            {
                FundamentalParameters = [.. fundamentalParameterItems.OrderByDescending(x => x.Score)]
            };

            int number = 1;

            foreach (var fundamentalParameterItem in response.FundamentalParameters)
                fundamentalParameterItem.Number = number++;

            return response;
        }

        /// <inheritdoc />
        public async Task<GetAnalyticFundamentalParameterBubbleDiagramResponse> GetAnalyticFundamentalParameterBubbleDiagramAsync(GetAnalyticFundamentalParameterBubbleDiagramRequest request)
        {
            var fundamentalParameters = (await finMarketStorageServiceApiClient.GetFundamentalParameterListAsync(new())).Result.FundamentalParameters;

            var instruments = (await instrumentRepository.GetInstrumentsAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var response = new GetAnalyticFundamentalParameterBubbleDiagramResponse();

            foreach (var instrument in instruments)
            {
                var ebitda = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ebitda, request.Year.ToString());
                var ev = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.Ev, request.Year.ToString());
                var netDebt = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.NetDebt, request.Year.ToString());
                var marketCap = GetFundamentalParameterValue(fundamentalParameters, instrument.Ticker, KnownFundamentalParameterTypes.MarketCap, request.Year.ToString());

                var evEbitda = GetEvEbitda(ev, ebitda);
                var netDebtEbitda = GetNetDebtEbitda(netDebt, ebitda);

                if (!evEbitda.HasValue) continue;
                if (!netDebtEbitda.HasValue) continue;
                if (!marketCap.HasValue) continue;

                response.Data.Add(
                    new GetAnalyticFundamentalParameterBubbleDiagramPointResponse
                    {
                        Ticker = instrument.Ticker,
                        EvEbitda = evEbitda.Value,
                        NetDebtEbitda = netDebtEbitda.Value,
                        MarketCap = marketCap.Value
                    });
            }

            return response;
        }

        private static double? GetFundamentalParameterValue(List<GetFundamentalParameterListItemResponse> fundamentalParameters, string ticker, string type, string period)
        {
            if (fundamentalParameters is null) 
                return null;

            var value = fundamentalParameters.Find(
                x => 
                    x.Ticker == ticker && 
                    x.Type == type && 
                    x.Period == period);

            return value?.Value;
        }

        private static double? GetEvEbitda(double? ev, double? ebitda)
        {
            if (ev is null || ebitda is null) 
                return null;

            if (ev == 0.0 || ebitda == 0.0) 
                return 0.0;

            return Math.Round(ev.Value / ebitda.Value, 2);
        }

        private static double? GetNetDebtEbitda(double? netDebt, double? ebitda)
        {
            if (netDebt is null || ebitda is null)
                return null;

            if (netDebt == 0.0 || ebitda == 0.0)
                return 0.0;

            return Math.Round(netDebt.Value / ebitda.Value, 2);
        }

        private static double? GetEbitdaRevenue(double? ebitda, double? revenue)
        {
            if (revenue is null || ebitda is null)
                return null;

            if (revenue == 0.0 || ebitda == 0.0)
                return 0.0;

            return Math.Round(ebitda.Value / revenue.Value, 2);
        }

        private async Task<double?> GetDeltaMinMaxAsync(string ticker, int year)
        {
            var response = await finMarketStorageServiceApiClient.GetCandleListAsync(
                new GetCandleListRequest
                {
                    Ticker = ticker,
                    From = DateOnly.FromDateTime(new DateTime(year, 1, 1)),
                    To = DateOnly.FromDateTime(new DateTime(year, 12, 31))
                });

            if (response?.Result?.Candles is null)
                return null;

            if (response?.Result?.Candles?.Count == 0)
                return null;

            var maxCandle = response?.Result?.Candles.MaxBy(x => x.Close);
            var minCandle = response?.Result?.Candles.MinBy(x => x.Close);

            if (maxCandle is null || minCandle is null)
                return null;

            double max = maxCandle.Close;
            double min = minCandle.Close;

            return maxCandle.Date < minCandle.Date
                ? -1 * Math.Abs(Math.Round((max - min) / max * 100.0, 2)) // Падение от максимума
                : Math.Abs(Math.Round((max - min) / min * 100.0, 2)); // Рост от минимума
        }

        private static double? GetScore(GetAnalyticFundamentalParameterListItemResponse parameter)
        {
            double score = 0.0;

            // P / E
            if (parameter.Pe2021 is not null && parameter.Pe2021 > 0 && parameter.Pe2021 <= 3.0) score++;
            if (parameter.Pe2022 is not null && parameter.Pe2022 > 0 && parameter.Pe2022 <= 3.0) score++;
            if (parameter.Pe2023 is not null && parameter.Pe2023 > 0 && parameter.Pe2023 <= 3.0) score++;
            if (parameter.Pe2024 is not null && parameter.Pe2024 > 0 && parameter.Pe2024 <= 3.0) score++;

            // P / BV
            if (parameter.Pbv2021 is not null && parameter.Pbv2021 > 0 && parameter.Pbv2021 <= 1.0) score++;
            if (parameter.Pbv2022 is not null && parameter.Pbv2022 > 0 && parameter.Pbv2022 <= 1.0) score++;
            if (parameter.Pbv2023 is not null && parameter.Pbv2023 > 0 && parameter.Pbv2023 <= 1.0) score++;
            if (parameter.Pbv2024 is not null && parameter.Pbv2024 > 0 && parameter.Pbv2024 <= 1.0) score++;

            // EV / EBITDA
            if (parameter.EvEbitda2021 is not null && parameter.EvEbitda2021 <= 3.0) score++;
            if (parameter.EvEbitda2022 is not null && parameter.EvEbitda2022 <= 3.0) score++;
            if (parameter.EvEbitda2023 is not null && parameter.EvEbitda2023 <= 3.0) score++;
            if (parameter.EvEbitda2024 is not null && parameter.EvEbitda2024 <= 3.0) score++;

            // Чистый долг / EBITDA
            if (parameter.NetDebtEbitda2021 is not null && parameter.NetDebtEbitda2021 <= 1.5) score++;
            if (parameter.NetDebtEbitda2022 is not null && parameter.NetDebtEbitda2022 <= 1.5) score++;
            if (parameter.NetDebtEbitda2023 is not null && parameter.NetDebtEbitda2023 <= 1.5) score++;
            if (parameter.NetDebtEbitda2024 is not null && parameter.NetDebtEbitda2024 <= 1.5) score++;

            // Выручка
            if (parameter.Revenue2021 is not null && parameter.Revenue2021 > 0) score++;
            if (parameter.Revenue2022 is not null && parameter.Revenue2022 > 0) score++;
            if (parameter.Revenue2023 is not null && parameter.Revenue2023 > 0) score++;
            if (parameter.Revenue2024 is not null && parameter.Revenue2024 > 0) score++;

            // Рост выручки
            if (parameter.Revenue2021 is not null && parameter.Revenue2022 is not null && parameter.Revenue2021 > 0 && parameter.Revenue2022 > parameter.Revenue2021) score++;
            if (parameter.Revenue2022 is not null && parameter.Revenue2023 is not null && parameter.Revenue2022 > 0 && parameter.Revenue2023 > parameter.Revenue2022) score++;
            if (parameter.Revenue2023 is not null && parameter.Revenue2024 is not null && parameter.Revenue2023 > 0 && parameter.Revenue2024 > parameter.Revenue2023) score++;
            if (parameter.Revenue2024 is not null && parameter.Revenue2025 is not null && parameter.Revenue2024 > 0 && parameter.Revenue2025 > parameter.Revenue2024) score++;

            // Чистая прибыль
            if (parameter.NetProfit2021 is not null && parameter.NetProfit2021 > 0) score++;
            if (parameter.NetProfit2022 is not null && parameter.NetProfit2022 > 0) score++;
            if (parameter.NetProfit2023 is not null && parameter.NetProfit2023 > 0) score++;
            if (parameter.NetProfit2024 is not null && parameter.NetProfit2024 > 0) score++;

            // Рост чистой прибыли
            if (parameter.NetProfit2021 is not null && parameter.NetProfit2022 is not null && parameter.NetProfit2021 > 0 && parameter.NetProfit2022 > parameter.NetProfit2021) score++;
            if (parameter.NetProfit2022 is not null && parameter.NetProfit2023 is not null && parameter.NetProfit2022 > 0 && parameter.NetProfit2023 > parameter.NetProfit2022) score++;
            if (parameter.NetProfit2023 is not null && parameter.NetProfit2024 is not null && parameter.NetProfit2023 > 0 && parameter.NetProfit2024 > parameter.NetProfit2023) score++;
            if (parameter.NetProfit2024 is not null && parameter.NetProfit2025 is not null && parameter.NetProfit2024 > 0 && parameter.NetProfit2025 > parameter.NetProfit2024) score++;

            // Дивидендная доходность
            if (parameter.DividendYield2021 is not null && parameter.DividendYield2021 > 0) score++;
            if (parameter.DividendYield2022 is not null && parameter.DividendYield2022 > 0) score++;
            if (parameter.DividendYield2023 is not null && parameter.DividendYield2023 > 0) score++;
            if (parameter.DividendYield2024 is not null && parameter.DividendYield2024 > 0) score++;

            // Рост дивидендной доходности
            if (parameter.DividendYield2021 is not null && parameter.DividendYield2022 is not null && parameter.DividendYield2021 > 0 && parameter.DividendYield2022 > parameter.DividendYield2021) score++;
            if (parameter.DividendYield2022 is not null && parameter.DividendYield2023 is not null && parameter.DividendYield2022 > 0 && parameter.DividendYield2023 > parameter.DividendYield2022) score++;
            if (parameter.DividendYield2023 is not null && parameter.DividendYield2024 is not null && parameter.DividendYield2023 > 0 && parameter.DividendYield2024 > parameter.DividendYield2023) score++;
            if (parameter.DividendYield2024 is not null && parameter.DividendYield2025 is not null && parameter.DividendYield2024 > 0 && parameter.DividendYield2025 > parameter.DividendYield2024) score++;

            // ROA
            if (parameter.Roa2021 is not null && parameter.Roa2021 > 15) score++;
            if (parameter.Roa2022 is not null && parameter.Roa2022 > 15) score++;
            if (parameter.Roa2023 is not null && parameter.Roa2023 > 15) score++;
            if (parameter.Roa2024 is not null && parameter.Roa2024 > 15) score++;

            // EBITDA / Revenue
            if (parameter.EbitdaRevenue2021 is not null && parameter.EbitdaRevenue2021 > 0.15) score++;
            if (parameter.EbitdaRevenue2022 is not null && parameter.EbitdaRevenue2022 > 0.15) score++;
            if (parameter.EbitdaRevenue2023 is not null && parameter.EbitdaRevenue2023 > 0.15) score++;
            if (parameter.EbitdaRevenue2024 is not null && parameter.EbitdaRevenue2024 > 0.15) score++;

            return score;
        }
    }
}
