using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface ILifePortfolioPositionRepository
    {
        Task AddLifePortfolioPositionAsync(LifePortfolioPosition lifePosition);
        Task DeleteAllLifePortfolioPositionAsync();
        Task EditLifePortfolioPositionAsync(string ticker, int size);
        Task<List<LifePortfolioPosition>> GetLifePortfolioPositionsAsync();
    }
}
