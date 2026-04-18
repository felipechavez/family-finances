// app/composables/use-formato.ts

export function useFormato() {
  function formatCLP(monto: number): string {
    return new Intl.NumberFormat('es-CL', {
      style: 'currency',
      currency: 'CLP',
      maximumFractionDigits: 0,
    }).format(monto)
  }

  function formatMesLabel(ym: string): string {
    const partes = ym.split('-')
    const anio = partes[0] ?? ''
    const mes = partes[1] ?? ''
    const meses = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']
    const nombreMes = meses[parseInt(mes, 10) - 1] ?? mes
    return `${nombreMes} ${anio}`
  }

  function pct(valor: number, total: number): number {
    if (total <= 0) return 0
    return Math.min(100, Math.round((valor / total) * 100))
  }

  return { formatCLP, formatMesLabel, pct }
}
