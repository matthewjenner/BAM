using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace StargateAPI.Controllers;

/// <summary>
/// API Controller for managing person records and astronaut details.
/// Provides CRUD operations for persons including creation, retrieval, and updates.
/// Uses MediatR for CQRS pattern implementation and includes comprehensive logging.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggingService _loggingService;
    
    /// <summary>
    /// Initializes a new instance of the PersonController.
    /// </summary>
    /// <param name="mediator">MediatR instance for handling commands and queries</param>
    /// <param name="loggingService">Service for logging operations and errors</param>
    public PersonController(IMediator mediator, ILoggingService loggingService)
    {
        _mediator = mediator;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Retrieves all people in the system with their current astronaut details.
    /// </summary>
    /// <returns>List of all people with their astronaut information</returns>
    [HttpGet]
    public async Task<IActionResult> GetPeople()
    {
        try
        {
            GetPeopleResult result = await _mediator.Send(new GetPeople()
            {

            });

            await _loggingService.LogSuccessAsync("Successfully retrieved all people", "PersonController.GetPeople");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Failed to retrieve all people", ex, "PersonController.GetPeople");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Retrieves a specific person by name with their current astronaut details.
    /// </summary>
    /// <param name="name">The name of the person to retrieve</param>
    /// <returns>Person details with current astronaut information</returns>
    [HttpGet("{name}")]
    public async Task<IActionResult> GetPersonByName([Required] string name)
    {
        try
        {
            GetPersonByNameResult result = await _mediator.Send(new GetPersonByName()
            {
                Name = name
            });

            await _loggingService.LogSuccessAsync($"Successfully retrieved person: {name}", "PersonController.GetPersonByName");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync($"Failed to retrieve person: {name}", ex, "PersonController.GetPersonByName");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="name">The unique name of the person to create</param>
    /// <returns>Result of the person creation operation</returns>
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody, Required] string name)
    {
        try
        {
            CreatePersonResult result = await _mediator.Send(new CreatePerson()
            {
                Name = name
            });

            await _loggingService.LogSuccessAsync($"Successfully created person: {name}", "PersonController.CreatePerson");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync($"Failed to create person: {name}", ex, "PersonController.CreatePerson");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Updates an existing person's astronaut details with partial updates.
    /// Only provided fields will be updated.
    /// </summary>
    /// <param name="name">The name of the person to update</param>
    /// <param name="request">The update request containing the fields to modify</param>
    /// <returns>Result of the person update operation</returns>
    [HttpPut("{name}")]
    public async Task<IActionResult> UpdatePerson([Required] string name, [FromBody] UpdatePersonRequest request)
    {
        try
        {
            UpdatePersonResult result = await _mediator.Send(new UpdatePerson()
            {
                Name = name,
                CurrentRank = request?.CurrentRank,
                CurrentDutyTitle = request?.CurrentDutyTitle,
                CareerStartDate = request?.CareerStartDate,
                CareerEndDate = request?.CareerEndDate
            });

            await _loggingService.LogSuccessAsync($"Successfully updated person: {name}", "PersonController.UpdatePerson");
            return this.GetResponse(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync($"Failed to update person: {name}", ex, "PersonController.UpdatePerson");
            return this.GetResponse(new BaseResponse()
            {
                Message = ex.Message,
                Success = false,
                ResponseCode = (int)HttpStatusCode.InternalServerError
            });
        }
    }
}

public class UpdatePersonRequest
{
    public string? CurrentRank { get; set; }
    public string? CurrentDutyTitle { get; set; }
    public DateTime? CareerStartDate { get; set; }
    public DateTime? CareerEndDate { get; set; }
}