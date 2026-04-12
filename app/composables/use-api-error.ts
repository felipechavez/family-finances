// app/composables/use-api-error.ts
import type { Ref } from 'vue'

interface ApiErrorData {
  error?: string
}

/**
 * Extracts the localized error message from a useFetch FetchError.
 * The backend returns { error: "..." } for all error responses.
 * Falls back to the generic common.error translation when the body lacks a message.
 */
export function useApiError(error: Ref<Error | null | undefined>) {
  const { t } = useI18n()

  const message = computed<string | null>(() => {
    if (!error.value) return null
    const data = (error.value as Error & { data?: ApiErrorData }).data
    return data?.error ?? data ?? t('common.error')
  })

  return { message }
}
