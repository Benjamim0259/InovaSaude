# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY ["InovaSaude.Blazor.csproj", "./"]
RUN dotnet restore "InovaSaude.Blazor.csproj"

# Copiar todo o código e compilar
COPY . .
RUN dotnet build "InovaSaude.Blazor.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "InovaSaude.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar ferramentas necessárias
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Expor porta
EXPOSE 5163

# Configurar usuário não-root
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Comando de inicialização
ENTRYPOINT ["dotnet", "InovaSaude.Blazor.dll"]
