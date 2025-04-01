FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Play.Catalog/src/Play.Catalog.Service/Play.Catalog.Service.csproj", "Play.Catalog/src/Play.Catalog.Service/"]
COPY ["Play.Common/src/Play.Common/Play.Common.csproj", "Play.Common/src/Play.Common/"]
RUN dotnet restore "Play.Catalog/src/Play.Catalog.Service/Play.Catalog.Service.csproj"
COPY . .
WORKDIR "/src/Play.Catalog/src/Play.Catalog.Service"
RUN dotnet build "Play.Catalog.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Catalog.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Catalog.Service.dll"]
