using BlueFrames.Api.Models.Customers;
using BlueFrames.Application.Customers.Commands.CreateCustomer;
using BlueFrames.Application.Customers.Queries.Common;
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
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] CreateCustomerDto customer,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateCustomerCommand(
                FirstName.From(customer.FirstName),
                LastName.From(customer.LastName),
                PhoneNumber.From(customer.Phone),
                Email.From(customer.Email));
        
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
    
    [EndpointSummary("Gets customer details by providing customer id")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id:guid}")]
    public Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
