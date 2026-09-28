<script setup>
/**
 * Centro de reportes: stock actual, movimientos por producto/fecha/usuario
 * y productos con stock crítico. Incluye exportación a CSV e impresión.
 */
import { onMounted, reactive, ref, computed } from 'vue'
import {
  FileBarChart,
  Package,
  ArrowLeftRight,
  TriangleAlert,
  Download,
  Printer,
  RotateCw,
} from 'lucide-vue-next'
import {
  reporteStockActual,
  reporteMovimientos,
  reporteStockCritico,
  listarProductos,
  listarUsuarios,
} from '../api/servicios'
import { useNotificaciones } from '../stores/notificaciones'
import { formatoMoneda, formatoNumero, formatoFecha } from '../utils/formato'
import { exportarCSV, imprimir } from '../utils/exportacion'
import TablaContenedora from '../componentes/TablaContenedora.vue'

const notificaciones = useNotificaciones()

/** Pestañas disponibles del centro de reportes. */
const pestanas = [
  { id: 'stock', etiqueta: 'Stock actual', icono: Package },
  { id: 'movimientos', etiqueta: 'Movimientos', icono: ArrowLeftRight },
  { id: 'critico', etiqueta: 'Stock crítico', icono: TriangleAlert },
]

const pestanaActiva = ref('stock')
const cargando = ref(true)

const filas = ref([])
const productos = ref([])
const usuarios = ref([])

/** Filtros compartidos por pestaña. */
const filtros = reactive({
  idProducto: '',
  fechaDesde: '',
  fechaHasta: '',
  idUsuario: '',
  tipo: '',
})

/** Encabezados y utilidades por pestaña. */
const configuracion = computed(() => {
  switch (pestanaActiva.value) {
    case 'movimientos':
      return {
        nombre: 'movimientos',
        columnas: ['Fecha', 'Tipo', 'Producto', 'Cantidad', 'Precio unit.', 'Usuario', 'Motivo', 'Documento'],
        filas: filas.value.map((m) => [
          formatoFecha(m.fecha),
          m.tipo,
          m.nombreProducto,
          formatoNumero(m.cantidad),
          formatoMoneda(m.precioUnitario),
          m.nombreUsuario,
          m.motivo || '—',
          m.documentoReferencia || '—',
        ]),
        totales: filas.value.reduce((s, m) => s + Number(m.subtotal || 0), 0),
        totalEtiqueta: 'Suma de subtotales',
        alineadasDerecha: [4],
      }
    case 'critico':
      return {
        nombre: 'stock-critico',
        columnas: ['Código', 'Producto', 'Categoría', 'Stock actual', 'Stock mínimo', 'Diferencia'],
        filas: filas.value.map((p) => [
          p.codigo,
          p.nombre,
          p.nombreCategoria,
          `${formatoNumero(p.stockActual)} ${p.unidadMedida}`,
          `${formatoNumero(p.stockMinimo)} ${p.unidadMedida}`,
          `${formatoNumero(p.diferencia)} ${p.unidadMedida}`,
        ]),
        totales: null,
        totalEtiqueta: '',
        alineadasDerecha: [3, 4, 5],
      }
    default:
      return {
        nombre: 'stock-actual',
        columnas: ['Código', 'Producto', 'Categoría', 'Almacén', 'Cantidad', 'P. compra', 'Valor'],
        filas: filas.value.map((s) => [
          s.codigoProducto,
          s.nombreProducto,
          s.nombreCategoria,
          s.nombreAlmacen,
          formatoNumero(s.cantidad),
          formatoMoneda(s.precioCompra),
          formatoMoneda(s.valor),
        ]),
        totales: filas.value.reduce((s, f) => s + Number(f.valor || 0), 0),
        totalEtiqueta: 'Valor total del stock listado',
        alineadasDerecha: [4, 5, 6],
      }
  }
})

const encabezados = computed(() => configuracion.value.columnas)
const totalReporte = computed(() => configuracion.value.totales)

/** Indica si la columna de la posición indicada se alinea a la derecha. */
function derecha(indice) {
  return configuracion.value.alineadasDerecha.includes(indice)
}

