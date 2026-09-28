<script setup>
/**
 * Tabla simple con estado de carga y mensaje cuando no hay filas.
 */
defineProps({
  cargando: { type: Boolean, default: false },
  vacio: { type: Boolean, default: false },
  mensajeVacio: { type: String, default: 'No se encontraron registros.' },
  colapsadas: { type: Number, default: 1 },
})
</script>

<template>
  <div class="overflow-x-auto rounded-xl border border-slate-200 bg-white shadow-sm">
    <table class="w-full min-w-[640px] text-left text-sm">
      <thead class="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
        <tr>
          <slot name="encabezados" />
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="cargando">
          <td :colspan="colapsadas" class="px-4 py-10 text-center text-slate-400">
            <span class="inline-block animate-pulse">Cargando información…</span>
          </td>
        </tr>
        <tr v-else-if="vacio">
          <td :colspan="colapsadas" class="px-4 py-10 text-center text-slate-400">
            {{ mensajeVacio }}
          </td>
        </tr>
        <slot v-else name="filas" />
      </tbody>
    </table>
  </div>
</template>
