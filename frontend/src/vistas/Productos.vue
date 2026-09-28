<script setup>
/**
 * Gestión de productos: búsqueda con filtros, alta/modificación/baja (solo
 * Administrador) y detalle de stock por almacén.
 */
import { computed, onMounted, reactive, ref } from 'vue'
import {
  Plus,
  Pencil,
  Trash2,
  Search,
  PackageSearch,
  Warehouse,
  RotateCw,
} from 'lucide-vue-next'
import {
  listarProductos,
  crearProducto,
  actualizarProducto,
  eliminarProducto,
  obtenerStockProducto,
  listarCategorias,
  listarProveedores,
} from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import { formatoMoneda } from '../utils/formato'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'
import BadgeStock from '../componentes/BadgeStock.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const productos = ref([])
const categorias = ref([])
const proveedores = ref([])
const cargando = ref(true)

const filtros = reactive({ busqueda: '', idCategoria: '', idProveedor: '' })

// Modal de alta/edición
const modalAbierto = ref(false)
const guardando = ref(false)
const modoEdicion = ref(false)
const idEdicion = ref(null)
const formulario = reactive({
  codigo: '',
  nombre: '',
  descripcion: '',
  idCategoria: '',
  idProveedor: '',
  precioCompra: 0,
  precioVenta: 0,
  unidadMedida: 'Unidad',
  stockMinimo: 0,
  activo: true,
})

// Modal de stock por almacén
const stockAbierto = ref(false)
const stockProducto = ref(null)
const stockDetalle = ref([])
const cargandoStock = ref(false)

// Confirmación de eliminación
const eliminarAbierto = ref(false)
const productoAEliminar = ref(null)

const esAdministrador = computed(() => autenticacion.esAdministrador)

