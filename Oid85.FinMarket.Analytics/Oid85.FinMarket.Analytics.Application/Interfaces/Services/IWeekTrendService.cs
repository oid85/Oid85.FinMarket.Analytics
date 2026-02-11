using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис WeekTrend
    /// </summary>
    public interface IWeekTrendService
    {
        /// <summary>
        /// Изменение цены по неделям
        /// </summary>
        Task<GetWeekDeltaResponse> GetWeekDeltaAsync(GetWeekDeltaRequest request);
    }
}
