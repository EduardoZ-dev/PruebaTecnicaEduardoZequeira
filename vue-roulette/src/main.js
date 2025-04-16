import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from './router/index.js' 
import { createPinia } from 'pinia'

import "./assets/styles/styles.scss";
import "./assets/styles/flipper.scss";

const app = createApp(App)
app.use(router)
app.use(createPinia())
app.mount('#app')
