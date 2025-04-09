import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from './router'

import "./assets/styles/styles.scss";
import "./assets/styles/flipper.scss";

createApp(App).use(router).mount("#app");
