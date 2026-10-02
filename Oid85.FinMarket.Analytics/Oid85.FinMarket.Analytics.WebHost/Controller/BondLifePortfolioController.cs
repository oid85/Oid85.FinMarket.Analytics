using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

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
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioPositionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioPositionListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BondLifePortfolioPositionListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BondLifePortfolioPositionListAsync(
        [FromBody] BondLifePortfolioPositionListRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetBondLifePortfolioPositionListAsync(request),
            result => new BaseResponse<BondLifePortfolioPositionListResponse> { Result = result });
}