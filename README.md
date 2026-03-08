# 💜 FinanzasApp Familiar — Nuxt 3

App de gestión de finanzas familiares construida con Nuxt 3, TypeScript estricto y las mejores prácticas del proyecto.

## Inicio rápido

```bash
npm install
npm run dev
```

## Estructura del proyecto

```
finanzas-familia/
├── shared/
│   └── types/
│       ├── finanzas.ts          # Tipos compartidos cliente + servidor
│       └── index.ts             # Barrel (✅ OK aquí)
│
├── app/
│   ├── types/
│   │   └── ui.ts                # Tipos solo de frontend (FormTransaccion, etc.)
│   │
│   ├── composables/
│   │   ├── index.ts             # ✅ Barrel ROOT para auto-import de composables anidados
│   │   ├── use-toast.ts
│   │   ├── use-categorias.ts
│   │   ├── use-formato.ts
│   │   ├── transacciones/
│   │   │   └── use-transacciones.ts
│   │   └── presupuestos/
│   │       └── use-presupuestos.ts
│   │
│   ├── components/
│   │   ├── layout/
│   │   │   └── Navbar.vue       # → <LayoutNavbar /> (sin duplicar "Layout")
│   │   ├── dashboard/
│   │   │   ├── ResumenBalance.vue   # → <DashboardResumenBalance />
│   │   │   └── GastosCategorias.vue # → <DashboardGastosCategorias />
│   │   ├── transacciones/
│   │   │   └── Row.vue          # → <TransaccionesRow />
│   │   ├── presupuestos/
│   │   │   └── Card.vue         # → <PresupuestosCard />
│   │   └── ui/
│   │       ├── Toast.vue        # → <UiToast />
│   │       └── Spinner.vue      # → <UiSpinner />
│   │
│   └── pages/
│       └── index.vue
│
├── server/
│   ├── api/
│   │   ├── transacciones/
│   │   │   ├── index.get.ts     # GET  /api/transacciones
│   │   │   ├── index.post.ts    # POST /api/transacciones
│   │   │   └── [id].delete.ts   # DELETE /api/transacciones/:id
│   │   └── presupuestos/
│   │       ├── index.get.ts     # GET /api/presupuestos
│   │       └── index.put.ts     # PUT /api/presupuestos
│   ├── types/
│   │   └── internal.ts          # Tipos solo del servidor
│   └── utils/
│       ├── errors.ts            # notFound(), badRequest(), internalError()
│       ├── schemas.ts           # Schemas Zod para validación
│       └── storage.ts           # Almacenamiento (reemplazable con DB)
│
├── nuxt.config.ts
├── tsconfig.json
└── package.json
```

## Reglas de AGENTS.md aplicadas

### Data Fetching (CRITICAL)
- ✅ `data-use-fetch` — Todos los componentes usan `useFetch`, nunca `fetch` crudo
- ✅ `data-key-unique` — Claves únicas reactivas: `key: () => \`transacciones-${mes.value}\``
- ✅ `data-transform` — Datos transformados en `transform:`, no en plantillas
- ✅ `data-error-handling` — Estados `pending`, `error`, `success` siempre manejados

### Auto-Imports & Organization (CRITICAL)
- ✅ `imports-no-barrel-autoimport` — Barrel solo en `composables/index.ts` root, no en subcarpetas
- ✅ `imports-component-naming` — Sin duplicar prefijo: `Row.vue` → `<TransaccionesRow />` (no `TransaccionesTransaccionRow`)
- ✅ `imports-type-locations` — Tipos en `shared/types/`, `app/types/`, `server/types/`
- ✅ `imports-composable-exports` — Composables exportan solo funciones, jamás tipos
- ✅ `imports-direct-composable-imports` — Composables entre sí usan imports directos: `import { useToast } from '~/composables/use-toast'`

### Server & API Routes (HIGH)
- ✅ `server-validated-input` — `getValidatedQuery` + `readValidatedBody` con Zod en todos los endpoints
- ✅ `server-error-handling` — `createError` con `notFound()`, `badRequest()` helpers
- ✅ Rutas nombradas por método: `index.get.ts`, `index.post.ts`, `[id].delete.ts`

### Rendering Modes (HIGH)
- ✅ `rendering-route-rules` — `routeRules` en `nuxt.config.ts` por ruta

### State Management (MEDIUM-HIGH)
- ✅ `state-use-state` — `useState()` para estado SSR-safe compartido (`tabActiva`, `filtroMes`)
- ✅ `state-computed-over-watch` — `computed()` para estado derivado, nunca `watch` innecesario

### Type Safety (MEDIUM)
- ✅ `types-no-inline` — Cero tipos inline en componentes o composables
- ✅ `types-import-paths` — Rutas correctas: `#shared/types`, `~/types/ui`, `~~/server/types`
- ✅ `types-no-any` — TypeScript strict, sin `any` en todo el proyecto
- ✅ `types-zod-schemas` — Schemas Zod en `server/utils/schemas.ts` con `z.infer<>`

### Modules & Plugins (LOW-MEDIUM)
- ✅ `modules-order` — Módulos ordenados correctamente en `nuxt.config.ts`

## Para producción

Reemplaza `server/utils/storage.ts` con Drizzle ORM o Prisma apuntando a tu base de datos.
