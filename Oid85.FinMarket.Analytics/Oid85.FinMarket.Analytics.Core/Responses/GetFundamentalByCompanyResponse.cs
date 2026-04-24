using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalByCompanyResponse
    {
        /// <summary>
        /// Тикер
        /// </summary>
        public string? Ticker { get; set; } = null;

        /// <summary>
        /// Инструмент в портфеле
        /// </summary>
        public bool InPortfolio { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string? Name { get; set; } = null;

        /// <summary>
        /// Сектор
        /// </summary>
        public string? Sector { get; set; } = null;

        /// <summary>
        /// Текущая цена
        /// </summary>
        public double? Price { get; set; } = null;

        /// <summary>
        /// Состояние тренда
        /// </summary>
        public string? TrendState { get; set; } = null;

        /// <summary>
        /// График с ценой
        /// </summary>
        public List<PriceDiagramDataPoint> PriceDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика чистой прибыли по периодам
        /// </summary>
        public List<BarDiagramDataPoint> NetProfitDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика дивидендов по периодам
        /// </summary>
        public List<BarDiagramDataPoint> DividendDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика P/E по периодам
        /// </summary>
        public List<BarDiagramDataPoint> PeDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика P/BV по периодам
        /// </summary>
        public List<BarDiagramDataPoint> PbvDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика FCF по периодам
        /// </summary>
        public List<BarDiagramDataPoint> FcfDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика EPS по периодам
        /// </summary>
        public List<BarDiagramDataPoint> EpsDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика EV/EBITDA по периодам
        /// </summary>
        public List<BarDiagramDataPoint> EvEbitdaDiagramData { get; set; } = [];

        /// <summary>
        /// Динамика NetDebt/EBITDA по периодам
        /// </summary>
        public List<BarDiagramDataPoint> NetDebtEbitdaDiagramData { get; set; } = [];

        /// <summary>
        /// Сравнение P/E с другими компаниями из сектора
        /// </summary>
        public List<BarDiagramDataPoint> PeSectorDiagramData { get; set; } = [];

        /// <summary>
        /// Сравнение P/BV с другими компаниями из сектора
        /// </summary>
        public List<BarDiagramDataPoint> PbvSectorDiagramData { get; set; } = [];

        /// <summary>
        /// Сравнение EV/EBITDA с другими компаниями из сектора
        /// </summary>
        public List<BarDiagramDataPoint> EvEbitdaSectorDiagramData { get; set; } = [];

        /// <summary>
        /// Сравнение NetDebt/EBITDA с другими компаниями из сектора
        /// </summary>
        public List<BarDiagramDataPoint> NetDebtEbitdaSectorDiagramData { get; set; } = [];

        /// <summary>
        /// Актуальный дивиденд
        /// </summary>
        public Dividend? Dividend { get; set; } = null;

        /// <summary>
        /// Консенсус прогноз
        /// </summary>
        public Forecast? ConsensusForecast { get; set; } = null;

        /// <summary>
        /// Прогноз от NataliaBaffetovna
        /// </summary>
        public Forecast? NataliaBaffetovnaForecast { get; set; } = null;

        /// <summary>
        /// Прогноз от FinanceMarker
        /// </summary>
        public Forecast? FinanceMarkerForecast { get; set; } = null;

        /// <summary>
        /// Прогноз от VladProDengi
        /// </summary>
        public Forecast? VladProDengiForecast { get; set; } = null;

        /// <summary>
        /// Прогноз методом прогноза чистой прибыли
        /// </summary>
        public Forecast? PredictNetProfitForecast { get; set; } = null;

        /// <summary>
        /// Рейтинг на основе фунд. анализа от 0 до 1
        /// </summary>
        public FundamentalScore? FundamentalScore { get; set; } = null;

        /// <summary>
        /// Актуальные фундаментальные параметры компании
        /// </summary>
        public FundamentalMetric? CompanyFundamentalMetric { get; set; } = null;

        /// <summary>
        /// Изменение относительно индекса полной доходности MCFTR
        /// </summary>
        public double? BenchmarkChange { get; set; } = null;
        
        /// <summary>
        /// Дивидендная политика
        /// </summary>
        public string? DividendPolyticInfo { get; set; } = null;

        /// <summary>
        /// Драйверы роста
        /// </summary>
        public string? GrowthDriverInfo { get; set; } = null;

        /// <summary>
        /// Риски
        /// </summary>
        public string? RiskInfo { get; set; } = null;

        /// <summary>
        /// Идея
        /// </summary>
        public string? Concept { get; set; } = null;
    }

    public class PriceDiagramDataPoint
    {
        public DateOnly? Date { get; set; } = null;
        public double? PriceValue { get; set; } = null;
        public double? UltimateSmootherValue { get; set; } = null;
    }

    public class BarDiagramDataPoint
    {
        public string? X { get; set; } = null;
        public double? Y { get; set; } = null;
    }
}
