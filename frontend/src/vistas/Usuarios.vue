<script setup>
/**
 * Administración de usuarios y roles (acceso exclusivo del Administrador).
 */
import { computed, onMounted, reactive, ref } from 'vue'
import { Plus, Pencil, Trash2, RotateCw, ShieldCheck, UserRound } from 'lucide-vue-next'
import {
  listarUsuarios,
  listarRoles,
  crearUsuario,
  actualizarUsuario,
  eliminarUsuario,
} from '../api/servicios'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'
import { formatoFechaCorta } from '../utils/formato'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import ModalBase from '../componentes/ModalBase.vue'

const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const usuarios = ref([])
const roles = ref([])
const cargando = ref(true)

const modalAbierto = ref(false)
const guardando = ref(false)
const modoEdicion = ref(false)
const idEdicion = ref(null)
const formulario = reactive({
  nombreUsuario: '',
  nombreCompleto: '',
  correo: '',
  contrasena: '',
  idRol: '',
  activo: true,
})

const eliminarAbierto = ref(false)
const usuarioAEliminar = ref(null)

/** Colores por rol para los badges. */
const coloresRol = {
  Administrador: 'bg-indigo-100 text-indigo-700',
  Usuario: 'bg-sky-100 text-sky-700',
  Auditor: 'bg-amber-100 text-amber-700',
}

async function cargar() {
  cargando.value = true
  try {
    const [usuariosRespuesta, rolesRespuesta] = await Promise.all([listarUsuarios(), listarRoles()])
    usuarios.value = usuariosRespuesta
    roles.value = rolesRespuesta
  } catch (e) {
    notificaciones.error(e.message)
  } finally {
    cargando.value = false
  }
}

function abrirAlta() {
  modoEdicion.value = false
  Object.assign(formulario, {
    nombreUsuario: '',
    nombreCompleto: '',
    correo: '',
    contrasena: '',
    idRol: roles.value.find((r) => r.nombre === 'Usuario')?.idRol || '',
    activo: true,
  })
  modalAbierto.value = true
}

function abrirEdicion(usuario) {
  modoEdicion.value = true
  idEdicion.value = usuario.idUsuario
  Object.assign(formulario, {
    nombreUsuario: usuario.nombreUsuario,
    nombreCompleto: usuario.nombreCompleto,
    correo: usuario.correo,
    contrasena: '', // vacío = no cambiar contraseña
    idRol: usuario.idRol,
    activo: usuario.activo,
  })
  modalAbierto.value = true
}

