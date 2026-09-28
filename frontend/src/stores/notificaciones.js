import { defineStore } from 'pinia'

/**
 * Store de notificaciones (toasts): avisos de éxito/error que se muestran
 * como cartel flotante en la esquina superior derecha.
 */
export const useNotificaciones = defineStore('notificaciones', {
  state: () => ({
    avisos: [],
    _contador: 0,
  }),

  actions: {
    /** Muestra un aviso. tipo: 'exito' | 'error' | 'informacion'. */
    mostrar(mensaje, tipo = 'exito', duracionMs = 4500) {
      const id = ++this._contador
      this.avisos.push({ id, mensaje, tipo })
      setTimeout(() => this.cerrar(id), duracionMs)
    },

    exito(mensaje) {
      this.mostrar(mensaje, 'exito')
    },

    error(mensaje) {
      this.mostrar(mensaje, 'error', 6000)
    },

    cerrar(id) {
      this.avisos = this.avisos.filter((a) => a.id !== id)
    },
  },
})
