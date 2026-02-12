using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Exceptions;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Infrastructure.ApiClients.Storage
{
    /// <inheritdoc />
    public class FinMarketStorageServiceApiClient(
        IMemoryCache memoryCache,
        IHttpClientFactory httpClientFactory)
        : IFinMarketStorageServiceApiClient
    {
        /// <inheritdoc />
        public async Task<GetCandleListResponse> GetCandleListAsync(GetCandleListRequest request)
        {
            string key = "GetCandleListAsync_" + JsonSerializer.Serialize(request);

            GetCandleListResponse? cacheResponse = null;

            if (memoryCache.TryGetValue(key, out cacheResponse))
                return cacheResponse;

            else
            {
                var response = await GetResponseAsync<GetCandleListRequest, GetCandleListResponse>("/api/candles/list", request);
                memoryCache.Set(key, response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));

                return response;
            }
        }

        /// <inheritdoc />
        public async Task<GetLastCandleResponse> GetLastCandleAsync(GetLastCandleRequest request)
        {
            string key = "GetLastCandleAsync" + JsonSerializer.Serialize(request);

            GetLastCandleResponse? cacheResponse = null;

            if (memoryCache.TryGetValue(key, out cacheResponse))
                return cacheResponse;

            else
            {
                var response = await GetResponseAsync<GetLastCandleRequest, GetLastCandleResponse>("/api/candles/last", request);
                memoryCache.Set(key, response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));

                return response;
            }
        }

        /// <inheritdoc />
        public async Task<GetInstrumentListResponse> GetInstrumentListAsync(GetInstrumentListRequest request)
        {
            string key = "GetInstrumentListAsync" + JsonSerializer.Serialize(request);

            GetInstrumentListResponse? cacheResponse = null;

            if (memoryCache.TryGetValue(key, out cacheResponse))
                return cacheResponse;

            else
            {
                var response = await GetResponseAsync<GetInstrumentListRequest, GetInstrumentListResponse>("/api/instruments/list", request);
                memoryCache.Set(key, response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));

                return response;
            }
        }

        /// <inheritdoc />
        public async Task<GetFundamentalParameterListResponse> GetFundamentalParameterListAsync(GetFundamentalParameterListRequest request)
        {
            string key = "GetFundamentalParameterListAsync" + JsonSerializer.Serialize(request);

            GetFundamentalParameterListResponse? cacheResponse = null;

            if (memoryCache.TryGetValue(key, out cacheResponse))
                return cacheResponse;

            else
            {
                var response = await GetResponseAsync<GetFundamentalParameterListRequest, GetFundamentalParameterListResponse>("/api/fundamental-parameters/list", request);
                memoryCache.Set(key, response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));

                return response;
            }
        }

        /// <inheritdoc />
        public async Task<CreateOrUpdateFundamentalParameterResponse> CreateOrUpdateFundamentalParameterAsync(CreateOrUpdateFundamentalParameterRequest request) =>
            await GetResponseAsync<CreateOrUpdateFundamentalParameterRequest, CreateOrUpdateFundamentalParameterResponse>("/api/fundamental-parameters/create-or-update", request);

        private async Task<TResponse> GetResponseAsync<TRequest, TResponse>(string url, TRequest request) where TResponse : new()
        {
            try
            {
                var content = JsonContent.Create(request);
                using var httpResponse = await SendPostRequestAsync(url, content);
                var data = await httpResponse.Content.ReadFromJsonAsync<TResponse>();
                return data ?? new TResponse();
            }

            catch (Exception exception)
            {
                throw new CustomBusinessException("500", "Ошибка при выполнении запроса", exception);
            }
        }

        private async Task<HttpResponseMessage> SendPostRequestAsync(string url, HttpContent content)
        {
            using var httpClient = httpClientFactory.CreateClient(KnownHttpClients.FinMarketStorageServiceApiClient);
            return await httpClient.PostAsync(url, content);
        }
    }
}
