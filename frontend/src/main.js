import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './style.css'

// Registro global de la aplicación Vue 3 (Pinia + Router).
const app = createApp(App)

app.use(createPinia())
app.use(router)
app.mount('#app')
