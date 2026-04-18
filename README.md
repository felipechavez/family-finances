# DomusPay — Gestión de Finanzas Familiares

App de finanzas familiares con Nuxt 3 (frontend) y ASP.NET Core 10 (backend), PostgreSQL vía Supabase y Redis para caché.

## Requisitos

- Node.js 20+
- .NET 10 SDK
- Redis (local o Docker)
- Cuenta Supabase con esquema `finances`

## Inicio rápido

### Frontend

```bash
cd frontend
npm install
npm run dev        # http://localhost:3000
```

### Backend

```bash
cd backend
dotnet run --project src/FinanceApp.API/   # http://localhost:58196
```

### Con Docker Compose

```bash
docker compose up
```

## Variables de entorno

### Frontend (`frontend/.env`)

```env
API_BASE_URL=http://localhost:58196/
```

### Backend (`backend/src/FinanceApp.API/appsettings.Development.json`)

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=...;Database=postgres;Username=postgres;Password=...;SslMode=Require;Search Path=finances",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "tu_secreto_jwt",
    "Issuer": "FinanceApp",
    "Audience": "FinanceApp"
  },
  "Resend": {
    "ApiKey": "re_...",
    "FromAddress": "noreply@domuspay.cl",
    "AppBaseUrl": "http://localhost:3000"
  },
  "EmailVerification": {
    "Enabled": true
  }
}
```

## Estructura del repositorio

```
family-finances/
├── frontend/                  # Nuxt 3 — interfaz de usuario
│   ├── app/
│   │   ├── components/        # Componentes Vue (auto-imported con prefijo)
│   │   ├── composables/       # Lógica reutilizable (barrel en index.ts)
│   │   ├── pages/             # Rutas de la app
│   │   ├── stores/            # Estado global con Pinia
│   │   ├── plugins/           # Plugin $api con JWT
│   │   ├── middleware/        # Guards de autenticación
│   │   └── types/             # Tipos solo de frontend (ui.ts)
│   ├── i18n/
│   │   └── locales/           # Traducciones ES / EN
│   ├── shared/
│   │   └── types/finanzas.ts  # Tipos de dominio compartidos
│   ├── public/                # Assets estáticos
│   ├── nuxt.config.ts
│   ├── tsconfig.json
│   └── package.json
│
├── backend/                   # ASP.NET Core 10 — Clean Architecture
│   ├── src/
│   │   ├── FinanceApp.Domain/         # Entidades + atributos Supabase
│   │   ├── FinanceApp.Application/    # MediatR handlers, FluentValidation
│   │   ├── FinanceApp.Infrastructure/ # Supabase SDK, JWT, Redis, Resend
│   │   └── FinanceApp.API/            # Minimal APIs, endpoints por recurso
│   ├── tests/
│   │   ├── FinanceApp.UnitTests/
│   │   └── FinanceApp.IntegrationTests/
│   └── FinanceApp.sln
│
├── db/                        # Migraciones SQL (aplicar manualmente en Supabase)
└── docker-compose.yml
```

## Comandos útiles

```bash
# Typecheck frontend
cd frontend && npm run typecheck

# Build frontend
cd frontend && npm run build

# Tests backend
cd backend && dotnet test

# Build backend
cd backend && dotnet build
```

## Claude Code Skills

Este proyecto usa [Claude Code](https://claude.ai/code) con los siguientes skills configurados en `.claude/skills/`:

| Skill | Propósito |
|---|---|
| `dotnet-10-csharp-14` | Patrones modernos de .NET 10 y C# 14: minimal APIs, Options, TypedResults, resilience |
| `vue-best-practices` | Composición API, Composition API con `<script setup>`, composables, data flow |
| `dotnet-localization` | Localización con `.resx`, `IStringLocalizer` y source generators |
| `csharp-tunit` | Tests unitarios con TUnit, incluyendo tests parametrizados |
| `docker-expert` | Optimización de contenedores, multi-stage builds, hardening de seguridad |
| `bms-git-log-summary` | Resúmenes de actividad del repositorio vía git log |

---

## Arquitectura

- **Multi-tenant**: todos los datos están acotados a `family_id` extraído del JWT.
- **Auth**: JWT custom con verificación de correo (Resend) y soporte 2FA (TOTP).
- **Estado frontend**: Pinia stores (`auth`, `transacciones`, `presupuestos`, `cuentas`).
- **API calls**: `useFetch` con claves reactivas; mutaciones vía plugin `$api`.
- **Base de datos**: Supabase Postgres (sin ORM), esquema `finances`.
