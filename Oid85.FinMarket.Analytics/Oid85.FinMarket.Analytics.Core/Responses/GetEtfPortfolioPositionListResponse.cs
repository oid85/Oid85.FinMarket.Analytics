namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetEtfPortfolioPositionListResponse
    {
        public double TotalSum { get; set; }
        public List<EtfPortfolioPositionListItem> PortfolioPositions { get; set; } = [];
    }

    public class EtfPortfolioPositionListItem
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
        /// Наименование компании
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ручной коэффициент
        /// </summary>
        public double ManualCoefficient { get; set; }

        /// <summary>
        /// Результирующий коэффициент
        /// </summary>
        public double ResultCoefficient { get; set; }

        /// <summary>
        /// Стоимость позиции
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Размер позиции расчетный
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
