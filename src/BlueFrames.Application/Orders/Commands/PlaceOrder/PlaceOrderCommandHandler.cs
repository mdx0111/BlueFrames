using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IDateTimeService dateTimeService,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<PlaceOrderCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _dateTimeService = dateTimeService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<Guid>("Customer not found");
            }
            
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
            {
                return Result.Failure<Guid>("Product not found");
            }

            var createdDate = OrderDate.From(_dateTimeService.UtcNow);
            var order = Order.Create(product.Id, customer.Id, createdDate, _dateTimeService.UtcNow);
            
            customer.PlaceOrder(order);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(order.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return Result.Failure<Guid>("Error creating order");
        }
    }
}