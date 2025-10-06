using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class GetAstronautDutiesByNameHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly GetAstronautDutiesByNameHandler _handler;

    public GetAstronautDutiesByNameHandlerTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _handler = new GetAstronautDutiesByNameHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnPersonAndDuties_WhenPersonExists()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDuty duty1 = new AstronautDuty
        {
            PersonId = person.Id,
            Rank = "Captain",
            DutyTitle = "Pilot",
            DutyStartDate = DateTime.Now.AddDays(-100),
            DutyEndDate = DateTime.Now.AddDays(-50)
        };

        AstronautDuty duty2 = new AstronautDuty
        {
            PersonId = person.Id,
            Rank = "Commander",
            DutyTitle = "Mission Specialist",
            DutyStartDate = DateTime.Now.AddDays(-49),
            DutyEndDate = null
        };

        _context.AstronautDuties.AddRange(duty1, duty2);
        await _context.SaveChangesAsync();

        GetAstronautDutiesByName request = new GetAstronautDutiesByName { Name = "John Doe" };

        #endregion

        #region Act
        GetAstronautDutiesByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.AstronautDuties.Should().HaveCount(2);
        result.AstronautDuties.Should().Contain(d => d.DutyTitle == "Pilot");
        result.AstronautDuties.Should().Contain(d => d.DutyTitle == "Mission Specialist");
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenPersonDoesNotExist()
    {
        #region Arrange
        GetAstronautDutiesByName request = new GetAstronautDutiesByName { Name = "Non Existent" };

        #endregion

        #region Act
        GetAstronautDutiesByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Person not found");
        result.ResponseCode.Should().Be(404);
        result.Person.Should().BeNull();
        result.AstronautDuties.Should().BeEmpty();
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyDutiesList_WhenPersonHasNoDuties()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        GetAstronautDutiesByName request = new GetAstronautDutiesByName { Name = "John Doe" };

        #endregion

        #region Act
        GetAstronautDutiesByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.AstronautDuties.Should().BeEmpty();
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldIncludeAstronautDetails_WhenPersonHasDetails()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDetail astronautDetail = new AstronautDetail
        {
            PersonId = person.Id,
            CurrentRank = "Commander",
            CurrentDutyTitle = "Mission Specialist",
            CareerStartDate = DateTime.Now.AddDays(-100)
        };
        _context.AstronautDetails.Add(astronautDetail);
        await _context.SaveChangesAsync();

        GetAstronautDutiesByName request = new GetAstronautDutiesByName { Name = "John Doe" };

        #endregion

        #region Act
        GetAstronautDutiesByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.Person.CurrentRank.Should().Be("Commander");
        result.Person.CurrentDutyTitle.Should().Be("Mission Specialist");
        result.Person.CareerStartDate.Should().Be(astronautDetail.CareerStartDate);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldOrderDutiesByStartDateDescending()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDuty duty1 = new AstronautDuty
        {
            PersonId = person.Id,
            Rank = "Captain",
            DutyTitle = "Pilot",
            DutyStartDate = DateTime.Now.AddDays(-100),
            DutyEndDate = DateTime.Now.AddDays(-50)
        };

        AstronautDuty duty2 = new AstronautDuty
        {
            PersonId = person.Id,
            Rank = "Commander",
            DutyTitle = "Mission Specialist",
            DutyStartDate = DateTime.Now.AddDays(-49),
            DutyEndDate = null
        };

        _context.AstronautDuties.AddRange(duty1, duty2);
        await _context.SaveChangesAsync();

        GetAstronautDutiesByName request = new GetAstronautDutiesByName { Name = "John Doe" };

        #endregion

        #region Act
        GetAstronautDutiesByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.AstronautDuties.Should().HaveCount(2);
        
        // Should be ordered by DutyStartDate descending (most recent first)
        List<AstronautDuty> duties = result.AstronautDuties.ToList();
        duties[0].DutyStartDate.Should().BeAfter(duties[1].DutyStartDate);
        duties[0].DutyTitle.Should().Be("Mission Specialist");
        duties[1].DutyTitle.Should().Be("Pilot");
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}