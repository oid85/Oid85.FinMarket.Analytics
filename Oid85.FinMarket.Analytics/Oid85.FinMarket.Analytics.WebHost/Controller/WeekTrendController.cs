using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Тренды по неделям
/// </summary>
[Route("api/week-trends")]
[ApiController]
public class WeekTrendController(
    IWeekTrendService weekTrendService)
    : BaseController
{
    /// <summary>
    /// Получить изменения цены по неделям
    /// </summary>
    [HttpPost("delta")]
    [ProducesResponseType(typeof(BaseResponse<GetWeekDeltaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetWeekDeltaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetWeekDeltaResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetWeekDeltaAsync(
        [FromBody] GetWeekDeltaRequest request) =>
        GetResponseAsync(
            () => weekTrendService.GetWeekDeltaAsync(request),
            result => new BaseResponse<GetWeekDeltaResponse> { Result = result });
}