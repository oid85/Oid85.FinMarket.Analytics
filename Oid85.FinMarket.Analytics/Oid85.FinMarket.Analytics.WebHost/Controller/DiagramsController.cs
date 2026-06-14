using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.WebHost.Controller.Base;

namespace Oid85.FinMarket.Analytics.WebHost.Controller;

/// <summary>
/// Графики
/// </summary>
[Route("api/diagrams")]
[ApiController]
public class DiagramsController(
    IDiagramService diagramService)
    : BaseController
{
    /// <summary>
    /// Графики цен
    /// </summary>
    [HttpPost("close-price")]
    [ProducesResponseType(typeof(BaseResponse<GetClosePriceDiagramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetClosePriceDiagramResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetClosePriceDiagramResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetClosePriceDiagramAsync(
        [FromBody] GetClosePriceDiagramRequest request) =>
        GetResponseAsync(
            () => diagramService.GetClosePriceDiagramAsync(request),
            result => new BaseResponse<GetClosePriceDiagramResponse> { Result = result });

    /// <summary>
    /// График индикатора TrendAggregate
    /// </summary>
    [HttpPost("trend-aggregate")]
    [ProducesResponseType(typeof(BaseResponse<GetTrendAggregateDiagramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetTrendAggregateDiagramResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetTrendAggregateDiagramResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetTrendAggregateDiagramAsync(
        [FromBody] GetTrendAggregateDiagramRequest request) =>
        GetResponseAsync(
            () => diagramService.GetTrendAggregateDiagramAsync(request),
            result => new BaseResponse<GetTrendAggregateDiagramResponse> { Result = result });
}