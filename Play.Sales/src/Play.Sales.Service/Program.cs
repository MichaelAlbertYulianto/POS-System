using Play.Common.MongoDB;
using Play.Sales.Service.Clients;
using Play.Sales.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
               .AddMongoRepository<Sale>("sales");

// Configure service clients
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    // client.BaseAddress = new Uri("http://catalog-service");
    client.BaseAddress = new Uri("https://localhost:5001");
});

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    // client.BaseAddress = new Uri("http://customer-service");
    client.BaseAddress = new Uri("https://localhost:5002");
});

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
