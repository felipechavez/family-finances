<!-- app/pages/index.vue -->
<script setup lang="ts">
import type { FormTransaccion } from '~/types/ui'
import { useTransacciones } from '~/composables/transacciones/use-transacciones'
import { usePresupuestos } from '~/composables/presupuestos/use-presupuestos'
import { useToast } from '~/composables/use-toast'
import { useCategorias } from '~/composables/use-categorias'
import { useFormato } from '~/composables/use-formato'

useHead({ title: 'FinanzasApp Familiar', meta: [{ name: 'viewport', content: 'width=device-width, initial-scale=1' }] })

type TabId = 'resumen' | 'movimientos' | 'agregar' | 'presupuesto'

const tabActiva = useState<TabId>('tab-activa', () => 'resumen')
const mesActual = new Date().toISOString().slice(0, 7)
const filtroMes = useState<string>('filtro-mes', () => mesActual)

const { ok: toastOk, error: toastError } = useToast()
const { categoriasGasto, categoriasIngreso } = useCategorias()
const { formatMesLabel } = useFormato()

const {
  transacciones,
  status: statusTx,
  error: errorTx,
  totalIngresos,
  totalGastos,
  balance,
  gastosPorCategoria,
  crearTransaccion,
  eliminarTransaccion,
} = useTransacciones(filtroMes)

const {
  presupuestoPorCategoria,
  status: statusPresup,
  error: errorPresup,
  guardarLimite,
} = usePresupuestos(filtroMes)

const form = ref<FormTransaccion>({
  tipo: 'gasto',
  categoria: 'alimentacion',
  monto: '',
  descripcion: '',
  fecha: new Date().toISOString().slice(0, 10),
  miembro: 'Todos',
})

const mesesDisponibles = computed(() => {
  const meses = new Set(transacciones.value.map(t => t.fecha.slice(0, 7)))
  meses.add(mesActual)
  return Array.from(meses).sort().reverse()
})

const categoriasPorTipo = computed(() =>
  form.value.tipo === 'gasto' ? categoriasGasto : categoriasIngreso,
)

function handleTabChange(tab: TabId): void {
  tabActiva.value = tab
}

async function handleCrearTransaccion(): Promise<void> {
  const monto = Number(form.value.monto)
  if (!monto || monto <= 0) { toastError('Ingresa un monto válido'); return }
  if (!form.value.descripcion.trim()) { toastError('La descripción es requerida'); return }
  try {
    await crearTransaccion(form.value)
    form.value = { ...form.value, monto: '', descripcion: '' }
    toastOk(form.value.tipo === 'gasto' ? 'Gasto registrado ✓' : 'Ingreso registrado ✓')
    tabActiva.value = 'movimientos'
  } catch {
    toastError('No se pudo guardar. Intenta de nuevo.')
  }
}

async function handleEliminar(id: string): Promise<void> {
  try {
    await eliminarTransaccion(id)
    toastOk('Eliminado')
  } catch {
    toastError('No se pudo eliminar')
  }
}

async function handleGuardarLimite(categoria: string, limite: number): Promise<void> {
  try {
    await guardarLimite(categoria, limite)
    toastOk('Límite guardado ✓')
  } catch {
    toastError('No se pudo guardar el límite')
  }
}

function handleTipoChange(tipo: 'gasto' | 'ingreso'): void {
  form.value.tipo = tipo
  form.value.categoria = tipo === 'gasto' ? 'alimentacion' : 'sueldo'
}
</script>

