<script setup>
/**
 * Panel principal: indicadores clave (KPIs), alertas de stock bajo y
 * últimos movimientos registrados.
 */
import { onMounted, ref } from 'vue'
import {
  Package,
  Boxes,
  ArrowLeftRight,
  TriangleAlert,
  CircleDollarSign,
  Truck,
  ArrowUpRight,
  ArrowDownRight,
} from 'lucide-vue-next'
import { obtenerResumen } from '../api/servicios'
import { formatoMoneda, formatoNumero, formatoFecha } from '../utils/formato'
import TarjetaKPI from '../componentes/TarjetaKPI.vue'
import TablaContenedora from '../componentes/TablaContenedora.vue'
import BadgeStock from '../componentes/BadgeStock.vue'

const resumen = ref(null)
const cargando = ref(true)

onMounted(async () => {
  try {
    resumen.value = await obtenerResumen()
  } finally {
    cargando.value = false
  }
})
</script>

<template>
  <div class="space-y-6">
    <!-- ===================== KPIs ===================== -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <TarjetaKPI
        titulo="Productos activos"
        :valor="cargando ? '…' : formatoNumero(resumen?.totalProductos)"
        :texto="`${formatoNumero(resumen?.totalProveedores || 0)} proveedores`"
        color="indigo"
      >
        <template #icono><Package :size="22" /></template>
      </TarjetaKPI>

      <TarjetaKPI
        titulo="Valor de inventario"
        :valor="cargando ? '…' : formatoMoneda(resumen?.valorInventario)"
        :texto="`${formatoNumero(resumen?.totalAlmacenes || 0)} almacenes`"
        color="emerald"
      >
        <template #icono><CircleDollarSign :size="22" /></template>
      </TarjetaKPI>

      <TarjetaKPI
        titulo="Movimientos del mes"
        :valor="cargando ? '…' : formatoNumero(resumen?.movimientosMes)"
        texto="Entradas y salidas"
        color="amber"
      >
        <template #icono><ArrowLeftRight :size="22" /></template>
      </TarjetaKPI>

      <TarjetaKPI
        titulo="Stock crítico"
        :valor="cargando ? '…' : formatoNumero(resumen?.productosStockCritico)"
        texto="Productos bajo el mínimo"
        color="rose"
      >
        <template #icono><TriangleAlert :size="22" /></template>
      </TarjetaKPI>
    </div>

    <div class="grid grid-cols-1 gap-6 xl:grid-cols-2">
      <!-- ================ Alertas de stock ================ -->
      <section class="rounded-xl border border-slate-200 bg-white shadow-sm">
        <div class="flex items-center justify-between border-b border-slate-100 px-5 py-4">
          <div class="flex items-center gap-2">
            <TriangleAlert :size="18" class="text-rose-500" />
            <h2 class="font-semibold text-slate-800">Alertas de stock bajo</h2>
          </div>
          <router-link
            :to="{ name: 'reportes' }"
            class="text-xs font-semibold text-indigo-600 hover:underline"
          >
            Ver reporte
          </router-link>
        </div>

        <div class="p-5">
          <p v-if="cargando" class="py-6 text-center text-sm text-slate-400">Cargando…</p>
          <p v-else-if="!resumen?.alertas?.length" class="py-6 text-center text-sm text-slate-400">
            No hay productos con stock crítico. ¡Todo en orden!
          </p>
          <ul v-else class="space-y-3">
            <li
              v-for="alerta in resumen.alertas"
              :key="alerta.idProducto"
              class="flex items-center justify-between gap-3 rounded-lg border border-rose-100 bg-rose-50/60 px-4 py-3"
            >
              <div class="min-w-0">
                <p class="truncate text-sm font-medium text-slate-800">{{ alerta.nombre }}</p>
                <p class="text-xs text-slate-500">
                  {{ alerta.codigo }} · {{ alerta.nombreCategoria }}
                </p>
              </div>
              <div class="shrink-0 text-right">
                <p class="text-sm font-bold text-rose-600">
                  {{ alerta.stockActual }} / {{ alerta.stockMinimo }}
                </p>
                <p class="text-xs text-slate-400">faltan {{ alerta.diferencia }}</p>
              </div>
            </li>
          </ul>
        </div>
      </section>

      <!-- ============ Últimos movimientos ============ -->
      <section class="rounded-xl border border-slate-200 bg-white shadow-sm">
        <div class="flex items-center justify-between border-b border-slate-100 px-5 py-4">
          <div class="flex items-center gap-2">
            <ArrowLeftRight :size="18" class="text-indigo-600" />
            <h2 class="font-semibold text-slate-800">Últimos movimientos</h2>
          </div>
          <router-link
            :to="{ name: 'movimientos' }"
            class="text-xs font-semibold text-indigo-600 hover:underline"
          >
            Ver todos
          </router-link>
        </div>

        <div class="p-5">
          <p v-if="cargando" class="py-6 text-center text-sm text-slate-400">Cargando…</p>
          <TablaContenedora
            v-else
            :cargando="false"
            :vacio="!resumen?.ultimosMovimientos?.length"
            mensaje-vacio="Todavía no hay movimientos registrados."
            :colapsadas="5"
          >
            <template #encabezados>
              <th class="px-4 py-3 font-semibold">Fecha</th>
              <th class="px-4 py-3 font-semibold">Tipo</th>
              <th class="px-4 py-3 font-semibold">Motivo</th>
              <th class="px-4 py-3 font-semibold">Documento</th>
              <th class="px-4 py-3 text-right font-semibold">Total</th>
            </template>
            <template #filas>
              <tr v-for="movimiento in resumen.ultimosMovimientos" :key="movimiento.idMovimiento" class="transition hover:bg-slate-50">
                <td class="whitespace-nowrap px-4 py-3 text-slate-600">
                  {{ formatoFecha(movimiento.fecha) }}
                </td>
                <td class="px-4 py-3">
                  <span
                    :class="[
                      'inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold',
                      movimiento.tipo === 'Entrada'
                        ? 'bg-emerald-100 text-emerald-700'
                        : 'bg-amber-100 text-amber-700',
                    ]"
                  >
                    <component
                      :is="movimiento.tipo === 'Entrada' ? ArrowDownRight : ArrowUpRight"
                      :size="13"
                    />
                    {{ movimiento.tipo }}
                  </span>
                </td>
                <td class="max-w-[200px] truncate px-4 py-3 text-slate-600">
                  {{ movimiento.motivo || '—' }}
                </td>
                <td class="px-4 py-3 text-slate-500">{{ movimiento.documentoReferencia || '—' }}</td>
                <td class="whitespace-nowrap px-4 py-3 text-right font-semibold text-slate-700">
                  {{ formatoMoneda(movimiento.total) }}
                </td>
              </tr>
            </template>
          </TablaContenedora>
        </div>
      </section>
    </div>

    <!-- ======== Resumen de productos con stock crítico (tabla) ======== -->
    <section v-if="resumen?.alertas?.length" class="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
      <div class="mb-4 flex items-center gap-2">
        <Boxes :size="18" class="text-rose-500" />
        <h2 class="font-semibold text-slate-800">Productos que requieren reposición</h2>
      </div>
      <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-3">
        <div
          v-for="alerta in resumen.alertas"
          :key="alerta.idProducto"
          class="flex items-center justify-between rounded-lg border border-slate-200 px-4 py-3"
        >
          <div class="min-w-0">
            <p class="truncate text-sm font-medium text-slate-700">{{ alerta.nombre }}</p>
            <p class="text-xs text-slate-400">{{ alerta.codigo }}</p>
          </div>
          <BadgeStock
            :critico="true"
            :stock-actual="alerta.stockActual"
            :stock-minimo="alerta.stockMinimo"
          />
        </div>
      </div>
    </section>
  </div>
</template>
