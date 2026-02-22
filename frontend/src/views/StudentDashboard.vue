<template>
  <div class="min-vh-100 bg-light">
    <!-- نوار بالایی موبایل -->
    <nav class="navbar navbar-light bg-white shadow-sm sticky-top">
      <div class="container-fluid px-3">
        <button class="btn btn-link text-dark p-0 border-0" @click="toggleSidebar">
          <i class="bi bi-list fs-3"></i>
        </button>

        <span class="fw-bold text-primary">TrainMatePro</span>

        <div class="dropdown">
          <button class="btn btn-link text-dark p-0 border-0" data-bs-toggle="dropdown">
            <i class="bi bi-three-dots-vertical fs-5"></i>
          </button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li>
              <a class="dropdown-item" href="#" @click.prevent="logout">
                <i class="bi bi-box-arrow-right me-2"></i>خروج
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- سایدبار موبایل (offcanvas) -->
    <div class="offcanvas offcanvas-start" ref="sidebar" tabindex="-1">
      <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title text-primary">منوی شاگرد</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
      </div>
      <div class="offcanvas-body p-0">
        <div class="p-4 border-bottom bg-light">
          <div class="d-flex align-items-center">
            <i class="bi bi-person-circle fs-1 text-secondary"></i>
            <div class="ms-3">
              <p class="fw-bold mb-1">{{ authStore.user?.fullName }}</p>
              <p class="small text-secondary mb-0">{{ authStore.user?.email }}</p>
            </div>
          </div>
        </div>

        <div class="list-group list-group-flush">
          <a href="#" class="list-group-item list-group-item-action active">
            <i class="bi bi-house-door me-2"></i>داشبورد
          </a>
          <router-link
            to="/student-dashboard/workout"
            class="list-group-item list-group-item-action"
          >
            <i class="bi bi-calendar-check me-2"></i>برنامه تمرینی
          </router-link>
          <a href="#" class="list-group-item list-group-item-action">
            <i class="bi bi-bar-chart me-2"></i>پیشرفت
          </a>
          <a
            href="#"
            class="list-group-item list-group-item-action text-danger"
            @click.prevent="logout"
          >
            <i class="bi bi-box-arrow-right me-2"></i>خروج
          </a>
        </div>
      </div>
    </div>

    <!-- محتوای اصلی -->
    <main class="container-fluid px-3 py-4">
      <!-- خوش‌آمدگویی با تاریخ امروز -->
      <div class="mb-4">
        <div class="d-flex justify-content-between align-items-center">
          <div>
            <h1 class="h4 fw-bold mb-1">سلام، {{ authStore.user?.fullName }}</h1>
            <p class="text-secondary small mb-0">{{ todayPersianDate }}</p>
          </div>
          <div class="bg-primary bg-opacity-10 p-2 rounded-3">
            <i class="bi bi-calendar3 text-primary fs-4"></i>
          </div>
        </div>
      </div>

      <!-- کارت وضعیت تمرین امروز -->
      <div class="today-workout-card bg-primary text-white p-4 rounded-4 mb-4">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <h2 class="h5 fw-bold mb-0">تمرین امروز</h2>
          <span class="badge bg-light text-primary px-3 py-2">
            {{ todayPersianDay }}
          </span>
        </div>

        <!-- تاریخ برنامه -->
        <p class="small opacity-75 mb-3">
          <i class="bi bi-calendar3 me-1"></i>
          {{ workoutDateRange }}
        </p>

        <div v-if="todayWorkout && todayWorkout.exercises.length > 0">
          <!-- خلاصه تمرین امروز -->
          <div class="bg-white bg-opacity-10 rounded-3 p-3 mb-3">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="fw-bold">{{ todayWorkout.focus || 'تمرین عمومی' }}</span>
              <span class="badge bg-light text-dark">
                {{ todayWorkout.exercises.length }} حرکت
              </span>
            </div>

            <!-- لیست خلاصه حرکات -->
            <div class="small">
              <div
                v-for="(ex, index) in todayWorkout.exercises.slice(0, 3)"
                :key="index"
                class="d-flex align-items-center mb-1"
              >
                <i class="bi bi-dot fs-4 lh-1"></i>
                <span>{{ ex.name }}</span>
                <span class="me-auto text-white-50"> {{ ex.sets }}×{{ ex.reps }} </span>
              </div>
              <div v-if="todayWorkout.exercises.length > 3" class="text-white-50 mt-1">
                + {{ todayWorkout.exercises.length - 3 }} حرکت دیگر
              </div>
            </div>
          </div>

          <div class="d-flex gap-2 mb-3">
            <span class="badge bg-light text-dark">
              <i class="bi bi-bar-chart me-1"></i>
              {{ todayWorkout.exercises.length }} حرکت
            </span>
            <span class="badge bg-light text-dark">
              <i class="bi bi-clock me-1"></i>
              {{ todayWorkout.duration || '۶۰' }} دقیقه
            </span>
          </div>

          <router-link to="/student-dashboard/workout" class="btn btn-light w-100">
            <i class="bi bi-eye me-2"></i>
            مشاهده برنامه امروز
          </router-link>
        </div>

        <div v-else>
          <div class="bg-white bg-opacity-10 rounded-3 p-4 text-center mb-3">
            <i class="bi bi-calendar-x display-6"></i>
            <p class="mt-2 mb-0">امروز تمرینی ندارید</p>
          </div>
          <p class="small opacity-75 text-center mb-0">روز استراحت یا برنامه غیرفعال</p>
        </div>
      </div>

      <!-- آمار کلی -->
      <div class="row g-3 mb-4">
        <div class="col-6">
          <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-3">
              <div class="d-flex align-items-center">
                <div class="bg-success bg-opacity-10 p-2 rounded-3">
                  <i class="bi bi-check-circle-fill text-success"></i>
                </div>
                <div class="ms-3">
                  <span class="small text-secondary d-block">انجام شده</span>
                  <span class="h5 fw-bold mb-0">{{ completedCount }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-6">
          <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-3">
              <div class="d-flex align-items-center">
                <div class="bg-warning bg-opacity-10 p-2 rounded-3">
                  <i class="bi bi-calendar-week text-warning"></i>
                </div>
                <div class="ms-3">
                  <span class="small text-secondary d-block">هفته جاری</span>
                  <span class="h5 fw-bold mb-0">{{ currentWeekNumber }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-6">
          <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-3">
              <div class="d-flex align-items-center">
                <div class="bg-info bg-opacity-10 p-2 rounded-3">
                  <i class="bi bi-flag-fill text-info"></i>
                </div>
                <div class="ms-3">
                  <span class="small text-secondary d-block">پایان برنامه</span>
                  <span class="h5 fw-bold mb-0">{{ daysLeft }} روز</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-6">
          <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-3">
              <div class="d-flex align-items-center">
                <div class="bg-primary bg-opacity-10 p-2 rounded-3">
                  <i class="bi bi-graph-up-arrow text-primary"></i>
                </div>
                <div class="ms-3">
                  <span class="small text-secondary d-block">پیشرفت</span>
                  <span class="h5 fw-bold mb-0">{{ progressPercent }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- هفته جاری (پیش نمایش) -->
      <div class="current-week-preview mb-4">
        <h2 class="h5 fw-bold mb-3">هفته {{ currentWeekNumber }} (جاری)</h2>

        <div class="week-days-progress mb-3">
          <div class="d-flex justify-content-between small mb-1">
            <span>پیشرفت هفته</span>
            <span>{{ weekProgress }}%</span>
          </div>
          <div class="progress" style="height: 8px">
            <div class="progress-bar bg-success" :style="{ width: weekProgress + '%' }"></div>
          </div>
        </div>

        <div class="row g-2">
          <div v-for="day in currentWeekDays" :key="day.dayNumber" class="col">
            <div
              class="day-indicator text-center p-2 rounded-3"
              :class="{
                'bg-primary text-white': isToday(day.dayNumber),
                'bg-light': !isToday(day.dayNumber),
                'opacity-50': !day.hasExercises,
              }"
            >
              <small>{{ getShortDayName(day.dayName) }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- لینک به برنامه کامل -->
      <router-link
        to="/student-dashboard/workout"
        class="btn btn-outline-primary w-100 py-3 rounded-4"
      >
        <i class="bi bi-calendar-week me-2"></i>
        مشاهده برنامه کامل
      </router-link>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { workoutService } from '../services/workouts'
import * as bootstrap from 'bootstrap'

const router = useRouter()
const authStore = useAuthStore()
const sidebar = ref(null)

// State
const workouts = ref([])
const currentWorkout = ref(null)
const loading = ref(false)

// تاریخ امروز به فارسی
const todayPersianDate = computed(() => {
  const today = new Date()
  return today.toLocaleDateString('fa-IR', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
})

const todayPersianDay = computed(() => {
  const today = new Date()
  return today.toLocaleDateString('fa-IR', { weekday: 'long' })
})

// محدوده تاریخ برنامه
const workoutDateRange = computed(() => {
  if (!currentWorkout.value) return 'برنامه‌ای وجود ندارد'

  const start = new Date(currentWorkout.value.startDate).toLocaleDateString('fa-IR')
  const end = new Date(currentWorkout.value.endDate).toLocaleDateString('fa-IR')
  return `${start} تا ${end}`
})

// محاسبات هفته جاری
const currentWeekNumber = computed(() => {
  if (!currentWorkout.value) return 1

  const startDate = new Date(currentWorkout.value.startDate)
  const today = new Date()

  // تنظیم ساعت به 00:00 برای مقایسه دقیق
  startDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  const diffTime = today - startDate
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays < 0) return 1 // برنامه شروع نشده

  const weekNum = Math.floor(diffDays / 7) + 1
  return Math.min(weekNum, currentWorkout.value.durationWeeks)
})

const currentWeek = computed(() => {
  if (!currentWorkout.value) return null
  return currentWorkout.value.weeks.find((w) => w.weekNumber === currentWeekNumber.value)
})

// تمرین امروز بر اساس تاریخ واقعی
const todayWorkout = computed(() => {
  if (!currentWeek.value) return null

  const startDate = new Date(currentWorkout.value.startDate)
  const today = new Date()

  startDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  const diffTime = today - startDate
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))

  // اگر برنامه هنوز شروع نشده یا تموم شده
  if (diffDays < 0 || diffDays >= currentWorkout.value.durationWeeks * 7) {
    return null
  }

  // محاسبه روز هفته بر اساس تاریخ شروع
  const dayOfWeek = diffDays % 7
  const dayMap = [1, 2, 3, 4, 5, 6, 7] // saturday=1, sunday=2, ..., friday=7

  return currentWeek.value.days.find((d) => d.dayNumber === dayMap[dayOfWeek])
})

const currentWeekDays = computed(() => {
  if (!currentWeek.value) return []

  return currentWeek.value.days.map((day) => ({
    ...day,
    dayName: getFullDayName(day.dayName),
    hasExercises: day.exercises.length > 0,
  }))
})

const completedCount = computed(() => {
  // اینجا باید از دیتابیس بیاد
  return 0
})

const daysLeft = computed(() => {
  if (!currentWorkout.value) return 0

  const endDate = new Date(currentWorkout.value.endDate)
  const today = new Date()

  endDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  const diffTime = endDate - today
  return Math.max(0, Math.ceil(diffTime / (1000 * 60 * 60 * 24)))
})

const progressPercent = computed(() => {
  if (!currentWorkout.value) return 0

  const startDate = new Date(currentWorkout.value.startDate)
  const endDate = new Date(currentWorkout.value.endDate)
  const today = new Date()

  startDate.setHours(0, 0, 0, 0)
  endDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  const totalDays = (endDate - startDate) / (1000 * 60 * 60 * 24) + 1
  const passedDays = (today - startDate) / (1000 * 60 * 60 * 24) + 1

  return Math.min(100, Math.max(0, Math.round((passedDays / totalDays) * 100)))
})

const weekProgress = computed(() => {
  if (!currentWeek.value) return 0

  const totalExercises = currentWeek.value.days.reduce((sum, day) => sum + day.exercises.length, 0)
  // اینجا باید از دیتابیس بیاد
  const completed = 0

  return totalExercises > 0 ? Math.round((completed / totalExercises) * 100) : 0
})

// توابع
const logout = async () => {
  await authStore.logout()
  router.push('/')
}

const toggleSidebar = () => {
  const bsOffcanvas = bootstrap.Offcanvas.getInstance(sidebar.value)
  if (bsOffcanvas) {
    bsOffcanvas.show()
  } else {
    new bootstrap.Offcanvas(sidebar.value).show()
  }
}

const getFullDayName = (dayName) => {
  const days = {
    saturday: 'شنبه',
    sunday: 'یکشنبه',
    monday: 'دوشنبه',
    tuesday: 'سه‌شنبه',
    wednesday: 'چهارشنبه',
    thursday: 'پنجشنبه',
    friday: 'جمعه',
  }
  return days[dayName] || dayName
}

const getShortDayName = (dayName) => {
  const days = {
    saturday: 'ش',
    sunday: 'ی',
    monday: 'د',
    tuesday: 'س',
    wednesday: 'چ',
    thursday: 'پ',
    friday: 'ج',
  }
  return days[dayName] || dayName?.slice(0, 1)
}

const isToday = (dayNumber) => {
  if (!currentWorkout.value || !currentWeek.value) return false

  const startDate = new Date(currentWorkout.value.startDate)
  const today = new Date()

  startDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  const diffTime = today - startDate
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays < 0 || diffDays >= currentWorkout.value.durationWeeks * 7) {
    return false
  }

  const dayOfWeek = diffDays % 7
  const dayMap = [1, 2, 3, 4, 5, 6, 7]

  return dayNumber === dayMap[dayOfWeek]
}

// بارگذاری داده‌ها
const loadData = async () => {
  loading.value = true
  try {
    const response = await workoutService.getWorkouts()
    if (response.success) {
      workouts.value = response.programs || []
      if (workouts.value.length > 0) {
        currentWorkout.value = workouts.value[0]
      }
    }
  } catch (error) {
    console.error('Error loading workouts:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.day-indicator {
  transition: all 0.2s ease;
  cursor: default;
  font-size: 0.75rem;
}

.day-indicator.bg-primary {
  box-shadow: 0 2px 8px rgba(13, 110, 253, 0.3);
}

.progress {
  background-color: #e9ecef;
  border-radius: 10px;
  overflow: hidden;
}

.progress-bar {
  transition: width 0.3s ease;
}

.today-workout-card {
  background: linear-gradient(135deg, #0d6efd 0%, #0b5ed7 100%);
}

.list-group-item.router-link-active,
.list-group-item.router-link-exact-active {
  background-color: #0d6efd;
  color: white !important;
}

.list-group-item.router-link-active i,
.list-group-item.router-link-exact-active i {
  color: white !important;
}
</style>