<template>
  <div class="layout">

    <!-- Sidebar / Bottom nav (responsivo internamente) -->
    <LayoutNavbar :tab-activa="tabActiva" @tab-change="handleTabChange" />

    <!-- Área de contenido -->
    <div class="content-area">

      <!-- Header -->
      <header class="header">
        <div>
          <p class="header-sub">Familia</p>
          <h1 class="header-titulo">💜 FinanzasApp</h1>
        </div>
        <select v-model="filtroMes" class="select-mes">
          <option v-for="mes in mesesDisponibles" :key="mes" :value="mes">
            {{ formatMesLabel(mes) }}
          </option>
        </select>
      </header>

      <!-- Contenido principal -->
      <main class="main">

        <!-- ══ RESUMEN ══ -->
        <section v-if="tabActiva === 'resumen'">
          <UiSpinner v-if="statusTx === 'pending'">Cargando...</UiSpinner>
          <div v-else-if="errorTx" class="error-state">
            <p>Error al cargar transacciones</p>
            <p class="error-msg">{{ errorTx.message }}</p>
          </div>
          <template v-else>
            <!-- En desktop el resumen se divide en dos columnas -->
            <div class="resumen-grid">
              <div class="resumen-col-izq">
                <DashboardResumenBalance
                  :total-ingresos="totalIngresos"
                  :total-gastos="totalGastos"
                  :balance="balance"
                />
                <DashboardGastosCategorias
                  :gastos-por-categoria="gastosPorCategoria"
                  :presupuesto-por-categoria="presupuestoPorCategoria"
                  :total-gastos="totalGastos"
                />
              </div>
              <div class="resumen-col-der">
                <div v-if="transacciones.length" class="card">
                  <div class="card-header">
                    <h3 class="card-titulo">Últimos movimientos</h3>
                    <button class="btn-ver-todos" @click="tabActiva = 'movimientos'">Ver todos →</button>
                  </div>
                  <TransaccionesRow
                    v-for="tx in transacciones.slice(0, 6)"
                    :key="tx.id"
                    :transaccion="tx"
                  />
                </div>
              </div>
            </div>
          </template>
        </section>

        <!-- ══ MOVIMIENTOS ══ -->
        <section v-else-if="tabActiva === 'movimientos'">
          <h2 class="seccion-titulo">Todos los movimientos</h2>
          <UiSpinner v-if="statusTx === 'pending'">Cargando...</UiSpinner>
          <div v-else-if="errorTx" class="error-state"><p>Error al cargar</p></div>
          <div v-else-if="transacciones.length === 0" class="empty-state">
            <span class="empty-icon">📭</span>
            <p>Sin movimientos este mes</p>
          </div>
          <div v-else class="card card--sin-padding">
            <TransaccionesRow
              v-for="tx in transacciones"
              :key="tx.id"
              :transaccion="tx"
              :mostrar-eliminar="true"
              @eliminar="handleEliminar"
            />
          </div>
        </section>

        <!-- ══ AGREGAR ══ -->
        <section v-else-if="tabActiva === 'agregar'">
          <h2 class="seccion-titulo">Nuevo movimiento</h2>
          <div class="form-card">
            <div class="segmento">
              <button class="seg-btn" :class="{ 'seg-btn--gasto': form.tipo === 'gasto' }" @click="handleTipoChange('gasto')">▼ Gasto</button>
              <button class="seg-btn" :class="{ 'seg-btn--ingreso': form.tipo === 'ingreso' }" @click="handleTipoChange('ingreso')">▲ Ingreso</button>
            </div>
            <div class="form">
              <label class="field-label">MONTO ($)</label>
              <input v-model="form.monto" class="input" type="number" placeholder="0" inputmode="numeric" />

              <label class="field-label">DESCRIPCIÓN</label>
              <input v-model="form.descripcion" class="input" type="text" placeholder="¿En qué se gastó?" />

              <label class="field-label">CATEGORÍA</label>
              <select v-model="form.categoria" class="input select">
                <option v-for="cat in categoriasPorTipo" :key="cat.id" :value="cat.id">
                  {{ cat.emoji }} {{ cat.label }}
                </option>
              </select>

              <div class="form-row">
                <div class="form-col">
                  <label class="field-label">MIEMBRO</label>
                  <select v-model="form.miembro" class="input select">
                    <option v-for="m in ['Todos','Mamá','Papá','Hijo/a 1','Hijo/a 2']" :key="m" :value="m">{{ m }}</option>
                  </select>
                </div>
                <div class="form-col">
                  <label class="field-label">FECHA</label>
                  <input v-model="form.fecha" class="input" type="date" />
                </div>
              </div>

              <button class="btn-primary" @click="handleCrearTransaccion">
                {{ form.tipo === 'gasto' ? 'Registrar gasto' : 'Registrar ingreso' }}
              </button>
            </div>
          </div>
        </section>

        <!-- ══ PRESUPUESTO ══ -->
        <section v-else-if="tabActiva === 'presupuesto'">
          <h2 class="seccion-titulo">Límites de gasto</h2>
          <p class="seccion-sub">Define cuánto quieres gastar por categoría cada mes</p>
          <UiSpinner v-if="statusPresup === 'pending'">Cargando...</UiSpinner>
          <div v-else-if="errorPresup" class="error-state"><p>Error al cargar presupuestos</p></div>
          <template v-else>
            <div class="presupuesto-grid">
              <PresupuestosCard
                v-for="cat in categoriasGasto"
                :key="cat.id"
                :categoria="cat"
                :gastado="gastosPorCategoria[cat.id] ?? 0"
                :limite="presupuestoPorCategoria[cat.id] ?? 0"
                @guardar-limite="handleGuardarLimite"
              />
            </div>
          </template>
        </section>

      </main>
    </div>

    <UiToast />
  </div>
</template>

<style>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700&display=swap');
*, *::before, *::after { box-sizing: border-box; }
body {
  margin: 0;
  background: #0f0f14;
  font-family: 'DM Sans', sans-serif;
  color: #f0eeff;
  -webkit-font-smoothing: antialiased;
}
input, select, button { font-family: inherit; }
input[type="number"]::-webkit-inner-spin-button { -webkit-appearance: none; }
</style>

