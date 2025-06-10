using MongoDB.Driver.GeoJsonObjectModel.Serializers;
using Play.Common.MongoDB;
using Play.Sales.Service.Clients;
using Play.Sales.Service.Entities;
using Polly;
using Polly.Retry;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);
var jitterer = new Random();

// Add services to the container.
builder.Services.AddMongo()
               .AddMongoRepository<Sale>("sales");

// Configure service clients
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    // client.BaseAddress = new Uri("http://catalog-service");
    client.BaseAddress = new Uri("https://localhost:5001");
})
.AddTransientHttpErrorPolicy(PolicyBuilder => PolicyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    10, // Number of retries
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)), // Exponential backoff
    onRetry: (Outcome, TimeSpan, retryAttemp) =>
    {
        Console.WriteLine($"Delaying for {TimeSpan.TotalSeconds} seconds, then making retry {retryAttemp}");
    }
))
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    10, // Break the circuit after 3 consecutive failures
    TimeSpan.FromSeconds(15), // Keep circuit broken for 15 seconds
    onBreak: (outcome, timespan) =>
    {
        Console.WriteLine("Opening the circuit for 15 seconds...");
    },
    onReset: () =>
    {
        Console.WriteLine("Closing the circuit...");
    }
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    // client.BaseAddress = new Uri("http://customer-service");
    client.BaseAddress = new Uri("https://localhost:5002");
})
.AddTransientHttpErrorPolicy(PolicyBuilder => PolicyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    10, // Number of retries
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)), // Exponential backoff
    onRetry: (Outcome, TimeSpan, retryAttemp) =>
    {
        Console.WriteLine($"Delaying for {TimeSpan.TotalSeconds} seconds, then making retry {retryAttemp}");
    }
))
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    10, // Break the circuit after 3 consecutive failures
    TimeSpan.FromSeconds(15), // Keep circuit broken for 15 seconds
    onBreak: (outcome, timespan) =>
    {
        Console.WriteLine("Opening the circuit for 15 seconds...");
    },
    onReset: () =>
    {
        Console.WriteLine("Closing the circuit...");
    }
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

builder.Services.AddControllers(
    options => options.SuppressAsyncSuffixInActionNames = false
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
