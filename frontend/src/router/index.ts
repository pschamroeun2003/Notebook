import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/notebook',
      name: 'notebook',
      component: () => import('../views/NoteFormComponent.vue')
    },
    {
      path: '/',
      name: "loginForm",
      component: () => import('../views/LoginFormComponent.vue')
    },
     {
      path: '/registerform',
      name: "registerForm",
      component: () => import('../views/RegisterForm.vue')
    }
  ],
})

export default router
