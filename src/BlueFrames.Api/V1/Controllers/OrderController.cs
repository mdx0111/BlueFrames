using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Api.Contracts.Orders.Responses;
using BlueFrames.Application.Orders.Commands.CancelOrder;
using BlueFrames.Application.Orders.Commands.CompleteOrder;
using BlueFrames.Application.Orders.Commands.PlaceOrder;
using BlueFrames.Application.Orders.Queries.GetCustomerOrder;
using BlueFrames.Application.Orders.Queries.GetCustomerOrderDetails;
using BlueFrames.Application.Orders.Queries.GetCustomerOrders;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products.Common;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize("User")]
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
                return CreatedAtAction(nameof(Get), new { customerId = request.CustomerId, orderId = result.Value }, Envelope.Ok(result.Value));
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
    [Authorize("Admin")]
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
    [Authorize("User")]
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
    
    [EndpointSummary("Gets an order by providing customer id and order id")]
    [ProducesResponseType(typeof(Envelope<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{customerId:guid}/{orderId:guid}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid customerId,
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerOrderQuery(OrderId.From(orderId), CustomerId.From(customerId));
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }
            
            var order = result.Value;
            if (order is null)
            {
                return NotFound(Envelope.Error("Order not found"));
            }

            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting order");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while getting order"));
        }
    }
    
    [EndpointSummary("Gets an order detail by providing customer id and order id")]
    [ProducesResponseType(typeof(Envelope<List<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{customerId:guid}/{orderId:guid}/details")]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid customerId,
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerOrderDetailsQuery(OrderId.From(orderId), CustomerId.From(customerId));
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }
            
            var orderDetails = result.Value;
            if (orderDetails is null)
            {
                return NotFound(Envelope.Error("Order not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting order details");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while getting order details"));
        }
    }
    
    [EndpointSummary("Gets all the orders for a customer by providing customer id")]
    [ProducesResponseType(typeof(Envelope<List<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{customerId:guid}/all")]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid customerId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerOrdersQuery(CustomerId.From(customerId));
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result.IsFailure)
            {
                return BadRequest(Envelope.Error(result.Error));
            }
            
            var orders = result.Value;
            if (orders is null || orders.Count == 0)
            {
                return NotFound(Envelope.Error("Order not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting orders");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while getting orders"));
        }
    }
}