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

            var score = new FundamentalScore
            {
                Pe = pe,
                Pbv = pbv,
                EvEbitda = evEbitda,
                NetDebtEbitda = netDebtEbitda,
                DividendAristocrat = dividendAristocrat,
                ScoreValue = scoreValue.RoundTo(2)
            };

            return score;
        }

        private async Task<AnalyseParameter<double?>?> GetPeAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePeAsync(ticker, predictYear);
            if (result!.Value.HasValue) return result;

            return await analyseParameterFactory.CreatePeAsync(ticker, year);
        }

        private async Task<AnalyseParameter<double?>?> GetPbvAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreatePbvAsync(ticker, predictYear);
            if (result!.Value.HasValue) return result;

            return await analyseParameterFactory.CreatePbvAsync(ticker, year);
        }

        private async Task<AnalyseParameter<double?>?> GeEvEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateEvEbitdaAsync(ticker, predictYear);
            if (result!.Value.HasValue) return result;

            return await analyseParameterFactory.CreateEvEbitdaAsync(ticker, year);
        }

        private async Task<AnalyseParameter<double?>?> GetNetDebtEbitdaAsync(string ticker)
        {
            string predictYear = (await parameterRepository.GetParameterValueAsync(KnownParameters.PredictYear))!;
            string year = (int.Parse(predictYear) - 1).ToString();

            var result = await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, predictYear);
            if (result!.Value.HasValue) return result;

            return await analyseParameterFactory.CreateNetDebtEbitdaAsync(ticker, year);
        }

        private async Task<AnalyseParameter<bool?>?> GetDividendAristocratAsync(string ticker) =>
            await analyseParameterFactory.CreateDividendAristocratAsync(ticker);
    }
}
