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
    IPortfolioService portfolioService)
    : BaseController
{
    /// <summary>
    /// Получить позиции модельного портфеля
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioPositionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioPositionListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioPositionListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetPortfolioPositionListAsync(
        [FromBody] GetPortfolioPositionListRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPortfolioPositionListAsync(request),
            result => new BaseResponse<GetPortfolioPositionListResponse> { Result = result });

    /// <summary>
    /// Редактировать настройки позиции модельного портфеля
    /// </summary>
    [HttpPost("position/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioPositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioPositionResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioPositionResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditPortfolioPositionAsync(
        [FromBody] EditPortfolioPositionRequest request) =>
        GetResponseAsync(
            () => portfolioService.EditPortfolioPositionAsync(request),
            result => new BaseResponse<EditPortfolioPositionResponse> { Result = result });

    /// <summary>
    /// Редактировать сумму портфеля
    /// </summary>
    [HttpPost("total-sum/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditPortfolioTotalSumAsync(
        [FromBody] EditPortfolioTotalSumRequest request) =>
        GetResponseAsync(
            () => portfolioService.EditPortfolioTotalSumAsync(request),
            result => new BaseResponse<EditPortfolioTotalSumResponse> { Result = result });
}