namespace UniversityCourseManagementApi.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected AppException(int statusCode, string errorCode, string message) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string errorCode, string message)
        : base(StatusCodes.Status404NotFound, errorCode, message)
    {
    }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string errorCode, string message)
        : base(StatusCodes.Status409Conflict, errorCode, message)
    {
    }
}

public sealed class BadRequestException : AppException
{
    public BadRequestException(string errorCode, string message)
        : base(StatusCodes.Status400BadRequest, errorCode, message)
    {
    }
}
