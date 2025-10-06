using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

/// <summary>
/// Query to retrieve a specific person by name with their current astronaut details.
/// Returns the person's information and current astronaut status if they have one.
/// </summary>
public class GetPersonByName : IRequest<GetPersonByNameResult>
{
    /// <summary>
    /// The name of the person to retrieve. Must be an exact match.
    /// </summary>
    public required string Name { get; set; } = string.Empty;
}

/// <summary>
/// Handler for GetPersonByName query that retrieves a specific person
/// and their current astronaut details from the database using Entity Framework.
/// </summary>
public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
{
    private readonly StargateContext _context;
    public GetPersonByNameHandler(StargateContext context)
    {
        _context = context;
    }

    public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
    {
        GetPersonByNameResult result = new GetPersonByNameResult();

        PersonAstronaut? person = await _context.People
            .Include(p => p.AstronautDetail)
            .Where(p => p.Name == request.Name)
            .Select(p => new PersonAstronaut
            {
                PersonId = p.Id,
                Name = p.Name,
                CurrentRank = p.AstronautDetail != null ? p.AstronautDetail.CurrentRank : null,
                CurrentDutyTitle = p.AstronautDetail != null ? p.AstronautDetail.CurrentDutyTitle : null,
                CareerStartDate = p.AstronautDetail != null ? p.AstronautDetail.CareerStartDate : null,
                CareerEndDate = p.AstronautDetail != null ? p.AstronautDetail.CareerEndDate : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        result.Person = person;

        return result;
    }
}

/// <summary>
/// Result object for GetPersonByName query containing the person's details
/// and current astronaut information if they have one.
/// </summary>
public class GetPersonByNameResult : BaseResponse
{
    /// <summary>
    /// The person's information and current astronaut details, or null if not found.
    /// </summary>
    public PersonAstronaut? Person { get; set; }
}