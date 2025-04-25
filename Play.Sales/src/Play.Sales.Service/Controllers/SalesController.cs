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
        var productDetails = new Dictionary<Guid, (ProductDto Product, int RequestedQuantity)>();
        decimal totalAmount = 0;

        // Step 1: Validate all products exist and get their details
        foreach (var itemDto in createSaleDto.Items)
        {
            var product = await catalogClient.GetProductAsync(itemDto.ProductId);
            if (product == null)
                return NotFound($"Product with ID {itemDto.ProductId} not found");

            // Store product and requested quantity for later validation
            productDetails[itemDto.ProductId] = (product, itemDto.Quantity);

            var saleItem = new SaleItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                Price = product.Price
            };

            totalAmount += saleItem.Price * saleItem.Quantity;
            saleItems.Add(saleItem);
        }

        // Step 2: Check if there's sufficient stock for all products
        foreach (var (productId, (product, requestedQuantity)) in productDetails)
        {
            if (product.StockQuantity < requestedQuantity)
            {
                return BadRequest($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {requestedQuantity}");
            }
        }

        // Step 3: Create sale entity but don't save it yet
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = createSaleDto.CustomerId,
            Items = saleItems,
            TotalAmount = totalAmount,
            SaleDate = DateTimeOffset.UtcNow,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Step 4: Update stock quantities atomically
        var updatedProducts = new List<Guid>();
        try
        {
            foreach (var (productId, (product, requestedQuantity)) in productDetails)
            {
                // Create UpdateProductDto with reduced stock
                var updateProductDto = new UpdateProductDto(
                    Name: product.Name,
                    CategoryId: product.CategoryId,
                    Price: product.Price,
                    StockQuantity: product.StockQuantity - requestedQuantity,
                    Description: product.Description
                );

                // Update the product's stock
                var updateSuccess = await catalogClient.UpdateProductAsync(productId, updateProductDto);
                if (!updateSuccess)
                {
                    throw new Exception($"Failed to update stock for product {productId}");
                }

                updatedProducts.Add(productId);
            }

            // Step 5: Save the sale only if all stock updates were successful
            await salesRepository.CreateAsync(sale);
        }
        catch (Exception ex)
        {
            // Step 6: Rollback stock updates if something went wrong
            foreach (var productId in updatedProducts)
            {
                var (product, requestedQuantity) = productDetails[productId];
                
                // Revert the stock update by adding the quantity back
                var revertProductDto = new UpdateProductDto(
                    Name: product.Name,
                    CategoryId: product.CategoryId,
                    Price: product.Price,
                    StockQuantity: product.StockQuantity, // Original stock quantity
                    Description: product.Description
                );

                await catalogClient.UpdateProductAsync(productId, revertProductDto);
            }

            return StatusCode(500, $"Error processing sale: {ex.Message}");
        }

        // Step 7: Return successful response
        var productNames = saleItems.ToDictionary(
            item => item.ProductId,
            item => productDetails.ContainsKey(item.ProductId) ? productDetails[item.ProductId].Product.Name : "Unknown Product"
        );

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = sale.Id },
            sale.AsDto(customer.Name, productNames)
        );
    }
}
