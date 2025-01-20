using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Application.Orders.Commands.PlaceOrder;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Api.V1.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class OrderController : ApiController
{
    private readonly IMediator _mediator;
    private readonly ILoggerAdapter<OrderController> _logger;

    public OrderController(
        IMediator mediator,
        ILoggerAdapter<OrderController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [EndpointSummary("Places order by providing order details and returns the created order details url")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] PlaceOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new PlaceOrderCommand(
                CustomerId.From(request.CustomerId),
                ProductId.From(request.ProductId));
        
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsFailure)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Value }, Envelope.Ok(result.Value));
            }

            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }

            _logger.LogError("Error occurred while placing order - {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while placing order"));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while placing order");
            return BadRequest(Envelope.Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while creating order"));
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}