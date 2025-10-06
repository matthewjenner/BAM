using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace StargateAPI.Controllers;

/// <summary>
/// API Controller for managing astronaut duty assignments.
/// Provides endpoints for retrieving duty history and creating new duty assignments.
/// Uses MediatR for CQRS pattern implementation and includes comprehensive logging.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AstronautDutyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggingService _loggingService;
    
    /// <summary>
    /// Initializes a new instance of the AstronautDutyController.
    /// </summary>
    /// <param name="mediator">MediatR instance for handling commands and queries</param>
    /// <param name="loggingService">Service for logging operations and errors</param>
    public AstronautDutyController(IMediator mediator, ILoggingService loggingService)
    {
        _mediator = mediator;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Retrieves all astronaut duty assignments for a specific person.
    /// </summary>
    /// <param name="name">The name of the person to retrieve duties for</param>
    /// <returns>List of duty assignments with person details</returns>
    [HttpGet("{name}")]
    public async Task<IActionResult> GetAstronautDutiesByName([Required] string name)
    {
        try
        {
            GetAstronautDutiesByNameResult result = await _mediator.Send(new GetAstronautDutiesByName()
            {
                Name = name
            });

            await _loggingService.LogSuccessAsync($"Successfully retrieved astronaut duties for: {name}", "AstronautDutyController.GetAstronautDutiesByName");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync($"Failed to retrieve astronaut duties for: {name}", ex, "AstronautDutyController.GetAstronautDutiesByName");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }            
    }

    /// <summary>
    /// Creates a new astronaut duty assignment for an existing person.
    /// Handles complex business logic including career transitions and retirement processing.
    /// </summary>
    /// <param name="request">The duty assignment details including person name, rank, title, and start date</param>
    /// <returns>Result of the duty creation operation</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAstronautDuty([FromBody, Required] CreateAstronautDuty request)
    {
        try
        {
            CreateAstronautDutyResult result = await _mediator.Send(request);
            await _loggingService.LogSuccessAsync($"Successfully created astronaut duty for: {request.Name}", "AstronautDutyController.CreateAstronautDuty");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync($"Failed to create astronaut duty for: {request.Name}", ex, "AstronautDutyController.CreateAstronautDuty");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }
    }
}