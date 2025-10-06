using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.ComponentModel.DataAnnotations;

namespace StargateAPI.Business.Commands;

/// <summary>
/// Command to create a new person in the system. This is a simple command
/// that only requires a name, as persons can exist without astronaut assignments.
/// </summary>
public class CreatePerson : IRequest<CreatePersonResult>
{
    /// <summary>
    /// The unique name of the person to create. Must be unique across all persons.
    /// </summary>
    [Required]
    public required string Name { get; set; } = string.Empty;
}

/// <summary>
/// Preprocessor for CreatePerson command that validates business rules
/// before the main handler executes. Ensures the person name is unique.
/// </summary>
public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
{
    private readonly StargateContext _context;
    public CreatePersonPreProcessor(StargateContext context)
    {
        _context = context;
    }
    public Task Process(CreatePerson request, CancellationToken cancellationToken)
    {
        Person? person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

        if (person is not null) throw new BadHttpRequestException("Bad Request");

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler for CreatePerson command that executes the business logic
/// for creating a new person in the system.
/// </summary>
public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
{
    private readonly StargateContext _context;

    public CreatePersonHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new CreatePersonResult()
                {
                    Success = false,
                    Message = "Name is required",
                    ResponseCode = 400
                };
            }

            if (request.Name.Length > 100)
            {
                return new CreatePersonResult()
                {
                    Success = false,
                    Message = "Name cannot exceed 100 characters",
                    ResponseCode = 400
                };
            }

            Person newPerson = new Person
            {
                Name = request.Name.Trim()
            };

            await _context.People.AddAsync(newPerson);
            await _context.SaveChangesAsync();

            return new CreatePersonResult()
            {
                Id = newPerson.Id,
                Success = true,
                Message = "Person created successfully"
            };
        }
        catch (Exception ex)
        {
            return new CreatePersonResult()
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}",
                ResponseCode = 500
            };
        }
    }
}

/// <summary>
/// Result object for CreatePerson command containing the outcome
/// of the person creation operation and the ID of the created person.
/// </summary>
public class CreatePersonResult : BaseResponse
{
    public int Id { get; set; }
}