async function cargar() {
  cargando.value = true
  try {
    if (pestanaActiva.value === 'movimientos') {
      filas.value = await reporteMovimientos({
        idProducto: filtros.idProducto || undefined,
        fechaDesde: filtros.fechaDesde || undefined,
        fechaHasta: filtros.fechaHasta || undefined,
        idUsuario: filtros.idUsuario || undefined,
        tipo: filtros.tipo || undefined,
      })
    } else if (pestanaActiva.value === 'critico') {
      filas.value = await reporteStockCritico()
    } else {
      filas.value = await reporteStockActual()
    }
  } catch (e) {
    notificaciones.error(e.message)
    filas.value = []
  } finally {
    cargando.value = false
  }
}

async function cambiarPestana(id) {
  pestanaActiva.value = id
  await cargar()
}

function exportar() {
  if (!filas.value.length) {
    notificaciones.error('No hay datos para exportar.')
    return
  }
  // exportarCSV espera objetos con columnas [{ clave, titulo }]
  const columnas = configuracion.value.columnas.map((titulo, i) => ({
    clave: `c${i}`,
    titulo,
  }))
  const filasExportar = configuracion.value.filas.map((fila) =>
    Object.fromEntries(fila.map((valor, i) => [`c${i}`, valor])),
  )
  exportarCSV(filasExportar, columnas, `reporte-${configuracion.value.nombre}`)
  notificaciones.exito('Reporte exportado a CSV correctamente.')
}

function imprimirReporte() {
  if (!filas.value.length) {
    notificaciones.error('No hay datos para imprimir.')
    return
  }
  imprimir()
}

function limpiarFiltros() {
  Object.assign(filtros, {
    idProducto: '',
    fechaDesde: '',
    fechaHasta: '',
    idUsuario: '',
    tipo: '',
  })
  cargar()
}

onMounted(async () => {
  // Listas auxiliares para los filtros de movimientos
  try {
    const [prods, usus] = await Promise.all([listarProductos(), listarUsuarios().catch(() => [])])
    productos.value = prods
    usuarios.value = usus
  } catch {
    /* los filtros auxiliares son opcionales */
  }
  await cargar()
})
</script>

