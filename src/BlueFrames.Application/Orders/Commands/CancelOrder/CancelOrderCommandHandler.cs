using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        ICustomerRepository customerRepository,
        IDateTimeService dateTimeService,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<CancelOrderCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _dateTimeService = dateTimeService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<Guid>("Customer not found");
            }
            
            var order = customer.FindOrderById(request.OrderId);
            if (order is null)
            {
                return Result.Failure<Guid>("Order not found");
            }
            
            order.Cancel(_dateTimeService.UtcNow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(order.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order");
            return Result.Failure<Guid>("Error cancelling order");
        }
    }
}