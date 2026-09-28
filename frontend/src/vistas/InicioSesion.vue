<script setup>
/**
 * Vista de inicio de sesión: autentica contra la API y redirige al panel.
 */
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Boxes, LogIn, Eye, EyeOff } from 'lucide-vue-next'
import { useAutenticacion } from '../stores/autenticacion'
import { useNotificaciones } from '../stores/notificaciones'

const navegador = useRouter()
const autenticacion = useAutenticacion()
const notificaciones = useNotificaciones()

const nombreUsuario = ref('')
const contrasena = ref('')
const mostrarContrasena = ref(false)
const cargando = ref(false)
const error = ref('')

async function enviar() {
  if (!nombreUsuario.value || !contrasena.value) {
    error.value = 'Ingrese su usuario y contraseña.'
    return
  }

  cargando.value = true
  error.value = ''

  try {
    await autenticacion.iniciarSesion({
      nombreUsuario: nombreUsuario.value.trim(),
      contrasena: contrasena.value,
    })
    notificaciones.exito('Bienvenido a ControlInventario.')
    navegador.push({ name: 'panel' })
  } catch (e) {
    error.value = e.message || 'No se pudo iniciar sesión.'
  } finally {
    cargando.value = false
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-indigo-900 p-4">
    <div class="w-full max-w-md">
      <!-- Tarjeta de acceso -->
      <div class="rounded-2xl bg-white p-8 shadow-2xl">
        <!-- Marca -->
        <div class="mb-8 text-center">
          <div class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-2xl bg-indigo-600 shadow-lg shadow-indigo-600/40">
            <Boxes :size="30" class="text-white" />
          </div>
          <h1 class="text-2xl font-bold text-slate-800">ControlInventario</h1>
          <p class="mt-1 text-sm text-slate-500">
            Ingrese sus credenciales para acceder al sistema
          </p>
        </div>

        <!-- Error -->
        <div
          v-if="error"
          class="mb-4 rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700"
          role="alert"
        >
          {{ error }}
        </div>

        <form class="space-y-5" novalidate @submit.prevent="enviar">
          <!-- Usuario -->
          <div>
            <label for="nombreUsuario" class="mb-1.5 block text-sm font-medium text-slate-700">
              Usuario
            </label>
            <input
              id="nombreUsuario"
              v-model="nombreUsuario"
              type="text"
              autocomplete="username"
              placeholder="Ej.: admin"
              class="w-full rounded-lg border border-slate-300 px-3.5 py-2.5 text-sm outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
            />
          </div>

          <!-- Contraseña -->
          <div>
            <label for="contrasena" class="mb-1.5 block text-sm font-medium text-slate-700">
              Contraseña
            </label>
            <div class="relative">
              <input
                id="contrasena"
                v-model="contrasena"
                :type="mostrarContrasena ? 'text' : 'password'"
                autocomplete="current-password"
                placeholder="••••••••"
                class="w-full rounded-lg border border-slate-300 px-3.5 py-2.5 pr-11 text-sm outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              />
              <button
                type="button"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"
                :aria-label="mostrarContrasena ? 'Ocultar contraseña' : 'Mostrar contraseña'"
                @click="mostrarContrasena = !mostrarContrasena"
              >
                <EyeOff v-if="mostrarContrasena" :size="18" />
                <Eye v-else :size="18" />
              </button>
            </div>
          </div>

          <!-- Botón -->
          <button
            type="submit"
            :disabled="cargando"
            class="flex w-full items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            <LogIn v-if="!cargando" :size="18" />
            <span v-else class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent"></span>
            {{ cargando ? 'Verificando…' : 'Iniciar sesión' }}
          </button>
        </form>

        <p class="mt-6 text-center text-xs text-slate-400">
          Acceso restringido a usuarios autorizados del sistema.
        </p>
      </div>

      <p class="mt-4 text-center text-xs text-slate-500">
        ¿Problemas para acceder? Contacte al administrador del sistema.
      </p>
    </div>
  </div>
</template>
