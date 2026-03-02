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
public class BondAnalyseController(
    IBondAnalyseService bondAnalyseService)
    : BaseController
{
    /// <summary>
    /// Получить анализ облигаций
    /// </summary>
    [HttpPost("analyse")]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetBondAnalyseResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetBondAnalyseAsync(
        [FromBody] GetBondAnalyseRequest request) =>
        GetResponseAsync(
            () => bondAnalyseService.GetBondAnalyseAsync(request),
            result => new BaseResponse<GetBondAnalyseResponse> { Result = result });
}