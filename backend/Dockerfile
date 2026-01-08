# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY InovaSaude.sln ./
COPY src/InovaSaude.API/InovaSaude.API.csproj ./src/InovaSaude.API/
COPY src/InovaSaude.Application/InovaSaude.Application.csproj ./src/InovaSaude.Application/
COPY src/InovaSaude.Core/InovaSaude.Core.csproj ./src/InovaSaude.Core/
COPY src/InovaSaude.Infrastructure/InovaSaude.Infrastructure.csproj ./src/InovaSaude.Infrastructure/
COPY tests/InovaSaude.Tests/InovaSaude.Tests.csproj ./tests/InovaSaude.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish src/InovaSaude.API/InovaSaude.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "InovaSaude.API.dll"]
