<script setup>
/**
 * Movimientos de inventario: listado con filtros, registro de entradas y
 * salidas (Administrador y Usuario) con detalle de líneas y actualización
 * automática de stock en el servidor.
 */
import { computed, onMounted, reactive, ref } from 'vue'
import {
  Plus,
  Search,
  RotateCw,
  Eye,
  ArrowUpRight,
  ArrowDownRight,
  Trash2,
  PackagePlus,
} from 'lucide-vue-next'
import {
  buscarMovimientos,
  crearMovimiento,
  listarProductos,
  listarAlmacenes,
} from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import { formatoMoneda, formatoFecha, formatoNumero } from '../utils/formato'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const movimientos = ref([])
const productos = ref([])
const almacenes = ref([])
const cargando = ref(true)

const filtros = reactive({ idProducto: '', tipo: '', fechaDesde: '', fechaHasta: '' })

// Alta de movimiento
const modalAbierto = ref(false)
const guardando = ref(false)
const formulario = reactive({
  tipo: 'Entrada',
  motivo: '',
  documentoReferencia: '',
  idAlmacen: '',
})
const lineas = ref([])

// Detalle de un movimiento
const detalleAbierto = ref(false)
const movimientoDetalle = ref(null)

const puedeRegistrar = computed(() => autenticacion.puedeRegistrarMovimientos)

/** Total de todas las líneas del formulario. */
const totalFormulario = computed(() =>
  lineas.value.reduce((suma, linea) => suma + Number(linea.subtotal || 0), 0),
)

