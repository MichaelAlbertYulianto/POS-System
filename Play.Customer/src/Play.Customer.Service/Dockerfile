FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Play.Customer/src/Play.Customer.Service/Play.Customer.Service.csproj", "Play.Customer/src/Play.Customer.Service/"]
COPY ["Play.Common/src/Play.Common/Play.Common.csproj", "Play.Common/src/Play.Common/"]
RUN dotnet restore "Play.Customer/src/Play.Customer.Service/Play.Customer.Service.csproj"
COPY . .
WORKDIR "/src/Play.Customer/src/Play.Customer.Service"
RUN dotnet build "Play.Customer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Customer.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Customer.Service.dll"]
