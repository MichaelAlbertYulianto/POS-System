using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> categoriesRepository;

    public CategoriesController(IRepository<Category> categoriesRepository)
    {
        this.categoriesRepository = categoriesRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAsync()
    {
        var categories = await categoriesRepository.GetAllAsync();
        return Ok(categories.Select(c => c.AsDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await categoriesRepository.GetAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category.AsDto());
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> PostAsync(CreateCategoryDto createCategoryDto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = createCategoryDto.Name,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await categoriesRepository.CreateAsync(category);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = category.Id }, category.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateCategoryDto updateCategoryDto)
    {
        var existingCategory = await categoriesRepository.GetAsync(id);
        if (existingCategory == null)
            return NotFound();

        existingCategory.Name = updateCategoryDto.Name;

        await categoriesRepository.UpdateAsync(existingCategory);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var category = await categoriesRepository.GetAsync(id);
        if (category == null)
            return NotFound();

        await categoriesRepository.DeleteAsync(id);

        return NoContent();
    }
}