async function cargarMovimientos() {
  cargando.value = true
  try {
    movimientos.value = await buscarMovimientos({
      idProducto: filtros.idProducto || undefined,
      tipo: filtros.tipo || undefined,
      fechaDesde: filtros.fechaDesde || undefined,
      fechaHasta: filtros.fechaHasta || undefined,
    })
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

async function cargarListas() {
  const [prods, alms] = await Promise.all([listarProductos(), listarAlmacenes()])
  productos.value = prods.filter((p) => p.activo)
  almacenes.value = alms.filter((a) => a.activo)
}

function abrirAlta() {
  Object.assign(formulario, {
    tipo: 'Entrada',
    motivo: '',
    documentoReferencia: '',
    idAlmacen: almacenes.value[0]?.idAlmacen || '',
  })
  lineas.value = []
  agregarLinea()
  modalAbierto.value = true
}

function agregarLinea() {
  lineas.value.push({ idProducto: '', cantidad: 1, precioUnitario: 0, subtotal: 0 })
}

function eliminarLinea(indice) {
  lineas.value.splice(indice, 1)
  if (!lineas.value.length) agregarLinea()
}

/** Calcula el subtotal de una línea al cambiar cantidad o precio. */
function calcularSubtotal(linea) {
  linea.subtotal = Number(linea.cantidad || 0) * Number(linea.precioUnitario || 0)
}

/** Al elegir producto, completa el precio de compra como referencia. */
function productoSeleccionado(linea) {
  const producto = productos.value.find((p) => p.idProducto === Number(linea.idProducto))
  if (producto) {
    linea.precioUnitario = Number(producto.precioCompra)
    calcularSubtotal(linea)
  }
}

async function guardar() {
  // Validaciones básicas del formulario
  const validas = lineas.value.filter((l) => l.idProducto && Number(l.cantidad) > 0)
  if (!validas.length) {
    notificaciones.error('Agregue al menos una línea con producto y cantidad mayor a cero.')
    return
  }
  if (!formulario.idAlmacen) {
    notificaciones.error('Seleccione el almacén donde se registra el movimiento.')
    return
  }

  guardando.value = true
  try {
    await crearMovimiento({
      tipo: formulario.tipo,
      motivo: formulario.motivo?.trim() || null,
      documentoReferencia: formulario.documentoReferencia?.trim() || null,
      idAlmacen: Number(formulario.idAlmacen),
      detalles: validas.map((l) => ({
        idProducto: Number(l.idProducto),
        cantidad: Number(l.cantidad),
        precioUnitario: Number(l.precioUnitario || 0),
      })),
    })
    notificaciones.exito(`Movimiento de ${formulario.tipo.toLowerCase()} registrado correctamente.`)
    modalAbierto.value = false
    await Promise.all([cargarMovimientos(), cargarListas()])
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    guardando.value = false
  }
}

function verDetalle(movimiento) {
  movimientoDetalle.value = movimiento
  detalleAbierto.value = true
}

function limpiarFiltros() {
  Object.assign(filtros, { idProducto: '', tipo: '', fechaDesde: '', fechaHasta: '' })
  cargarMovimientos()
}

onMounted(async () => {
  await Promise.all([cargarMovimientos(), cargarListas()])
})
</script>

<template>
  <div class="space-y-4">
    <!-- ================= Barra de herramientas ================= -->
    <div class="flex flex-col gap-3 xl:flex-row xl:items-center xl:justify-between">
      <!-- Filtros -->
      <div class="flex flex-1 flex-wrap items-center gap-2">
        <select
          v-model="filtros.idProducto"
          class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          @change="cargarMovimientos"
        >
          <option value="">Todos los productos</option>
          <option v-for="producto in productos" :key="producto.idProducto" :value="producto.idProducto">
            {{ producto.codigo }} — {{ producto.nombre }}
          </option>
        </select>

        <select
          v-model="filtros.tipo"
          class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          @change="cargarMovimientos"
        >
          <option value="">Entradas y salidas</option>
          <option value="Entrada">Solo entradas</option>
          <option value="Salida">Solo salidas</option>
        </select>

        <label class="flex items-center gap-1.5 text-sm text-slate-500">
          Desde
          <input
            v-model="filtros.fechaDesde"
            type="date"
            class="rounded-lg border border-slate-300 px-2 py-1.5 text-sm outline-none focus:border-indigo-500"
            @change="cargarMovimientos"
          />
        </label>
        <label class="flex items-center gap-1.5 text-sm text-slate-500">
          Hasta
          <input
            v-model="filtros.fechaHasta"
            type="date"
            class="rounded-lg border border-slate-300 px-2 py-1.5 text-sm outline-none focus:border-indigo-500"
            @change="cargarMovimientos"
          />
        </label>

        <button
          type="button"
          class="rounded-lg border border-slate-300 bg-white p-2 text-slate-500 transition hover:bg-slate-50"
          title="Limpiar filtros y actualizar"
          @click="limpiarFiltros"
        >
          <RotateCw :size="16" />
        </button>
      </div>

      <!-- Registro (Administrador y Usuario) -->
      <button
        v-if="puedeRegistrar"
        type="button"
        class="flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700"
        @click="abrirAlta"
      >
        <Plus :size="17" />
        Registrar movimiento
      </button>
    </div>

    <!-- ================= Listado ================= -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!movimientos.length"
      mensaje-vacio="No hay movimientos que coincidan con los filtros."
      :colapsadas="8"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Fecha</th>
        <th class="px-4 py-3 font-semibold">Tipo</th>
        <th class="px-4 py-3 font-semibold">Motivo</th>
        <th class="px-4 py-3 font-semibold">Documento</th>
        <th class="px-4 py-3 font-semibold">Almacén</th>
        <th class="px-4 py-3 font-semibold">Usuario</th>
        <th class="px-4 py-3 text-right font-semibold">Total</th>
        <th class="px-4 py-3 text-center font-semibold">Detalle</th>
      </template>

      <template #filas>
        <tr
          v-for="movimiento in movimientos"
          :key="movimiento.idMovimiento"
          class="cursor-pointer transition hover:bg-slate-50"
          @click="verDetalle(movimiento)"
        >
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">
            {{ formatoFecha(movimiento.fecha) }}
          </td>
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold',
                movimiento.tipo === 'Entrada'
                  ? 'bg-emerald-100 text-emerald-700'
                  : 'bg-amber-100 text-amber-700',
              ]"
            >
              <component
                :is="movimiento.tipo === 'Entrada' ? ArrowDownRight : ArrowUpRight"
                :size="13"
              />
              {{ movimiento.tipo }}
            </span>
          </td>
          <td class="max-w-[220px] truncate px-4 py-3 text-slate-600">
            {{ movimiento.motivo || '—' }}
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-500">
            {{ movimiento.documentoReferencia || '—' }}
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">
            {{ movimiento.nombreAlmacen }}
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">
            {{ movimiento.nombreUsuario }}
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-right font-semibold text-slate-700">
            {{ formatoMoneda(movimiento.total) }}
          </td>
          <td class="px-4 py-3 text-center">
            <button
              type="button"
              class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
              title="Ver detalle"
              @click.stop="verDetalle(movimiento)"
            >
              <Eye :size="16" />
            </button>
          </td>
        </tr>
      </template>
    </TablaContenedora>

    <!-- ================= Modal registro ================= -->
    <ModalBase
      :abierto="modalAbierto"
      titulo="Registrar movimiento"
      ancho="max-w-3xl"
      @cerrar="modalAbierto = false"
    >
      <form class="space-y-5" @submit.prevent="guardar">
        <!-- Cabecera -->
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Tipo de movimiento *</label>
            <div class="grid grid-cols-2 gap-2">
              <button
                v-for="tipo in ['Entrada', 'Salida']"
                :key="tipo"
                type="button"
                :class="[
                  'flex items-center justify-center gap-2 rounded-lg border px-3 py-2.5 text-sm font-semibold transition',
                  formulario.tipo === tipo
                    ? tipo === 'Entrada'
                      ? 'border-emerald-500 bg-emerald-50 text-emerald-700'
                      : 'border-amber-500 bg-amber-50 text-amber-700'
                    : 'border-slate-300 text-slate-500 hover:bg-slate-50',
                ]"
                @click="formulario.tipo = tipo"
              >
                <component :is="tipo === 'Entrada' ? ArrowDownRight : ArrowUpRight" :size="16" />
                {{ tipo }}
              </button>
            </div>
          </div>

          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Almacén *</label>
            <select
              v-model="formulario.idAlmacen"
              required
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm outline-none focus:border-indigo-500"
            >
              <option disabled value="">Seleccione…</option>
              <option v-for="almacen in almacenes" :key="almacen.idAlmacen" :value="almacen.idAlmacen">
                {{ almacen.nombre }}
              </option>
            </select>
          </div>

          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">
              Documento / factura
            </label>
            <input
              v-model="formulario.documentoReferencia"
              type="text"
              maxlength="50"
              placeholder="Ej.: FACT-000123"
              class="w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            />
          </div>

          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Motivo</label>
            <input
              v-model="formulario.motivo"
              type="text"
              maxlength="300"
              placeholder="Ej.: Compra a proveedor, Venta a cliente…"
              class="w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            />
          </div>
        </div>

        <!-- Detalle -->
        <div>
          <div class="mb-2 flex items-center justify-between">
            <p class="flex items-center gap-1.5 text-sm font-semibold text-slate-700">
              <PackagePlus :size="16" class="text-indigo-600" />
              Productos del movimiento
            </p>
            <button
              type="button"
              class="flex items-center gap-1 rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-semibold text-slate-600 transition hover:bg-slate-50"
              @click="agregarLinea"
            >
              <Plus :size="14" />
              Agregar línea
            </button>
          </div>

          <div class="space-y-2">
            <div
              v-for="(linea, indice) in lineas"
              :key="indice"
              class="grid grid-cols-12 items-end gap-2 rounded-lg border border-slate-200 bg-slate-50/60 p-3"
            >
              <div class="col-span-12 sm:col-span-5">
                <label class="mb-1 block text-xs font-medium text-slate-500">Producto *</label>
                <select
                  v-model="linea.idProducto"
                  required
                  class="w-full rounded-lg border border-slate-300 bg-white px-2.5 py-2 text-sm outline-none focus:border-indigo-500"
                  @change="productoSeleccionado(linea)"
                >
                  <option disabled value="">Seleccione…</option>
                  <option v-for="producto in productos" :key="producto.idProducto" :value="producto.idProducto">
                    {{ producto.codigo }} — {{ producto.nombre }}
                  </option>
                </select>
              </div>

              <div class="col-span-6 sm:col-span-2">
                <label class="mb-1 block text-xs font-medium text-slate-500">Cantidad *</label>
                <input
                  v-model.number="linea.cantidad"
                  type="number"
                  min="1"
                  required
                  class="w-full rounded-lg border border-slate-300 px-2.5 py-2 text-sm outline-none focus:border-indigo-500"
                  @input="calcularSubtotal(linea)"
                />
              </div>

              <div class="col-span-6 sm:col-span-2">
                <label class="mb-1 block text-xs font-medium text-slate-500">Precio unit.</label>
                <input
                  v-model.number="linea.precioUnitario"
                  type="number"
                  min="0"
                  step="0.01"
                  class="w-full rounded-lg border border-slate-300 px-2.5 py-2 text-sm outline-none focus:border-indigo-500"
                  @input="calcularSubtotal(linea)"
                />
              </div>

              <div class="col-span-6 sm:col-span-2">
                <label class="mb-1 block text-xs font-medium text-slate-500">Subtotal</label>
                <p class="py-2 text-sm font-semibold text-slate-700">
                  {{ formatoMoneda(linea.subtotal) }}
                </p>
              </div>

              <div class="col-span-6 flex justify-end sm:col-span-1">
                <button
                  type="button"
                  class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
                  title="Quitar línea"
                  @click="eliminarLinea(indice)"
                >
                  <Trash2 :size="16" />
                </button>
              </div>
            </div>
          </div>

          <!-- Total -->
          <div class="mt-3 flex items-center justify-between rounded-lg bg-slate-100 px-4 py-3">
            <span class="text-sm font-medium text-slate-600">Total del movimiento</span>
            <span class="text-lg font-bold text-slate-800">{{ formatoMoneda(totalFormulario) }}</span>
          </div>

          <p class="mt-2 text-xs text-slate-400">
            {{ formulario.tipo === 'Entrada'
              ? 'Las entradas incrementan el stock de los productos en el almacén seleccionado.'
              : 'Las salidas decrementan el stock; no se permite salir más que el stock disponible.' }}
          </p>
        </div>
      </form>

      <template #pie>
        <button
          type="button"
          class="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
          @click="modalAbierto = false"
        >
          Cancelar
        </button>
        <button
          type="button"
          :disabled="guardando"
          class="flex items-center gap-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-indigo-700 disabled:opacity-60"
          @click="guardar"
        >
          <span v-if="guardando" class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent"></span>
          Registrar movimiento
        </button>
      </template>
    </ModalBase>

    <!-- ================= Modal detalle ================= -->
    <ModalBase
      :abierto="detalleAbierto"
      :titulo="`Detalle del movimiento #${movimientoDetalle?.idMovimiento || ''}`"
      ancho="max-w-3xl"
      @cerrar="detalleAbierto = false"
    >
      <template v-if="movimientoDetalle">
        <!-- Datos de cabecera -->
        <div class="mb-4 grid grid-cols-2 gap-3 rounded-lg bg-slate-50 p-4 text-sm sm:grid-cols-3">
          <div>
            <p class="text-xs text-slate-400">Fecha</p>
            <p class="font-medium text-slate-700">{{ formatoFecha(movimientoDetalle.fecha) }}</p>
          </div>
          <div>
            <p class="text-xs text-slate-400">Tipo</p>
            <p class="font-medium text-slate-700">{{ movimientoDetalle.tipo }}</p>
          </div>
          <div>
            <p class="text-xs text-slate-400">Almacén</p>
            <p class="font-medium text-slate-700">{{ movimientoDetalle.nombreAlmacen }}</p>
          </div>
          <div>
            <p class="text-xs text-slate-400">Usuario responsable</p>
            <p class="font-medium text-slate-700">{{ movimientoDetalle.nombreUsuario }}</p>
          </div>
          <div>
            <p class="text-xs text-slate-400">Documento / factura</p>
            <p class="font-medium text-slate-700">
              {{ movimientoDetalle.documentoReferencia || '—' }}
            </p>
          </div>
          <div>
            <p class="text-xs text-slate-400">Proveedor</p>
            <p class="font-medium text-slate-700">
              {{ movimientoDetalle.nombreProveedor || '—' }}
            </p>
          </div>
          <div class="col-span-2 sm:col-span-3">
            <p class="text-xs text-slate-400">Motivo</p>
            <p class="font-medium text-slate-700">{{ movimientoDetalle.motivo || '—' }}</p>
          </div>
        </div>

        <!-- Líneas -->
        <TablaContenedora
          :cargando="false"
          :vacio="!movimientoDetalle.detalles?.length"
          mensaje-vacio="Este movimiento no tiene líneas de detalle."
          :colapsadas="4"
        >
          <template #encabezados>
            <th class="px-4 py-3 font-semibold">Producto</th>
            <th class="px-4 py-3 text-right font-semibold">Cantidad</th>
            <th class="px-4 py-3 text-right font-semibold">Precio unit.</th>
            <th class="px-4 py-3 text-right font-semibold">Subtotal</th>
          </template>
          <template #filas>
            <tr
              v-for="(detalle, indice) in movimientoDetalle.detalles"
              :key="indice"
              class="transition hover:bg-slate-50"
            >
              <td class="px-4 py-3">
                <p class="font-medium text-slate-800">{{ detalle.nombreProducto }}</p>
                <p class="text-xs text-slate-400">{{ detalle.codigoProducto }}</p>
              </td>
              <td class="whitespace-nowrap px-4 py-3 text-right text-slate-700">
                {{ formatoNumero(detalle.cantidad) }} {{ detalle.unidadMedida }}
              </td>
              <td class="whitespace-nowrap px-4 py-3 text-right text-slate-600">
                {{ formatoMoneda(detalle.precioUnitario) }}
              </td>
              <td class="whitespace-nowrap px-4 py-3 text-right font-semibold text-slate-800">
                {{ formatoMoneda(detalle.subtotal) }}
              </td>
            </tr>
          </template>
        </TablaContenedora>

        <div class="mt-4 flex items-center justify-between rounded-lg bg-indigo-50 px-4 py-3">
          <span class="text-sm font-semibold text-indigo-700">Total del movimiento</span>
          <span class="text-lg font-bold text-indigo-700">
            {{ formatoMoneda(movimientoDetalle.total) }}
          </span>
        </div>
      </template>
    </ModalBase>
  </div>
</template>
