using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Customers.Responses;
using BlueFrames.Application.Customers.Commands.CreateCustomer;
using BlueFrames.Application.Customers.Commands.DeleteCustomer;
using BlueFrames.Application.Customers.Commands.UpdateCustomer;
using BlueFrames.Application.Customers.Queries.GetAllCustomers;
using BlueFrames.Application.Customers.Queries.GetCustomerById;
using BlueFrames.Domain.Customers.Common;
using Microsoft.AspNetCore.Authorization;

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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("Admin")]
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("Admin")]
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("User")]
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
    
    [EndpointSummary("Gets all the customers by the provided limit and offset")]
    [ProducesResponseType(typeof(Envelope<List<CustomerResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("Admin")]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int limit,
        [FromQuery] int offset,
        CancellationToken cancellationToken)
    {
        switch (limit)
        {
            case < 0:
                return BadRequest(Envelope.Error("limit", "Limit must be greater than 0"));
            case > 100:
                return BadRequest(Envelope.Error("limit", "Limit must be less than or equal to 100"));
        }

        if (offset < 0)
        {
            return BadRequest(Envelope.Error("offset", "Offset must be greater than 0"));
        }
        
        try
        {
            var query = new GetAllCustomersQuery(limit, offset);
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while returning customers - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning customers"));
            }
            
            var customers = result.Value;
            if (customers is null)
            {
                return NotFound(Envelope.Error("Customers not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning customers");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning customers"));
        }
    }
    
    [EndpointSummary("Deletes customer by providing customer id")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteCustomerCommand(CustomerId.From(id));
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while deleting customer - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while deleting customer"));
            }
            
            if (result.Value == Guid.Empty)
            {
                return NotFound(Envelope.Error("Customer not found"));
            }
            
            return Ok(Envelope.Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting customer");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while deleting customer"));
        }
    }
}
