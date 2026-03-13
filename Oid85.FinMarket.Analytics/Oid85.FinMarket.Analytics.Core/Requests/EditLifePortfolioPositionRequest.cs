using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class EditLifePortfolioPositionRequest
    {
        public string Ticker { get; set; }
        public int Size { get; set; }
    }
}
