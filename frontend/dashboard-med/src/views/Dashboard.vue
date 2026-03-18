<script setup>
import { computed, onMounted } from 'vue'
import StatCard from '@/components/ui/StatsCard.vue'
import TablaCitasHoy from '@/components/citas/TablaCitasHoy.vue'
import { useCitasStore } from '@/stores/citas'

const citasStore = useCitasStore()

const citas = computed(() => citasStore.citas)

//onMounted (con refresco cada 30 segundos)
onMounted(() => {
  citasStore.cargarCitas()
  // refrescar cada 30 segundos
  setInterval(() => {
    citasStore.cargarCitas()
  }, 30000)
})

/* SALUDO */
const saludo = computed(() => {
  const hora = new Date().getHours()
  if (hora < 12) return "Buenos días"
  if (hora < 19) return "Buenas tardes"
  return "Buenas noches"
})

/* FECHA */
const fechaHoy = new Date().toLocaleDateString('es-SV', {
  weekday: 'long',
  year: 'numeric',
  month: 'long',
  day: 'numeric'
})

/* CITAS DE HOY */
const citasHoyLista = computed(() => {

  if (!Array.isArray(citas.value)) return []

  const hoy = new Date().toDateString()

  return citas.value.filter(c =>
    new Date(c.Start).toDateString() === hoy
  )
})

/* PRÓXIMA CITA */
const proximaCita = computed(() => {

  const ahora = new Date()

  const futuras = citasHoyLista.value
    .filter(c => new Date(c.Start) > ahora)
    .sort((a,b) => new Date(a.Start) - new Date(b.Start))

  return futuras[0] || null

})

/* STATS */

const citasHoy = computed(() => citasHoyLista.value.length)

const enEspera = computed(() =>
  citasHoyLista.value.filter(c => c.Estado === "Pendiente").length
)

const enConsulta = computed(() =>
  citasHoyLista.value.filter(c => c.Estado === "EnConsulta").length
)

const finalizadas = computed(() =>
  citasHoyLista.value.filter(c => c.Estado === "Finalizada").length
)

</script>

<template>

<div class="space-y-6">

  <!-- SALUDO -->
  <div>
    <h1 class="text-2xl font-bold text-gray-800">
      {{ saludo }}, Doctor
    </h1>

    <p class="text-gray-500 capitalize">
      {{ fechaHoy }}
    </p>
  </div>

 <!-- PRÓXIMA CITA -->
<div
  v-if="proximaCita"
  class="bg-white border rounded-xl shadow p-4"
>

  <h2 class="font-semibold text-steel-blue mb-2">
    Próxima consulta
  </h2>

  <p>
    <strong>Paciente:</strong>
    {{ proximaCita.PacienteNombreCompleto }}
  </p>

  <p>
    <strong>Hora:</strong>
    {{ new Date(proximaCita.Start).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) }}
  </p>

  <p>
    <strong>Motivo:</strong>
    {{ proximaCita.Titulo }}
  </p>

</div>

  <!-- STATS -->
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">

    <StatCard
      title="Citas hoy"
      :value="citasHoy"
      color="text-deep-sky-blue"
    />

    <StatCard
      title="En espera"
      :value="enEspera"
      color="text-amber-500"
    />

    <StatCard
      title="En consulta"
      :value="enConsulta"
      color="text-teal"
    />

    <StatCard
      title="Finalizadas"
      :value="finalizadas"
      color="text-emerald-500"
    />

  </div>

  <!-- TABLA -->
  <TablaCitasHoy
    :citas="citasHoyLista"
  />
</div>
</template>