namespace Play.Sales.Service.Dtos;

public record UpdateProductDto(
    string Name,
    Guid CategoryId,
    decimal Price,
    int StockQuantity,
    string Description
);
public record SaleDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    DateTimeOffset SaleDate,
    decimal TotalAmount,
    IEnumerable<SaleItemDto> Items,
    DateTimeOffset CreatedDate
);

public record SaleItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price
);

public record CreateSaleDto(
    Guid CustomerId,
    IEnumerable<CreateSaleItemDto> Items
);

public record CreateSaleItemDto(
    Guid ProductId,
    int Quantity
);

public record ProductDto(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    int StockQuantity,
    string Description
);

public record CustomerDto(
    Guid Id,
    string Name
);