<template>
  <div class="space-y-4">
    <!-- ================= Pestañas ================= -->
    <div class="flex flex-wrap gap-2">
      <button
        v-for="pestana in pestanas"
        :key="pestana.id"
        type="button"
        :class="[
          'flex items-center gap-2 rounded-lg px-4 py-2.5 text-sm font-semibold transition',
          pestanaActiva === pestana.id
            ? 'bg-indigo-600 text-white shadow'
            : 'border border-slate-300 bg-white text-slate-600 hover:bg-slate-50',
        ]"
        @click="cambiarPestana(pestana.id)"
      >
        <component :is="pestana.icono" :size="16" />
        {{ pestana.etiqueta }}
      </button>
    </div>

    <!-- ================= Filtros y acciones ================= -->
    <div
      class="flex flex-col gap-3 rounded-xl border border-slate-200 bg-white p-4 shadow-sm lg:flex-row lg:items-end lg:justify-between"
    >
      <!-- Filtros según pestaña -->
      <div class="flex flex-1 flex-wrap items-end gap-3">
        <template v-if="pestanaActiva === 'movimientos'">
          <div>
            <label class="mb-1 block text-xs font-medium text-slate-500">Producto</label>
            <select
              v-model="filtros.idProducto"
              class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
              @change="cargar"
            >
              <option value="">Todos</option>
              <option
                v-for="producto in productos"
                :key="producto.idProducto"
                :value="producto.idProducto"
              >
                {{ producto.codigo }} — {{ producto.nombre }}
              </option>
            </select>
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-slate-500">Tipo</label>
            <select
              v-model="filtros.tipo"
              class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
              @change="cargar"
            >
              <option value="">Todos</option>
              <option value="Entrada">Entrada</option>
              <option value="Salida">Salida</option>
            </select>
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-slate-500">Desde</label>
            <input
              v-model="filtros.fechaDesde"
              type="date"
              class="rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500"
              @change="cargar"
            />
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-slate-500">Hasta</label>
            <input
              v-model="filtros.fechaHasta"
              type="date"
              class="rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500"
              @change="cargar"
            />
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-slate-500">Usuario responsable</label>
            <select
              v-model="filtros.idUsuario"
              class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
              @change="cargar"
            >
              <option value="">Todos</option>
              <option
                v-for="usuario in usuarios"
                :key="usuario.idUsuario"
                :value="usuario.idUsuario"
              >
                {{ usuario.nombreCompleto }}
              </option>
            </select>
          </div>
        </template>

        <p v-else class="text-sm text-slate-500">
          {{
            pestanaActiva === 'stock'
              ? 'Valorización del inventario actual por almacén.'
              : 'Productos que se encuentran por debajo del stock mínimo definido.'
          }}
        </p>

        <button
          type="button"
          class="rounded-lg border border-slate-300 bg-white p-2 text-slate-500 transition hover:bg-slate-50"
          title="Limpiar filtros y actualizar"
          @click="limpiarFiltros"
        >
          <RotateCw :size="16" />
        </button>
      </div>

      <!-- Acciones -->
      <div class="flex items-center gap-2">
        <button
          type="button"
          class="flex items-center gap-2 rounded-lg border border-slate-300 bg-white px-4 py-2.5 text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
          @click="exportar"
        >
          <Download :size="16" />
          Exportar CSV
        </button>
        <button
          type="button"
          class="flex items-center gap-2 rounded-lg border border-slate-300 bg-white px-4 py-2.5 text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
          @click="imprimirReporte"
        >
          <Printer :size="16" />
          Imprimir
        </button>
      </div>
    </div>

    <!-- ================= Resultado ================= -->
    <section class="rounded-xl border border-slate-200 bg-white shadow-sm">
      <div class="flex items-center justify-between border-b border-slate-100 px-5 py-4">
        <div class="flex items-center gap-2">
          <FileBarChart :size="18" class="text-indigo-600" />
          <h2 class="font-semibold text-slate-800">
            Reporte de {{ configuracion.nombre.replace(/-/g, ' ') }}
          </h2>
        </div>
        <span class="text-sm text-slate-400">
          {{ formatoNumero(filas.length) }} registro{{ filas.length === 1 ? '' : 's' }}
        </span>
      </div>

      <div class="p-5">
        <TablaContenedora
          :cargando="cargando"
          :vacio="!filas.length"
          mensaje-vacio="No hay datos para mostrar con los filtros indicados."
          :colapsadas="encabezados.length"
        >
          <template #encabezados>
            <th
              v-for="(columna, indice) in encabezados"
              :key="columna"
              class="px-4 py-3 font-semibold"
              :class="derecha(indice) ? 'text-right' : ''"
            >
              {{ columna }}
            </th>
          </template>

          <template #filas>
            <tr
              v-for="(fila, indice) in configuracion.filas"
              :key="indice"
              class="transition hover:bg-slate-50"
            >
              <td
                v-for="(valor, celda) in fila"
                :key="celda"
                class="px-4 py-3"
                :class="[
                  derecha(celda) ? 'whitespace-nowrap text-right text-slate-700' : 'text-slate-600',
                  celda === 0 ? 'font-mono text-xs font-semibold text-slate-600' : '',
                  pestanaActiva === 'critico' && celda === 5 ? 'font-bold text-rose-600' : '',
                  pestanaActiva === 'movimientos' && celda === 1 ? 'font-semibold' : '',
                ]"
              >
                <span
                  v-if="pestanaActiva === 'movimientos' && celda === 1"
                  :class="[
                    'rounded-full px-2 py-0.5 text-xs font-semibold',
                    valor === 'Entrada' ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700',
                  ]"
                >
                  {{ valor }}
                </span>
                <span v-else>{{ valor }}</span>
              </td>
            </tr>
          </template>
        </TablaContenedora>

        <!-- Total -->
        <div
          v-if="totalReporte !== null && !cargando && filas.length"
          class="mt-4 flex items-center justify-between rounded-lg bg-indigo-50 px-4 py-3"
        >
          <span class="text-sm font-semibold text-indigo-700">
            {{ configuracion.totalEtiqueta }}
          </span>
          <span class="text-lg font-bold text-indigo-700">{{ formatoMoneda(totalReporte) }}</span>
        </div>
      </div>
    </section>
  </div>
</template>
