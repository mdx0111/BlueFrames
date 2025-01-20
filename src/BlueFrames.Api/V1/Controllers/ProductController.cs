using BlueFrames.Api.Contracts.Products.Requests;
using BlueFrames.Api.Contracts.Products.Responses;
using BlueFrames.Application.Products.Commands.CreateProduct;
using BlueFrames.Application.Products.Commands.DeleteProduct;
using BlueFrames.Application.Products.Commands.UpdateProduct;
using BlueFrames.Application.Products.Queries.GetAllProducts;
using BlueFrames.Application.Products.Queries.GetProductById;
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

    [EndpointSummary("Gets product details by providing product id")]
    [ProducesResponseType(typeof(Envelope<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetProductByIdQuery(ProductId.From(id));
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while returning product - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning product"));
            }
            
            var product = result.Value;
            if (product is null)
            {
                return NotFound(Envelope.Error("Product not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning product");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning product"));
        }
    }
    
    [EndpointSummary("Gets all the products by the provided limit and offset")]
    [ProducesResponseType(typeof(Envelope<List<ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            var query = new GetAllProductsQuery(limit, offset);
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while returning products - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning products"));
            }
            
            var products = result.Value;
            if (products is null)
            {
                return NotFound(Envelope.Error("Products not found"));
            }
            
            return Ok(Envelope.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning products");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while returning products"));
        }
    }
    
    [EndpointSummary("Deletes product by providing product id")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteProductCommand(ProductId.From(id));
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Error occurred while deleting product - {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    Envelope.Error("An error occurred while deleting product"));
            }
            
            if (result.Value == Guid.Empty)
            {
                return NotFound(Envelope.Error("Product not found"));
            }
            
            return Ok(Envelope.Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product");
            return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error("An error occurred while deleting product"));
        }
    }
}