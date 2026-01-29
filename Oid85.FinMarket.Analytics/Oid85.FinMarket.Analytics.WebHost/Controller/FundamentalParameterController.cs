using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Фундаментальные параметры
/// </summary>
[Route("api/fundamental-parameters")]
[ApiController]
public class FundamentalParameterController(
    IFundamentalParameterService fundamentalParameterService)
    : BaseController
{
    /// <summary>
    /// Получить фундаментальные параметры
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetAnalyticFundamentalParameterListAsync(
        [FromBody] GetAnalyticFundamentalParameterListRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.GetAnalyticFundamentalParameterListAsync(request),
            result => new BaseResponse<GetAnalyticFundamentalParameterListResponse> { Result = result });

    /// <summary>
    /// Создать или изменить фундаментальный параметр
    /// </summary>
    [HttpPost("create-or-update")]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticFundamentalParameterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticFundamentalParameterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticFundamentalParameterResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CreateOrUpdateAnalyticFundamentalParameterAsync(
        [FromBody] CreateOrUpdateAnalyticFundamentalParameterRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.CreateOrUpdateAnalyticFundamentalParameterAsync(request),
            result => new BaseResponse<CreateOrUpdateAnalyticFundamentalParameterResponse> { Result = result });
}