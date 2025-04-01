using Play.Sales.Service.Dtos;

namespace Play.Sales.Service.Clients;

public class CustomerClient
{
    private readonly HttpClient httpClient;

    public CustomerClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid customerId)
    {
        var response = await httpClient.GetAsync($"customers/{customerId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }
}
