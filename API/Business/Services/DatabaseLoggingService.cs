using StargateAPI.Business.Data;

namespace StargateAPI.Business.Services
{
    /// <summary>
    /// Implementation of ILoggingService that stores log entries in the database.
    /// Provides persistent logging for audit trails, debugging, and system monitoring.
    /// All log entries are stored in the LogEntries table with timestamps and context information.
    /// </summary>
    public class DatabaseLoggingService : ILoggingService
    {
        private readonly StargateContext _context;

        /// <summary>
        /// Initializes a new instance of the DatabaseLoggingService.
        /// </summary>
        /// <param name="context">The Entity Framework database context for storing log entries</param>
        public DatabaseLoggingService(StargateContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logs an informational message to the database.
        /// </summary>
        public async Task LogInfoAsync(string message, string? source = null, string? userId = null, string? requestId = null)
        {
            await LogAsync("INFO", message, null, source, userId, requestId);
        }

        /// <summary>
        /// Logs an error message with optional exception details to the database.
        /// </summary>
        public async Task LogErrorAsync(string message, Exception? exception = null, string? source = null, string? userId = null, string? requestId = null)
        {
            string? exceptionDetails = exception?.ToString();
            await LogAsync("ERROR", message, exceptionDetails, source, userId, requestId);
        }

        /// <summary>
        /// Logs a success message to the database.
        /// </summary>
        public async Task LogSuccessAsync(string message, string? source = null, string? userId = null, string? requestId = null)
        {
            await LogAsync("SUCCESS", message, null, source, userId, requestId);
        }

        /// <summary>
        /// Private helper method that creates and saves a log entry to the database.
        /// Handles the common logic for all logging methods.
        /// </summary>
        private async Task LogAsync(string level, string message, string? exception = null, string? source = null, string? userId = null, string? requestId = null)
        {
            try
            {
                LogEntry logEntry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow,
                    Level = level,
                    Message = message,
                    Exception = exception,
                    Source = source,
                    UserId = userId,
                    RequestId = requestId
                };

                _context.LogEntries.Add(logEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Write to console instead of throwing an exception as a backup logging mechanism
                Console.WriteLine($"Error logging: {ex.Message}");
            }
        }
    }
}
