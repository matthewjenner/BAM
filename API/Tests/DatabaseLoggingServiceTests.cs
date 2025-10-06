using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;
using Xunit;
using FluentAssertions;

namespace StargateAPI.Tests;

public class DatabaseLoggingServiceTests : IDisposable
{
    private readonly StargateContext _context;
    private readonly DatabaseLoggingService _service;

    public DatabaseLoggingServiceTests()
    {
        DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StargateContext(options);
        _service = new DatabaseLoggingService(_context);
    }

    [Fact]
    public async Task LogInfoAsync_ShouldCreateLogEntry_WhenCalled()
    {
        #region Arrange
        string message = "Test info message";
        string source = "TestSource";

        #endregion

        #region Act
        await _service.LogInfoAsync(message, source);

        #endregion

        #region Assert
        LogEntry? logEntry = await _context.LogEntries.FirstOrDefaultAsync();
        logEntry.Should().NotBeNull();
        logEntry!.Level.Should().Be("INFO");
        logEntry.Message.Should().Be(message);
        logEntry.Source.Should().Be(source);
        logEntry.Exception.Should().BeNull();
        logEntry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        #endregion
    }

    [Fact]
    public async Task LogErrorAsync_ShouldCreateLogEntry_WhenCalled()
    {
        #region Arrange
        string message = "Test error message";
        Exception exception = new Exception("Test exception");
        string source = "TestSource";

        #endregion

        #region Act
        await _service.LogErrorAsync(message, exception, source);

        #endregion

        #region Assert
        LogEntry? logEntry = await _context.LogEntries.FirstOrDefaultAsync();
        logEntry.Should().NotBeNull();
        logEntry!.Level.Should().Be("ERROR");
        logEntry.Message.Should().Be(message);
        logEntry.Source.Should().Be(source);
        logEntry.Exception.Should().Contain("Test exception");
        logEntry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        #endregion
    }

    [Fact]
    public async Task LogSuccessAsync_ShouldCreateLogEntry_WhenCalled()
    {
        #region Arrange
        string message = "Test success message";
        string source = "TestSource";

        #endregion

        #region Act
        await _service.LogSuccessAsync(message, source);

        #endregion

        #region Assert
        LogEntry? logEntry = await _context.LogEntries.FirstOrDefaultAsync();
        logEntry.Should().NotBeNull();
        logEntry!.Level.Should().Be("SUCCESS");
        logEntry.Message.Should().Be(message);
        logEntry.Source.Should().Be(source);
        logEntry.Exception.Should().BeNull();
        logEntry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        #endregion
    }

    [Fact]
    public async Task LogErrorAsync_ShouldCreateLogEntry_WhenExceptionIsNull()
    {
        #region Arrange
        string message = "Test error message without exception";
        string source = "TestSource";

        #endregion

        #region Act
        await _service.LogErrorAsync(message, null, source);

        #endregion

        #region Assert
        LogEntry? logEntry = await _context.LogEntries.FirstOrDefaultAsync();
        logEntry.Should().NotBeNull();
        logEntry!.Level.Should().Be("ERROR");
        logEntry.Message.Should().Be(message);
        logEntry.Source.Should().Be(source);
        logEntry.Exception.Should().BeNull();
        #endregion
    }

    [Fact]
    public async Task LogInfoAsync_ShouldCreateLogEntry_WithUserIdAndRequestId()
    {
        #region Arrange
        string message = "Test info message";
        string source = "TestSource";
        string userId = "user123";
        string requestId = "req456";

        #endregion

        #region Act
        await _service.LogInfoAsync(message, source, userId, requestId);

        #endregion

        #region Assert
        LogEntry? logEntry = await _context.LogEntries.FirstOrDefaultAsync();
        logEntry.Should().NotBeNull();
        logEntry!.Level.Should().Be("INFO");
        logEntry.Message.Should().Be(message);
        logEntry.Source.Should().Be(source);
        logEntry.UserId.Should().Be(userId);
        logEntry.RequestId.Should().Be(requestId);
        #endregion
    }

    [Fact]
    public async Task LogInfoAsync_ShouldNotThrowException_WhenDatabaseErrorOccurs()
    {
        #region Arrange
        // Dispose the context to simulate a database error
        _context.Dispose();
        string message = "Test message";

        #endregion

        #region Act & Assert
        // Should not throw an exception, should handle gracefully
        await _service.LogInfoAsync(message);
        #endregion
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
