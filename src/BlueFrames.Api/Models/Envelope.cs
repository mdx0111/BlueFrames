namespace BlueFrames.Api.Models;

public class Envelope<T>
{
    public T Result { get; init; }
    public DateTime TimeGenerated { get; init; }
    public bool IsSuccess => (Errors?.Count ?? 0) == 0;
    public IDictionary<string, string[]> Errors { get; init; }
    public bool IsFailure => !IsSuccess;

    internal Envelope(T result)
        : this(result, null)
    {
    }

    // Needed for JSON deserialization
    public Envelope()
    {
    }

    protected Envelope(T result, string errorTitle, string errorDescription)
        : this(result, new Dictionary<string, string[]> { { errorTitle, [errorDescription] } })
    {
    }

    protected Envelope(T result, IDictionary<string, string[]> errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }
}

public class Envelope : Envelope<string>
{
    private Envelope(string errorTitle, string errorDescription)
        : base(string.Empty, errorTitle, errorDescription)
    {
    }

    private Envelope(IDictionary<string, string[]> errors)
        : base(string.Empty, errors)
    {
    }

    // Needed for JSON deserialization
    public Envelope()
        : base(string.Empty)
    {
    }

    public static Envelope<T> Ok<T>(T result)
    {
        return new Envelope<T>(result);
    }

    public static Envelope Ok()
    {
        return new Envelope();
    }

    public static Envelope Error(string errorTitle, string errorDescription)
    {
        return new Envelope(errorTitle, errorDescription);
    }

    public static Envelope Error(IDictionary<string, string[]> errors)
    {
        return new Envelope(errors);
    }

    public static Envelope Error(string error)
    {
        return new Envelope("error", error);
    }
}