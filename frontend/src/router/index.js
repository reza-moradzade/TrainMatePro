import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue'),
      meta: { public: true },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: { public: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
      meta: { public: true },
    },
    {
      path: '/contact',
      name: 'contact',
      component: () => import('../views/ContactView.vue'),
      meta: { public: true },
    },
    {
      path: '/dashboard',
      name: 'coach-dashboard',
      component: () => import('../views/CoachDashboard.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/student-dashboard',
      name: 'student-dashboard',
      component: () => import('../views/StudentDashboard.vue'),
      meta: { requiresAuth: true, role: 'student' },
    },
    {
      path: '/student-dashboard/workout',
      name: 'student-workout',
      component: () => import('../views/StudentWorkoutView.vue'),
      meta: { requiresAuth: true, role: 'student' },
    },
    {
      path: '/coach/workout/create/:studentId?',
      name: 'create-workout',
      component: () => import('../views/CreateWorkoutView.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/coach/workout/edit/:id',
      name: 'edit-workout',
      component: () => import('../views/EditWorkoutView.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/coach/workouts/:studentId?',
      name: 'coach-workouts',
      component: () => import('../views/CoachWorkoutsView.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/coach/students',
      name: 'coach-students',
      component: () => import('../views/CoachStudentsView.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/coach/reports',
      name: 'coach-reports',
      component: () => import('../views/CoachReportsView.vue'),
      meta: { requiresAuth: true, role: 'coach' },
    },
    {
      path: '/student-dashboard/progress',
      name: 'student-progress',
      component: () => import('../views/StudentProgressView.vue'),
      meta: { requiresAuth: true, role: 'student' },
    },
    {
      path: '/student-dashboard/history',
      name: 'student-history',
      component: () => import('../views/StudentHistoryView.vue'),
      meta: { requiresAuth: true, role: 'student' },
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('../views/NotFoundView.vue'),
      meta: { public: true },
    },
  ],
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // صفحات عمومی - همه می‌تونن ببینن
  if (to.meta.public) {
    // اگه کاربر لاگین هست و می‌خواد بره به صفحه لاگین یا رجیستر، ببرش به داشبورد مناسب
    if (authStore.isAuthenticated && (to.path === '/login' || to.path === '/register')) {
      if (authStore.user?.role === 'coach') {
        next('/dashboard')
      } else if (authStore.user?.role === 'student') {
        next('/student-dashboard')
      } else {
        next('/')
      }
    } else {
      next()
    }
  }
  // صفحات نیازمند احراز هویت
  else if (to.meta.requiresAuth) {
    if (!authStore.isAuthenticated) {
      next('/login')
    }
    // چک کردن نقش
    else if (to.meta.role && authStore.user?.role !== to.meta.role) {
      // اگر role اشتباه بود، به صفحه مناسب هدایت کن
      if (authStore.user?.role === 'coach') {
        next('/dashboard')
      } else if (authStore.user?.role === 'student') {
        next('/student-dashboard')
      } else {
        next('/')
      }
    } else {
      next()
    }
  } else {
    next()
  }
})

// afterEach برای لاگ (اختیاری)
router.afterEach((to, from) => {
  console.log(`Navigated from ${from.path} to ${to.path}`)
})

export default router
