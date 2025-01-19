using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Result>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<CompleteOrderCommandHandler> _logger;

    public CompleteOrderCommandHandler(
        ICustomerRepository customerRepository,
        IDateTimeService dateTimeService,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<CompleteOrderCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _dateTimeService = dateTimeService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<Guid>("Customer not found");
            }
            
            var order = customer.FindOrderById(request.OrderId.Value);
            if (order is null)
            {
                return Result.Failure<Guid>("Order not found");
            }
            
            order.Complete(_dateTimeService.UtcNow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(order.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing order");
            return Result.Failure<Guid>("Error completing order");
        }
    }
}