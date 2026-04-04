namespace Oid85.FinMarket.Analytics.Core.Models;

public class Dividend
{
    /// <summary>
    /// Тикер
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Дата фиксации реестра
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Выплата, руб
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Доходность, %
    /// </summary>
    public double? Yield { get; set; }
}