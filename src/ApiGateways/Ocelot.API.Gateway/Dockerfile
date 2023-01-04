FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApiGateways/Ocelot.API.Gateway/Ocelot.API.Gateway.csproj", "ApiGateways/Ocelot.API.Gateway/"]
RUN dotnet restore "ApiGateways/Ocelot.API.Gateway/Ocelot.API.Gateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Ocelot.API.Gateway"
RUN dotnet build "Ocelot.API.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ocelot.API.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.API.Gateway.dll"]
