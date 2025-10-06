using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

/// <summary>
/// Query to retrieve all astronaut duty assignments for a specific person.
/// Returns both current and historical duty assignments with complete details.
/// </summary>
public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
{
    /// <summary>
    /// The name of the person to retrieve duty assignments for.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Handler for GetAstronautDutiesByName query that retrieves all duty assignments
/// for a specific person from the database using Entity Framework.
/// </summary>
public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
{
    private readonly StargateContext _context;

    public GetAstronautDutiesByNameHandler(StargateContext context)
    {
        _context = context;
    }

    public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
    {
        GetAstronautDutiesByNameResult result = new GetAstronautDutiesByNameResult();

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

        if (person == null)
        {
            result.Success = false;
            result.Message = "Person not found";
            result.ResponseCode = 404;
            return result;
        }

        result.Person = person;

        List<AstronautDuty> duties = await _context.AstronautDuties
            .Where(d => d.PersonId == person.PersonId)
            .OrderByDescending(d => d.DutyStartDate)
            .ToListAsync(cancellationToken);

        result.AstronautDuties = duties;

        return result;
    }
}

/// <summary>
/// Result object for GetAstronautDutiesByName query containing the person details
/// and all their duty assignments (both current and historical).
/// </summary>
public class GetAstronautDutiesByNameResult : BaseResponse
{
    /// <summary>
    /// The person's basic information and current astronaut details.
    /// </summary>
    public PersonAstronaut? Person { get; set; }
    
    /// <summary>
    /// List of all duty assignments for the person, ordered by start date.
    /// </summary>
    public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
}