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

            var pe = await GetPeAsync(ticker);
            var pbv = await GetPbvAsync(ticker);
            var evEbitda = await GeEvEbitdaAsync(ticker);
            var netDebtEbitda = await GetNetDebtEbitdaAsync(ticker);
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

            double scoreValue = pe?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += pbv?.Ratio ?? 0.0; criteriaCount++;
            if (!isBanks) scoreValue += evEbitda?.Ratio ?? 0.0; criteriaCount++;
            if (!isBanks) scoreValue += netDebtEbitda?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += debtRatio?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += debtEquity?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += netProfit?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += fcf?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += eps?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += roa?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += roe?.Ratio ?? 0.0; criteriaCount++;
            if (!isBanks) scoreValue += ebitdaRevenue?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += dividendYield?.Ratio ?? 0.0; criteriaCount++;
            scoreValue += dividendAristocrat?.Ratio ?? 0.0; criteriaCount++;

            if (isBanks) criteriaCount -= 3;

            double limitLo = criteriaCount / 3.0;
            double limitHi = limitLo * 2.0;

            double scoreValueScale = scoreValue / criteriaCount * 10.0;

            var score = new FundamentalScore
            {
                Pe = pe,
                Pbv = pbv,
                EvEbitda = evEbitda,
                NetDebtEbitda = netDebtEbitda,
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
                    ColorFill = GetColorFill(),
                    Description = string.Empty,
                    Text = GetText()
                }
            };

            string GetColorFill()
            {
                if (scoreValue >= limitHi) return KnownColors.Green;
                if (scoreValue >= limitLo) return KnownColors.Yellow;
                return KnownColors.White;
            }

            string GetText()
            {
                if (scoreValue >= limitHi) return $"✅ {scoreValueScale.RoundTo(2)} из 10";
                if (scoreValue >= limitLo) return $"⚠️ {scoreValueScale.RoundTo(2)} из 10";
                return $"❗{scoreValueScale.RoundTo(2)} из 10";
            }

            return score;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPeAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePeAsync(ticker, predictYear);
            if (result is null) return new();
            if (result.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePeAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPbvAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePbvAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePbvAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GeEvEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetDebtEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDebtRatioAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDebtRatioAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateDebtRatioAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDebtEquityAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDebtEquityAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateDebtEquityAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetProfitAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetProfitAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetProfitAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetFcfAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateFcfAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateFcfAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetEpsAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEpsAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEpsAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetRoaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateRoaAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateRoaAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetRoeAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateRoeAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateRoeAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetEbitdaRevenueAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEbitdaRevenueAsync(ticker, predictYear);
            if (result is null) return new();
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEbitdaRevenueAsync(ticker, year);
            if (result is null) return new();

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetDividendYieldAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateDividendYieldAsync(ticker, predictYear);
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
