/**
 * Exportación de reportes a CSV (compatible con Excel) e impresión.
 */

/**
 * Descarga un arreglo de objetos como archivo CSV con separador punto y coma
 * y codificación UTF-8 con BOM (para que Excel muestre bien los acentos).
 *
 * @param {Array<Object>} filas    Datos a exportar.
 * @param {Array<{clave: string, titulo: string}>} columnas Definición de columnas.
 * @param {string} nombreArchivo   Nombre base del archivo (sin extensión).
 */
export function exportarCSV(filas, columnas, nombreArchivo) {
  const escapar = (valor) => {
    const texto = valor === null || valor === undefined ? '' : String(valor)
    // Se reemplazan comillas y se envuelve entre comillas si contiene separadores.
    const limpio = texto.replace(/"/g, '""')
    return /[";\n]/.test(limpio) ? `"${limpio}"` : limpio
  }

  const encabezado = columnas.map((c) => escapar(c.titulo)).join(';')
  const cuerpo = filas.map((fila) =>
    columnas.map((c) => escapar(fila[c.clave])).join(';'),
  )

  const contenido = [encabezado, ...cuerpo].join('\r\n')
  const bom = '\uFEFF'
  const blob = new Blob([bom + contenido], { type: 'text/csv;charset=utf-8;' })

  const enlace = document.createElement('a')
  enlace.href = URL.createObjectURL(blob)
  enlace.download = `${nombreArchivo}_${new Date().toISOString().slice(0, 10)}.csv`
  enlace.click()
  URL.revokeObjectURL(enlace.href)
}

/** Abre el diálogo de impresión del navegador (para PDF). */
export function imprimir() {
  window.print()
}
