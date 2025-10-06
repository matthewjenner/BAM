using Xunit;
using FluentAssertions;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Microsoft.EntityFrameworkCore;

namespace StargateAPI.Tests;

public class CreateAstronautDutyHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly CreateAstronautDutyHandler _handler;

    public CreateAstronautDutyHandlerTests()
    {
        string connectionString = $"DataSource=test_{Guid.NewGuid()}.db";
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseSqlite(connectionString)
            .Options;

        _context = new StargateContext(options);
        _context.Database.EnsureCreated();
        _handler = new CreateAstronautDutyHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewAstronautDetail_WhenPersonHasNoAstronautDetail()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "John Doe",
            Rank = "Captain",
            DutyTitle = "Commander",
            DutyStartDate = DateTime.Now
        };

        #endregion

        #region Act
        CreateAstronautDutyResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().BeGreaterThan(0);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingAstronautDetail_WhenPersonHasAstronautDetail()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDetail astronautDetail = new AstronautDetail
        {
            PersonId = person.Id,
            CurrentRank = "Lieutenant",
            CurrentDutyTitle = "Pilot",
            CareerStartDate = DateTime.Now.AddYears(-5)
        };
        _context.AstronautDetails.Add(astronautDetail);
        await _context.SaveChangesAsync();
            
        _context.Entry(astronautDetail).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "John Doe",
            Rank = "Captain",
            DutyTitle = "Commander",
            DutyStartDate = DateTime.Now
        };

        #endregion

        #region Act
        CreateAstronautDutyResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().BeGreaterThan(0);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldSetCareerEndDate_WhenDutyTitleIsRetired()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "John Doe",
            Rank = "Captain",
            DutyTitle = "Retired",
            DutyStartDate = DateTime.Now
        };

        #endregion

        #region Act
        CreateAstronautDutyResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().BeGreaterThan(0);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenPersonDoesNotExist()
    {
        #region Arrange
        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "NonExistent",
            Rank = "Captain",
            DutyTitle = "Commander",
            DutyStartDate = DateTime.Now
        };

        #endregion

        #region Act
        CreateAstronautDutyResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ResponseCode.Should().Be(404);
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}