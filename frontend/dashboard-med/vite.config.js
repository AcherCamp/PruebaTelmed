import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite' // Si usas Tailwind v4 como plugin

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  resolve: {
    alias: {
      // Mapea '@' a la carpeta 'src'
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
