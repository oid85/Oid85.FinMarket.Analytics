namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class PortfolioBacktestResponse
    {
        public List<PortfolioBacktestSeries> Series { get; set; } = [];
        public List<PortfolioPositionItem> PortfolioPositions { get; set; } = [];
        public double Yield { get; set; }
        public double? MaxDrawdown { get; set; } = null;
        public double? CurrentDrawdown { get; set; } = null;
        public double? DividendSum { get; set; } = null;
        public double? CouponSum { get; set; } = null;
        public double MoneySum { get; set; }
    }

    public class PortfolioPositionItem
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Тикер
        /// </summary>
        public string Ticker { get; set; } = string.Empty;

        /// <summary>
        /// Сектор
        /// </summary>
        public string Sector { get; set; } = string.Empty;

        /// <summary>
        /// Наименование компании
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Коэффициент по фундаментальному рейтингу
        /// </summary>
        public double FundamentalScoreCoefficient { get; set; } = 1;

        /// <summary>
        /// Дивидендный коэффициент
        /// </summary>
        public double DividendCoefficient { get; set; } = 1;

        /// <summary>
        /// Коэффициент капитализации
        /// </summary>
        public double MarketCapCoefficient { get; set; } = 1;

        /// <summary>
        /// Результирующий коэффициент
        /// </summary>
        public double ResultCoefficient { get; set; } = 1;

        /// <summary>
        /// Доля, %
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Текущая дивидендная доходность
        /// </summary>
        public double? CurrentDividendYield { get; set; } = null;
    }

    public class PortfolioBacktestSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<PortfolioRebalanceSeriesItem> Data { get; set; } = [];
    }

    public class PortfolioRebalanceSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
