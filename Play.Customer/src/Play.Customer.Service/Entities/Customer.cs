using Play.Common;

namespace Play.Customer.Service.Entities;

public class Customer : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
}
