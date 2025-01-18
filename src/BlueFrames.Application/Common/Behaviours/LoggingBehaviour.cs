using Microsoft.Extensions.Logging;

namespace BlueFrames.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        _logger.LogInformation("[START] {RequestNameWithGuid}", requestNameWithGuid);
        TResponse response;

        try
        {
            response = await next();
        }
        finally
        {
            _logger.LogInformation("[END] {RequestNameWithGuid}", requestNameWithGuid);
        }

        return response;
    }
}
