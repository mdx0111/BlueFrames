namespace BlueFrames.Application.Common.Logging;

public interface ILoggerAdapter<T>
{
    void LogInformation(string message, params object[] args);

    void LogError(Exception exception, string message, params object[] args);
    void LogError(string message, params object[] args);
}
