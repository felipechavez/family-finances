// server/utils/storage.ts
// In-memory storage — replace with DB (Drizzle, Prisma, etc.) as needed

import type { StorageState } from '~~/server/types/internal'
import type { Transaccion, Presupuesto } from '#shared/types'

const hoy = new Date().toISOString().slice(0, 10)
const mesActual = hoy.slice(0, 7)

const state: StorageState = {
  transacciones: [
    { id: '1', tipo: 'ingreso', categoria: 'sueldo', monto: 1200000, descripcion: 'Sueldo Mamá', fecha: `${mesActual}-01`, miembro: 'Mamá', creadoEn: new Date().toISOString() },
    { id: '2', tipo: 'ingreso', categoria: 'sueldo', monto: 950000, descripcion: 'Sueldo Papá', fecha: `${mesActual}-01`, miembro: 'Papá', creadoEn: new Date().toISOString() },
    { id: '3', tipo: 'gasto', categoria: 'alimentacion', monto: 180000, descripcion: 'Supermercado', fecha: `${mesActual}-03`, miembro: 'Mamá', creadoEn: new Date().toISOString() },
    { id: '4', tipo: 'gasto', categoria: 'transporte', monto: 60000, descripcion: 'Bencina', fecha: `${mesActual}-05`, miembro: 'Papá', creadoEn: new Date().toISOString() },
    { id: '5', tipo: 'gasto', categoria: 'educacion', monto: 120000, descripcion: 'Mensualidad colegio', fecha: `${mesActual}-05`, miembro: 'Hijo/a 1', creadoEn: new Date().toISOString() },
    { id: '6', tipo: 'gasto', categoria: 'hogar', monto: 85000, descripcion: 'Cuenta de luz', fecha: `${mesActual}-07`, miembro: 'Papá', creadoEn: new Date().toISOString() },
    { id: '7', tipo: 'gasto', categoria: 'entretenimiento', monto: 25000, descripcion: 'Streaming', fecha: `${mesActual}-08`, miembro: 'Todos', creadoEn: new Date().toISOString() },
  ],
  presupuestos: [
    { categoria: 'alimentacion', limite: 300000, mes: mesActual },
    { categoria: 'transporte', limite: 100000, mes: mesActual },
    { categoria: 'hogar', limite: 150000, mes: mesActual },
    { categoria: 'educacion', limite: 150000, mes: mesActual },
    { categoria: 'entretenimiento', limite: 50000, mes: mesActual },
  ],
}

export function getTransacciones(): Transaccion[] {
  return state.transacciones
}

export function getTransaccionesByMes(mes: string): Transaccion[] {
  return state.transacciones.filter(t => t.fecha.startsWith(mes))
}

export function getTransaccionById(id: string): Transaccion | undefined {
  return state.transacciones.find(t => t.id === id)
}

export function addTransaccion(tx: Transaccion): Transaccion {
  state.transacciones.unshift(tx)
  return tx
}

export function deleteTransaccion(id: string): boolean {
  const idx = state.transacciones.findIndex(t => t.id === id)
  if (idx === -1) return false
  state.transacciones.splice(idx, 1)
  return true
}

export function getPresupuestosByMes(mes: string): Presupuesto[] {
  return state.presupuestos.filter(p => p.mes === mes)
}

export function upsertPresupuesto(presupuesto: Presupuesto): Presupuesto {
  const idx = state.presupuestos.findIndex(
    p => p.categoria === presupuesto.categoria && p.mes === presupuesto.mes,
  )
  if (idx >= 0) {
    state.presupuestos[idx] = presupuesto
  } else {
    state.presupuestos.push(presupuesto)
  }
  return presupuesto
}
