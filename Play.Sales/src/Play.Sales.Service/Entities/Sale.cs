using Play.Common;

namespace Play.Sales.Service.Entities;

public class Sale : IEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTimeOffset SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public List<SaleItem> Items { get; set; } = new();
}

public class SaleItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
