#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ZeeslagFrontEnd/Server/ZeeslagFrontEnd.Server.csproj", "src/ZeeslagFrontEnd/Server/"]
COPY ["src/ZeeslagFrontEnd/Client/ZeeslagFrontEnd.Client.csproj", "src/ZeeslagFrontEnd/Client/"]
COPY ["src/ZeeslagFrontEnd/Shared/ZeeslagFrontEnd.Shared.csproj", "src/ZeeslagFrontEnd/Shared/"]
RUN dotnet restore "src/ZeeslagFrontEnd/Server/ZeeslagFrontEnd.Server.csproj"
COPY . .
WORKDIR "/src/src/ZeeslagFrontEnd/Server"
RUN dotnet build "ZeeslagFrontEnd.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZeeslagFrontEnd.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZeeslagFrontEnd.Server.dll"]