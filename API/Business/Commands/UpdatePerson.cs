using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.ComponentModel.DataAnnotations;

namespace StargateAPI.Business.Commands;

/// <summary>
/// Command to update an existing person's astronaut details. This command
/// allows partial updates of astronaut information including rank, duty title,
/// and career dates. Only provided fields will be updated.
/// </summary>
public class UpdatePerson : IRequest<UpdatePersonResult>
{
    /// <summary>
    /// The name of the person to update. Must be an existing person in the system.
    /// </summary>
    [Required]
    public required string Name { get; set; } = string.Empty;
        
    /// <summary>
    /// The new military rank for the person. Optional - only updates if provided.
    /// </summary>
    public string? CurrentRank { get; set; }
        
    /// <summary>
    /// The new duty title for the person. Optional - only updates if provided.
    /// </summary>
    public string? CurrentDutyTitle { get; set; }
        
    /// <summary>
    /// The new career start date for the person. Optional - only updates if provided.
    /// </summary>
    public DateTime? CareerStartDate { get; set; }
        
    /// <summary>
    /// The new career end date for the person. Optional - only updates if provided.
    /// </summary>
    public DateTime? CareerEndDate { get; set; }
}

/// <summary>
/// Preprocessor for UpdatePerson command that validates business rules
/// before the main handler executes. Ensures the person exists and validates
/// the provided data.
/// </summary>
public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
{
    private readonly StargateContext _context;
    public UpdatePersonPreProcessor(StargateContext context)
    {
        _context = context;
    }
    public Task Process(UpdatePerson request, CancellationToken cancellationToken)
    {
        Person? person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

        if (person is null) throw new BadHttpRequestException("Person not found");

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler for UpdatePerson command that executes the business logic
/// for updating an existing person's astronaut details with partial updates.
/// </summary>
public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
{
    private readonly StargateContext _context;

    public UpdatePersonHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new UpdatePersonResult()
                {
                    Success = false,
                    Message = "Name is required",
                    ResponseCode = 400
                };
            }

            // Validate career dates if provided
            if (request.CareerStartDate.HasValue && request.CareerEndDate.HasValue)
            {
                if (request.CareerStartDate.Value > request.CareerEndDate.Value)
                {
                    return new UpdatePersonResult()
                    {
                        Success = false,
                        Message = "Career start date cannot be after career end date",
                        ResponseCode = 400
                    };
                }
            }

            if (request.CareerStartDate.HasValue && request.CareerStartDate.Value > DateTime.Now)
            {
                return new UpdatePersonResult()
                {
                    Success = false,
                    Message = "Career start date cannot be in the future",
                    ResponseCode = 400
                };
            }

            Person? person = await _context.People
                .Include(p => p.AstronautDetail)
                .FirstOrDefaultAsync(z => z.Name == request.Name);

            if (person is null)
            {
                return new UpdatePersonResult()
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

            // Update AstronautDetail if it exists
            if (person.AstronautDetail != null)
            {
                bool hasChanges = false;

                if (!string.IsNullOrWhiteSpace(request.CurrentRank))
                {
                    person.AstronautDetail.CurrentRank = request.CurrentRank.Trim();
                    hasChanges = true;
                }
                    
                if (!string.IsNullOrWhiteSpace(request.CurrentDutyTitle))
                {
                    person.AstronautDetail.CurrentDutyTitle = request.CurrentDutyTitle.Trim();
                    hasChanges = true;
                }
                    
                if (request.CareerStartDate.HasValue)
                {
                    person.AstronautDetail.CareerStartDate = request.CareerStartDate.Value.Date;
                    hasChanges = true;
                }
                    
                if (request.CareerEndDate.HasValue)
                {
                    person.AstronautDetail.CareerEndDate = request.CareerEndDate.Value.Date;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    _context.AstronautDetails.Update(person.AstronautDetail);
                }
            }
            else
            {
                // Create new AstronautDetail if it doesn't exist and at least one field is provided
                if (!string.IsNullOrWhiteSpace(request.CurrentRank) || 
                    !string.IsNullOrWhiteSpace(request.CurrentDutyTitle) || 
                    request.CareerStartDate.HasValue || 
                    request.CareerEndDate.HasValue)
                {
                    AstronautDetail newAstronautDetail = new AstronautDetail
                    {
                        PersonId = person.Id,
                        CurrentRank = request.CurrentRank?.Trim() ?? string.Empty,
                        CurrentDutyTitle = request.CurrentDutyTitle?.Trim() ?? string.Empty,
                        CareerStartDate = request.CareerStartDate?.Date ?? DateTime.Now.Date,
                        CareerEndDate = request.CareerEndDate?.Date
                    };

                    await _context.AstronautDetails.AddAsync(newAstronautDetail);
                }
            }

            await _context.SaveChangesAsync();

            return new UpdatePersonResult()
            {
                Id = person.Id,
                Success = true,
                Message = "Person updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new UpdatePersonResult()
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}",
                ResponseCode = 500
            };
        }
    }
}

/// <summary>
/// Result object for UpdatePerson command containing the outcome
/// of the person update operation.
/// </summary>
public class UpdatePersonResult : BaseResponse
{
    public int Id { get; set; }
}