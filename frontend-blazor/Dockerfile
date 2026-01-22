FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InovaSaude.Web/InovaSaude.Web.csproj", "InovaSaude.Web/"]
RUN dotnet restore "InovaSaude.Web/InovaSaude.Web.csproj"
COPY . .
WORKDIR "/src/InovaSaude.Web"
RUN dotnet build "InovaSaude.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InovaSaude.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InovaSaude.Web.dll"]
