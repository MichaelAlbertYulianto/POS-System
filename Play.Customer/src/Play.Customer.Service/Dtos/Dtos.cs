namespace Play.Customer.Service.Dtos;

public record CustomerDto(
    Guid Id,
    string Name,
    string ContactNumber,
    string Email,
    string Address,
    DateTimeOffset CreatedDate
);

public record CreateCustomerDto(
    string Name,
    string ContactNumber,
    string Email,
    string Address
);

public record UpdateCustomerDto(
    string Name,
    string ContactNumber,
    string Email,
    string Address
);
