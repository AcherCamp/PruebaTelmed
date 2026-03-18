/*Este componente solo recibe las citas como prop*/

<script setup>
const props = defineProps({
  citas: {
    type: Array,
    default: () => []
  }
})

const citaActual = (cita) => {

  const ahora = new Date()
  const inicio = new Date(cita.Start)
  const fin = new Date(cita.End)

  return ahora >= inicio && ahora <= fin
}

const entrarConsulta = (link) => {
  window.open(link, '_blank')
}
</script>

<template>

<div class="bg-white border rounded-xl shadow p-4">

  <h2 class="text-lg font-semibold mb-4">
    Citas de hoy
  </h2>

  <div class="overflow-x-auto">

  <table class="w-full text-left">

    <thead class="border-b text-gray-500">
      <tr>
        <th class="py-2">Hora</th>
        <th>Paciente</th>
        <th>Motivo</th>
        <th>Tipo</th>
        <th>Estado</th>
        <th>Acciones</th>
      </tr>
    </thead>

    <tbody>

      <tr
        v-for="cita in citas"
        :key="cita.IdCita"
        :class="[
          'border-b',
          citaActual(cita)
            ? 'bg-teal/10 border-teal'
            : 'hover:bg-gray-50'
        ]"
      >

        <td class="py-2">
          {{ new Date(cita.Start).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) }}
        </td>

        <td class="text-steel-blue font-medium cursor-pointer hover:underline">
          {{ cita.PacienteNombreCompleto }}
        </td>

        <td>
          {{ cita.Motivo }}
        </td>

        <td>
          {{ cita.TipoConsulta }}
        </td>

        <td>
          {{ cita.Estado }}
        </td>

        <td>

          <button
            v-if="cita.LinkReunion"
            @click="entrarConsulta(cita.LinkReunion)"
            class="text-teal hover:underline"
          >
            Entrar
          </button>

        </td>

      </tr>

      <tr v-if="citas.length === 0">
        <td colspan="6" class="text-gray-400 py-6 text-center">
          No hay citas programadas para hoy
        </td>
      </tr>

    </tbody>

  </table>

  </div>

</div>

</template>