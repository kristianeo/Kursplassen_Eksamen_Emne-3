namespace Kursplassen_Eksamen_Emne_3.Logging;

public static partial class Log
{
    // Logging for authentication
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed login attempt")]
    public static partial void LoginFailed(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} logged in")]
    public static partial void LoginSuccessful(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} registered")]
    public static partial void UserRegistered(ILogger logger, Guid userId);

    // Logging for courses
    [LoggerMessage(Level = LogLevel.Information, Message = "Course {CourseId} created by {UserId}")]
    public static partial void CourseCreated(ILogger logger, Guid courseId, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Course {CourseId} deleted by {UserId}")]
    public static partial void CourseDeleted(ILogger logger, Guid courseId, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Course {CourseId} edited by user {UserId}")]
    public static partial void CourseEdited(ILogger logger, Guid courseId, Guid userId);

    // Logging for registrations
    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} registered for course {CourseId}")]
    public static partial void CourseRegistered(ILogger logger, Guid courseId, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Registration {RegistrationId} cancelled")]
    public static partial void RegistrationCancelled(ILogger logger, Guid registrationId);
    
    // Logging for exceptions
    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception on {Path}")]
    public static partial void Status500Error(ILogger logger, Exception exception, PathString path);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request rejected with {Status}: {Message}")]
    public static partial void Status400sError(ILogger logger, int status, string message);
}