using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class UpdatePersonPreProcessorTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly UpdatePersonPreProcessor _preProcessor;

    public UpdatePersonPreProcessorTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _preProcessor = new UpdatePersonPreProcessor(_context);
    }

    [Fact]
    public async Task Process_ShouldThrowBadHttpRequestException_WhenPersonDoesNotExist()
    {
        #region Arrange
        UpdatePerson request = new UpdatePerson { Name = "NonExistent" };

        #endregion

        #region Act & Assert
        BadHttpRequestException exception = await Assert.ThrowsAsync<BadHttpRequestException>(
            () => _preProcessor.Process(request, CancellationToken.None));

        exception.Message.Should().Be("Person not found");
        #endregion
    }

    [Fact]
    public async Task Process_ShouldCompleteSuccessfully_WhenPersonExists()
    {
        #region Arrange
        Person existingPerson = new Person { Name = "John Doe" };
        await _context.People.AddAsync(existingPerson);
        await _context.SaveChangesAsync();

        UpdatePerson request = new UpdatePerson { Name = "John Doe" };

        #endregion

        #region Act
        await _preProcessor.Process(request, CancellationToken.None);

        #endregion

        #region Assert - Should not throw any exception
        // This test passes if no exception is thrown
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}