using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Моделирование портфеля акций
/// </summary>
[Route("api/portfolio")]
[ApiController]
public class PortfolioController(
    IPortfolioBacktestService portfolioBacktestService)
    : BaseController
{
    /// <summary>
    /// Бектест портфеля
    /// </summary>
    [HttpPost("backtest")]
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioRebalanceAsync(
        [FromBody] PortfolioBacktestRequest request) =>
        GetResponseAsync(
            () => portfolioBacktestService.PortfolioBacktestAsync(request),
            result => new BaseResponse<PortfolioBacktestResponse> { Result = result });
}