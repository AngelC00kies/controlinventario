<script setup>
/**
 * CRUD de almacenes (el sistema soporta uno o varios almacenes).
 */
import { computed, onMounted, reactive, ref } from 'vue'
import { Plus, Pencil, Trash2, RotateCw, Warehouse, MapPin } from 'lucide-vue-next'
import { listarAlmacenes, crearAlmacen, actualizarAlmacen, eliminarAlmacen } from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const almacenes = ref([])
const cargando = ref(true)
const incluirInactivos = ref(false)

const modalAbierto = ref(false)
const guardando = ref(false)
const modoEdicion = ref(false)
const idEdicion = ref(null)
const formulario = reactive({ nombre: '', ubicacion: '', descripcion: '', activo: true })

const eliminarAbierto = ref(false)
const almacenAEliminar = ref(null)

const esAdministrador = computed(() => autenticacion.esAdministrador)

async function cargar() {
  cargando.value = true
  try {
    almacenes.value = await listarAlmacenes(incluirInactivos.value)
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

function abrirAlta() {
  modoEdicion.value = false
  Object.assign(formulario, { nombre: '', ubicacion: '', descripcion: '', activo: true })
  modalAbierto.value = true
}

function abrirEdicion(almacen) {
  modoEdicion.value = true
  idEdicion.value = almacen.idAlmacen
  Object.assign(formulario, {
    nombre: almacen.nombre,
    ubicacion: almacen.ubicacion || '',
    descripcion: almacen.descripcion || '',
    activo: almacen.activo,
  })
  modalAbierto.value = true
}

async function guardar() {
  guardando.value = true
  try {
    const datos = {
      nombre: formulario.nombre.trim(),
      ubicacion: formulario.ubicacion?.trim() || null,
      descripcion: formulario.descripcion?.trim() || null,
      activo: formulario.activo,
    }

    if (modoEdicion.value) {
      await actualizarAlmacen(idEdicion.value, datos)
      notificaciones.exito('Almacén actualizado correctamente.')
    } else {
      await crearAlmacen(datos)
      notificaciones.exito('Almacén creado correctamente.')
    }

    modalAbierto.value = false
    await cargar()
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    guardando.value = false
  }
}

async function confirmarEliminacion() {
  try {
    await eliminarAlmacen(almacenAEliminar.value.idAlmacen)
    notificaciones.exito('Almacén dado de baja correctamente.')
    eliminarAbierto.value = false
    await cargar()
  } catch (e) {
    notificaciones.error(e.message)
  }
}

onMounted(cargar)
</script>

<template>
  <div class="space-y-4">
    <!-- Barra de herramientas -->
    <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div class="flex items-center gap-3">
        <label class="flex cursor-pointer items-center gap-2 text-sm text-slate-600">
          <input v-model="incluirInactivos" type="checkbox" class="h-4 w-4 rounded border-slate-300" @change="cargar" />
          Incluir inactivos
        </label>
        <button
          type="button"
          class="rounded-lg border border-slate-300 bg-white p-2 text-slate-500 transition hover:bg-slate-50"
          title="Actualizar listado"
          @click="cargar"
        >
          <RotateCw :size="16" />
        </button>
      </div>

      <button
        v-if="esAdministrador"
        type="button"
        class="flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700"
        @click="abrirAlta"
      >
        <Plus :size="17" />
        Nuevo almacén
      </button>
    </div>

    <!-- Tabla -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!almacenes.length"
      mensaje-vacio="No hay almacenes registrados."
      :colapsadas="esAdministrador ? 5 : 4"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Nombre</th>
        <th class="px-4 py-3 font-semibold">Ubicación</th>
        <th class="px-4 py-3 font-semibold">Descripción</th>
        <th class="px-4 py-3 font-semibold">Estado</th>
        <th v-if="esAdministrador" class="px-4 py-3 text-right font-semibold">Acciones</th>
      </template>

      <template #filas>
        <tr
          v-for="almacen in almacenes"
          :key="almacen.idAlmacen"
          :class="['transition hover:bg-slate-50', !almacen.activo && 'opacity-50']"
        >
          <td class="px-4 py-3 font-medium text-slate-800">
            <span class="flex items-center gap-2">
              <Warehouse :size="15" class="text-indigo-500" />
              {{ almacen.nombre }}
            </span>
          </td>
          <td class="px-4 py-3">
            <span class="flex items-center gap-1.5 text-slate-600">
              <MapPin :size="13" class="text-slate-400" />
              {{ almacen.ubicacion || '—' }}
            </span>
          </td>
          <td class="max-w-[280px] truncate px-4 py-3 text-slate-500">
            {{ almacen.descripcion || '—' }}
          </td>
          <td class="px-4 py-3">
            <span
              :class="[
                'rounded-full px-2.5 py-0.5 text-xs font-semibold',
                almacen.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-500',
              ]"
            >
              {{ almacen.activo ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td v-if="esAdministrador" class="px-4 py-3 text-right">
            <div class="inline-flex gap-1">
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
                title="Modificar"
                @click="abrirEdicion(almacen)"
              >
                <Pencil :size="16" />
              </button>
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
                title="Dar de baja"
                @click="almacenAEliminar = almacen; eliminarAbierto = true"
              >
                <Trash2 :size="16" />
              </button>
            </div>
          </td>
        </tr>
      </template>
    </TablaContenedora>

    <!-- Modal alta/edición -->
    <ModalBase
      :abierto="modalAbierto"
      :titulo="modoEdicion ? 'Modificar almacén' : 'Nuevo almacén'"
      @cerrar="modalAbierto = false"
    >
      <form class="space-y-4" @submit.prevent="guardar">
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Nombre *</label>
          <input
            v-model="formulario.nombre"
            type="text"
            maxlength="100"
            required
            placeholder="Ej.: Almacén Central"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Ubicación</label>
          <input
            v-model="formulario.ubicacion"
            type="text"
            maxlength="200"
            placeholder="Ej.: Bodega principal - Zona Industrial"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Descripción</label>
          <textarea
            v-model="formulario.descripcion"
            rows="2"
            maxlength="300"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div v-if="modoEdicion" class="flex items-center gap-2">
          <input id="alm-activo" v-model="formulario.activo" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
          <label for="alm-activo" class="text-sm text-slate-600">Almacén activo</label>
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
          class="rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-indigo-700 disabled:opacity-60"
          @click="guardar"
        >
          {{ modoEdicion ? 'Guardar cambios' : 'Crear almacén' }}
        </button>
      </template>
    </ModalBase>

    <!-- Confirmación de baja -->
    <ModalBase
      :abierto="eliminarAbierto"
      titulo="Confirmar baja de almacén"
      @cerrar="eliminarAbierto = false"
    >
      <p class="text-sm text-slate-600">
        ¿Está seguro de dar de baja el almacén
        <strong>{{ almacenAEliminar?.nombre }}</strong>?
        Solo podrá darse de baja si no tiene stock ni movimientos asociados.
      </p>
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
