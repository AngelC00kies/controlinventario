<script setup>
/**
 * Modal reutilizable: fondo oscuro, encabezado con título y pie con botones.
 * Se cierra con Esc o al pulsar fuera del panel.
 */
import { onMounted, onUnmounted } from 'vue'
import { X } from 'lucide-vue-next'

const props = defineProps({
  titulo: { type: String, required: true },
  abierto: { type: Boolean, default: false },
  ancho: { type: String, default: 'max-w-lg' },
})

const emitir = defineEmits(['cerrar'])

function alPresionarEsc(evento) {
  if (evento.key === 'Escape' && props.abierto) emitir('cerrar')
}

onMounted(() => document.addEventListener('keydown', alPresionarEsc))
onUnmounted(() => document.removeEventListener('keydown', alPresionarEsc))
</script>

<template>
  <Teleport to="body">
    <div
      v-if="abierto"
      class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/50 p-4"
      @mousedown.self="emitir('cerrar')"
    >
      <div :class="['w-full rounded-xl bg-white shadow-2xl', ancho]">
        <!-- Encabezado -->
        <div class="flex items-center justify-between border-b border-slate-200 px-6 py-4">
          <h3 class="text-lg font-semibold text-slate-800">{{ titulo }}</h3>
          <button
            type="button"
            class="rounded-lg p-1 text-slate-400 transition hover:bg-slate-100 hover:text-slate-600"
            aria-label="Cerrar"
            @click="emitir('cerrar')"
          >
            <X :size="20" />
          </button>
        </div>

        <!-- Contenido -->
        <div class="max-h-[70vh] overflow-y-auto px-6 py-4">
          <slot />
        </div>

        <!-- Pie (botones de acción) -->
        <div v-if="$slots.pie" class="flex justify-end gap-3 border-t border-slate-200 px-6 py-4">
          <slot name="pie" />
        </div>
      </div>
    </div>
  </Teleport>
</template>
