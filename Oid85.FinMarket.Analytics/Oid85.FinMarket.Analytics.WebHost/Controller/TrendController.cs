using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Тренды
/// </summary>
[Route("api/trends")]
[ApiController]
public class TrendController(
    ITrendDynamicService trendDynamicService,
    ICompareTrendService compareTrendService)
    : BaseController
{
    /// <summary>
    /// Получить анализ динамики трендов
    /// </summary>
    [HttpPost("dynamic")]
    [ProducesResponseType(typeof(BaseResponse<GetTrendDynamicResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetTrendDynamicResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetTrendDynamicResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetTrendDynamicAsync(
        [FromBody] GetTrendDynamicRequest request) =>
        GetResponseAsync(
            () => trendDynamicService.GetTrendDynamicAsync(request),
            result => new BaseResponse<GetTrendDynamicResponse> { Result = result });

    /// <summary>
    /// Получить анализ сравнения трендов
    /// </summary>
    [HttpPost("compare")]
    [ProducesResponseType(typeof(BaseResponse<GetCompareTrendResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetCompareTrendResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetCompareTrendResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetCompareTrendAsync(
        [FromBody] GetCompareTrendRequest request) =>
        GetResponseAsync(
            () => compareTrendService.GetCompareTrendAsync(request),
            result => new BaseResponse<GetCompareTrendResponse> { Result = result });
}