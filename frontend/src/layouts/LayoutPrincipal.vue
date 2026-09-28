<script setup>
/**
 * Layout autenticado: barra lateral de navegación, encabezado con alertas de
 * stock bajo, notificaciones flotantes y pie de página.
 */
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  Boxes,
  LogOut,
  Menu,
  Package,
  Tags,
  Truck,
  Warehouse,
  ArrowLeftRight,
  FileBarChart,
  Users,
  LayoutDashboard,
  Bell,
  TriangleAlert,
} from 'lucide-vue-next'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import { obtenerAlertasStock } from '../api/servicios'
import ContenedorNotificaciones from '../componentes/ContenedorNotificaciones.vue'

const ruta = useRoute()
const navegador = useRouter()
const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const menuAbierto = ref(false)
const alertasAbiertas = ref(false)
const alertas = ref([])

/** Enlaces de navegación: icono, nombre y disponibilidad por rol. */
const enlaces = [
  { nombre: 'panel', etiqueta: 'Panel principal', icono: LayoutDashboard },
  { nombre: 'productos', etiqueta: 'Productos', icono: Package },
  { nombre: 'categorias', etiqueta: 'Categorías', icono: Tags },
  { nombre: 'proveedores', etiqueta: 'Proveedores', icono: Truck },
  { nombre: 'almacenes', etiqueta: 'Almacenes', icono: Warehouse },
  { nombre: 'movimientos', etiqueta: 'Movimientos', icono: ArrowLeftRight },
  { nombre: 'reportes', etiqueta: 'Reportes', icono: FileBarChart },
  { nombre: 'usuarios', etiqueta: 'Usuarios', icono: Users, soloAdministrador: true },
]

const enlacesVisibles = computed(() =>
  enlaces.filter((e) => !e.soloAdministrador || autenticacion.esAdministrador),
)

const tituloActual = computed(() => ruta.meta.titulo || 'Panel principal')

/** Carga las alertas de stock bajo (visibles para todos los roles). */
async function cargarAlertas() {
  try {
    alertas.value = await obtenerAlertasStock()
  } catch {
    alertas.value = []
  }
}

function cerrarSesion() {
  autenticacion.cerrarSesion()
  notificaciones.exito('Sesión cerrada correctamente.')
  navegador.push({ name: 'inicio-sesion' })
}

onMounted(cargarAlertas)
</script>

