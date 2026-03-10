using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис данных
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Получить данные по свечам
        /// </summary>
        Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по индикатору Ultimate Smoother
        /// </summary>
        Task<Dictionary<string, List<DateValue<double>>>> GetUltimateSmootherDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные для диаграммы временного ряда
        /// </summary>
        Task<Dictionary<string, List<DateValue<double>>>> GetClosePriceDiagramDataAsync(List<string> tickers);
    }
}
