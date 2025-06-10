using Play.Sales.Service.Dtos;
using Polly;
using Polly.CircuitBreaker;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging; // Added for logging

namespace Play.Sales.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient httpClient;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy;
    private readonly AsyncPolicy<HttpResponseMessage> retryPolicy;
    private readonly ILogger<CatalogClient> logger; // Added for logging


    public CatalogClient(HttpClient httpClient, ILogger<CatalogClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;

        // Retry policy: coba ulang 5 kali jika gagal, dengan logging
        this.retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .RetryAsync(5, onRetry: (result, retryCount) =>
            {
                logger.LogWarning("Retry attempt {RetryCount} for catalog service request due to {ExceptionMessage}",
                    retryCount, result.Exception?.Message ?? $"HTTP {result.Result?.StatusCode}");
            });

        // Circuit breaker: buka setelah 3 kegagalan berturut-turut, selama 30 detik
        this.circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        HttpResponseMessage response;
        try
        {
            // Wrap retry policy around circuit breaker policy
            response = await retryPolicy.WrapAsync(circuitBreakerPolicy)
                .ExecuteAsync(() => httpClient.GetAsync($"products/{productId}"));
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open for catalog service, request for product {ProductId} aborted", productId);
            return null;
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
        return null;
    }

    public async Task<bool> UpdateProductAsync(Guid productId, UpdateProductDto updateProductDto)
    {
        HttpResponseMessage response;
        try
        {
            // Wrap retry policy around circuit breaker policy
            response = await retryPolicy.WrapAsync(circuitBreakerPolicy)
                .ExecuteAsync(() => httpClient.PutAsJsonAsync($"products/{productId}", updateProductDto));
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open for catalog service, update request for product {ProductId} aborted", productId);
            return false;
        }
        return response.IsSuccessStatusCode;
    }
}