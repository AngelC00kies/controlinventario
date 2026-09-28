import axios from 'axios'

/**
 * Cliente HTTP de la API ControlInventario.
 * - Usa el proxy de Vite en desarrollo (/api -> http://localhost:5087).
 * - Adjunta el token JWT en cada petición.
 * - Devuelve errores con el mensaje en español del backend ({ mensaje }).
 */
const cliente = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

// Interceptor de petición: agrega el Bearer token si existe.
cliente.interceptors.request.use((config) => {
  const token = localStorage.getItem('token_controlinventario')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Interceptor de respuesta: normaliza el mensaje de error y cierra sesión
// automáticamente si el token expiró (401).
cliente.interceptors.response.use(
  (respuesta) => respuesta,
  (error) => {
    const mensaje =
      error.response?.data?.mensaje ||
      'No se pudo conectar con el servidor. Verifique que la API esté en ejecución.'

    if (error.response?.status === 401) {
      localStorage.removeItem('token_controlinventario')
      localStorage.removeItem('usuario_controlinventario')
      if (!window.location.pathname.startsWith('/inicio-sesion')) {
        window.location.href = '/inicio-sesion'
      }
    }

    return Promise.reject(new Error(mensaje))
  },
)

export default cliente
