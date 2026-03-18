import { defineStore } from 'pinia'
import { getCalendario } from '@/services/api'

export const useCitasStore = defineStore('citas', {

  state: () => ({
    citas: [],
    cargando: false
  }),

  actions: {

    async cargarCitas() {

      this.cargando = true

      try {

        const data = await getCalendario()

        this.citas = data || []

      } catch (error) {

        console.error("Error cargando citas:", error)

        this.citas = []

      }

      this.cargando = false

    }

  }

})