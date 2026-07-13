# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY SalonCRM.sln ./
COPY src/Domain/SalonCRM.Domain.csproj src/Domain/
COPY src/Application/SalonCRM.Application.csproj src/Application/
COPY src/Infrastructure/SalonCRM.Infrastructure.csproj src/Infrastructure/
COPY src/Api/SalonCRM.Api.csproj src/Api/

RUN dotnet restore src/Api/SalonCRM.Api.csproj

COPY src/ src/
RUN dotnet publish src/Api/SalonCRM.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage (lightweight image for Render Docker Web Service)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Render sets PORT at runtime; default to 8080 for local docker runs.
ENV PORT=8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SalonCRM.Api.dll"]
