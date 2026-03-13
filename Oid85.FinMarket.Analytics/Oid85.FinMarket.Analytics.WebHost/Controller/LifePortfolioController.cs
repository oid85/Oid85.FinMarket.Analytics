using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Реальный портфель
/// </summary>
[Route("api/life-portfolio")]
[ApiController]
public class LifePortfolioController(
    ILifePortfolioService lifePortfolioService)
    : BaseController
{
    /// <summary>
    /// Импорт позиций
    /// </summary>
    [HttpPost("position/import")]
    [ProducesResponseType(typeof(BaseResponse<ImportLifePortfolioPositionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<ImportLifePortfolioPositionListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<ImportLifePortfolioPositionListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> ImportLifePortfolioPositionListAsync(
        [FromBody] ImportLifePortfolioPositionListRequest request) =>
        GetResponseAsync(
            () => lifePortfolioService.ImportLifePortfolioPositionListAsync(request),
            result => new BaseResponse<ImportLifePortfolioPositionListResponse> { Result = result });

    /// <summary>
    /// Редактирование позиции
    /// </summary>
    [HttpPost("position/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditLifePortfolioPositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditLifePortfolioPositionResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditLifePortfolioPositionResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditLifePortfolioPositionAsync(
        [FromBody] EditLifePortfolioPositionRequest request) =>
        GetResponseAsync(
            () => lifePortfolioService.EditLifePortfolioPositionAsync(request),
            result => new BaseResponse<EditLifePortfolioPositionResponse> { Result = result });
}