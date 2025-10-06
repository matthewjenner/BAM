using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class CreatePersonHandlerTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly CreatePersonHandler _handler;

    public CreatePersonHandlerTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _handler = new CreatePersonHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldCreatePerson_WhenValidRequest()
    {
        #region Arrange
        CreatePerson request = new CreatePerson { Name = "John Doe" };

        #endregion

        #region Act
        CreatePersonResult result = await _handler.Handle(request, CancellationToken.None);

        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Success.Should().BeTrue();

        Person? createdPerson = await _context.People.FirstOrDefaultAsync(p => p.Name == "John Doe");
        createdPerson.Should().NotBeNull();
        createdPerson!.Name.Should().Be("John Doe");
        #endregion
    }

    [Fact]
    public async Task Handle_ShouldReturnUniqueId_WhenMultiplePersonsCreated()
    {
        #region Arrange
        CreatePerson request1 = new CreatePerson { Name = "John Doe" };
        CreatePerson request2 = new CreatePerson { Name = "Jane Smith" };

        #endregion

        #region Act
        CreatePersonResult result1 = await _handler.Handle(request1, CancellationToken.None);
        CreatePersonResult result2 = await _handler.Handle(request2, CancellationToken.None);

        #endregion

        #region Assert
        result1.Id.Should().NotBe(result2.Id);
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}