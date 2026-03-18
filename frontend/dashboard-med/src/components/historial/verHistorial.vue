<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const props = defineProps({
  usuarioSeleccionado: Object
})

const emit = defineEmits(['cerrar'])

const historial = ref([])
const consultaSeleccionada = ref(null)

onMounted(async () => {
  try {
    const res = await api.get(`/consultas/paciente/${props.usuarioSeleccionado.id}`)

    historial.value = res.data.map(c => ({
      id: c.idConsulta,
      fecha: new Date(c.fecha).toLocaleDateString(),
      doctor: c.cita?.doctor
        ? `${c.cita.doctor.nombre} ${c.cita.doctor.apellido}`
        : "Sin asignar",
      especialidad: "General",
      diagnostico: c.diagnostico,
      urlReceta: `http://localhost:5050/api/consultas/${c.idConsulta}/pdf`
    }))

  } catch (error) {
    console.error("Error cargando historial:", error)
  }
})

const verDetalle = (consulta) => {
  consultaSeleccionada.value = consulta
}
</script>

<template>
  <div class="fixed inset-0 z-[99999] flex items-center justify-center bg-black/60" @click.self="emit('cerrar')">

    <div class="bg-white w-[700px] max-h-[90vh] overflow-y-auto rounded-xl p-6">

      <button @click="emit('cerrar')" class="float-right text-xl">✖</button>

      <h2 v-if="!consultaSeleccionada" class="text-xl font-bold mb-4">
        Historial del paciente
      </h2>

      <!-- LISTA -->
      <div v-if="!consultaSeleccionada">

        <div v-for="c in historial" :key="c.id" class="border p-3 rounded mb-3">

          <p><strong>Fecha:</strong> {{ c.fecha }}</p>
          <p><strong>Doctor:</strong> {{ c.doctor }}</p>

          <button @click="verDetalle(c)" class="text-blue-600">
            Ver detalles
          </button>

        </div>

      </div>

      <!-- DETALLE -->
      <div v-else>

        <button @click="consultaSeleccionada = null" class="mb-3 text-sm">
          ← Volver
        </button>

        <p><strong>Fecha:</strong> {{ consultaSeleccionada.fecha }}</p>
        <p><strong>Doctor:</strong> {{ consultaSeleccionada.doctor }}</p>

        <p class="mt-3">
          <strong>Diagnóstico:</strong><br>
          {{ consultaSeleccionada.diagnostico }}
        </p>

        <a :href="consultaSeleccionada.urlReceta" target="_blank"
          class="block mt-4 bg-green-600 text-white px-4 py-2 rounded text-center">
          Descargar PDF
        </a>
      </div>
    </div>
  </div>
</template>