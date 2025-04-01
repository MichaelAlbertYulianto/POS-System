using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> productsRepository;

    public ProductsController(IRepository<Product> productsRepository)
    {
        this.productsRepository = productsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAsync()
    {
        var products = await productsRepository.GetAllAsync();
        return Ok(products.Select(p => p.AsDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetByIdAsync(Guid id)
    {
        var product = await productsRepository.GetAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product.AsDto());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostAsync(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            CategoryId = createProductDto.CategoryId,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await productsRepository.CreateAsync(product);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = product.Id }, product.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateProductDto updateProductDto)
    {
        var existingProduct = await productsRepository.GetAsync(id);
        if (existingProduct == null)
            return NotFound();

        existingProduct.Name = updateProductDto.Name;
        existingProduct.CategoryId = updateProductDto.CategoryId;
        existingProduct.Description = updateProductDto.Description;
        existingProduct.Price = updateProductDto.Price;
        existingProduct.StockQuantity = updateProductDto.StockQuantity;

        await productsRepository.UpdateAsync(existingProduct);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var product = await productsRepository.GetAsync(id);
        if (product == null)
            return NotFound();

        await productsRepository.DeleteAsync(id);

        return NoContent();
    }
}
