<script setup>
/**
 * Tarjeta de indicador (KPI) del panel principal.
 */
defineProps({
  titulo: { type: String, required: true },
  valor: { type: [String, Number], required: true },
  texto: { type: String, default: '' },
  color: { type: String, default: 'indigo' }, // indigo | emerald | amber | rose
})

const fondos = {
  indigo: 'bg-indigo-50 text-indigo-600',
  emerald: 'bg-emerald-50 text-emerald-600',
  amber: 'bg-amber-50 text-amber-600',
  rose: 'bg-rose-50 text-rose-600',
}
</script>

<template>
  <div class="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
    <div class="flex items-center gap-4">
      <div :class="['flex h-12 w-12 shrink-0 items-center justify-center rounded-lg', fondos[color]]">
        <slot name="icono" />
      </div>
      <div class="min-w-0">
        <p class="truncate text-sm font-medium text-slate-500">{{ titulo }}</p>
        <p class="truncate text-2xl font-bold text-slate-800">{{ valor }}</p>
        <p v-if="texto" class="truncate text-xs text-slate-400">{{ texto }}</p>
      </div>
    </div>
  </div>
</template>
