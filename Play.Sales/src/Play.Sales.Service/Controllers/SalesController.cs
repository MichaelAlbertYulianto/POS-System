using Microsoft.AspNetCore.Mvc;
using Play.Sales.Service.Clients;
using Play.Sales.Service.Dtos;
using Play.Sales.Service.Entities;
using Play.Common;

namespace Play.Sales.Service.Controllers;

[ApiController]
[Route("sales")]
public class SalesController : ControllerBase
{
    private readonly IRepository<Sale> salesRepository;
    private readonly CatalogClient catalogClient;
    private readonly CustomerClient customerClient;

    public SalesController(
        IRepository<Sale> salesRepository,
        CatalogClient catalogClient,
        CustomerClient customerClient)
    {
        this.salesRepository = salesRepository;
        this.catalogClient = catalogClient;
        this.customerClient = customerClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAsync()
    {
        var sales = await salesRepository.GetAllAsync();
        var salesDtos = new List<SaleDto>();

        foreach (var sale in sales)
        {
            var customer = await customerClient.GetCustomerAsync(sale.CustomerId);
            if (customer == null) continue;

            var productNames = new Dictionary<Guid, string>();
            foreach (var item in sale.Items)
            {
                var product = await catalogClient.GetProductAsync(item.ProductId);
                if (product != null)
                {
                    productNames[item.ProductId] = product.Name;
                }
            }

            salesDtos.Add(sale.AsDto(customer.Name, productNames));
        }

        return Ok(salesDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetByIdAsync(Guid id)
    {
        var sale = await salesRepository.GetAsync(id);
        if (sale == null)
            return NotFound();

        var customer = await customerClient.GetCustomerAsync(sale.CustomerId);
        if (customer == null)
            return NotFound("Customer not found");

        var productNames = new Dictionary<Guid, string>();
        foreach (var item in sale.Items)
        {
            var product = await catalogClient.GetProductAsync(item.ProductId);
            if (product != null)
            {
                productNames[item.ProductId] = product.Name;
            }
        }

        return Ok(sale.AsDto(customer.Name, productNames));
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> PostAsync(CreateSaleDto createSaleDto)
    {
        var customer = await customerClient.GetCustomerAsync(createSaleDto.CustomerId);
        if (customer == null)
            return NotFound("Customer not found");

        var saleItems = new List<SaleItem>();
        decimal totalAmount = 0;

        foreach (var itemDto in createSaleDto.Items)
        {
            var product = await catalogClient.GetProductAsync(itemDto.ProductId);
            if (product == null)
                return NotFound($"Product with ID {itemDto.ProductId} not found");

            var saleItem = new SaleItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                Price = product.Price
            };

            totalAmount += saleItem.Price * saleItem.Quantity;
            saleItems.Add(saleItem);
        }

        var sale = new Sale
        {
            CustomerId = createSaleDto.CustomerId,
            Items = saleItems,
            TotalAmount = totalAmount,
            SaleDate = DateTimeOffset.UtcNow,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await salesRepository.CreateAsync(sale);

        var productNames = saleItems.ToDictionary(
            item => item.ProductId,
            item => "Unknown Product"
        );

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = sale.Id },
            sale.AsDto(customer.Name, productNames)
        );
    }
}
