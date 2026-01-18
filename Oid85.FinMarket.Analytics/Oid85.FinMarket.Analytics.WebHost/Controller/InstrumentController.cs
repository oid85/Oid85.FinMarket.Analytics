using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Инструменты
/// </summary>
[Route("api/instruments")]
[ApiController]
public class InstrumentController(
    IInstrumentService instrumentService)
    : BaseController
{
    /// <summary>
    /// Получить список инструментов
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticInstrumentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticInstrumentListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticInstrumentListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetAnalyticInstrumentListAsync(
        [FromBody] GetAnalyticInstrumentListRequest request) =>
        GetResponseAsync(
            () => instrumentService.GetAnalyticInstrumentListAsync(request),
            result => new BaseResponse<GetAnalyticInstrumentListResponse> { Result = result });

    /// <summary>
    /// Выделить инструмент
    /// </summary>
    [HttpPost("select")]
    [ProducesResponseType(typeof(BaseResponse<SelectInstrumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<SelectInstrumentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<SelectInstrumentResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> SelectInstrumentAsync(
        [FromBody] SelectInstrumentRequest request) =>
        GetResponseAsync(
            () => instrumentService.SelectInstrumentAsync(request),
            result => new BaseResponse<SelectInstrumentResponse> { Result = result });
}