<script setup>
import { ref } from 'vue'
import api from '@/services/api'

const props = defineProps({
  cita: Object
})

const sintomas = ref('')
const evolucion = ref('')
const diagnostico = ref('')
const tratamiento = ref('')
const observaciones = ref('')

const medicamentos = ref([
  { nombre: '', dosis: '', frecuencia: '', duracion: '' }
])

const agregarMedicamento = () => {
  medicamentos.value.push({ nombre: '', dosis: '', frecuencia: '', duracion: '' })
}

const eliminarMedicamento = (index) => {
  medicamentos.value.splice(index, 1)
}

const guardarConsulta = async () => {
  try {

    const payload = {
      citaId: props.cita.idCita,
      sintomas: sintomas.value,
      evolucion: evolucion.value,
      diagnostico: diagnostico.value,
      tratamiento: tratamiento.value,
      observaciones: observaciones.value,
      medicamentosJson: JSON.stringify(medicamentos.value)
    }

    const res = await api.post('/consultas', payload, {
      responseType: 'blob' // IMPORTANTE PARA CORRECTA DESCARGA DE PDF
    })

    // Crear descarga
    const url = window.URL.createObjectURL(new Blob([res.data]))
    const link = document.createElement('a')

    link.href = url
    link.setAttribute('download', 'consulta.pdf')
    document.body.appendChild(link)
    link.click()

    alert("Consulta guardada y PDF generado")

  } catch (error) {
    console.error(error)
    alert("Error al guardar consulta")
  }
}
</script>

<template>
  <div class="space-y-4">

    <h2 class="text-lg font-semibold">
      Ficha clínica
    </h2>

    <textarea v-model="sintomas" class="w-full border p-2 rounded" placeholder="Síntomas"></textarea>

    <textarea v-model="evolucion" class="w-full border p-2 rounded" placeholder="Evolución"></textarea>

    <textarea v-model="diagnostico" class="w-full border p-2 rounded" placeholder="Diagnóstico"></textarea>

    <textarea v-model="tratamiento" class="w-full border p-2 rounded" placeholder="Tratamiento"></textarea>

    <textarea v-model="observaciones" class="w-full border p-2 rounded" placeholder="Observaciones"></textarea>

    <div>
      <h3 class="font-semibold">Medicamentos</h3>

      <div v-for="(med, index) in medicamentos" :key="index" class="border p-2 rounded mb-2 space-y-2">

        <input v-model="med.nombre" class="w-full border p-1 rounded" placeholder="Nombre" />
        <input v-model="med.dosis" class="w-full border p-1 rounded" placeholder="Dosis" />

        <button @click="eliminarMedicamento(index)" class="text-red-500 text-sm">
          Eliminar
        </button>
      </div>

      <button @click="agregarMedicamento" class="bg-gray-800 text-white px-2 py-1 rounded">
        + Agregar medicamento
      </button>
    </div>

    <button
      @click="guardarConsulta"
      class="bg-blue-600 text-white px-4 py-2 rounded-lg w-full">
      Guardar diagnóstico
    </button>

  </div>
</template>