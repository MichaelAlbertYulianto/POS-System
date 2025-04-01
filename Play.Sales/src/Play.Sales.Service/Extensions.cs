using Play.Sales.Service.Dtos;
using Play.Sales.Service.Entities;

namespace Play.Sales.Service;

public static class Extensions
{
    public static SaleDto AsDto(this Sale sale, string customerName, Dictionary<Guid, string> productNames)
    {
        return new SaleDto(
            sale.Id,
            sale.CustomerId,
            customerName,
            sale.SaleDate,
            sale.TotalAmount,
            sale.Items.Select(item => new SaleItemDto(
                item.ProductId,
                productNames.GetValueOrDefault(item.ProductId, "Unknown Product"),
                item.Quantity,
                item.Price
            )),
            sale.CreatedDate
        );
    }
}
