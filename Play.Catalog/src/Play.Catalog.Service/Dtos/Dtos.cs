namespace Play.Catalog.Service.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    int StockQuantity,
    string Description,
    DateTimeOffset CreatedDate
);

public record CreateProductDto(
    string Name,
    Guid CategoryId,
    decimal Price,
    int StockQuantity,
    string Description
);

public record UpdateProductDto(
    string Name,
    Guid CategoryId,
    decimal Price,
    int StockQuantity,
    string Description
);

public record CategoryDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedDate
);

public record CreateCategoryDto(
    string Name
);

public record UpdateCategoryDto(
    string Name
);