<template>
  <div class="flex min-h-screen bg-slate-100">
    <!-- ================= Barra lateral ================= -->
    <aside
      :class="[
        'fixed inset-y-0 left-0 z-40 flex w-64 transform flex-col bg-slate-900 text-slate-200 transition-transform duration-200 lg:static lg:translate-x-0',
        menuAbierto ? 'translate-x-0' : '-translate-x-full',
      ]"
    >
      <!-- Marca -->
      <div class="flex items-center gap-3 border-b border-slate-800 px-5 py-5">
        <div class="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-600">
          <Boxes :size="22" class="text-white" />
        </div>
        <div>
          <p class="font-bold leading-tight text-white">ControlInventario</p>
          <p class="text-xs text-slate-400">Gestión de stock</p>
        </div>
      </div>

      <!-- Navegación -->
      <nav class="flex-1 space-y-1 overflow-y-auto px-3 py-4">
        <router-link
          v-for="enlace in enlacesVisibles"
          :key="enlace.nombre"
          :to="{ name: enlace.nombre }"
          :class="[
            'flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition',
            ruta.name === enlace.nombre
              ? 'bg-indigo-600 text-white shadow'
              : 'text-slate-300 hover:bg-slate-800 hover:text-white',
          ]"
          @click="menuAbierto = false"
        >
          <component :is="enlace.icono" :size="18" />
          {{ enlace.etiqueta }}
        </router-link>
      </nav>

      <!-- Usuario y cierre de sesión -->
      <div class="border-t border-slate-800 px-4 py-4">
        <div class="flex items-center gap-3">
          <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-indigo-500 text-sm font-bold text-white">
            {{ (autenticacion.usuario?.nombreCompleto || '?').charAt(0).toUpperCase() }}
          </div>
          <div class="min-w-0 flex-1">
            <p class="truncate text-sm font-semibold text-white">
              {{ autenticacion.usuario?.nombreCompleto }}
            </p>
            <p class="truncate text-xs text-slate-400">{{ autenticacion.rol }}</p>
          </div>
          <button
            type="button"
            class="rounded-lg p-2 text-slate-400 transition hover:bg-slate-800 hover:text-rose-400"
            title="Cerrar sesión"
            @click="cerrarSesion"
          >
            <LogOut :size="18" />
          </button>
        </div>
      </div>
    </aside>

    <!-- Fondo para cerrar el menú en móvil -->
    <div
      v-if="menuAbierto"
      class="fixed inset-0 z-30 bg-slate-900/50 lg:hidden"
      @click="menuAbierto = false"
    />

    <!-- ================= Contenido ================= -->
    <div class="flex min-w-0 flex-1 flex-col">
      <!-- Encabezado -->
      <header class="sticky top-0 z-20 flex items-center justify-between border-b border-slate-200 bg-white px-4 py-3 shadow-sm lg:px-6">
        <div class="flex items-center gap-3">
          <button
            type="button"
            class="rounded-lg p-2 text-slate-500 transition hover:bg-slate-100 lg:hidden"
            aria-label="Abrir menú"
            @click="menuAbierto = true"
          >
            <Menu :size="20" />
          </button>
          <h1 class="text-lg font-semibold text-slate-800">{{ tituloActual }}</h1>
        </div>

        <div class="flex items-center gap-2">
          <!-- Alertas de stock bajo -->
          <div class="relative">
            <button
              type="button"
              class="relative rounded-lg p-2 text-slate-500 transition hover:bg-slate-100"
              title="Alertas de stock bajo"
              @click="alertasAbiertas = !alertasAbiertas"
            >
              <Bell :size="20" />
              <span
                v-if="alertas.length"
                class="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-rose-500 px-1 text-[10px] font-bold text-white"
              >
                {{ alertas.length }}
              </span>
            </button>

            <!-- Panel de alertas -->
            <div
              v-if="alertasAbiertas"
              class="absolute right-0 z-30 mt-2 w-80 rounded-xl border border-slate-200 bg-white shadow-xl"
            >
              <div class="flex items-center justify-between border-b border-slate-100 px-4 py-3">
                <p class="text-sm font-semibold text-slate-700">Alertas de stock bajo</p>
                <button
                  type="button"
                  class="text-slate-400 hover:text-slate-600"
                  aria-label="Cerrar alertas"
                  @click="alertasAbiertas = false"
                >
                  ✕
                </button>
              </div>
              <div class="max-h-80 overflow-y-auto">
                <p v-if="!alertas.length" class="px-4 py-6 text-center text-sm text-slate-400">
                  No hay productos con stock crítico.
                </p>
                <ul v-else class="divide-y divide-slate-100">
                  <li
                    v-for="alerta in alertas"
                    :key="alerta.idProducto"
                    class="flex items-center gap-3 px-4 py-3"
                  >
                    <TriangleAlert :size="16" class="shrink-0 text-rose-500" />
                    <div class="min-w-0 flex-1">
                      <p class="truncate text-sm font-medium text-slate-700">{{ alerta.nombre }}</p>
                      <p class="text-xs text-slate-400">
                        {{ alerta.codigo }} · Stock {{ alerta.stockActual }} /
                        mínimo {{ alerta.stockMinimo }}
                      </p>
                    </div>
                  </li>
                </ul>
              </div>
              <div class="border-t border-slate-100 px-4 py-2 text-center">
                <router-link
                  :to="{ name: 'reportes' }"
                  class="text-xs font-semibold text-indigo-600 hover:underline"
                  @click="alertasAbiertas = false"
                >
                  Ver reporte de stock crítico
                </router-link>
              </div>
            </div>
          </div>

          <!-- Rol del usuario -->
          <span class="hidden rounded-full bg-indigo-50 px-3 py-1 text-xs font-semibold text-indigo-700 sm:inline">
            {{ autenticacion.rol }}
          </span>
        </div>
      </header>

      <!-- Vista actual -->
      <main class="flex-1 p-4 lg:p-6">
        <router-view />
      </main>

      <!-- Pie -->
      <footer class="border-t border-slate-200 bg-white px-6 py-3 text-center text-xs text-slate-400">
        ControlInventario · Sistema de gestión de inventario · © {{ new Date().getFullYear() }}
      </footer>
    </div>

    <!-- Avisos flotantes -->
    <ContenedorNotificaciones />
  </div>
</template>
