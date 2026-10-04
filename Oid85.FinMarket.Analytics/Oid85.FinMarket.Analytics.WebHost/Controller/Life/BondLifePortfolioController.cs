using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller.Life;

/// <summary>
/// Реальный портфель (облигации)
/// </summary>
[Route("api/bond-life-portfolio")]
[ApiController]
public class BondLifePortfolioController(
    IBondLifePortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Список позиций
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BondLifePortfolioAsync(
        [FromBody] BondLifePortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPositionListAsync(request),
            result => new BaseResponse<BondLifePortfolioResponse> { Result = result });
}