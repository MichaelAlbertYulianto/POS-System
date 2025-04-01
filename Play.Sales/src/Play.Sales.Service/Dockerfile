FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Play.Sales/src/Play.Sales.Service/Play.Sales.Service.csproj", "Play.Sales/src/Play.Sales.Service/"]
COPY ["Play.Common/src/Play.Common/Play.Common.csproj", "Play.Common/src/Play.Common/"]
RUN dotnet restore "Play.Sales/src/Play.Sales.Service/Play.Sales.Service.csproj"
COPY . .
WORKDIR "/src/Play.Sales/src/Play.Sales.Service"
RUN dotnet build "Play.Sales.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Sales.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Sales.Service.dll"]
