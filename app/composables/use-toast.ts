// app/composables/use-toast.ts
// Exports functions ONLY

import type { ToastMessage } from '~/types/ui'

const toasts = ref<ToastMessage[]>([])

export function useToast() {
  function mostrar(texto: string, tipo: ToastMessage['tipo'] = 'ok'): void {
    const id = crypto.randomUUID()
    toasts.value.push({ id, texto, tipo })
    setTimeout(() => {
      toasts.value = toasts.value.filter(t => t.id !== id)
    }, 2800)
  }

  function ok(texto: string): void { mostrar(texto, 'ok') }
  function error(texto: string): void { mostrar(texto, 'error') }
  function info(texto: string): void { mostrar(texto, 'info') }

  function quitar(id: string): void {
    toasts.value = toasts.value.filter(t => t.id !== id)
  }

  return { toasts: readonly(toasts), mostrar, ok, error, info, quitar }
}
