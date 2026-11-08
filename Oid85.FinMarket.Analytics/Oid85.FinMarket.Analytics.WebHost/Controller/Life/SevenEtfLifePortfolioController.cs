using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
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
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> SevenEtfLifePortfolioAsync(
        [FromBody] LifePortfolioRequest request) =>
        GetResponseAsync(
            () => portfolioService.GetPositionListAsync(request),
            result => new BaseResponse<LifePortfolioResponse> { Result = result });

    /// <summary>
    /// Импорт позиций
    /// </summary>
    [HttpPost("position/import")]
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioImportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioImportResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<LifePortfolioImportResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> SevenEtfLifePortfolioImportAsync(
        [FromBody] LifePortfolioImportRequest request) =>
        GetResponseAsync(
            () => portfolioService.ImportPositionListAsync(request),
            result => new BaseResponse<LifePortfolioImportResponse> { Result = result });
}