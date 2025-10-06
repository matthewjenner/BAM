using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

/// <summary>
/// Query to retrieve all people in the system with their current astronaut details.
/// Returns a list of all persons and their associated astronaut information.
/// </summary>
public class GetPeople : IRequest<GetPeopleResult>
{

}

/// <summary>
/// Handler for GetPeople query that retrieves all people and their
/// current astronaut details from the database using Entity Framework.
/// </summary>
public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
{
    public readonly StargateContext _context;
    public GetPeopleHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
    {
        GetPeopleResult result = new GetPeopleResult();

        List<PersonAstronaut> people = await _context.People
            .Include(p => p.AstronautDetail)
            .Select(p => new PersonAstronaut
            {
                PersonId = p.Id,
                Name = p.Name,
                CurrentRank = p.AstronautDetail != null ? p.AstronautDetail.CurrentRank : null,
                CurrentDutyTitle = p.AstronautDetail != null ? p.AstronautDetail.CurrentDutyTitle : null,
                CareerStartDate = p.AstronautDetail != null ? p.AstronautDetail.CareerStartDate : null,
                CareerEndDate = p.AstronautDetail != null ? p.AstronautDetail.CareerEndDate : null
            })
            .ToListAsync(cancellationToken);

        result.People = people;

        return result;
    }
}

/// <summary>
/// Result object for GetPeople query containing a list of all people
/// and their current astronaut details.
/// </summary>
public class GetPeopleResult : BaseResponse
{
    /// <summary>
    /// List of all people in the system with their current astronaut information.
    /// </summary>
    public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };
}