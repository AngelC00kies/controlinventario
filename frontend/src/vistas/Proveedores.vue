<script setup>
/**
 * CRUD de proveedores con sus datos de contacto.
 */
import { computed, onMounted, reactive, ref } from 'vue'
import { Plus, Pencil, Trash2, RotateCw, Truck, Phone, Mail, MapPin } from 'lucide-vue-next'
import { listarProveedores, crearProveedor, actualizarProveedor, eliminarProveedor } from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const proveedores = ref([])
const cargando = ref(true)
const incluirInactivos = ref(false)

const modalAbierto = ref(false)
const guardando = ref(false)
const modoEdicion = ref(false)
const idEdicion = ref(null)
const formulario = reactive({
  nombre: '',
  contacto: '',
  telefono: '',
  correo: '',
  direccion: '',
  activo: true,
})

const eliminarAbierto = ref(false)
const proveedorAEliminar = ref(null)

const esAdministrador = computed(() => autenticacion.esAdministrador)

async function cargar() {
  cargando.value = true
  try {
    proveedores.value = await listarProveedores(incluirInactivos.value)
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

function abrirAlta() {
  modoEdicion.value = false
  Object.assign(formulario, {
    nombre: '',
    contacto: '',
    telefono: '',
    correo: '',
    direccion: '',
    activo: true,
  })
  modalAbierto.value = true
}

function abrirEdicion(proveedor) {
  modoEdicion.value = true
  idEdicion.value = proveedor.idProveedor
  Object.assign(formulario, {
    nombre: proveedor.nombre,
    contacto: proveedor.contacto || '',
    telefono: proveedor.telefono || '',
    correo: proveedor.correo || '',
    direccion: proveedor.direccion || '',
    activo: proveedor.activo,
  })
  modalAbierto.value = true
}

async function guardar() {
  guardando.value = true
  try {
    const datos = {
      nombre: formulario.nombre.trim(),
      contacto: formulario.contacto?.trim() || null,
      telefono: formulario.telefono?.trim() || null,
      correo: formulario.correo?.trim() || null,
      direccion: formulario.direccion?.trim() || null,
      activo: formulario.activo,
    }

    if (modoEdicion.value) {
      await actualizarProveedor(idEdicion.value, datos)
      notificaciones.exito('Proveedor actualizado correctamente.')
    } else {
      await crearProveedor(datos)
      notificaciones.exito('Proveedor creado correctamente.')
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
    await eliminarProveedor(proveedorAEliminar.value.idProveedor)
    notificaciones.exito('Proveedor dado de baja correctamente.')
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
        Nuevo proveedor
      </button>
    </div>

    <!-- Tabla -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!proveedores.length"
      mensaje-vacio="No hay proveedores registrados."
      :colapsadas="esAdministrador ? 6 : 5"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Nombre</th>
        <th class="px-4 py-3 font-semibold">Contacto</th>
        <th class="px-4 py-3 font-semibold">Teléfono</th>
        <th class="px-4 py-3 font-semibold">Correo</th>
        <th class="px-4 py-3 text-right font-semibold">Productos</th>
        <th v-if="esAdministrador" class="px-4 py-3 text-right font-semibold">Acciones</th>
      </template>

      <template #filas>
        <tr
          v-for="proveedor in proveedores"
          :key="proveedor.idProveedor"
          :class="['transition hover:bg-slate-50', !proveedor.activo && 'opacity-50']"
        >
          <td class="px-4 py-3">
            <p class="flex items-center gap-2 font-medium text-slate-800">
              <Truck :size="15" class="text-indigo-500" />
              {{ proveedor.nombre }}
            </p>
            <p class="ml-6 text-xs text-slate-400">{{ proveedor.direccion || 'Sin dirección' }}</p>
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-600">{{ proveedor.contacto || '—' }}</td>
          <td class="whitespace-nowrap px-4 py-3">
            <span class="flex items-center gap-1.5 text-slate-600">
              <Phone :size="13" class="text-slate-400" />
              {{ proveedor.telefono || '—' }}
            </span>
          </td>
          <td class="whitespace-nowrap px-4 py-3">
            <span class="flex items-center gap-1.5 text-slate-600">
              <Mail :size="13" class="text-slate-400" />
              {{ proveedor.correo || '—' }}
            </span>
          </td>
          <td class="px-4 py-3 text-right font-semibold text-slate-700">{{ proveedor.totalProductos }}</td>
          <td v-if="esAdministrador" class="px-4 py-3 text-right">
            <div class="inline-flex gap-1">
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
                title="Modificar"
                @click="abrirEdicion(proveedor)"
              >
                <Pencil :size="16" />
              </button>
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
                title="Dar de baja"
                @click="proveedorAEliminar = proveedor; eliminarAbierto = true"
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
      :titulo="modoEdicion ? 'Modificar proveedor' : 'Nuevo proveedor'"
      ancho="max-w-2xl"
      @cerrar="modalAbierto = false"
    >
      <form class="grid grid-cols-1 gap-4 sm:grid-cols-2" @submit.prevent="guardar">
        <div class="sm:col-span-2">
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Nombre *</label>
          <input
            v-model="formulario.nombre"
            type="text"
            maxlength="150"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Persona de contacto</label>
          <input
            v-model="formulario.contacto"
            type="text"
            maxlength="100"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Teléfono</label>
          <input
            v-model="formulario.telefono"
            type="text"
            maxlength="30"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Correo</label>
          <input
            v-model="formulario.correo"
            type="email"
            maxlength="100"
            placeholder="ventas@proveedor.cl"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Dirección</label>
          <input
            v-model="formulario.direccion"
            type="text"
            maxlength="200"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>
        <div v-if="modoEdicion" class="flex items-center gap-2">
          <input id="prov-activo" v-model="formulario.activo" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
          <label for="prov-activo" class="text-sm text-slate-600">Proveedor activo</label>
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
          {{ modoEdicion ? 'Guardar cambios' : 'Crear proveedor' }}
        </button>
      </template>
    </ModalBase>

    <!-- Confirmación de baja -->
    <ModalBase
      :abierto="eliminarAbierto"
      titulo="Confirmar baja de proveedor"
      @cerrar="eliminarAbierto = false"
    >
      <p class="text-sm text-slate-600">
        ¿Está seguro de dar de baja al proveedor
        <strong>{{ proveedorAEliminar?.nombre }}</strong>?
        Sus productos y movimientos históricos se conservarán.
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
