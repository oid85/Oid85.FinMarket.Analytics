using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller.Life;

/// <summary>
/// Реальный портфель (7 ETF)
/// </summary>
[Route("api/seven-etf-life-portfolio")]
[ApiController]
public class SevenEtfLifePortfolioController(
    ISevenEtfLifePortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Список позиций
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<SevenEtfLifePortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<SevenEtfLifePortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<SevenEtfLifePortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> SevenEtfLifePortfolioAsync(
        [FromBody] SevenEtfLifePortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPositionListAsync(request),
            result => new BaseResponse<SevenEtfLifePortfolioResponse> { Result = result });
}