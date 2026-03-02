using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Облигации
/// </summary>
[Route("api/bond")]
[ApiController]
public class BondAnalyticController(
    IBondAnalyticService bondAnalyticService)
    : BaseController
{
    /// <summary>
    /// Получить анализ облигаций
    /// </summary>
    [HttpPost("analytic")]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyticResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyticResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyticResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetBondAnalyticAsync(
        [FromBody] GetBondAnalyticRequest request) =>
        GetResponseAsync(
            () => bondAnalyticService.GetBondAnalyticAsync(request),
            result => new BaseResponse<GetBondAnalyticResponse> { Result = result });
}