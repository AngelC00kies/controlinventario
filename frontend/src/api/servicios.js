import cliente from './cliente'

/**
 * Servicios de la API: un método por endpoint del backend.
 * Todos devuelven directamente el `data` de la respuesta axios.
 */

/* ---------------------------------- Auth --------------------------------- */
export const iniciarSesion = (datos) =>
  cliente.post('/auth/login', datos).then((r) => r.data)

export const obtenerPerfil = () => cliente.get('/auth/yo').then((r) => r.data)

/* ------------------------------- Productos ------------------------------- */
export const listarProductos = (filtros = {}) =>
  cliente.get('/productos', { params: filtros }).then((r) => r.data)

export const obtenerProducto = (id) =>
  cliente.get(`/productos/${id}`).then((r) => r.data)

export const obtenerStockProducto = (id) =>
  cliente.get(`/productos/${id}/stock`).then((r) => r.data)

export const crearProducto = (datos) =>
  cliente.post('/productos', datos).then((r) => r.data)

export const actualizarProducto = (id, datos) =>
  cliente.put(`/productos/${id}`, datos).then((r) => r.data)

export const eliminarProducto = (id) =>
  cliente.delete(`/productos/${id}`).then((r) => r.data)

/* ------------------------------- Categorías ------------------------------ */
export const listarCategorias = (incluirInactivas = false) =>
  cliente.get('/categorias', { params: { incluirInactivas } }).then((r) => r.data)

export const crearCategoria = (datos) =>
  cliente.post('/categorias', datos).then((r) => r.data)

export const actualizarCategoria = (id, datos) =>
  cliente.put(`/categorias/${id}`, datos).then((r) => r.data)

export const eliminarCategoria = (id) =>
  cliente.delete(`/categorias/${id}`).then((r) => r.data)

/* ------------------------------ Proveedores ------------------------------ */
export const listarProveedores = (incluirInactivos = false) =>
  cliente.get('/proveedores', { params: { incluirInactivos } }).then((r) => r.data)

export const crearProveedor = (datos) =>
  cliente.post('/proveedores', datos).then((r) => r.data)

export const actualizarProveedor = (id, datos) =>
  cliente.put(`/proveedores/${id}`, datos).then((r) => r.data)

export const eliminarProveedor = (id) =>
  cliente.delete(`/proveedores/${id}`).then((r) => r.data)

/* -------------------------------- Almacenes ------------------------------ */
export const listarAlmacenes = (incluirInactivos = false) =>
  cliente.get('/almacenes', { params: { incluirInactivos } }).then((r) => r.data)

export const crearAlmacen = (datos) =>
  cliente.post('/almacenes', datos).then((r) => r.data)

export const actualizarAlmacen = (id, datos) =>
  cliente.put(`/almacenes/${id}`, datos).then((r) => r.data)

export const eliminarAlmacen = (id) =>
  cliente.delete(`/almacenes/${id}`).then((r) => r.data)

/* -------------------------------- Usuarios ------------------------------- */
export const listarUsuarios = () => cliente.get('/usuarios').then((r) => r.data)

export const listarRoles = () =>
  cliente.get('/usuarios/roles').then((r) => r.data)

export const crearUsuario = (datos) =>
  cliente.post('/usuarios', datos).then((r) => r.data)

export const actualizarUsuario = (id, datos) =>
  cliente.put(`/usuarios/${id}`, datos).then((r) => r.data)

export const eliminarUsuario = (id) =>
  cliente.delete(`/usuarios/${id}`).then((r) => r.data)

/* ------------------------------ Movimientos ------------------------------ */
export const buscarMovimientos = (filtros = {}) =>
  cliente.get('/movimientos', { params: filtros }).then((r) => r.data)

export const obtenerMovimiento = (id) =>
  cliente.get(`/movimientos/${id}`).then((r) => r.data)

export const crearMovimiento = (datos) =>
  cliente.post('/movimientos', datos).then((r) => r.data)

/* -------------------------------- Alertas -------------------------------- */
export const obtenerAlertasStock = () =>
  cliente.get('/alertas/stock-bajo').then((r) => r.data)

/* -------------------------------- Reportes ------------------------------- */
export const reporteStockActual = (filtros = {}) =>
  cliente.get('/reportes/stock-actual', { params: filtros }).then((r) => r.data)

export const reporteMovimientos = (filtros = {}) =>
  cliente.get('/reportes/movimientos', { params: filtros }).then((r) => r.data)

export const reporteStockCritico = () =>
  cliente.get('/reportes/stock-critico').then((r) => r.data)

/* -------------------------------- Dashboard ------------------------------ */
export const obtenerResumen = () =>
  cliente.get('/dashboard/resumen').then((r) => r.data)
