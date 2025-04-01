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
`
POS System/
+-- Play.Catalog/            # Product Catalog Service
+-- Play.Customer/           # Customer Service
+-- Play.Sales/             # Sales Service
+-- Play.Common/            # Shared Library
+-- Play.Infra/             # Infrastructure (Docker Compose)
+-- scripts/                # Utility Scripts
`

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- MongoDB (via Docker)

### Running the Services

1. Start the services using the convenience script:
   `powershell
   .\scripts\start-services.ps1
   `

2. Access Swagger UI:
   - Catalog Service: http://localhost:5001/swagger
   - Customer Service: http://localhost:5002/swagger
   - Sales Service: http://localhost:5003/swagger

### Running with Docker Compose
`ash
docker-compose -f Play.Infra/docker-compose.yml up -d
`

## Data Models

### Product Catalog Service
- Products:
  * ProductId (Guid)
  * ProductName (string)
  * CategoryId (Guid)
  * Price (decimal)
  * StockQuantity (int)
  * Description (string)
- Categories:
  * CategoryId (Guid)
  * CategoryName (string)

### Customer Service
- Customers:
  * CustomerId (Guid)
  * CustomerName (string)
  * ContactNumber (string)
  * Email (string)
  * Address (string)

### Sales Service
- Sales:
  * SaleId (Guid)
  * CustomerId (Guid)
  * SaleDate (DateTimeOffset)
  * TotalAmount (decimal)
  * Items (List<SaleItem>)
- SaleItems:
  * ProductId (Guid)
  * Quantity (int)
  * Price (decimal)

## Architecture

The system follows a microservices architecture with:
- Independent services with their own databases
- Service-to-service communication via HTTP
- Shared common library for MongoDB integration
- Docker containerization
- RESTful API design
