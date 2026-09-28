<script setup>
/**
 * Badge de estado de stock: "Crítico" (rojo) o "Normal" (verde).
 */
defineProps({
  critico: { type: Boolean, default: false },
  stockActual: { type: Number, default: null },
  stockMinimo: { type: Number, default: null },
})
</script>

<template>
  <span
    v-if="critico"
    class="inline-flex items-center gap-1 rounded-full bg-rose-100 px-2.5 py-0.5 text-xs font-semibold text-rose-700"
  >
    <span class="h-1.5 w-1.5 rounded-full bg-rose-500"></span>
    Crítico<span v-if="stockActual !== null">({{ stockActual }}/{{ stockMinimo }})</span>
  </span>
  <span
    v-else
    class="inline-flex items-center gap-1 rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-semibold text-emerald-700"
  >
    <span class="h-1.5 w-1.5 rounded-full bg-emerald-500"></span>
    Normal<span v-if="stockActual !== null">({{ stockActual }})</span>
  </span>
</template>
