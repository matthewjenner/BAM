using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class GetPersonByNameHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly GetPersonByNameHandler _handler;

    public GetPersonByNameHandlerTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _handler = new GetPersonByNameHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnPerson_WhenPersonExists()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        GetPersonByName request = new GetPersonByName { Name = "John Doe" };

        #endregion

        #region Act
        GetPersonByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.Person.PersonId.Should().Be(person.Id);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        #region Arrange
        GetPersonByName request = new GetPersonByName { Name = "Non Existent" };

        #endregion

        #region Act
        GetPersonByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Person.Should().BeNull();
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
            CareerStartDate = DateTime.Now.AddDays(-100),
            CareerEndDate = DateTime.Now.AddDays(-10)
        };
        _context.AstronautDetails.Add(astronautDetail);
        await _context.SaveChangesAsync();

        GetPersonByName request = new GetPersonByName { Name = "John Doe" };

        #endregion

        #region Act
        GetPersonByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.Person.CurrentRank.Should().Be("Commander");
        result.Person.CurrentDutyTitle.Should().Be("Mission Specialist");
        result.Person.CareerStartDate.Should().Be(astronautDetail.CareerStartDate);
        result.Person.CareerEndDate.Should().Be(astronautDetail.CareerEndDate);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnNullAstronautDetails_WhenPersonHasNoDetails()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        GetPersonByName request = new GetPersonByName { Name = "John Doe" };

        #endregion

        #region Act
        GetPersonByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.Person.CurrentRank.Should().BeNull();
        result.Person.CurrentDutyTitle.Should().BeNull();
        result.Person.CareerStartDate.Should().BeNull();
        result.Person.CareerEndDate.Should().BeNull();
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldBeCaseSensitive_WhenSearchingByName()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        GetPersonByName request = new GetPersonByName { Name = "john doe" };

        #endregion

        #region Act
        GetPersonByNameResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Person.Should().BeNull();
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}