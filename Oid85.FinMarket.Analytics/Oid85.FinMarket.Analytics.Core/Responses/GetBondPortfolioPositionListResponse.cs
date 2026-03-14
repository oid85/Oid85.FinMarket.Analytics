using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetBondPortfolioPositionListResponse
    {
        public double TotalSum { get; set; }
        public double YearCouponSum { get; set; }
        public double YearCouponPrc { get; set; }
        public double MonthCouponSum { get; set; }
        public List<GetBondPortfolioPositionListItemResponse> PortfolioPositions { get; set; } = [];
    }

    public class GetBondPortfolioPositionListItemResponse
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
        /// Купонный коэффициент
        /// </summary>
        public double CouponCoefficient { get; set; }

        /// <summary>
        /// Временной коэффициент
        /// </summary>
        public double TimeCoefficient { get; set; }

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
        /// Годовой купон
        /// </summary>
        public double YearCoupon { get; set; }

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
        
        /// <summary>
        /// Позиция в реальном портфеле
        /// </summary>
        public int LifeSize { get; set; }
    }
}
