/**
 * Utilidades de formato: moneda chilena, fechas y estados de stock.
 */

/** Formatea un número como moneda chilena (CLP). */
export function formatoMoneda(valor) {
  return new Intl.NumberFormat('es-CL', {
    style: 'currency',
    currency: 'CLP',
    maximumFractionDigits: 0,
  }).format(valor ?? 0)
}

/** Formatea un número con separador de miles. */
export function formatoNumero(valor) {
  return new Intl.NumberFormat('es-CL').format(valor ?? 0)
}

/** Formatea una fecha ISO como dd-mm-aaaa hh:mm. */
export function formatoFecha(fecha) {
  if (!fecha) return '—'
  const fechaObj = new Date(fecha)
  return new Intl.DateTimeFormat('es-CL', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(fechaObj)
}

/** Formatea una fecha ISO solo con fecha. */
export function formatoFechaCorta(fecha) {
  if (!fecha) return '—'
  return new Intl.DateTimeFormat('es-CL', { dateStyle: 'short' }).format(
    new Date(fecha),
  )
}

/** Devuelve la fecha actual en formato yyyy-MM-dd (para inputs date). */
export function hoyFormatoInput() {
  const hoy = new Date()
  const mes = String(hoy.getMonth() + 1).padStart(2, '0')
  const dia = String(hoy.getDate()).padStart(2, '0')
  return `${hoy.getFullYear()}-${mes}-${dia}`
}
