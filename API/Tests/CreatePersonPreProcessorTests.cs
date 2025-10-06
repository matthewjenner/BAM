using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class CreatePersonPreProcessorTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly CreatePersonPreProcessor _preProcessor;

    public CreatePersonPreProcessorTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _preProcessor = new CreatePersonPreProcessor(_context);
    }

    [Fact]
    public async Task Process_ShouldThrowBadHttpRequestException_WhenPersonAlreadyExists()
    {
        #region Arrange
        Person existingPerson = new Person { Name = "John Doe" };
        await _context.People.AddAsync(existingPerson);
        await _context.SaveChangesAsync();

        CreatePerson request = new CreatePerson { Name = "John Doe" };

        #endregion

        #region Act & Assert
        BadHttpRequestException exception = await Assert.ThrowsAsync<BadHttpRequestException>(
            () => _preProcessor.Process(request, CancellationToken.None));

        exception.Message.Should().Be("Bad Request");
        #endregion
    }

    [Fact]
    public async Task Process_ShouldCompleteSuccessfully_WhenPersonDoesNotExist()
    {
        #region Arrange
        CreatePerson request = new CreatePerson { Name = "John Doe" };

        #endregion

        #region Act
        await _preProcessor.Process(request, CancellationToken.None);

        #endregion

        #region Assert - Should not throw any exception
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}