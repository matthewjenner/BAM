namespace StargateAPI.Business.Services
{
    /// <summary>
    /// Interface for logging service that provides methods for logging different types
    /// of events and operations throughout the application. Supports database-stored logging
    /// for audit trails and debugging purposes.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Logs an informational message with optional context information.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="source">The source component or method that generated the log</param>
        /// <param name="userId">The ID of the user associated with the operation</param>
        /// <param name="requestId">The ID of the request for tracing purposes</param>
        Task LogInfoAsync(string message, string? source = null, string? userId = null, string? requestId = null);
        
        /// <summary>
        /// Logs an error message with optional exception details and context information.
        /// </summary>
        /// <param name="message">The error message to log</param>
        /// <param name="exception">The exception that caused the error</param>
        /// <param name="source">The source component or method that generated the error</param>
        /// <param name="userId">The ID of the user associated with the operation</param>
        /// <param name="requestId">The ID of the request for tracing purposes</param>
        Task LogErrorAsync(string message, Exception? exception = null, string? source = null, string? userId = null, string? requestId = null);
        
        /// <summary>
        /// Logs a success message with optional context information.
        /// </summary>
        /// <param name="message">The success message to log</param>
        /// <param name="source">The source component or method that generated the log</param>
        /// <param name="userId">The ID of the user associated with the operation</param>
        /// <param name="requestId">The ID of the request for tracing purposes</param>
        Task LogSuccessAsync(string message, string? source = null, string? userId = null, string? requestId = null);
    }
}
