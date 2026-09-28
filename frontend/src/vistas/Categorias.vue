<script setup>
/**
 * CRUD de categorías de productos (alta/modificación/baja solo Administrador).
 */
import { computed, onMounted, reactive, ref } from 'vue'
import { Plus, Pencil, Trash2, RotateCw, Tags } from 'lucide-vue-next'
import { listarCategorias, crearCategoria, actualizarCategoria, eliminarCategoria } from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const categorias = ref([])
const cargando = ref(true)
const incluirInactivas = ref(false)

const modalAbierto = ref(false)
const guardando = ref(false)
const modoEdicion = ref(false)
const idEdicion = ref(null)
const formulario = reactive({ nombre: '', descripcion: '', activo: true })

const eliminarAbierto = ref(false)
const categoriaAEliminar = ref(null)

const esAdministrador = computed(() => autenticacion.esAdministrador)

async function cargar() {
  cargando.value = true
  try {
    categorias.value = await listarCategorias(incluirInactivas.value)
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

function abrirAlta() {
  modoEdicion.value = false
  Object.assign(formulario, { nombre: '', descripcion: '', activo: true })
  modalAbierto.value = true
}

function abrirEdicion(categoria) {
  modoEdicion.value = true
  idEdicion.value = categoria.idCategoria
  Object.assign(formulario, {
    nombre: categoria.nombre,
    descripcion: categoria.descripcion || '',
    activo: categoria.activo,
  })
  modalAbierto.value = true
}

async function guardar() {
  guardando.value = true
  try {
    const datos = {
      nombre: formulario.nombre.trim(),
      descripcion: formulario.descripcion?.trim() || null,
      activo: formulario.activo,
    }

    if (modoEdicion.value) {
      await actualizarCategoria(idEdicion.value, datos)
      notificaciones.exito('Categoría actualizada correctamente.')
    } else {
      await crearCategoria(datos)
      notificaciones.exito('Categoría creada correctamente.')
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
    await eliminarCategoria(categoriaAEliminar.value.idCategoria)
    notificaciones.exito('Categoría dada de baja correctamente.')
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
          <input v-model="incluirInactivas" type="checkbox" class="h-4 w-4 rounded border-slate-300" @change="cargar" />
          Incluir inactivas
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
        Nueva categoría
      </button>
    </div>

    <!-- Tabla -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!categorias.length"
      mensaje-vacio="No hay categorías registradas."
      :colapsadas="esAdministrador ? 5 : 4"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Nombre</th>
        <th class="px-4 py-3 font-semibold">Descripción</th>
        <th class="px-4 py-3 text-right font-semibold">Productos</th>
        <th class="px-4 py-3 font-semibold">Estado</th>
        <th v-if="esAdministrador" class="px-4 py-3 text-right font-semibold">Acciones</th>
      </template>

      <template #filas>
        <tr
          v-for="categoria in categorias"
          :key="categoria.idCategoria"
          :class="['transition hover:bg-slate-50', !categoria.activo && 'opacity-50']"
        >
          <td class="px-4 py-3">
            <span class="flex items-center gap-2 font-medium text-slate-800">
              <Tags :size="15" class="text-indigo-500" />
              {{ categoria.nombre }}
            </span>
          </td>
          <td class="max-w-[320px] truncate px-4 py-3 text-slate-500">
            {{ categoria.descripcion || '—' }}
          </td>
          <td class="px-4 py-3 text-right font-semibold text-slate-700">
            {{ categoria.totalProductos }}
          </td>
          <td class="px-4 py-3">
            <span
              :class="[
                'rounded-full px-2.5 py-0.5 text-xs font-semibold',
                categoria.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-500',
              ]"
            >
              {{ categoria.activo ? 'Activa' : 'Inactiva' }}
            </span>
          </td>
          <td v-if="esAdministrador" class="px-4 py-3 text-right">
            <div class="inline-flex gap-1">
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
                title="Modificar"
                @click="abrirEdicion(categoria)"
              >
                <Pencil :size="16" />
              </button>
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
                title="Dar de baja"
                @click="categoriaAEliminar = categoria; eliminarAbierto = true"
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
      :titulo="modoEdicion ? 'Modificar categoría' : 'Nueva categoría'"
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
            placeholder="Ej.: Electrónica"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Descripción</label>
          <textarea
            v-model="formulario.descripcion"
            rows="3"
            maxlength="300"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div v-if="modoEdicion" class="flex items-center gap-2">
          <input id="cat-activo" v-model="formulario.activo" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
          <label for="cat-activo" class="text-sm text-slate-600">Categoría activa</label>
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
          {{ modoEdicion ? 'Guardar cambios' : 'Crear categoría' }}
        </button>
      </template>
    </ModalBase>

    <!-- Confirmación de baja -->
    <ModalBase
      :abierto="eliminarAbierto"
      titulo="Confirmar baja de categoría"
      @cerrar="eliminarAbierto = false"
    >
      <p class="text-sm text-slate-600">
        ¿Está seguro de dar de baja la categoría
        <strong>{{ categoriaAEliminar?.nombre }}</strong>?
        Los productos asociados se conservarán.
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
