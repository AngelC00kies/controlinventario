import { defineStore } from 'pinia'
import { iniciarSesion as apiIniciarSesion, obtenerPerfil } from '../api/servicios'

const CLAVE_TOKEN = 'token_controlinventario'
const CLAVE_USUARIO = 'usuario_controlinventario'

/**
 * Store de autenticación: mantiene el token JWT y el perfil del usuario,
 * persistidos en localStorage para sobrevivir a recargas de página.
 */
export const useAutenticacion = defineStore('autenticacion', {
  state: () => ({
    token: localStorage.getItem(CLAVE_TOKEN) || null,
    usuario: JSON.parse(localStorage.getItem(CLAVE_USUARIO) || 'null'),
  }),

  getters: {
    /** true si hay sesión activa. */
    autenticado: (state) => Boolean(state.token && state.usuario),

    /** Rol del usuario autenticado ("Administrador", "Usuario", "Auditor"). */
    rol: (state) => state.usuario?.rol || '',

    /** El usuario puede crear/editar/eliminar (Administrador). */
    esAdministrador: (state) => state.usuario?.rol === 'Administrador',

    /** El usuario puede registrar movimientos (Administrador o Usuario). */
    puedeRegistrarMovimientos: (state) =>
      ['Administrador', 'Usuario'].includes(state.usuario?.rol),
  },

  actions: {
    /** Autentica contra la API y guarda token + perfil. */
    async iniciarSesion(datos) {
      const respuesta = await apiIniciarSesion(datos)
      this.token = respuesta.token
      this.usuario = {
        idUsuario: respuesta.idUsuario,
        nombreUsuario: respuesta.nombreUsuario,
        nombreCompleto: respuesta.nombreCompleto,
        correo: respuesta.correo,
        rol: respuesta.rol,
      }
      localStorage.setItem(CLAVE_TOKEN, respuesta.token)
      localStorage.setItem(CLAVE_USUARIO, JSON.stringify(this.usuario))
      return respuesta
    },

    /** Refresca el perfil desde el backend (opcional). */
    async refrescarPerfil() {
      const respuesta = await obtenerPerfil()
      this.usuario = {
        idUsuario: respuesta.idUsuario,
        nombreUsuario: respuesta.nombreUsuario,
        nombreCompleto: respuesta.nombreCompleto,
        correo: respuesta.correo,
        rol: respuesta.rol,
      }
      localStorage.setItem(CLAVE_USUARIO, JSON.stringify(this.usuario))
    },

    /** Cierra la sesión y limpia el almacenamiento local. */
    cerrarSesion() {
      this.token = null
      this.usuario = null
      localStorage.removeItem(CLAVE_TOKEN)
      localStorage.removeItem(CLAVE_USUARIO)
    },
  },
})
