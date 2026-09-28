import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

// Configuración de Vite para ControlInventario:
// - Plugin de Vue 3
// - Tailwind CSS v4
// - Proxy /api -> API .NET en el puerto 5087 (evita CORS en desarrollo)
export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5087',
        changeOrigin: true,
      },
    },
  },
})
