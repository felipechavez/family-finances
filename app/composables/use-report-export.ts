// app/composables/use-report-export.ts
import type { Transaccion } from '#shared/types'

interface DistribucionItem {
  label: string
  emoji: string
  monto: number
  porcentaje: number
}

export interface ReportData {
  transacciones: Transaccion[]
  totalIngresos: number
  totalGastos: number
  balance: number
  distribucion: DistribucionItem[]
  mes: string
  familyName?: string
}

// ── helpers ──────────────────────────────────────────────────────────────────

function formatCLPRaw(monto: number): string {
  return new Intl.NumberFormat('es-CL', {
    style: 'currency',
    currency: 'CLP',
    maximumFractionDigits: 0,
  }).format(monto)
}

function monthLabel(ym: string): string {
  const [year, month] = ym.split('-')
  const meses = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']
  return `${meses[parseInt(month ?? '1', 10) - 1] ?? month} ${year}`
}

function downloadBlob(content: string, filename: string, mimeType: string) {
  const blob = new Blob(['\uFEFF' + content], { type: mimeType })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  a.click()
  URL.revokeObjectURL(url)
}

// ── composable ────────────────────────────────────────────────────────────────

export function useReportExport() {
  const exporting = ref(false)

  // ── CSV ────────────────────────────────────────────────────────────────────
  function exportCSV(data: ReportData) {
    const lines: string[] = []
    lines.push('Fecha,Tipo,Categoría,Descripción,Monto')
    for (const tx of data.transacciones) {
      const fecha = tx.fecha.slice(0, 10)
      const tipo  = tx.tipo === 'gasto' ? 'Gasto' : 'Ingreso'
      const desc  = `"${tx.descripcion.replace(/"/g, '""')}"`
      lines.push(`${fecha},${tipo},${tx.categoria},${desc},${tx.monto}`)
    }
    lines.push('')
    lines.push('RESUMEN')
    lines.push(`Ingresos,${data.totalIngresos}`)
    lines.push(`Gastos,${data.totalGastos}`)
    lines.push(`Balance,${data.balance}`)
    downloadBlob(lines.join('\n'), `reporte-${data.mes}.csv`, 'text/csv;charset=utf-8;')
  }

  // ── PDF ────────────────────────────────────────────────────────────────────
  async function exportPDF(data: ReportData) {
    exporting.value = true
    try {
      const { default: jsPDF } = await import('jspdf')
      const { default: autoTable } = await import('jspdf-autotable')
      const { totalIngresos, totalGastos, balance, distribucion, mes, familyName } = data
      const periodo = monthLabel(mes)
      const titulo  = familyName ? `${familyName} — ${periodo}` : periodo

      const doc    = new jsPDF({ unit: 'mm', format: 'a4' })
      const pageW  = doc.internal.pageSize.getWidth()
      let y = 18

      // Header
      doc.setFontSize(18)
      doc.setTextColor(60, 40, 120)
      doc.text('Reporte Financiero', pageW / 2, y, { align: 'center' })
      y += 8

      doc.setFontSize(12)
      doc.setTextColor(100, 100, 120)
      doc.text(titulo, pageW / 2, y, { align: 'center' })
      y += 10

      doc.setDrawColor(180, 160, 220)
      doc.line(14, y, pageW - 14, y)
      y += 8

      // Summary table
      doc.setFontSize(13)
      doc.setTextColor(60, 40, 120)
      doc.text('Resumen mensual', 14, y)
      y += 4

      autoTable(doc, {
        startY: y,
        head: [['Concepto', 'Monto']],
        body: [
          ['Ingresos', formatCLPRaw(totalIngresos)],
          ['Gastos',   formatCLPRaw(totalGastos)],
          ['Balance',  formatCLPRaw(balance)],
        ],
        theme: 'striped',
        headStyles:    { fillColor: [124, 58, 237], textColor: 255, fontStyle: 'bold' },
        columnStyles:  { 1: { halign: 'right' } },
        margin:        { left: 14, right: 14 },
        styles:        { fontSize: 11 },
      })
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      y = (doc as any).lastAutoTable.finalY + 12

      // Category breakdown
      if (distribucion.length > 0) {
        doc.setFontSize(13)
        doc.setTextColor(60, 40, 120)
        doc.text('Distribución de gastos', 14, y)
        y += 4

        autoTable(doc, {
          startY: y,
          head: [['Categoría', 'Gastado', '%']],
          body: distribucion.map(d => [d.label, formatCLPRaw(d.monto), `${d.porcentaje}%`]),
          theme: 'striped',
          headStyles:   { fillColor: [124, 58, 237], textColor: 255, fontStyle: 'bold' },
          columnStyles: { 1: { halign: 'right' }, 2: { halign: 'right' } },
          margin:       { left: 14, right: 14 },
          styles:        { fontSize: 11 },
        })
      }

      // Footer on every page
      const pageCount = doc.getNumberOfPages()
      const dateStr   = new Date().toLocaleDateString('es-CL')
      for (let i = 1; i <= pageCount; i++) {
        doc.setPage(i)
        doc.setFontSize(9)
        doc.setTextColor(150, 150, 170)
        const pageH = doc.internal.pageSize.getHeight()
        doc.text(`Generado el ${dateStr} · DomusPay`, 14, pageH - 8)
        doc.text(`${i} / ${pageCount}`, pageW - 14, pageH - 8, { align: 'right' })
      }

      doc.save(`reporte-${mes}.pdf`)
    }
    finally {
      exporting.value = false
    }
  }

  return { exportCSV, exportPDF, exporting }
}
