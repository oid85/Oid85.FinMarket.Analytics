using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller.Life;

/// <summary>
/// Реальный портфель (общий)
/// </summary>
[Route("api/life-super-portfolio")]
[ApiController]
public class LifeSuperPortfolioController(
    ILifeSuperPortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Общий портфель
    /// </summary>
    [HttpPost("get")]
    [ProducesResponseType(typeof(BaseResponse<LifeSuperPortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<LifeSuperPortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<LifeSuperPortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> LifeSuperPortfolioAsync(
        [FromBody] LifeSuperPortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetLifeSuperPortfolioAsync(request),
            result => new BaseResponse<LifeSuperPortfolioResponse> { Result = result });
}