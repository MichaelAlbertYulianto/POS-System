# POS System Microservices

A Point of Sale (POS) system implemented using a microservices architecture, built with .NET and MongoDB.

## Services

### 1. Product Catalog Service (Port: 5001)

- Manages products and categories
- Endpoints:
  * GET /products - List all products
  * GET /products/{id} - Get product by ID
  * POST /products - Create new product
  * PUT /products/{id} - Update product
  * DELETE /products/{id} - Delete product
  * Similar endpoints for /categories

### 2. Customer Service (Port: 5002)

- Manages customer information
- Endpoints:
  * GET /customers - List all customers
  * GET /customers/{id} - Get customer by ID
  * POST /customers - Create new customer
  * PUT /customers/{id} - Update customer
  * DELETE /customers/{id} - Delete customer

### 3. Sales Service (Port: 5003)

- Manages sales transactions
- Integrates with Product Catalog and Customer services
- Endpoints:
  * GET /sales - List all sales with product and customer details
  * GET /sales/{id} - Get sale by ID with details
  * POST /sales - Create new sale

## Technology Stack

- .NET 9.0
- MongoDB
- Docker & Docker Compose
- Swagger/OpenAPI

## Project Structure

* `POS System/ +-- Play.Catalog/`
* `Product Catalog Service +-- Play.Customer/`
* `Customer Service +-- Play.Sales/`
* `Sales Service +-- Play.Common/`

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- MongoDB

### Running the Services

1. Start the services :

   - Product Catalog Service: `dotnet run --project Play.Catalog/src/Play.Catalog.Service/Play.Catalog.Service.csproj`
   - Customer Service: `dotnet run --project Play.Customer/src/Play.Customer.Service/Play.Customer.Service.csproj`
   - Sales Service: `dotnet run --project Play.Sales/src/Play.Sales.Service/Play.Sales.Service.csproj`
2. Access Swagger UI:

   - Catalog Service: http://localhost:5001/swagger
   - Customer Service: http://localhost:5002/swagger
   - Sales Service: http://localhost:5003/swagger
     ## Data Models
