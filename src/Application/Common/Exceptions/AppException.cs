namespace SalonCRM.Application.Common.Exceptions;

/// <summary>
/// Error categories the API layer maps to HTTP status codes.
/// </summary>
public enum AppErrorType
{
    Validation,
    Unauthorized,
    Forbidden,
    Conflict,
    NotFound
}

/// <summary>
/// Thrown by executors when a business flow cannot proceed. Carries an
/// AppErrorType so the Api layer can translate it to the right HTTP status
/// without the Application layer depending on ASP.NET Core.
/// </summary>
public class AppException : Exception
{
    public AppErrorType ErrorType { get; }

    public AppException(string message, AppErrorType errorType)
        : base(message)
    {
        ErrorType = errorType;
    }
}
