using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Application.Orders.Commands.CancelOrder;
using BlueFrames.Application.Orders.Commands.CompleteOrder;
using BlueFrames.Application.Orders.Commands.PlaceOrder;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    
    [EndpointSummary("Completes order by providing order id and returns the completed order details url")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("complete")]
    public async Task<IActionResult> Complete(
        [FromBody] CompleteOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CompleteOrderCommand(
                OrderId.From(request.OrderId),
                CustomerId.From(request.CustomerId));
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }

            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while trying to complete the order");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while trying to complete the order"));
        }
    }

    [EndpointSummary("Cancels order by providing order id and returns the cancelled order details url")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("cancel")]
    public async Task<IActionResult> Cancel(
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelOrderCommand(
                OrderId.From(request.OrderId),
                CustomerId.From(request.CustomerId));
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }

            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while trying to cancel the order");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while trying to cancel the order"));
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}