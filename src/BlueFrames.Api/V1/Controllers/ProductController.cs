using BlueFrames.Api.Contracts.Products.Requests;
using BlueFrames.Application.Products.Commands.CreateProduct;
using BlueFrames.Application.Products.Commands.UpdateProduct;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Api.V1.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class ProductController : ApiController
{
    private readonly IMediator _mediator;
    private readonly ILoggerAdapter<ProductController> _logger;

    public ProductController(
        IMediator mediator,
        ILoggerAdapter<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [EndpointSummary("Creates product by providing product details and returns the created product details url")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] ProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateProductCommand(
                ProductName.From(request.Name),
                ProductDescription.From(request.Description),
                ProductSKU.From(request.SKU));
        
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsFailure)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Value }, Envelope.Ok(result.Value));
            }

            _logger.LogError("Error occurred while creating product - {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while creating product"));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while creating product");
            return BadRequest(Envelope.Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while creating product"));
        }
    }
    
    [EndpointSummary("Updates product details by providing product id and product details")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(
        [FromMultiSource] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateProductCommand(
                ProductId.From(request.Id),
                ProductName.From(request.Product.Name),
                ProductDescription.From(request.Product.Description),
                ProductSKU.From(request.Product.SKU));
        
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsFailure)
            {
                return Ok(Envelope.Ok(result.Value));
            }

            _logger.LogError("Error occurred while updating product - {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while updating product"));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while updating product");
            return BadRequest(Envelope.Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while updating product"));
        }
    }

    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok();
    }
}