using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Исследование портфеля портфеля
/// </summary>
[Route("api/portfolio-science")]
[ApiController]
public class PortfolioScienceController(
    IPortfolioRebalanceService portfolioRebalanceService)
    : BaseController
{
    /// <summary>
    /// Ребалансировка портфеля
    /// </summary>
    [HttpPost("rebalance")]
    [ProducesResponseType(typeof(BaseResponse<PortfolioRebalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioRebalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioRebalanceResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioRebalanceAsync(
        [FromBody] PortfolioRebalanceRequest request) =>
        GetResponseAsync(
            () => portfolioRebalanceService.PortfolioRebalanceAsync(request),
            result => new BaseResponse<PortfolioRebalanceResponse> { Result = result });
}