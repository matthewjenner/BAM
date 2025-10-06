using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.ComponentModel.DataAnnotations;

namespace StargateAPI.Business.Commands;

/// <summary>
/// Command to create a new astronaut duty assignment for an existing person.
/// This command handles the complex business logic for career transitions, including
/// retirement processing and historical duty tracking.
/// </summary>
public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
{
    /// <summary>
    /// The name of the person to assign the duty to. Must be an existing person in the system.
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// The military rank for this duty assignment (e.g., "2LT", "1LT", "CPT").
    /// </summary>
    [Required]
    public required string Rank { get; set; }

    /// <summary>
    /// The title of the duty assignment (e.g., "PILOT", "MISSION_SPECIALIST", "RETIRED").
    /// </summary>
    [Required]
    public required string DutyTitle { get; set; }

    /// <summary>
    /// The start date for this duty assignment. Must be in the future or today.
    /// </summary>
    [Required]
    public DateTime DutyStartDate { get; set; }
}

/// <summary>
/// Preprocessor for CreateAstronautDuty command that validates business rules
/// before the main handler executes. Ensures data integrity and prevents
/// invalid duty assignments.
/// </summary>
public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
{
    private readonly StargateContext _context;

    public CreateAstronautDutyPreProcessor(StargateContext context)
    {
        _context = context;
    }

    public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
    {
        Person? person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

        if (person is null) throw new BadHttpRequestException("Bad Request");

        AstronautDuty? verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);

        if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Bad Request");

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler for CreateAstronautDuty command that executes the complex business logic
/// for creating astronaut duty assignments. Handles career transitions, retirement
/// processing, and historical duty tracking according to business rules.
/// </summary>
public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
{
    private readonly StargateContext _context;

    public CreateAstronautDutyHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Name is required",
                    ResponseCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(request.Rank))
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Rank is required",
                    ResponseCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(request.DutyTitle))
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Duty Title is required",
                    ResponseCode = 400
                };
            }

            if (request.DutyStartDate > DateTime.Now)
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Duty Start Date cannot be in the future",
                    ResponseCode = 400
                };
            }

            Person? person = await _context.People
                .FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

            if (person == null)
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

            // Check if person already has a current duty (no end date)
            AstronautDuty? currentDuty = await _context.AstronautDuties
                .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DutyEndDate == null, cancellationToken);

            if (currentDuty != null)
            {
                return new CreateAstronautDutyResult()
                {
                    Success = false,
                    Message = "Person already has an active duty assignment",
                    ResponseCode = 400
                };
            }

            // Get or create AstronautDetail
            AstronautDetail? astronautDetail = await _context.AstronautDetails
                .FirstOrDefaultAsync(ad => ad.PersonId == person.Id, cancellationToken);

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetail
                {
                    PersonId = person.Id,
                    CurrentDutyTitle = request.DutyTitle,
                    CurrentRank = request.Rank,
                    CareerStartDate = request.DutyStartDate.Date,
                    CareerEndDate = request.DutyTitle == "RETIRED" ? request.DutyStartDate.Date : null
                };
                await _context.AstronautDetails.AddAsync(astronautDetail, cancellationToken);
            }
            else
            {
                // Update existing astronaut detail
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                
                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                }
                
                _context.AstronautDetails.Update(astronautDetail);
            }

            // End any previous duty assignments
            AstronautDuty? previousDuty = await _context.AstronautDuties
                .Where(d => d.PersonId == person.Id && d.DutyEndDate == null)
                .OrderByDescending(d => d.DutyStartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (previousDuty != null)
            {
                previousDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(previousDuty);
            }

            // Create new astronaut duty
            AstronautDuty newAstronautDuty = new AstronautDuty
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id,
                Success = true,
                Message = "Astronaut duty created successfully"
            };
        }
        catch (Exception ex)
        {
            return new CreateAstronautDutyResult()
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}",
                ResponseCode = 500
            };
        }
    }
}

/// <summary>
/// Result object for CreateAstronautDuty command containing the outcome
/// of the duty assignment operation and the ID of the created duty record.
/// </summary>
public class CreateAstronautDutyResult : BaseResponse
{
    /// <summary>
    /// The ID of the newly created astronaut duty record, if successful.
    /// </summary>
    public int? Id { get; set; }
}