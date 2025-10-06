using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class GetPeopleHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly GetPeopleHandler _handler;

    public GetPeopleHandlerTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _handler = new GetPeopleHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllPeople_WhenPeopleExist()
    {
        #region Arrange
        Person person1 = new Person { Name = "John Doe" };
        Person person2 = new Person { Name = "Jane Smith" };
        _context.People.AddRange(person1, person2);
        await _context.SaveChangesAsync();

        GetPeople request = new GetPeople();

        #endregion

        #region Act
        GetPeopleResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.People.Should().HaveCount(2);
        result.People.Should().Contain(p => p.Name == "John Doe");
        result.People.Should().Contain(p => p.Name == "Jane Smith");
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPeopleExist()
    {
        #region Arrange
        GetPeople request = new GetPeople();

        #endregion

        #region Act
        GetPeopleResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.People.Should().BeEmpty();
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

        GetPeople request = new GetPeople();

        #endregion

        #region Act
        GetPeopleResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.People.Should().HaveCount(1);
        
        PersonAstronaut personResult = result.People.First();
        personResult.Name.Should().Be("John Doe");
        personResult.CurrentRank.Should().Be("Commander");
        personResult.CurrentDutyTitle.Should().Be("Mission Specialist");
        personResult.CareerStartDate.Should().Be(astronautDetail.CareerStartDate);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnNullAstronautDetails_WhenPersonHasNoDetails()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        GetPeople request = new GetPeople();

        #endregion

        #region Act
        GetPeopleResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.People.Should().HaveCount(1);
        
        PersonAstronaut personResult = result.People.First();
        personResult.Name.Should().Be("John Doe");
        personResult.CurrentRank.Should().BeNull();
        personResult.CurrentDutyTitle.Should().BeNull();
        personResult.CareerStartDate.Should().BeNull();
        personResult.CareerEndDate.Should().BeNull();
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
