using Play.Sales.Service.Dtos;

namespace Play.Sales.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var response = await httpClient.GetAsync($"products/{productId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
        return null;
    }
}
