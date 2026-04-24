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

    /// <summary>
    /// Удалить фундаментальный параметр
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(BaseResponse<DeleteAnalyticFundamentalParameterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<DeleteAnalyticFundamentalParameterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<DeleteAnalyticFundamentalParameterResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> DeleteAnalyticFundamentalParameterAsync(
        [FromBody] DeleteAnalyticFundamentalParameterRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.DeleteAnalyticFundamentalParameterAsync(request),
            result => new BaseResponse<DeleteAnalyticFundamentalParameterResponse> { Result = result });

    /// <summary>
    /// Пузырьковая диаграмма
    /// </summary>
    [HttpPost("bubble")]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterBubbleDiagramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterBubbleDiagramResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticFundamentalParameterBubbleDiagramResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetAnalyticFundamentalParameterBubbleDiagramAsync(
        [FromBody] GetAnalyticFundamentalParameterBubbleDiagramRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.GetAnalyticFundamentalParameterBubbleDiagramAsync(request),
            result => new BaseResponse<GetAnalyticFundamentalParameterBubbleDiagramResponse> { Result = result });

    /// <summary>
    /// Получить фундаментальные параметры по сектору
    /// </summary>
    [HttpPost("sector")]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalBySectorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalBySectorResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalBySectorResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetFundamentalBySectorAsync(
        [FromBody] GetFundamentalBySectorRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.GetFundamentalBySectorAsync(request),
            result => new BaseResponse<GetFundamentalBySectorResponse> { Result = result });

    /// <summary>
    /// Получить фундаментальные параметры по компании
    /// </summary>
    [HttpPost("company")]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalByCompanyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalByCompanyResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetFundamentalByCompanyResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetFundamentalByCompanyAsync(
        [FromBody] GetFundamentalByCompanyRequest request) =>
        GetResponseAsync(
            () => fundamentalParameterService.GetFundamentalByCompanyAsync(request),
            result => new BaseResponse<GetFundamentalByCompanyResponse> { Result = result });
}