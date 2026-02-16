using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис инструментов
    /// </summary>
    public interface IInstrumentService
    {
        /// <summary>
        /// Получить список инструментов
        /// </summary>
        Task<GetAnalyticInstrumentListResponse> GetAnalyticInstrumentListAsync(GetAnalyticInstrumentListRequest request);

        /// <summary>
        /// Выделить инстремент
        /// </summary>
        Task<SelectInstrumentResponse> SelectInstrumentAsync(SelectInstrumentRequest request);

        /// <summary>
        /// Получить инструменты с хранилища
        /// </summary>
        Task<List<Instrument>> GetStorageInstrumentAsync();

        /// <summary>
        /// Синхронизировать инструменты со Storage
        /// </summary>
        Task SyncInstrumentListAsync();
    }
}