async function guardar() {
  guardando.value = true
  try {
    const datos = {
      nombreUsuario: formulario.nombreUsuario.trim(),
      nombreCompleto: formulario.nombreCompleto.trim(),
      correo: formulario.correo.trim(),
      idRol: Number(formulario.idRol),
      activo: formulario.activo,
      ...(modoEdicion.value
        ? { contrasena: formulario.contrasena?.trim() || null }
        : { contrasena: formulario.contrasena }),
    }

    if (modoEdicion.value) {
      await actualizarUsuario(idEdicion.value, datos)
      notificaciones.exito('Usuario actualizado correctamente.')
    } else {
      await crearUsuario(datos)
      notificaciones.exito('Usuario creado correctamente.')
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
    await eliminarUsuario(usuarioAEliminar.value.idUsuario)
    notificaciones.exito('Usuario dado de baja correctamente.')
    eliminarAbierto.value = false
    await cargar()
  } catch (e) {
    notificaciones.error(e.message)
  }
}

const noPuedeEliminarse = computed(
  () => usuarioAEliminar.value?.idUsuario === autenticacion.usuario?.idUsuario,
)

onMounted(cargar)
</script>

<template>
  <div class="space-y-4">
    <!-- Barra de herramientas -->
    <div class="flex items-center justify-between">
      <p class="text-sm text-slate-500">
        Gestione los usuarios del sistema y sus roles de acceso.
      </p>
      <div class="flex items-center gap-3">
        <button
          type="button"
          class="rounded-lg border border-slate-300 bg-white p-2 text-slate-500 transition hover:bg-slate-50"
          title="Actualizar listado"
          @click="cargar"
        >
          <RotateCw :size="16" />
        </button>
        <button
          type="button"
          class="flex items-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700"
          @click="abrirAlta"
        >
          <Plus :size="17" />
          Nuevo usuario
        </button>
      </div>
    </div>

    <!-- Tabla -->
    <TablaContenedora
      :cargando="cargando"
      :vacio="!usuarios.length"
      mensaje-vacio="No hay usuarios registrados."
      :colapsadas="7"
    >
      <template #encabezados>
        <th class="px-4 py-3 font-semibold">Usuario</th>
        <th class="px-4 py-3 font-semibold">Nombre completo</th>
        <th class="px-4 py-3 font-semibold">Correo</th>
        <th class="px-4 py-3 font-semibold">Rol</th>
        <th class="px-4 py-3 font-semibold">Creado</th>
        <th class="px-4 py-3 font-semibold">Estado</th>
        <th class="px-4 py-3 text-right font-semibold">Acciones</th>
      </template>

      <template #filas>
        <tr
          v-for="usuario in usuarios"
          :key="usuario.idUsuario"
          :class="['transition hover:bg-slate-50', !usuario.activo && 'opacity-50']"
        >
          <td class="px-4 py-3">
            <span class="flex items-center gap-2 font-medium text-slate-800">
              <span class="flex h-8 w-8 items-center justify-center rounded-full bg-slate-200 text-xs font-bold text-slate-600">
                {{ usuario.nombreCompleto?.charAt(0)?.toUpperCase() }}
              </span>
              {{ usuario.nombreUsuario }}
            </span>
          </td>
          <td class="px-4 py-3 text-slate-600">{{ usuario.nombreCompleto }}</td>
          <td class="px-4 py-3 text-slate-600">{{ usuario.correo }}</td>
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold',
                coloresRol[usuario.nombreRol] || 'bg-slate-100 text-slate-600',
              ]"
            >
              <ShieldCheck :size="12" />
              {{ usuario.nombreRol }}
            </span>
          </td>
          <td class="whitespace-nowrap px-4 py-3 text-slate-500">
            {{ formatoFechaCorta(usuario.fechaCreacion) }}
          </td>
          <td class="px-4 py-3">
            <span
              :class="[
                'rounded-full px-2.5 py-0.5 text-xs font-semibold',
                usuario.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-500',
              ]"
            >
              {{ usuario.activo ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td class="px-4 py-3 text-right">
            <div class="inline-flex gap-1">
              <button
                type="button"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-indigo-50 hover:text-indigo-600"
                title="Modificar"
                @click="abrirEdicion(usuario)"
              >
                <Pencil :size="16" />
              </button>
              <button
                type="button"
                :disabled="usuario.idUsuario === autenticacion.usuario?.idUsuario"
                class="rounded-lg p-2 text-slate-400 transition hover:bg-rose-50 hover:text-rose-600 disabled:cursor-not-allowed disabled:opacity-30"
                title="Dar de baja"
                @click="usuarioAEliminar = usuario; eliminarAbierto = true"
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
      :titulo="modoEdicion ? 'Modificar usuario' : 'Nuevo usuario'"
      @cerrar="modalAbierto = false"
    >
      <form class="space-y-4" @submit.prevent="guardar">
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Nombre de usuario *</label>
            <input
              v-model="formulario.nombreUsuario"
              type="text"
              maxlength="50"
              required
              autocomplete="off"
              class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            />
          </div>
          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Nombre completo *</label>
            <input
              v-model="formulario.nombreCompleto"
              type="text"
              maxlength="100"
              required
              class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            />
          </div>
        </div>

        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Correo *</label>
          <input
            v-model="formulario.correo"
            type="email"
            maxlength="100"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>

        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">
            Contraseña {{ modoEdicion ? '(vacía = sin cambio)' : '*' }}
          </label>
          <input
            v-model="formulario.contrasena"
            type="password"
            minlength="8"
            :required="!modoEdicion"
            autocomplete="new-password"
            :placeholder="modoEdicion ? '••••••••' : 'Mínimo 8 caracteres'"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
          />
        </div>

        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label class="mb-1.5 block text-sm font-medium text-slate-700">Rol *</label>
            <select
              v-model="formulario.idRol"
              required
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-indigo-500"
            >
              <option disabled value="">Seleccione…</option>
              <option v-for="rol in roles" :key="rol.idRol" :value="rol.idRol">
                {{ rol.nombre }}
              </option>
            </select>
            <p class="mt-1 text-xs text-slate-400">
              {{ roles.find((r) => r.idRol === formulario.idRol)?.descripcion }}
            </p>
          </div>
          <div class="flex items-end pb-1">
            <label class="flex cursor-pointer items-center gap-2 text-sm text-slate-600">
              <input v-model="formulario.activo" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
              Usuario activo
            </label>
          </div>
        </div>

        <!-- Referencia de roles -->
        <div class="rounded-lg bg-slate-50 p-3 text-xs text-slate-500">
          <p class="mb-1 flex items-center gap-1 font-semibold text-slate-600">
            <UserRound :size="13" /> Permisos por rol:
          </p>
          <ul class="list-inside list-disc space-y-0.5">
            <li><strong>Administrador:</strong> acceso total a todo el sistema.</li>
            <li><strong>Usuario:</strong> consulta productos y registra movimientos.</li>
            <li><strong>Auditor:</strong> solo lectura: consultas y reportes.</li>
          </ul>
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
          {{ modoEdicion ? 'Guardar cambios' : 'Crear usuario' }}
        </button>
      </template>
    </ModalBase>

    <!-- Confirmación de baja -->
    <ModalBase
      :abierto="eliminarAbierto"
      titulo="Confirmar baja de usuario"
      @cerrar="eliminarAbierto = false"
    >
      <p v-if="noPuedeEliminarse" class="text-sm text-rose-600">
        No puede darse de baja a sí mismo.
      </p>
      <p v-else class="text-sm text-slate-600">
        ¿Está seguro de dar de baja al usuario
        <strong>{{ usuarioAEliminar?.nombreUsuario }}</strong>?
        Dejará de poder iniciar sesión; su historial de movimientos se conservará.
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
          v-if="!noPuedeEliminarse"
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
