/*<body>
  <div id="app"></div>
</body>;*/
import { createRouter, createWebHistory } from "vue-router";
import Welcome from "@/views/Welcome.vue";
import GameView from "@/views/GameView.vue";

const routes = [
  {
    path: "/",
    redirect: "/welcome",
    
  },
  {
  path: '/welcome',
  name: 'Welcome',
  component: Welcome
  },
  

  {
    path: "/roulette",
    name: "Roulette",
    component: GameView,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
