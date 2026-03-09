// server/utils/storage.ts
// In-memory storage — replace with DB (PostgreSQL + EF Core) in production

import type { Transaccion, Presupuesto, Account, Family } from '#shared/types'

const hoy = new Date().toISOString().slice(0, 10)
const mesActual = hoy.slice(0, 7)
const now = new Date().toISOString()

// ── Users ──

interface StoredUser {
  id: string
  name: string
  email: string
  passwordHash: string
  createdAt: string
}

const users: StoredUser[] = [
  { id: 'u1', name: 'Demo User', email: 'demo@familia.cl', passwordHash: 'demo123', createdAt: now },
]

export function getUserByEmail(email: string): StoredUser | undefined {
  return users.find(u => u.email === email)
}

export function addUser(input: { name: string; email: string; password: string }): StoredUser {
  const user: StoredUser = {
    id: crypto.randomUUID(),
    name: input.name,
    email: input.email,
    passwordHash: input.password, // In production: bcrypt hash
    createdAt: now,
  }
  users.push(user)
  return user
}

// ── Families ──

const families: Family[] = [
  { id: 'f1', name: 'Mi Familia', ownerUserId: 'u1', createdAt: now },
]

export function getFamilyByUserId(userId: string): Family | null {
  return families.find(f => f.ownerUserId === userId) ?? null
}

// ── Accounts ──

const accounts: Account[] = [
  { id: 'a1', familyId: 'f1', name: 'Cuenta Corriente', type: 'bank', balance: 1500000, createdAt: now },
  { id: 'a2', familyId: 'f1', name: 'Efectivo', type: 'cash', balance: 50000, createdAt: now },
  { id: 'a3', familyId: 'f1', name: 'Ahorro', type: 'savings', balance: 800000, createdAt: now },
]

export function getAccounts(): Account[] {
  return accounts
}

export function addAccount(input: { name: string; type: Account['type'] }): Account {
  const account: Account = {
    id: crypto.randomUUID(),
    familyId: 'f1',
    name: input.name,
    type: input.type,
    balance: 0,
    createdAt: now,
  }
  accounts.push(account)
  return account
}

export function deleteAccount(id: string): boolean {
  const idx = accounts.findIndex(a => a.id === id)
  if (idx === -1) return false
  accounts.splice(idx, 1)
  return true
}

// ── Transactions ──

const transacciones: Transaccion[] = [
  { id: '1', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'ingreso', categoria: 'sueldo', monto: 1200000, currency: 'CLP', descripcion: 'Sueldo Mamá', fecha: `${mesActual}-01`, creadoEn: now },
  { id: '2', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'ingreso', categoria: 'sueldo', monto: 950000, currency: 'CLP', descripcion: 'Sueldo Papá', fecha: `${mesActual}-01`, creadoEn: now },
  { id: '3', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'gasto', categoria: 'alimentacion', monto: 180000, currency: 'CLP', descripcion: 'Supermercado', fecha: `${mesActual}-03`, creadoEn: now },
  { id: '4', familyId: 'f1', accountId: 'a2', userId: 'u1', tipo: 'gasto', categoria: 'transporte', monto: 60000, currency: 'CLP', descripcion: 'Bencina', fecha: `${mesActual}-05`, creadoEn: now },
  { id: '5', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'gasto', categoria: 'educacion', monto: 120000, currency: 'CLP', descripcion: 'Mensualidad colegio', fecha: `${mesActual}-05`, creadoEn: now },
  { id: '6', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'gasto', categoria: 'hogar', monto: 85000, currency: 'CLP', descripcion: 'Cuenta de luz', fecha: `${mesActual}-07`, creadoEn: now },
  { id: '7', familyId: 'f1', accountId: 'a1', userId: 'u1', tipo: 'gasto', categoria: 'entretenimiento', monto: 25000, currency: 'CLP', descripcion: 'Streaming', fecha: `${mesActual}-08`, creadoEn: now },
]

export function getTransacciones(): Transaccion[] {
  return transacciones
}

export function getTransaccionesByMes(mes: string): Transaccion[] {
  return transacciones.filter(t => t.fecha.startsWith(mes))
}

export function getTransaccionById(id: string): Transaccion | undefined {
  return transacciones.find(t => t.id === id)
}

export function addTransaccion(tx: Transaccion): Transaccion {
  transacciones.unshift(tx)
  return tx
}

export function deleteTransaccion(id: string): boolean {
  const idx = transacciones.findIndex(t => t.id === id)
  if (idx === -1) return false
  transacciones.splice(idx, 1)
  return true
}

// ── Budgets ──

const presupuestos: Presupuesto[] = [
  { id: 'p1', familyId: 'f1', categoria: 'alimentacion', limite: 300000, mes: mesActual },
  { id: 'p2', familyId: 'f1', categoria: 'transporte', limite: 100000, mes: mesActual },
  { id: 'p3', familyId: 'f1', categoria: 'hogar', limite: 150000, mes: mesActual },
  { id: 'p4', familyId: 'f1', categoria: 'educacion', limite: 150000, mes: mesActual },
  { id: 'p5', familyId: 'f1', categoria: 'entretenimiento', limite: 50000, mes: mesActual },
]

export function getPresupuestosByMes(mes: string): Presupuesto[] {
  return presupuestos.filter(p => p.mes === mes)
}

export function upsertPresupuesto(input: { categoria: string; limite: number; mes: string }): Presupuesto {
  const idx = presupuestos.findIndex(
    p => p.categoria === input.categoria && p.mes === input.mes,
  )
  if (idx >= 0) {
    presupuestos[idx]!.limite = input.limite
    return presupuestos[idx]!
  }
  const presupuesto: Presupuesto = {
    id: crypto.randomUUID(),
    familyId: 'f1',
    categoria: input.categoria as Presupuesto['categoria'],
    limite: input.limite,
    mes: input.mes,
  }
  presupuestos.push(presupuesto)
  return presupuesto
}
