namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetPortfolioPositionListResponse
    {
        public List<GetPortfolioPositionListItemResponse> PortfolioPositions { get; set; } = [];
    }

    public class GetPortfolioPositionListItemResponse
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
        /// Информационное сообщение
        /// </summary>
        public string Message { get; set; } = string.Empty;        
    }
}
