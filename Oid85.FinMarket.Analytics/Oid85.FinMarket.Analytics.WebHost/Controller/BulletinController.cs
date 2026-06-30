using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Бюллетень
/// </summary>
[Route("api/bulletin")]
[ApiController]
public class BulletinController(
    IBulletinService bulletinService)
    : BaseController
{
    /// <summary>
    /// Получить спреды
    /// </summary>
    [HttpPost("get")]
    [ProducesResponseType(typeof(BaseResponse<GetBulletinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetBulletinResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetBulletinResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetBulletinAsync(
        [FromBody] GetBulletinRequest request) =>
        GetResponseAsync(
            () => bulletinService.GetBulletinAsync(request),
            result => new BaseResponse<GetBulletinResponse> { Result = result });
}