using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller.Life;

/// <summary>
/// Реальный портфель (акции)
/// </summary>
[Route("api/share-life-portfolio")]
[ApiController]
public class ShareLifePortfolioController(
    IShareLifePortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Список позиций
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<ShareLifePortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<ShareLifePortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<ShareLifePortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> ShareLifePortfolioAsync(
        [FromBody] ShareLifePortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPositionListAsync(request),
            result => new BaseResponse<ShareLifePortfolioResponse> { Result = result });
}