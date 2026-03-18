<script setup>
import { ref, computed, onMounted } from 'vue'
import Calendar from '@/components/calendar/Calendar.vue'
import StatCard from '@/components/ui/StatsCard.vue'
import { useRouter } from 'vue-router'
import { useCitasStore } from '@/stores/citas'

const router = useRouter()
const citasStore = useCitasStore()

const citas = computed(() => citasStore.citas)
const selectedCita = ref(null)

/* CARGAR CITAS */
onMounted(() => {
  citasStore.cargarCitas()
})

/* CITAS DE HOY */
const citasHoyLista = computed(() => {

  if (!Array.isArray(citas.value)) return []

  const hoy = new Date().toDateString()

  return citas.value.filter(c =>
    new Date(c.Start).toDateString() === hoy
  )
})

/* SELECCIONAR CITA */
function seleccionarCita(cita) {

  console.log("Cita seleccionada:", cita)

  selectedCita.value = cita

  if (cita?.IdCita) {
    router.push(`/consultas/${cita.IdCita}`)
  }
}

/* MOVER CITA */
function moverCita(cita){
  console.log("Cita movida:", cita)
}

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

<div class="space-y-6 w-full">

  <h1 class="text-2xl font-bold text-gray-800">
    Agenda Médica
  </h1>

  <div class="grid grid-cols-1 lg:grid-cols-4 gap-6 w-full">

    <!-- STATS -->
    <div class="space-y-4 lg:col-span-1">
      <StatCard title="Citas hoy" :value="citasHoy" color="text-deep-sky-blue"/>
      <StatCard title="En espera" :value="enEspera" color="text-amber-500"/>
      <StatCard title="En consulta" :value="enConsulta" color="text-teal"/>
      <StatCard title="Finalizadas" :value="finalizadas" color="text-emerald-500"/>
    </div>

    <!-- CALENDARIO -->
    <div class="lg:col-span-3 bg-white p-6 rounded-xl shadow border w-full">

      <Calendar
        v-if="citas.length"
        :key="citas.length"
        :citas="citas"
        @cita-click="seleccionarCita"
        @cita-movida="moverCita"
      />

      <div v-else class="text-gray-400 text-sm">
        Cargando citas...
      </div>

    </div>

    <!-- DETALLE -->
    <div class="bg-white p-6 rounded-xl shadow border mt-6 lg:col-span-4">

      <h2 class="text-lg border-l-4 border-blue-500 font-semibold text-gray-700 mb-4">
        Detalle de la cita
      </h2>

      <div v-if="selectedCita">

        <p>
          <strong>Paciente:</strong>
          {{ selectedCita.PacienteNombreCompleto }}
        </p>

        <p>
          <strong>Hora:</strong>
          {{ new Date(selectedCita.Start).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) }}
        </p>

        <p>
          <strong>Tipo:</strong>
          {{ selectedCita.TipoConsulta }}
        </p>

        <p>
          <strong>Estado:</strong>
          {{ selectedCita.Estado }}
        </p>

        <router-link
          :to="`/consultas/${selectedCita.IdCita}`"
          class="inline-block mt-4 bg-blue-600 text-white px-4 py-2 rounded-lg"
        >
          Entrar a consulta
        </router-link>

      </div>

      <div v-else class="text-gray-400 text-sm">
        Selecciona una cita en el calendario para ver los detalles.
      </div>
    </div>
  </div>
</div>
</template>