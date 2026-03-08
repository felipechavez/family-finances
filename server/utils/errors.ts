// server/utils/errors.ts
// Re-usable error helpers — auto-imported by Nuxt in server context

export function notFound(recurso: string, id?: string): never {
  throw createError({
    statusCode: 404,
    statusMessage: 'Not Found',
    message: id ? `${recurso} con ID "${id}" no encontrado` : `${recurso} no encontrado`,
  })
}

export function badRequest(mensaje: string, data?: unknown): never {
  throw createError({
    statusCode: 400,
    statusMessage: 'Bad Request',
    message: mensaje,
    data,
  })
}

export function internalError(mensaje = 'Error interno del servidor'): never {
  throw createError({
    statusCode: 500,
    statusMessage: 'Internal Server Error',
    message: mensaje,
  })
}
