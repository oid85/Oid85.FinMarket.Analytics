using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Блог
/// </summary>
[Route("api/blog")]
[ApiController]
public class BlogController(
    IBlogService blogService)
    : BaseController
{
    /// <summary>
    /// Сгенерировать пост Сделки за неделю
    /// </summary>
    [HttpPost("posts/week-trades/create")]
    [ProducesResponseType(typeof(BaseResponse<CreateWeekTradesPostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateWeekTradesPostResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CreateWeekTradesPostResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CreateWeekTradesPostAsync(
        [FromBody] CreateWeekTradesPostRequest request) =>
        GetResponseAsync(
            () => blogService.CreateWeekTradesPostAsync(request),
            result => new BaseResponse<CreateWeekTradesPostResponse> { Result = result });
}