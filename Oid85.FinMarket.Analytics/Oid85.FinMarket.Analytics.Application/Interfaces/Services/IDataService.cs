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
        Task<Dictionary<string, List<DateValue<double>>>> GetClosePriceDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по купонам
        /// </summary>
        Task<Dictionary<string, List<BondCoupon>>> GetBondCouponsAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по дивиденду (последний в БД)
        /// </summary>
        Task<Dictionary<string, Dividend>> GetDividendDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по изменению относительно индекса полной доходности MCFTR
        /// </summary>
        Task<Dictionary<string, double>> GetBenchmarkChangeDataAsync(List<string> tickers);

        /// <summary>
        /// Получить рейтинг по фундаментальным данным
        /// </summary>
        Task<Dictionary<string, FundamentalScore>> GetFundamentalScoreDataAsync(List<string> tickers);

        /// <summary>
        /// Получить метрики по фундаментальным данным
        /// </summary>
        Task<Dictionary<string, List<FundamentalMetric>>> GetFundamentalMetricDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по прогнозам
        /// </summary>
        Task<Dictionary<string, Forecast>> GetConsensusForecastDataAsync();

        /// <summary>
        /// Получить данные по прогнозам (NataliaBaffetovna)
        /// </summary>
        Task<Dictionary<string, Forecast>> GetNataliaBaffetovnaForecastDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по прогнозам (FinanceMarker)
        /// </summary>
        Task<Dictionary<string, Forecast>> GetFinanceMarkerForecastDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по прогнозам (VladProDengi)
        /// </summary>
        Task<Dictionary<string, Forecast>> GetVladProDengiForecastDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по прогнозам (Mozgovik)
        /// </summary>
        Task<Dictionary<string, Forecast>> GetMozgovikForecastDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по прогнозам (прогноз чистой прибыли)
        /// </summary>
        Task<Dictionary<string, Forecast>> GetPredictNetProfitForecastDataAsync(List<string> tickers);

        /// <summary>
        /// Получить данные по див политикеб драйверам роста, рискам
        /// </summary>
        Task<Dictionary<string, (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)>> GetExtDataAsync(List<string> tickers);

        /// <summary>
        /// Получить флаг заполненности данных по фундаменталу
        /// </summary>
        Task<Dictionary<string, bool>> GetFillFundamentalDataAsync(List<string> tickers);

        /// <summary>
        /// Получить контекст
        /// </summary>
        Task<AnalyseDataContext> GetAnalyseDataContextAsync();
    }
}
