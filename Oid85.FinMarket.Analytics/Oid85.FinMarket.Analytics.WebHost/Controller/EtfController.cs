using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Моделирование портфеля ETF
/// </summary>
[Route("api/etf-portfolio")]
[ApiController]
public class EtfController(
    IEtfPortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Получить позиции модельного портфеля
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<GetEtfPortfolioPositionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetEtfPortfolioPositionListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetEtfPortfolioPositionListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetEtfPortfolioPositionListAsync(
        [FromBody] GetEtfPortfolioPositionListRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPortfolioPositionListAsync(request),
            result => new BaseResponse<GetEtfPortfolioPositionListResponse> { Result = result });

    /// <summary>
    /// Редактировать настройки позиции модельного портфеля
    /// </summary>
    [HttpPost("position/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditEtfPortfolioPositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditEtfPortfolioPositionResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditEtfPortfolioPositionResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditEtfPortfolioPositionAsync(
        [FromBody] EditEtfPortfolioPositionRequest request) =>
        GetResponseAsync(
            () => portfolioService.EditPortfolioPositionAsync(request),
            result => new BaseResponse<EditEtfPortfolioPositionResponse> { Result = result });
}