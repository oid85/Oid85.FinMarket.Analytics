namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetPortfolioPositionListResponse
    {
        public double TotalSum { get; set; }
        public List<PortfolioPositionListItem> PortfolioPositions { get; set; } = [];
    }

    public class PortfolioPositionListItem
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
        /// Дивидендный коэффициент
        /// </summary>
        public double DividendCoefficient { get; set; }

        /// <summary>
        /// Трендовый коэффициент
        /// </summary>
        public double TrendCoefficient { get; set; }

        /// <summary>
        /// Ручной коэффициент
        /// </summary>
        public double ManualCoefficient { get; set; }

        /// <summary>
        /// Коэффициент капитализации
        /// </summary>
        public double MarketCapCoefficient { get; set; }

        /// <summary>
        /// Результирующий коэффициент
        /// </summary>
        public double ResultCoefficient { get; set; }

        /// <summary>
        /// Стоимость позиции
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Размер позиции
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Доля, %
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Позиция в реальном портфеле
        /// </summary>
        public int LifeSize { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной позицией
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной позицией в процентах
        /// </summary>
        public double DeltaPercent { get; set; }        
    }
}
