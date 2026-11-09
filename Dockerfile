FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RestaurantApi.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_hostBuilder__reloadConfigOnChange=false

EXPOSE 8080

ENTRYPOINT ["dotnet", "RestaurantApi.dll"]
