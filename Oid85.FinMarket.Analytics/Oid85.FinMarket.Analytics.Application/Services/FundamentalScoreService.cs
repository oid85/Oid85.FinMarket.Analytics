using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class FundamentalScoreService(
        IParameterRepository parameterRepository,
        IAnalyseParameterFactory analyseParameterFactory) 
        : IFundamentalScoreService
    {
        /// <inheritdoc />
        public async Task<FundamentalScore?> GetFundamentalScoreAsync(string ticker)
        {
            var pe = await GetPeAsync(ticker);
            var pbv = await GetPbvAsync(ticker);
            var evEbitda = await GeEvEbitdaAsync(ticker);
            var netDebtEbitda = await GetNetDebtEbitdaAsync(ticker);
            var dividendAristocrat = await GetDividendAristocratAsync(ticker);

            double scoreValue = pe?.Ratio ?? 0.0;
            scoreValue += pbv?.Ratio ?? 0.0;
            scoreValue += evEbitda?.Ratio ?? 0.0;
            scoreValue += netDebtEbitda?.Ratio ?? 0.0;
            scoreValue += dividendAristocrat?.Ratio ?? 0.0;

            double limitLo = 5.0 / 3.0;
            double limitHi = limitLo * 2.0;
            
            var score = new FundamentalScore
            {
                Pe = pe,
                Pbv = pbv,
                EvEbitda = evEbitda,
                NetDebtEbitda = netDebtEbitda,
                DividendAristocrat = dividendAristocrat,
                Score = new AnalyseParameter<double>
                {
                    Value = scoreValue.RoundTo(2),
                    ColorFill = scoreValue >= limitHi
                        ? KnownColors.Green 
                        : scoreValue >= limitLo
                            ? KnownColors.Yellow
                            : KnownColors.White,
                    Description = string.Empty
                }
            };

            return score;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPeAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePeAsync(ticker, predictYear);
            if (result is null) return null;
            if (result.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePeAsync(ticker, year);
            if (result is null) return null;

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetPbvAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePbvAsync(ticker, predictYear);
            if (result is null) return null;
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreatePbvAsync(ticker, year);
            if (result is null) return null;

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GeEvEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, predictYear);
            if (result is null) return null;
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, year);
            if (result is null) return null;

            return result;
        }

        private async Task<AnalyseRatioParameter<double?>?> GetNetDebtEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, predictYear);
            if (result is null) return null;
            if (result!.Value.HasValue) return result;

            result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, year);
            if (result is null) return null;

            return result;
        }

        private async Task<AnalyseRatioParameter<bool?>?> GetDividendAristocratAsync(string ticker) =>
            await analyseParameterFactory.CreateDividendAristocratAsync(ticker);
    }
}
