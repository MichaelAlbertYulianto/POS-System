using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service;

public static class Extensions
{
    public static ProductDto AsDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.CategoryId,
            product.Price,
            product.StockQuantity,
            product.Description,
            product.CreatedDate
        );
    }

    public static CategoryDto AsDto(this Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.CreatedDate
        );
    }
}
