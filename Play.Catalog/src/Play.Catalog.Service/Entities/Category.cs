using Play.Common;

namespace Play.Catalog.Service.Entities;

public class Category : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
}
