using System.Net.WebSockets;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BlogService(
        IParameterRepository parameterRepository,
        IBondAnalyseService bondAnalyseService,
        IFundamentalService fundamentalService,
        IInstrumentService instrumentService,
        IPortfolioService portfolioService) 
        : IBlogService
    {
        public async Task<CreateWeekTradesPostResponse> CreateWeekTradesPostAsync(CreateWeekTradesPostRequest request)
        {
            var bondAnalyseItems = (await bondAnalyseService.GetBondAnalyseAsync(new())).Items;
            var fundamentalRatingListItems = (await fundamentalService.GetFundamentalRatingListAsync(new())).Items;
            var instruments = await instrumentService.GetInstrumentListAsync();
            var positionListResponse = (await portfolioService.GetPortfolioPositionListAsync(new()));

            // Тикеры, по которым были получены дивиденды
            List<string> receivedDividendTickers = ["MOEX", "MTSS"];

            // Тикеры купленных облигаций
            List<string> buyBondTickers = ["RU000A107050", "RU000A10E7W8"];

            // Тикеры купленных акций
            List<string> buyShareTickers = ["MOEX", "MTSS", "IRAO", "NLMK"];

            string filePath = @"c:\Users\79131\Downloads\week-trades.txt";

            var lines = new List<string>();

            lines.Add("Сделки за неделю");
            lines.Add("");

            if (receivedDividendTickers.Count > 0)
            {
                lines.Add("На неделе пришли дивиденды по акциям:");

                foreach (var ticker in receivedDividendTickers)
                {
                    var instrument = instruments.Find(x => x.Ticker == ticker); 
                    
                    lines.Add($"📌 {instrument?.Ticker} - {instrument?.Name}");
                }

                lines.Add("");
            }

            if (buyBondTickers.Count > 0)
            {
                lines.Add("💼 Облигации");
                lines.Add("");
                lines.Add("Облигации в портфель приобретаю с рейтингом АКРА не ниже A (AAA, AA и A)");
                lines.Add("и доходностью больше ключевой ставки (текущее значение 14.25 %)");
                lines.Add("");
                lines.Add("Приобрел выпуски:");

                foreach (var ticker in buyBondTickers)
                {
                    var instrument = instruments.Find(x => x.Ticker == ticker);
                    var bondAnalyseItem = bondAnalyseItems.Find(x => x.Ticker == ticker);

                    lines.Add($"📌 {instrument?.Ticker} - {instrument?.Name}");
                    lines.Add($"   Рейтинг эмитента: {bondAnalyseItem?.Rating}");
                    lines.Add($"   Доходность купонов: {bondAnalyseItem?.Yield} %");
                    lines.Add("");
                }
            }

            if (buyShareTickers.Count > 0)
            {
                lines.Add("💼 Акции");
                lines.Add("");

                foreach (var ticker in buyShareTickers)
                {
                    var instrument = instruments.Find(x => x.Ticker == ticker);
                    var fundamentalRatingListItem = fundamentalRatingListItems.Find(x => x.Ticker == ticker);
                    var position = positionListResponse.PortfolioPositions.Find(x => x.Ticker == ticker);

                    var lifePercent = ((position.LifeSize * position.Price) / positionListResponse.TotalSum * 100.0).Value.RoundTo(2);

                    lines.Add($"📌 {instrument?.Ticker} - {instrument?.Name}");

                    lines.Add($"   ДД к текущей цене: {position?.CurrentDividendYield} %");
                    lines.Add($"   Доля в портфеле: {lifePercent} %");
                    lines.Add("");
                }

                lines.Add("");
            }

            lines.Add("Регулярные покупки являются основой долгосрочной стратегии");
            lines.Add("");
            lines.Add("👉 Подписывайтесь на мои каналы");
            lines.Add("🔗 [ТГ] 🔗 [MAX] 🔗 [Дзен]");
            lines.Add("");
            lines.Add("👉 Если понравилось, ставьте 👍");
            lines.Add("");
            lines.Add("Всем успешных инвестиций!");
            lines.Add("");
            lines.Add("* Личное мнение автора. Не является индивидуальной инвестиционной рекомендацией");
            lines.Add("");
            lines.Add("#ИнвестСделки");

            File.WriteAllLines(filePath, lines);

            return new();
        }
    }
}
