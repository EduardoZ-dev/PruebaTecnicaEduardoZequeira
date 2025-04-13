import { createRouter, createWebHistory } from "vue-router";
import GameMenu from "../views/GameMenu.vue";
import GameView from "../views/GameView.vue";    
import RouletteWheel from "../views/RouletteWheel.vue";

const routes = [
  {
    path: "/",
    name: "GameMenu",
    component: GameMenu
    
  },

  {
    path: '/juego',
    name: 'GameView',
    component: GameView
  },

  {
    path: "/ruleta",
    name: "RouletteWheel",
    component: RouletteWheel
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
