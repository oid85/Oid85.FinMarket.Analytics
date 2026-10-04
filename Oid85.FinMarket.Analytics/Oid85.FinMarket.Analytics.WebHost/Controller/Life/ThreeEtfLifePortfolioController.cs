using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller.Life;

/// <summary>
/// Реальный портфель (3 ETF)
/// </summary>
[Route("api/three-etf-life-portfolio")]
[ApiController]
public class ThreeEtfLifePortfolioController(
    IThreeEtfLifePortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Список позиций
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<ThreeEtfLifePortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<ThreeEtfLifePortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<ThreeEtfLifePortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> ThreeEtfLifePortfolioAsync(
        [FromBody] ThreeEtfLifePortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPositionListAsync(request),
            result => new BaseResponse<ThreeEtfLifePortfolioResponse> { Result = result });
}