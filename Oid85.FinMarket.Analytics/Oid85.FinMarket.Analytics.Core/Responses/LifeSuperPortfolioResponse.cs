namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class LifeSuperPortfolioResponse
    {
        public double TotalSum { get; set; }
        public List<LifePortfolioItem> Portfolios { get; set; } = [];
    }

    public class LifePortfolioItem
    {
        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Доля (расч.)
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Стоимость (расч.)
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Стоимость (life)
        /// </summary>
        public double LifeCost { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной стоимостью в процентах
        /// </summary>
        public double DeltaPercent { get; set; }

        /// <summary>
        /// Разница между реальной и расчетной стоимостью в процентах (текст)
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
