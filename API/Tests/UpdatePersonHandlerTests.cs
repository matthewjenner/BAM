using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class UpdatePersonHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly UpdatePersonHandler _handler;

    public UpdatePersonHandlerTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _handler = new UpdatePersonHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePerson_WhenValidRequest()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe",
            CurrentRank = "Commander",
            CurrentDutyTitle = "Mission Specialist"
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(person.Id);

        AstronautDetail? astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id);
        astronautDetail.Should().NotBeNull();
        astronautDetail!.CurrentRank.Should().Be("Commander");
        astronautDetail.CurrentDutyTitle.Should().Be("Mission Specialist");
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenPersonNotFound()
    {
        #region Arrange
        UpdatePerson request = new UpdatePerson
        {
            Name = "Non Existent",
            CurrentRank = "Commander"
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Person not found");
        result.ResponseCode.Should().Be(404);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNameIsEmpty()
    {
        #region Arrange
        UpdatePerson request = new UpdatePerson
        {
            Name = "",
            CurrentRank = "Commander"
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Name is required");
        result.ResponseCode.Should().Be(400);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCareerStartDateIsInFuture()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe",
            CareerStartDate = DateTime.Now.AddDays(1)
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Career start date cannot be in the future");
        result.ResponseCode.Should().Be(400);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCareerStartDateIsAfterEndDate()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe",
            CareerStartDate = DateTime.Now.AddDays(-10),
            CareerEndDate = DateTime.Now.AddDays(-20)
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Career start date cannot be after career end date");
        result.ResponseCode.Should().Be(400);
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingAstronautDetail_WhenDetailExists()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDetail existingDetail = new AstronautDetail
        {
            PersonId = person.Id,
            CurrentRank = "Captain",
            CurrentDutyTitle = "Pilot",
            CareerStartDate = DateTime.Now.AddDays(-100)
        };
        _context.AstronautDetails.Add(existingDetail);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe",
            CurrentRank = "Commander",
            CurrentDutyTitle = "Mission Specialist"
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeTrue();

        AstronautDetail? updatedDetail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id);
        updatedDetail.Should().NotBeNull();
        updatedDetail!.CurrentRank.Should().Be("Commander");
        updatedDetail.CurrentDutyTitle.Should().Be("Mission Specialist");
        updatedDetail.CareerStartDate.Should().Be(existingDetail.CareerStartDate); // Should remain unchanged
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateDatabase_WhenNoChangesProvided()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        AstronautDetail existingDetail = new AstronautDetail
        {
            PersonId = person.Id,
            CurrentRank = "Captain",
            CurrentDutyTitle = "Pilot",
            CareerStartDate = DateTime.Now.AddDays(-100)
        };
        _context.AstronautDetails.Add(existingDetail);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe"
            // No changes provided
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeTrue();

        AstronautDetail? detail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id);
        detail.Should().NotBeNull();
        detail!.CurrentRank.Should().Be("Captain"); // Should remain unchanged
        detail.CurrentDutyTitle.Should().Be("Pilot"); // Should remain unchanged
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldTrimWhitespace_WhenUpdatingStrings()
    {
        #region Arrange
        Person person = new Person { Name = "John Doe" };
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson
        {
            Name = "John Doe",
            CurrentRank = "  Commander  ",
            CurrentDutyTitle = "  Mission Specialist  "
        };

        #endregion

        #region Act
        UpdatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Success.Should().BeTrue();

        AstronautDetail? astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id);
        astronautDetail.Should().NotBeNull();
        astronautDetail!.CurrentRank.Should().Be("Commander");
        astronautDetail.CurrentDutyTitle.Should().Be("Mission Specialist");
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}