namespace CourseHub.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : this(message, new Dictionary<string, string[]>())
    {
    }

    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}