async function cargarProductos() {
  cargando.value = true
  try {
    productos.value = await listarProductos({
      busqueda: filtros.busqueda || undefined,
      idCategoria: filtros.idCategoria || undefined,
      idProveedor: filtros.idProveedor || undefined,
    })
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

async function cargarListas() {
  const [cats, pros] = await Promise.all([listarCategorias(), listarProveedores()])
  categorias.value = cats
  proveedores.value = pros
}

function limpiarFormulario() {
  Object.assign(formulario, {
    codigo: '',
    nombre: '',
    descripcion: '',
    idCategoria: categorias.value[0]?.idCategoria || '',
    idProveedor: proveedores.value[0]?.idProveedor || '',
    precioCompra: 0,
    precioVenta: 0,
    unidadMedida: 'Unidad',
    stockMinimo: 0,
    activo: true,
  })
}

function abrirAlta() {
  modoEdicion.value = false
  idEdicion.value = null
  limpiarFormulario()
  modalAbierto.value = true
}

function abrirEdicion(producto) {
  modoEdicion.value = true
  idEdicion.value = producto.idProducto
  Object.assign(formulario, {
    codigo: producto.codigo,
    nombre: producto.nombre,
    descripcion: producto.descripcion || '',
    idCategoria: producto.idCategoria,
    idProveedor: producto.idProveedor,
    precioCompra: producto.precioCompra,
    precioVenta: producto.precioVenta,
    unidadMedida: producto.unidadMedida,
    stockMinimo: producto.stockMinimo,
    activo: producto.activo,
  })
  modalAbierto.value = true
}

async function guardar() {
  guardando.value = true
  try {
    const datos = {
      ...formulario,
      idCategoria: Number(formulario.idCategoria),
      idProveedor: Number(formulario.idProveedor),
      precioCompra: Number(formulario.precioCompra),
      precioVenta: Number(formulario.precioVenta),
      stockMinimo: Number(formulario.stockMinimo),
    }

    if (modoEdicion.value) {
      await actualizarProducto(idEdicion.value, datos)
      notificaciones.exito('Producto actualizado correctamente.')
    } else {
      await crearProducto(datos)
      notificaciones.exito('Producto creado correctamente.')
    }

    modalAbierto.value = false
    await cargarProductos()
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    guardando.value = false
  }
}

async function confirmarEliminacion() {
  try {
    await eliminarProducto(productoAEliminar.value.idProducto)
    notificaciones.exito('Producto dado de baja correctamente.')
    eliminarAbierto.value = false
    await cargarProductos()
  } catch (e) {
    notificaciones.error(e.message)
  }
}

async function verStock(producto) {
  stockProducto.value = producto
  stockAbierto.value = true
  cargandoStock.value = true
  try {
    stockDetalle.value = await obtenerStockProducto(producto.idProducto)
  } catch (e) {
    notificaciones.error(e.message)
    stockDetalle.value = []
  } finally {
    cargandoStock.value = false
  }
}

onMounted(async () => {
  await Promise.all([cargarProductos(), cargarListas()])
})
</script>

<template>
  <div class="space-y-4">
    <!-- ================= Barra de herramientas ================= -->
    <div class="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
      <!-- Búsqueda y filtros -->
      <div class="flex flex-1 flex-col gap-2 sm:flex-row sm:items-center">
        <div class="relative flex-1 max-w-md">
          <Search :size="16" class="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            v-model="filtros.busqueda"
            type="search"
            placeholder="Buscar por código, nombre o descripción…"
            class="w-full rounded-lg border border-slate-300 bg-white py-2 pl-9 pr-3 text-sm outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            @keyup.enter="cargarProductos"
            @input="filtros.busqueda === '' && cargarProductos()"
          />
        </div>

        <select
          v-model="filtros.idCategoria"
          class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          @change="cargarProductos"
        >
          <option value="">Todas las categorías</option>
          <option v-for="categoria in categorias" :key="categoria.idCategoria" :value="categoria.idCategoria">
            {{ categoria.nombre }}
          </option>
        </select>

        <select
          v-model="filtros.idProveedor"
          class="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          @change="cargarProductos"
        >
          <option value="">Todos los proveedores</option>
          <option v-for="proveedor in proveedores" :key="proveedor.idProveedor" :value="proveedor.idProveedor">
            {{ proveedor.nombre }}
          </option>
        </select>

        <button
          type="button"
          class="rounded-lg border border-slate-300 bg-white p-2 text-slate-500 transition hover:bg-slate-50"
          title="Actualizar listado"
          @click="cargarProductos"
        >
          <RotateCw :size="16" />
        </button>
      </div>

      <!-- Alta (solo Administrador) -->
      <button
        v-if="esAdministrador"
        type="button"
        class="flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700"
        @click="abrirAlta"
      >
        <Plus :size="17" />
        Nuevo producto
      </button>
    </div>

    <!-- ================= Tabla ================= -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!productos.length"
      mensaje-vacio="No se encontraron productos con los filtros indicados."
      :colapsadas="esAdministrador ? 8 : 7"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Código</th>
        <th class="px-4 py-3 font-semibold">Nombre</th>
        <th class="px-4 py-3 font-semibold">Categoría</th>
        <th class="px-4 py-3 font-semibold">Proveedor</th>
        <th class="px-4 py-3 text-right font-semibold">P. compra</th>
        <th class="px-4 py-3 text-right font-semibold">P. venta</th>
        <th class="px-4 py-3 font-semibold">Estado stock</th>
        <th v-if="esAdministrador" class="px-4 py-3 text-right font-semibold">Acciones</th>
      </template>

      <template #filas>
        <tr
          v-for="producto in productos"
          :key="producto.idProducto"
          :class="['transition hover:bg-slate-50', !producto.activo && 'opacity-50']"
        >
          <td class="whitespace-nowrap px-4 py-3 font-mono text-xs font-semibold text-slate-600">
            {{ producto.codigo }}
          </td>
          <td class="px-4 py-3">
            <p class="font-medium text-slate-800">{{ producto.nombre }}</p>
            <p class="max-w-[240px] truncate text-xs text-slate-400">
              {{ producto.descripcion || 'Sin descripción' }} · {{ producto.unidadMedida }}
            </p>
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">{{ producto.nombreCategoria }}</td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">{{ producto.nombreProveedor }}</td>
          <td class="whitespace-nowrap px-4 py-3 text-right text-slate-600">
            {{ formatoMoneda(producto.precioCompra) }}
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-right font-medium text-slate-800">
            {{ formatoMoneda(producto.precioVenta) }}
          </td>
          <td class="whitespace-nowrap px-4 py-3">
            <button type="button" title="Ver stock por almacén" @click="verStock(producto)">
              <BadgeStock
                :critico="producto.stockCritico"
                :stock-actual="producto.stockActual"
                :stock-minimo="producto.stockMinimo"
              />
            </button>
          </td>
          <td v-if="esAdministrador" class="whitespace-nowrap px-4 py-3 text-right">
            <div class="inline-flex gap-1">
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
                title="Modificar"
                @click="abrirEdicion(producto)"
              >
                <Pencil :size="16" />
              </button>
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
                title="Dar de baja"
                @click="productoAEliminar = producto; eliminarAbierto = true"
              >
                <Trash2 :size="16" />
              </button>
            </div>
          </td>
        </tr>
      </template>
    </TablaContenedora>

    <!-- ================= Modal alta/edición ================= -->
    <ModalBase
      :abierto="modalAbierto"
      :titulo="modoEdicion ? 'Modificar producto' : 'Nuevo producto'"
      ancho="max-w-2xl"
      @cerrar="modalAbierto = false"
    >
      <form class="grid grid-cols-1 gap-4 sm:grid-cols-2" @submit.prevent="guardar">
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Código *</label>
          <input
            v-model="formulario.codigo"
            type="text"
            maxlength="30"
            required
            placeholder="Ej.: ELEC-001"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Nombre *</label>
          <input
            v-model="formulario.nombre"
            type="text"
            maxlength="150"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div class="sm:col-span-2">
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Descripción</label>
          <textarea
            v-model="formulario.descripcion"
            rows="2"
            maxlength="500"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Categoría *</label>
          <select
            v-model="formulario.idCategoria"
            required
            class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          >
            <option disabled value="">Seleccione…</option>
            <option v-for="categoria in categorias" :key="categoria.idCategoria" :value="categoria.idCategoria">
              {{ categoria.nombre }}
            </option>
          </select>
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Proveedor *</label>
          <select
            v-model="formulario.idProveedor"
            required
            class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
          >
            <option disabled value="">Seleccione…</option>
            <option v-for="proveedor in proveedores" :key="proveedor.idProveedor" :value="proveedor.idProveedor">
              {{ proveedor.nombre }}
            </option>
          </select>
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Precio de compra</label>
          <input
            v-model.number="formulario.precioCompra"
            type="number"
            min="0"
            step="0.01"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Precio de venta</label>
          <input
            v-model.number="formulario.precioVenta"
            type="number"
            min="0"
            step="0.01"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Unidad de medida *</label>
          <input
            v-model="formulario.unidadMedida"
            type="text"
            maxlength="30"
            required
            placeholder="Ej.: Unidad, Caja, Kg"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Stock mínimo *</label>
          <input
            v-model.number="formulario.stockMinimo"
            type="number"
            min="0"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div v-if="modoEdicion" class="flex items-center gap-2 sm:col-span-2">
          <input id="activo" v-model="formulario.activo" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
          <label for="activo" class="text-sm text-slate-600">Producto activo</label>
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
          {{ modoEdicion ? 'Guardar cambios' : 'Crear producto' }}
        </button>
      </template>
    </ModalBase>

    <!-- ================= Modal stock por almacén ================= -->
    <ModalBase
      :abierto="stockAbierto"
      :titulo="`Stock por almacén — ${stockProducto?.nombre || ''}`"
      @cerrar="stockAbierto = false"
    >
      <p v-if="cargandoStock" class="py-6 text-center text-sm text-slate-400">Cargando stock…</p>
      <template v-else>
        <div class="mb-4 flex items-center justify-between rounded-lg bg-slate-50 px-4 py-3">
          <span class="text-sm text-slate-600">Stock total</span>
          <span class="text-lg font-bold text-slate-800">
            {{ stockProducto?.stockActual }} {{ stockProducto?.unidadMedida }}
          </span>
        </div>
        <ul class="space-y-2">
          <li
            v-for="registro in stockDetalle"
            :key="registro.idAlmacen"
            class="flex items-center justify-between rounded-lg border border-slate-200 px-4 py-3"
          >
            <span class="flex items-center gap-2 text-sm text-slate-600">
              <Warehouse :size="16" class="text-slate-400" />
              {{ registro.nombreAlmacen }}
            </span>
            <span class="font-semibold text-slate-800">{{ registro.cantidad }}</span>
          </li>
        </ul>
        <p v-if="!stockDetalle.length" class="py-4 text-center text-sm text-slate-400">
          Este producto aún no tiene stock registrado en ningún almacén.
        </p>
      </template>
    </ModalBase>

    <!-- ================= Confirmación de baja ================= -->
    <ModalBase
      :abierto="eliminarAbierto"
      titulo="Confirmar baja de producto"
      @cerrar="eliminarAbierto = false"
    >
      <div class="flex items-start gap-3">
        <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-rose-100">
          <Trash2 :size="18" class="text-rose-600" />
        </div>
        <p class="text-sm text-slate-600">
          ¿Está seguro de dar de baja el producto
          <strong>{{ productoAEliminar?.nombre }}</strong>
          ({{ productoAEliminar?.codigo }})? El producto dejará de aparecer en las consultas,
          pero se conservará su historial de movimientos.
        </p>
      </div>
      <template #pie>
        <button
          type="button"
          class="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
          @click="eliminarAbierto = false"
        >
          Cancelar
        </button>
        <button
          type="button"
          class="rounded-lg bg-rose-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-rose-700"
          @click="confirmarEliminacion"
        >
          Dar de baja
        </button>
      </template>
    </ModalBase>
  </div>
</template>
