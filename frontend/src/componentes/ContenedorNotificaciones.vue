<script setup>
/**
 * Contenedor de notificaciones flotantes (toasts) de esquina superior derecha.
 */
import { CircleCheck, CircleAlert, Info, X } from 'lucide-vue-next'
import { useNotificaciones } from '../stores/notificaciones'

const notificaciones = useNotificaciones()

const estilos = {
  exito: 'border-emerald-200 bg-emerald-50 text-emerald-800',
  error: 'border-rose-200 bg-rose-50 text-rose-800',
  informacion: 'border-sky-200 bg-sky-50 text-sky-800',
}
</script>

<template>
  <div class="fixed right-4 top-4 z-[60] flex w-80 flex-col gap-2">
    <TransitionGroup name="aviso">
      <div
        v-for="aviso in notificaciones.avisos"
        :key="aviso.id"
        :class="['flex items-start gap-3 rounded-lg border p-3 shadow-lg', estilos[aviso.tipo] || estilos.informacion]"
        role="alert"
      >
        <CircleCheck v-if="aviso.tipo === 'exito'" :size="18" class="mt-0.5 shrink-0" />
        <CircleAlert v-else-if="aviso.tipo === 'error'" :size="18" class="mt-0.5 shrink-0" />
        <Info v-else :size="18" class="mt-0.5 shrink-0" />
        <p class="flex-1 text-sm">{{ aviso.mensaje }}</p>
        <button
          type="button"
          class="shrink-0 opacity-60 transition hover:opacity-100"
          aria-label="Cerrar aviso"
          @click="notificaciones.cerrar(aviso.id)"
        >
          <X :size="16" />
        </button>
      </div>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.aviso-enter-active,
.aviso-leave-active {
  transition: all 0.3s ease;
}

.aviso-enter-from,
.aviso-leave-to {
  opacity: 0;
  transform: translateX(24px);
}
</style>
