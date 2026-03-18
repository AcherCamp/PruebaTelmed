<script setup>
import { ref, onMounted, nextTick, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getCitaById, iniciarConsulta, finalizarConsulta } from '@/services/api'
import { useCitasStore } from '@/stores/citas'
import FormConsulta from '@/components/consultas/FormConsulta.vue'
import VerHistorial from '@/components/historial/verHistorial.vue'

//Sección Const
const props = defineProps({
  id: String
})
const route = useRoute()
const citasStore = useCitasStore()
const id = computed(() => props.id)
const cita = ref(null)
const loading = ref(true)
const jitsiContainer = ref(null)
const mostrarHistorial = ref(false)
//onMounted
onMounted(async () => {

  if (!id.value) {
    console.error("No se recibió ID de cita")
    loading.value = false
    return
  }

  try {

    console.log("ID consulta:", id.value)

    const data = await getCitaById(id.value)
    cita.value = data

    await iniciarConsulta(id.value)

    await citasStore.cargarCitas()

    await nextTick()

    if (window.JitsiMeetExternalAPI && data?.linkReunion) {

      const domain = "meet.jit.si"
      const roomName = data.linkReunion.split("/").pop()

      const options = {
        roomName,
        width: "100%",
        height: 500,
        parentNode: jitsiContainer.value
      }

      const api = new window.JitsiMeetExternalAPI(domain, options)

      api.addEventListener("videoConferenceLeft", async () => {

        console.log("Consulta finalizada")

        await finalizarConsulta(id.value)

        await citasStore.cargarCitas()

      })

    }

  } catch (error) {

    console.error("Error cargando consulta:", error)

  } finally {

    loading.value = false
  }

})

/* BOTÓN FINALIZAR */

const finalizar = async () => {

  await finalizarConsulta(id.value)

  await citasStore.cargarCitas()
}
</script>

<template>

<div class="space-y-6 w-full">

  <h1 class="text-2xl font-bold text-gray-800">
    Consulta Médica
  </h1>

 <div class="bg-white p-4 rounded-xl shadow border">

  <div v-if="loading">
    Cargando datos de la cita...
  </div>

  <div v-else-if="cita">

    <p><strong>Paciente:</strong> {{ cita.pacienteNombreCompleto }}</p>

    <p>
      <strong>Hora:</strong>
      {{ new Date(cita.start).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) }}
    </p>

    <p><strong>Tipo:</strong> {{ cita.tipoConsulta }}</p>

  </div>

  <div v-else>
    No se pudo cargar la cita.
  </div>

</div>

<div class="flex gap-3 mt-3">

  <button
  @click="finalizar"
  class="bg-red-500 text-white px-4 py-2 rounded-lg">
    Finalizar consulta
  </button>

      <button 
      @click="mostrarHistorial = true"
      :disabled="!cita"
      class="bg-blue-500 text-white px-4 py-2 rounded-lg disabled:opacity-50">
      Abrir historial
    </button>
</div>

<div class="grid grid-cols-1 lg:grid-cols-2 gap-6">

  <!-- VIDEOLLAMADA -->

  <div class="bg-white rounded-xl shadow border p-4">

    <h2 class="text-lg font-semibold mb-4">
      Videollamada
    </h2>

    <div
    ref="jitsiContainer"
    class="w-full h-[500px] bg-gray-100 rounded-lg">
    </div>
  </div>

      <!-- FICHA CLINICA -->
          <div class="bg-white rounded-xl shadow border p-6">
          <FormConsulta :cita="cita" />
        </div>
    </div>
</div>

<!-- 🔥 MODAL HISTORIAL -->
<VerHistorial
  v-if="mostrarHistorial && cita"
  :usuarioSeleccionado="{
    nombre: cita.pacienteNombreCompleto,
    id: cita.pacienteId
  }"
  @cerrar="mostrarHistorial = false"
/>

</template>