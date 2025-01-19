using BlueFrames.Api.Contracts;
using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Customers.Responses;
using BlueFrames.Application.Customers.Commands.CreateCustomer;
using BlueFrames.Application.Customers.Commands.UpdateCustomer;
using BlueFrames.Application.Customers.Queries.GetCustomerById;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Api.V1.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class CustomerController : ApiController
{
    private readonly IMediator _mediator;
    private readonly ILoggerAdapter<CustomerController> _logger;

    public CustomerController(
        IMediator mediator,
        ILoggerAdapter<CustomerController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [EndpointSummary("Creates customer by providing customer details and returns the created customer details url")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] CustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateCustomerCommand(
                FirstName.From(request.FirstName),
                LastName.From(request.LastName),
                PhoneNumber.From(request.Phone),
                Email.From(request.Email));
        
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsFailure)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Value }, Envelope.Ok(result.Value));
            }

            _logger.LogError("Error occurred while creating customer - {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while creating customer"));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while creating customer");
            return BadRequest(Envelope.Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating customer");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while creating customer"));
        }
    }

    [EndpointSummary("Updates customer details by providing customer id and customer details")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(
        [FromMultiSource] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateCustomerCommand(
                CustomerId.From(request.Id),
                FirstName.From(request.Customer.FirstName),
                LastName.From(request.Customer.LastName),
                PhoneNumber.From(request.Customer.Phone),
                Email.From(request.Customer.Email));
        
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsFailure)
            {
                return Ok(Envelope.Ok(result.Value));
            }

            _logger.LogError("Error occurred while updating customer - {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while updating customer"));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while updating customer");
            return BadRequest(Envelope.Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while updating customer"));
        }
    }
    
    [EndpointSummary("Gets customer details by providing customer id")]
    [ProducesResponseType(typeof(Envelope<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerByIdQuery(CustomerId.From(id));
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while returning customer - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning customer"));
            }
            
            var customer = result.Value;
            if (customer is null)
            {
                return NotFound(Envelope.Error("Customer not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning customer");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning customer"));
        }
    }
}
