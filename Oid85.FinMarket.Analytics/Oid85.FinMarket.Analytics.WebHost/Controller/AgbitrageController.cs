using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Арбитраж
/// </summary>
[Route("api/agbitrage")]
[ApiController]
public class AgbitrageController(
    IArbitrageService arbitrageService)
    : BaseController
{
    /// <summary>
    /// Получить спреды
    /// </summary>
    [HttpPost("spreads")]
    [ProducesResponseType(typeof(BaseResponse<GetSpreadListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetSpreadListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetSpreadListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetSpreadListAsync(
        [FromBody] GetSpreadListRequest request) =>
        GetResponseAsync(
            () => arbitrageService.GetSpreadListAsync(request),
            result => new BaseResponse<GetSpreadListResponse> { Result = result });
}