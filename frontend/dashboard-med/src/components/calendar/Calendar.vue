<script setup>
import FullCalendar from '@fullcalendar/vue3'
import dayGridPlugin from '@fullcalendar/daygrid'
import timeGridPlugin from '@fullcalendar/timegrid'
import interactionPlugin from '@fullcalendar/interaction'
import { ref, computed } from 'vue'
import { updateCita } from '@/services/api'
import { useCitasStore } from '@/stores/citas'

const citasStore = useCitasStore()

// Props
const props = defineProps({
  citas: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['cita-click','cita-movida'])

// Estado del modal
const showModal = ref(false)
const selectedEvent = ref(null)

// Colores según estado
const getEstadoColor = (estado) => {
  switch (estado) {
    case 'Confirmada': return '#10b981'
    case 'Pendiente': return '#f59e0b'
    case 'Cancelada': return '#ef4444'
    case 'EnConsulta': return '#3b82f6'
    case 'Finalizada': return '#6b7280'
    default: return '#3b82f6'
  }
}

// Convertimos citas en eventos
const eventos = computed(() =>
  props.citas.map(cita => ({
    id: cita.IdCita,
    idCita: cita.IdCita,
    title: `${cita.PacienteNombreCompleto} - ${cita.Titulo}`,
    start: cita.Start,
    end: cita.End,

    backgroundColor: getEstadoColor(cita.Estado),
    borderColor: getEstadoColor(cita.Estado),

    extendedProps: {
      idCita: cita.IdCita,
      estado: cita.Estado,
      tipo: cita.TipoConsulta,
      motivo: cita.Titulo,
      paciente: cita.PacienteNombreCompleto
    }
  }))
)

// Click en evento
const handleEventClick = (info) => {

  const cita = {
    idCita: info.event.id,
    title: info.event.title,
    start: info.event.start,
    end: info.event.end,
    ...info.event.extendedProps
  }

  console.log("Click Cita", cita)

  selectedEvent.value = cita
  showModal.value = true

  emit('cita-click', cita)
}

// Drag & Drop citas
const handleEventDrop = async (info) => {

  if (info.event.extendedProps.estado !== "Pendiente") {
    info.revert()
    return
  }

  const id = info.event.id

  try {

    await updateCita(id, {
      FechaInicio: info.event.start,
      FechaFin: info.event.end,
      Motivo: info.event.extendedProps.motivo,
      TipoConsulta: info.event.extendedProps.tipo
    })

    console.log("Cita movida correctamente")

    await citasStore.cargarCitas()

  } catch (error) {

    console.error("Error al mover cita:", error.response?.data)

    info.revert()

  }

}

// Opciones calendario
const calendarOptions = computed(() => ({
  plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],

  initialView: 'dayGridMonth',

  height: 500,
  expandRows: true,

  slotDuration: '00:30:00',
  slotLabelInterval: '00:30',
  slotMinTime: '08:00:00',
  slotMaxTime: '17:00:00',

  headerToolbar: {
    left: 'prev,next today',
    center: 'title',
    right: 'dayGridMonth,timeGridWeek,timeGridDay'
  },

  locale: 'es',

  events: eventos,

  editable: true,
  selectable: true,

  dateClick: (info) => {

    if(info.view.type === 'dayGridMonth'){
      info.view.calendar.changeView('timeGridDay', info.date)
    }

  },

  eventClick: handleEventClick,
  eventDrop: handleEventDrop
}))
</script>

<template>

<div class="bg-white p-4 rounded-2xl shadow-sm border border-gray-100 w-full overflow-hidden">

  <FullCalendar :options="calendarOptions" />

</div>

<!-- Modal detalle cita -->

<Transition name="fade">

<div
v-if="showModal"
class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4"
>

<div class="bg-white rounded-2xl shadow-xl max-w-md w-full overflow-hidden border border-gray-100">

<!-- Header -->

<div class="p-6 border-b border-gray-50 flex justify-between items-start">

<div>
<h3 class="text-xl font-bold text-gray-900">
{{ selectedEvent?.title }}
</h3>

<p class="text-sm text-gray-500">
Detalles de la cita médica
</p>

</div>

<button
@click="showModal = false"
class="text-gray-400 hover:text-gray-600"
>
✕
</button>

</div>

<!-- Cuerpo -->

<div class="p-6 space-y-4 text-gray-700">

<div class="flex items-center gap-3">

<span
class="px-3 py-1 rounded-full text-xs font-semibold"
:class="selectedEvent?.estado === 'Confirmada'
? 'bg-emerald-100 text-emerald-700'
: 'bg-amber-100 text-amber-700'"
>

{{ selectedEvent?.estado }}

</span>

<span class="text-sm font-medium text-blue-600">

{{ selectedEvent?.tipo }}

</span>

</div>

<div>

<p class="text-xs uppercase font-bold text-gray-400">
Paciente
</p>

<p class="text-sm">

{{ selectedEvent?.paciente }}

</p>

</div>

<div>

<p class="text-xs uppercase font-bold text-gray-400">
Fecha y hora
</p>

<p class="text-sm">

{{ selectedEvent?.start?.toLocaleString() }}

</p>

</div>

</div>

<!-- Footer -->

<div class="p-6 bg-gray-50 flex justify-end">

<button
@click="showModal = false"
class="px-4 py-2 bg-gray-900 text-white rounded-xl"
>

Cerrar

</button>

</div>

</div>

</div>

</Transition>

</template>