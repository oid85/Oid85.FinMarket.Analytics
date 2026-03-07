using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Моделирование портфеля облигаций
/// </summary>
[Route("api/bond-portfolio")]
[ApiController]
public class BondPortfolioController(
    IBondPortfolioService bondPortfolioService)
    : BaseController
{
    /// <summary>
    /// Получить позиции модельного портфеля
    /// </summary>
    [HttpPost("position/list")]
    [ProducesResponseType(typeof(BaseResponse<GetBondPortfolioPositionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetBondPortfolioPositionListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetBondPortfolioPositionListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetBondPortfolioPositionListAsync(
        [FromBody] GetBondPortfolioPositionListRequest request) =>
        GetResponseAsync(
            () => bondPortfolioService.GetBondPortfolioPositionListAsync(request),
            result => new BaseResponse<GetBondPortfolioPositionListResponse> { Result = result });

    /// <summary>
    /// Редактировать настройки позиции модельного портфеля
    /// </summary>
    [HttpPost("position/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioPositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioPositionResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioPositionResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditBondPortfolioPositionAsync(
        [FromBody] EditBondPortfolioPositionRequest request) =>
        GetResponseAsync(
            () => bondPortfolioService.EditBondPortfolioPositionAsync(request),
            result => new BaseResponse<EditBondPortfolioPositionResponse> { Result = result });

    /// <summary>
    /// Редактировать сумму портфеля
    /// </summary>
    [HttpPost("total-sum/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioTotalSumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioTotalSumResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditBondPortfolioTotalSumResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditBondPortfolioTotalSumAsync(
        [FromBody] EditBondPortfolioTotalSumRequest request) =>
        GetResponseAsync(
            () => bondPortfolioService.EditBondPortfolioTotalSumAsync(request),
            result => new BaseResponse<EditBondPortfolioTotalSumResponse> { Result = result });
}