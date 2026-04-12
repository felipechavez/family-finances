# ──────────────────────────────────────────────
# Stage 1: Base runtime
# ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN groupadd -g 1001 appgroup && useradd -u 1001 -g appgroup -s /bin/false appuser
WORKDIR /app
EXPOSE 8080

# ──────────────────────────────────────────────
# Stage 2: Build
# ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar sólo .csproj primero para aprovechar caché de NuGet
COPY ["src/FinanceApp.API/FinanceApp.API.csproj",             "src/FinanceApp.API/"]
COPY ["src/FinanceApp.Application/FinanceApp.Application.csproj", "src/FinanceApp.Application/"]
COPY ["src/FinanceApp.Infrastructure/FinanceApp.Infrastructure.csproj", "src/FinanceApp.Infrastructure/"]
COPY ["src/FinanceApp.Domain/FinanceApp.Domain.csproj",       "src/FinanceApp.Domain/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "src/FinanceApp.API/FinanceApp.API.csproj"

COPY . .
WORKDIR "/src/src/FinanceApp.API"
RUN dotnet build "FinanceApp.API.csproj" -c Release -o /app/build

# ──────────────────────────────────────────────
# Stage 3: Publish
# ──────────────────────────────────────────────
FROM build AS publish
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "FinanceApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ──────────────────────────────────────────────
# Stage 4: Runtime final (imagen mínima)
# ──────────────────────────────────────────────
FROM base AS final
WORKDIR /app
COPY --from=publish --chown=appuser:appgroup /app/publish .
USER appuser
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "FinanceApp.API.dll"]
