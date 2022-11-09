#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ShipService/ShipService.csproj", "src/ShipService/"]
RUN dotnet restore "src/ShipService/ShipService.csproj"
COPY . .
WORKDIR "/src/src/ShipService"
RUN dotnet build "ShipService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShipService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShipService.dll"]