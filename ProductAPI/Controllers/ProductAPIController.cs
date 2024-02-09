using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;
using ProductAPI.Model;
using ProductAPI.Exceptions;

namespace ProductAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ProductAPIController : ControllerBase
{
    private readonly ILogger<ProductAPIController> logger;
	private readonly IProductAPIRepositoryService productAPIRepositoryService;

	public ProductAPIController(ILogger<ProductAPIController> logger, IProductAPIRepositoryService productAPIRepositoryService)
    {
        this.logger = logger;
		this.productAPIRepositoryService = productAPIRepositoryService;
    }

	[HttpPost(Name = "CreateProduct")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Post(ProductDto newProduct)
	{
		try
		{
			await productAPIRepositoryService.AddProductAsync(newProduct);
			return Ok($"Product has been added.");
		}
		catch (ProductSKUAlreadyExistsException ex1)
		{
			return StatusCode(409, ex1.Message);
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

    [HttpGet("{sku}", Name = "GetProductBySKU")]
    public async Task<ActionResult> Get(string sku)
    {
		try
		{
			var product = await productAPIRepositoryService.GetProductAsync(sku);

			if (product == null)
			{
				return NotFound();
			}
			else
			{
				return Ok(product);
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

	[HttpPut(Name = "UpdateProduct")]
	public async Task<IActionResult> Put(ProductDto updatedProduct)
	{
		try
		{
			if (updatedProduct == null)
			{
				return BadRequest("Product is null.");
			}

			if (updatedProduct.SKU == null)
			{
				return BadRequest("Product's SKU is null.");
			}

			var existingProduct = await productAPIRepositoryService.GetProductAsync(updatedProduct.SKU);

			if (existingProduct == null)
			{
				return NotFound();
			}

			updatedProduct.Title ??= existingProduct.Title;
			updatedProduct.Description ??= existingProduct.Description;
			updatedProduct.BuyerId ??= existingProduct.BuyerId;

			var savedProduct = await productAPIRepositoryService.UpdateProductAsync(updatedProduct);

			return Ok(savedProduct);

		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

	[HttpDelete("{sku}", Name = "DeleteProduct")]
	public async Task<IActionResult> Delete(string sku)
	{
		try
		{
			var existingProduct = await productAPIRepositoryService.GetProductAsync(sku);

			if (existingProduct == null)
			{
				return NotFound();
			}

			await productAPIRepositoryService.DeleteProductAsync(existingProduct);

			return NoContent();
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

	[HttpGet("GetAllProducts", Name = "GetAllProducts")]
	public async Task<IActionResult> GetAllProducts()
	{
		try
		{
			var result = await productAPIRepositoryService.GetAllProductsAsync();

			return Ok(result);
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}
}
