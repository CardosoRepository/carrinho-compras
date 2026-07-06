FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/CarrinhoCompras.Domain/CarrinhoCompras.Domain.csproj \
    src/CarrinhoCompras.Domain/

COPY src/CarrinhoCompras.Application/CarrinhoCompras.Application.csproj \
    src/CarrinhoCompras.Application/

COPY src/CarrinhoCompras.Infrastructure/CarrinhoCompras.Infrastructure.csproj \
    src/CarrinhoCompras.Infrastructure/

COPY src/CarrinhoCompras.Api/CarrinhoCompras.Api.csproj \
    src/CarrinhoCompras.Api/

RUN dotnet restore \
    src/CarrinhoCompras.Api/CarrinhoCompras.Api.csproj

COPY . .

RUN dotnet publish \
    src/CarrinhoCompras.Api/CarrinhoCompras.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CarrinhoCompras.Api.dll"]