<style scoped>
/* ── LAYOUT PRINCIPAL ── */
.layout {
  display: flex;
  min-height: 100dvh;
}

/* Móvil: el sidebar está oculto, el contenido ocupa todo */
.content-area {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

/* ── HEADER ── */
.header {
  padding: 20px 20px 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #1a1a2e;
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-sub { font-size: 11px; color: #6b6b8a; font-weight: 600; letter-spacing: 1px; text-transform: uppercase; margin: 0; }
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 4px 0 0; }

.select-mes {
  background: #1a1228; border: 1.5px solid #3a2a60; border-radius: 10px;
  color: #a78bfa; padding: 8px 12px; font-size: 13px; font-weight: 600; outline: none;
  -webkit-appearance: none; cursor: pointer;
}

/* ── MAIN ── */
.main {
  padding: 16px 16px 100px;
  flex: 1;
}

/* ── TIPOGRAFÍA SECCIONES ── */
.seccion-titulo { font-size: 18px; font-weight: 700; color: #c4b5fd; margin: 0 0 14px; }
.seccion-sub { font-size: 13px; color: #6b6b8a; margin: -10px 0 18px; }

/* ── CARDS ── */
.card { background: #18182a; border: 1px solid #2a2a40; border-radius: 18px; padding: 16px; margin-bottom: 14px; }
.card--sin-padding { padding: 0 16px; }
.card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.card-titulo { font-size: 14px; font-weight: 700; color: #c4b5fd; margin: 0; }
.btn-ver-todos { background: none; border: none; color: #7c3aed; font-size: 13px; font-weight: 600; cursor: pointer; padding: 0; }

/* ── ESTADOS ── */
.error-state { text-align: center; color: #f87171; padding: 40px 20px; }
.error-msg { font-size: 12px; color: #6b6b8a; margin-top: 4px; }
.empty-state { text-align: center; color: #6b6b8a; padding: 60px 20px; }
.empty-icon { font-size: 48px; display: block; margin-bottom: 12px; }

/* ── GRIDS MÓVIL (columna única) ── */
.resumen-grid { display: flex; flex-direction: column; }
.presupuesto-grid { display: grid; grid-template-columns: 1fr; gap: 0; }

/* ── FORM ── */
.form-card { max-width: 560px; }
.segmento { display: flex; gap: 8px; margin-bottom: 16px; }
.seg-btn {
  flex: 1; padding: 10px; border: 1.5px solid #2a2a40; background: transparent;
  color: #8888aa; border-radius: 12px; font-size: 14px; font-weight: 600; cursor: pointer;
}
.seg-btn--gasto { background: #2d1a1a; border-color: #ef4444; color: #f87171; }
.seg-btn--ingreso { background: #1a2d1a; border-color: #22c55e; color: #4ade80; }
.form { display: flex; flex-direction: column; gap: 10px; }
.field-label { font-size: 12px; color: #8888aa; font-weight: 600; letter-spacing: 0.5px; display: block; margin-bottom: -4px; }
.input {
  width: 100%; background: #0f0f18; border: 1.5px solid #2a2a40; border-radius: 12px;
  padding: 13px 16px; font-size: 15px; color: #f0eeff; outline: none;
}
.input:focus { border-color: #7c3aed; }
.select { -webkit-appearance: none; cursor: pointer; }
.form-row { display: flex; gap: 10px; }
.form-col { flex: 1; display: flex; flex-direction: column; gap: 6px; }
.btn-primary {
  background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff; border: none;
  border-radius: 14px; padding: 14px 24px; font-size: 15px; font-weight: 600;
  cursor: pointer; width: 100%; margin-top: 8px; -webkit-tap-highlight-color: transparent;
}
.btn-primary:active { opacity: 0.85; transform: scale(0.98); }

/* ── DESKTOP ≥ 768px ── */
@media (min-width: 768px) {
  .main {
    padding: 24px 32px 40px;
    max-width: 1100px;
    width: 100%;
  }

  /* Resumen: balance+categorías a la izquierda, movimientos recientes a la derecha */
  .resumen-grid {
    flex-direction: row;
    gap: 24px;
    align-items: flex-start;
  }
  .resumen-col-izq { flex: 1; min-width: 0; }
  .resumen-col-der { width: 360px; flex-shrink: 0; }

  /* Presupuesto: 2 columnas */
  .presupuesto-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }
}

/* ── LARGE DESKTOP ≥ 1280px ── */
@media (min-width: 1280px) {
  .main { padding: 32px 48px 40px; }

  .resumen-col-der { width: 400px; }

  /* Presupuesto: 3 columnas */
  .presupuesto-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
