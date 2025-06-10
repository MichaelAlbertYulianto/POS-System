using Play.Sales.Service.Dtos;
using Polly;
using Polly.CircuitBreaker;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Play.Sales.Service.Clients;

public class CustomerClient
{
    private readonly HttpClient httpClient;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy;
    private readonly AsyncPolicy<HttpResponseMessage> retryPolicy;
    private readonly ILogger<CustomerClient> logger;

    public CustomerClient(HttpClient httpClient, ILogger<CustomerClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;

        // Retry policy: coba ulang 5 kali jika gagal, dengan logging
        this.retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .RetryAsync(5, onRetry: (result, retryCount) =>
            {
                logger.LogWarning("Retry attempt {RetryCount} for customer service request due to {ExceptionMessage}",
                    retryCount, result.Exception?.Message ?? $"HTTP {result.Result?.StatusCode}");
            });

        // Circuit breaker: buka setelah 3 kegagalan berturut-turut, selama 30 detik
        this.circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid customerId)
    {
        HttpResponseMessage response;
        try
        {
            // Wrap retry policy around circuit breaker policy
            response = await retryPolicy.WrapAsync(circuitBreakerPolicy)
                .ExecuteAsync(() => httpClient.GetAsync($"customers/{customerId}"));
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open for customer service, request for customer {CustomerId} aborted", customerId);
            return null;
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }
}