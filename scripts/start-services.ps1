# Start MongoDB
Write-Host "Starting MongoDB container..."
docker-compose -f Play.Infra/docker-compose.yml up -d mongo

# Wait for MongoDB to be ready
Start-Sleep -Seconds 5

# Start all services
Write-Host "Starting Catalog Service..."
Start-Process "dotnet" -ArgumentList "run --project Play.Catalog/src/Play.Catalog.Service/Play.Catalog.Service.csproj --urls http://localhost:5001"

Write-Host "Starting Customer Service..."
Start-Process "dotnet" -ArgumentList "run --project Play.Customer/src/Play.Customer.Service/Play.Customer.Service.csproj --urls http://localhost:5002"

Write-Host "Starting Sales Service..."
Start-Process "dotnet" -ArgumentList "run --project Play.Sales/src/Play.Sales.Service/Play.Sales.Service.csproj --urls http://localhost:5003"

Write-Host "All services are running!"
Write-Host "Catalog Service: http://localhost:5001/swagger"
Write-Host "Customer Service: http://localhost:5002/swagger"
Write-Host "Sales Service: http://localhost:5003/swagger"

Write-Host "Press any key to stop all services..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
