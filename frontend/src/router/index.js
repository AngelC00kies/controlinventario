import { createRouter, createWebHistory } from 'vue-router'
import { useAutenticacion } from '../stores/autenticacion'

/**
 * Rutas de ControlInventario.
 * - meta.requiereAuth: exige sesión activa.
 * - meta.roles: lista de roles con acceso permitido.
 */
const rutas = [
  {
    path: '/inicio-sesion',
    name: 'inicio-sesion',
    component: () => import('../vistas/InicioSesion.vue'),
    meta: { publica: true },
  },
  {
    path: '/',
    component: () => import('../layouts/LayoutPrincipal.vue'),
    meta: { requiereAuth: true },
    children: [
      {
        path: '',
        name: 'panel',
        component: () => import('../vistas/PanelPrincipal.vue'),
        meta: { titulo: 'Panel principal' },
      },
      {
        path: 'productos',
        name: 'productos',
        component: () => import('../vistas/Productos.vue'),
        meta: { titulo: 'Productos' },
      },
      {
        path: 'categorias',
        name: 'categorias',
        component: () => import('../vistas/Categorias.vue'),
        meta: { titulo: 'Categorías' },
      },
      {
        path: 'proveedores',
        name: 'proveedores',
        component: () => import('../vistas/Proveedores.vue'),
        meta: { titulo: 'Proveedores' },
      },
      {
        path: 'almacenes',
        name: 'almacenes',
        component: () => import('../vistas/Almacenes.vue'),
        meta: { titulo: 'Almacenes' },
      },
      {
        path: 'movimientos',
        name: 'movimientos',
        component: () => import('../vistas/Movimientos.vue'),
        meta: { titulo: 'Movimientos' },
      },
      {
        path: 'reportes',
        name: 'reportes',
        component: () => import('../vistas/Reportes.vue'),
        meta: { titulo: 'Reportes' },
      },
      {
        path: 'usuarios',
        name: 'usuarios',
        component: () => import('../vistas/Usuarios.vue'),
        meta: { titulo: 'Usuarios', roles: ['Administrador'] },
      },
    ],
  },
  {
    // Ruta comodín: cualquier dirección desconocida va al panel.
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes: rutas,
})

// Guardia global de navegación.
router.beforeEach((to) => {
  const autenticacion = useAutenticacion()

  // Rutas privadas sin sesión -> login.
  if (to.meta.requiereAuth && !autenticacion.autenticado) {
    return { name: 'inicio-sesion' }
  }

  // Ruta de login con sesión activa -> panel.
  if (to.meta.publica && autenticacion.autenticado) {
    return { name: 'panel' }
  }

  // Control de acceso por rol.
  if (to.meta.roles && !to.meta.roles.includes(autenticacion.rol)) {
    return { name: 'panel' }
  }

  return true
})

// Título de la pestaña según la vista.
router.afterEach((to) => {
  document.title = to.meta.titulo
    ? `${to.meta.titulo} | ControlInventario`
    : 'ControlInventario'
})

export default router
