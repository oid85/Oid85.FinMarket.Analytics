using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Параметры макроэкономики
/// </summary>
[Route("api/macro-parameters")]
[ApiController]
public class MacroParameterController(
    IMacroService macroParameterService)
    : BaseController
{
    /// <summary>
    /// Создать или изменить макро параметр
    /// </summary>
    [HttpPost("create-or-update")]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticMacroParameterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticMacroParameterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrUpdateAnalyticMacroParameterResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CreateOrUpdateAnalyticMacroParameterAsync(
        [FromBody] CreateOrUpdateAnalyticMacroParameterRequest request) =>
        GetResponseAsync(
            () => macroParameterService.CreateOrUpdateAnalyticMacroParameterAsync(request),
            result => new BaseResponse<CreateOrUpdateAnalyticMacroParameterResponse> { Result = result });

    /// <summary>
    /// Получить макро параметры (таблица)
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetAnalyticMacroParameterListAsync(
        [FromBody] GetAnalyticMacroParameterListRequest request) =>
        GetResponseAsync(
            () => macroParameterService.GetAnalyticMacroParameterListAsync(request),
            result => new BaseResponse<GetAnalyticMacroParameterListResponse> { Result = result });

    /// <summary>
    /// Получить макро параметры (диаграммы)
    /// </summary>
    [HttpPost("diagram")]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterDiagramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterDiagramResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetAnalyticMacroParameterDiagramResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetAnalyticMacroParameterDiagramAsync(
        [FromBody] GetAnalyticMacroParameterDiagramRequest request) =>
        GetResponseAsync(
            () => macroParameterService.GetAnalyticMacroParameterDiagramAsync(request),
            result => new BaseResponse<GetAnalyticMacroParameterDiagramResponse> { Result = result });
}