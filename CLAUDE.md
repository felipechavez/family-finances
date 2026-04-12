# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Family finances app (FinanzasApp Familiar) with a **Nuxt 3 frontend** (`app/`) and an **ASP.NET Core 10 backend** (`backend/`). Shared TypeScript types live in `shared/types/`. PostgreSQL is managed via Supabase; SQL migrations are in `db/`.

## Development Commands

### Frontend (`app/`)
```bash
npm install
npm run dev        # Dev server (default port 3000)
npm run build
npm run typecheck
```

### Backend (`backend/`)
```bash
cd backend
dotnet build
dotnet run --project src/FinanceApp.API/   # API at http://localhost:58196
dotnet test                                 # All tests
dotnet test tests/FinanceApp.UnitTests/FinanceApp.UnitTests.csproj --filter "FullyQualifiedName=<TestClass>"
dotnet test tests/FinanceApp.IntegrationTests/FinanceApp.IntegrationTests.csproj --filter "FullyQualifiedName=<TestClass>"
```

### Required Environment Variables (Backend)
- `SUPABASE_URL`, `SUPABASE_KEY`, `SUPABASE_SCHEMA` (defaults to `"public"`)
- JWT settings in `appsettings.json`: `Issuer`, `Audience`, `Secret`, `ExpirationDays`
- Redis connection string for caching

Frontend API base URL defaults to `http://localhost:58196/`; override with `API_BASE_URL`.

## Architecture

### Frontend (Nuxt 3)
- **State**: Pinia stores in `app/stores/` — `auth`, `transacciones`, `presupuestos`, `cuentas`
- **API calls**: All fetching uses `useFetch` with reactive keys for cache invalidation; mutations use the `$api` plugin. Never use raw `fetch`.
- **Auth**: JWT stored in `localStorage`, injected via `app/plugins/api.ts` as `Authorization: Bearer` header. On 401, token is cleared and user redirected to login.
- **Components**: Auto-imported with path prefixes — `app/components/transacciones/Row.vue` becomes `<TransaccionesRow />`. Never duplicate the prefix (e.g., not `TransaccionesTransaccionRow`).
- **Composables**: Barrel export only at `app/composables/index.ts` (not in subdirectories). Composables import each other with direct paths (`~/composables/use-toast`), never through the barrel.
- **Types**: Shared domain types in `shared/types/finanzas.ts`, frontend-only UI types in `app/types/ui.ts`. No inline types in components or composables. No `any`.

### Backend (Clean Architecture)
```
Domain → Application → Infrastructure → API
```
- **Domain** (`FinanceApp.Domain`): Entities (no EF Core; use Supabase attributes `[Table]`, `[Column]`). Entities inherit `Entity` base class.
- **Application** (`FinanceApp.Application`): MediatR handlers (CQRS). FluentValidation via `ValidationBehavior` pipeline. Interfaces only — no infrastructure dependencies.
- **Infrastructure** (`FinanceApp.Infrastructure`): Supabase C# SDK for DB access, BCrypt for password hashing, JWT generation, Redis caching.
- **API** (`FinanceApp.API`): ASP.NET Core Minimal APIs. Endpoints organized by resource in `Endpoints/` — each file maps routes for one resource. Claims extraction via `ClaimsPrincipalExtensions`.

### Database
No ORM — direct Supabase SDK (Postgrest) queries. Domain entities carry Supabase mapping attributes. Migrations in `db/` (run manually against Supabase).

Multi-tenant: all data is scoped to a `family_id` extracted from the JWT claims (`FamilyId`). Every handler must scope queries to the authenticated family.

## Key Conventions

### Frontend
- Use `useFetch` with a reactive `key` function so the cache invalidates when filters change: `key: () => \`transacciones-${mes.value}\``
- Transform data in `transform:` option, not in template expressions
- Always handle `pending`, `error`, and `success` states
- Validate server input with `getValidatedQuery` / `readValidatedBody` + Zod schemas; use `createError` for errors

### Backend
- Each feature folder under `Application/Features/` contains a handler, command/query record, and validator
- Endpoints call `mediator.Send(command)` — no business logic in endpoint files
- Use `ClaimsPrincipalExtensions` to extract `UserId` and `FamilyId` from JWT claims; always authorize with `.RequireAuthorization()`
- Return DTOs, not domain entities, from handlers
