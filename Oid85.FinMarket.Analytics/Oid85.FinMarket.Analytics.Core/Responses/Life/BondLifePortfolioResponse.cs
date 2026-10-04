namespace Oid85.FinMarket.Analytics.Core.Responses.Life
{
    public class BondLifePortfolioResponse
    {
        public double TotalSum { get; set; }
        public List<BondLifePositionListItem> PortfolioPositions { get; set; } = [];
    }

    public class BondLifePositionListItem
    {
        /// <summary>
        /// Порядковый номер
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
        /// Купонная доходность, %
        /// </summary>
        public double Yield { get; set; }

        /// <summary>
        /// Кредитный рейтинг
        /// </summary>
        public string Rating { get; set; } = string.Empty;

        /// <summary>
        /// Стоимость позиции (расч.)
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Размер позиции (расч.)
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Размер позиции (life)
        /// </summary>
        public int LifeSize { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Доля (расч.)
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной позицией
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной позицией (текст)
        /// </summary>
        public string DeltaText { get; set; } = string.Empty;

        /// <summary>
        /// Разница между реальной и расчетной позицией в процентах
        /// </summary>
        public double DeltaPercent { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной позицией в процентах (текст)
        /// </summary>
        public string DeltaPercentText { get; set; } = string.Empty;

        /// <summary>
        /// Рекомендация
        /// </summary>
        public string Recommendation { get; set; } = string.Empty;

        /// <summary>
        /// Цвет
        /// </summary>
        public string ColorFill { get; set; } = string.Empty;
    }
}
