import { createRouter, createWebHistory } from 'vue-router'

import Dashboard from '@/views/Dashboard.vue'
import Agenda from '@/views/Agenda.vue'
import Pacientes from '@/views/Pacientes.vue'
import Consultas from '@/views/Consultas.vue'
import Historial from '@/views/Historial.vue'
import SalaTelmed from '@/views/SalaTelmed.vue'

const routes = [
  { path: '/', component: Dashboard },
  { path: '/agenda', component: Agenda },
  { path: '/pacientes', component: Pacientes },
  { path: '/consultas', component: Consultas },
  {
    path: '/consultas/:id',
    name: 'consulta',
    component: Consultas,
    props: true
  },
  { path: '/historial', component: Historial },
  { path: '/sala', component: SalaTelmed }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router