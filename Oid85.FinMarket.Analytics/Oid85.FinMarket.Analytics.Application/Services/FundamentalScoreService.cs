using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class FundamentalScoreService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IAnalyseParameterFactory analyseParameterFactory)
        : IFundamentalScoreService
    {
        /// <inheritdoc />
        public async Task<FundamentalScore?> GetFundamentalScoreAsync(string ticker)
        {
            var instrument = (await instrumentRepository.GetInstrumentsAsync() ?? [])
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .FirstOrDefault(x => x.Ticker == ticker);

            string sector = instrument?.Sector ?? string.Empty;

            bool isBanks = sector == "Банки";

            var marketCap = await GetMarketCapAsync(ticker);
            var pe = await GetPeAsync(ticker);            
            var pbv = await GetPbvAsync(ticker);
            var evEbitda = await GeEvEbitdaAsync(ticker);
            var netDebtEbitda = await GetNetDebtEbitdaAsync(ticker);
            var netDebt = await GetNetDebtAsync(ticker);
            var debtRatio = await GetDebtRatioAsync(ticker);
            var debtEquity = await GetDebtEquityAsync(ticker);
            var netProfit = await GetNetProfitAsync(ticker);
            var fcf = await GetFcfAsync(ticker);
            var eps = await GetEpsAsync(ticker);
            var roa = await GetRoaAsync(ticker);
            var roe = await GetRoeAsync(ticker);
            var ebitdaRevenue = await GetEbitdaRevenueAsync(ticker);
            var dividendYield = await GetDividendYieldAsync(ticker);
            var dividendAristocrat = await GetDividendAristocratAsync(ticker);

            int criteriaCount = 0;

            double scoreValue = marketCap?.Ratio ?? 0.0; criteriaCount++;
            scoreValue = pe?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += pbv?.Ratio ?? 0.0; criteriaCount++;
            if (isBanks) scoreValue++; else scoreValue += evEbitda?.Ratio ?? 0.0; criteriaCount++;
            if (isBanks) scoreValue++; else scoreValue += netDebtEbitda?.Ratio ?? 0.0; criteriaCount++;
            if (isBanks) scoreValue++; else scoreValue += netDebt?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += debtRatio?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += debtEquity?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += netProfit?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += fcf?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += eps?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += roa?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += roe?.Ratio ?? 0.0; criteriaCount++;
            if (isBanks) scoreValue++; else scoreValue += ebitdaRevenue?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += dividendYield?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += dividendAristocrat?.Ratio ?? 0.0; criteriaCount++;            

            double limitLo = criteriaCount / 3.0;
            double limitHi = limitLo * 2.0;

            double scoreValueScale = scoreValue / criteriaCount * 10.0;

            var score = new FundamentalScore
            {
                MarketCap = marketCap,
                Pe = pe,
                Pbv = pbv,
                EvEbitda = evEbitda,
                NetDebtEbitda = netDebtEbitda,
                NetDebt = netDebt,
                DebtRatio = debtRatio,
                DebtEquity = debtEquity,
                NetProfit = netProfit,
                Fcf = fcf,
                Eps = eps,
                Roa = roa,
                Roe = roe,
                EbitdaRevenue = ebitdaRevenue,
                DividendYield = dividendYield,
                DividendAristocrat = dividendAristocrat,
                Score = new AnalyseParameter<double>
                {
                    Value = scoreValueScale.RoundTo(2),
                    ColorFill = GetScoreColorFill(),
                    Description = string.Empty,
                    Text = GetScoreText()
                }
            };

            string GetScoreColorFill()
            {
                if (scoreValue >= limitHi) return KnownColors.Green;
                if (scoreValue >= limitLo) return KnownColors.Yellow;
                return KnownColors.Red;
            }

            string GetScoreText()
            {
                if (scoreValue >= limitHi) return $"✅ {scoreValueScale.RoundTo(2)} из 10";
                if (scoreValue >= limitLo) return $"⚠️ {scoreValueScale.RoundTo(2)} из 10";
                return $"❗{scoreValueScale.RoundTo(2)} из 10";
            }

            return score;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetMarketCapAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateMarketCapAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateMarketCapAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPeAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePeAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePeAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPbvAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePbvAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePbvAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GeEvEbitdaAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetDebtEbitdaAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetDebtAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetDebtAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetDebtAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDebtRatioAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDebtRatioAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateDebtRatioAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDebtEquityAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDebtEquityAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateDebtEquityAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetProfitAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetProfitAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetProfitAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetFcfAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateFcfAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateFcfAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetEpsAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEpsAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEpsAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetRoaAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateRoaAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateRoaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetRoeAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateRoeAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateRoeAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetEbitdaRevenueAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEbitdaRevenueAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEbitdaRevenueAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDividendYieldAsync(string ticker)
        {
            string ttmYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.TTMYear))!;
            string year = (int.Parse(ttmYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDividendYieldAsync(ticker, ttmYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateDividendYieldAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<bool?>?> GetDividendAristocratAsync(string ticker) =>
            await analyseParameterFactory.CreateDividendAristocratAsync(ticker);
    }
}
