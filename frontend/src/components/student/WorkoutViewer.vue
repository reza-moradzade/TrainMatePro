<template>
  <div class="workout-viewer">
    <!-- انتخاب برنامه (اگه چندتا برنامه داره) -->
    <div v-if="workouts.length > 1" class="workout-selector mb-4">
      <label class="form-label small fw-bold">برنامه تمرینی:</label>
      <select class="form-select" v-model="selectedWorkoutId" @change="loadWorkout">
        <option v-for="w in workouts" :key="w.id" :value="w.id">
          {{ w.title }} ({{ formatDate(w.startDate) }} - {{ formatDate(w.endDate) }})
        </option>
      </select>
    </div>

    <!-- نمایش برنامه -->
    <div v-if="currentWorkout" class="workout-detail">
      <!-- هدر برنامه -->
      <div class="workout-header bg-primary text-white p-4 rounded-4 mb-4">
        <h2 class="h4 fw-bold mb-2">{{ currentWorkout.title }}</h2>
        <p class="mb-1 small opacity-75">{{ currentWorkout.description }}</p>
        <div class="d-flex gap-3 mt-3">
          <span class="badge bg-light text-dark">
            <i class="bi bi-calendar me-1"></i>
            شروع: {{ formatPersianDate(currentWorkout.startDate) }}
          </span>
          <span class="badge bg-light text-dark">
            <i class="bi bi-calendar-check me-1"></i>
            پایان: {{ formatPersianDate(currentWorkout.endDate) }}
          </span>
        </div>
      </div>

      <!-- هفته جاری -->
      <div class="current-week-section mb-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h3 class="h5 fw-bold">هفته {{ currentWeekNumber }} (جاری)</h3>
          <span class="badge bg-success">{{ getWeekProgress() }}% تکمیل</span>
        </div>

        <!-- روزهای هفته جاری -->
        <div class="week-days">
          <div
            v-for="day in currentWeek.days"
            :key="day.id"
            class="day-card card border-0 shadow-sm mb-3"
            :class="{ today: isToday(day.dayNumber) }"
          >
            <div
              class="card-header bg-white border-0 d-flex justify-content-between align-items-center"
            >
              <h4 class="h6 fw-bold mb-0">
                {{ getDayName(day.dayName) }}
                <span v-if="isToday(day.dayNumber)" class="badge bg-primary ms-2">امروز</span>
              </h4>
              <span class="badge bg-light text-dark">{{ day.exercises.length }} حرکت</span>
            </div>

            <div class="card-body">
              <!-- تمرکز روز -->
              <div v-if="day.focus" class="day-focus small text-secondary mb-3">
                <i class="bi bi-bullseye me-1"></i> {{ day.focus }}
              </div>

              <!-- حرکات روز -->
              <div class="exercises-list">
                <div
                  v-for="(ex, index) in day.exercises"
                  :key="index"
                  class="exercise-item card bg-light border-0 mb-2"
                >
                  <div class="card-body p-3">
                    <div class="d-flex gap-3">
                      <!-- GIF حرکت -->
                      <div class="exercise-gif" style="width: 80px; height: 80px">
                        <img
                          :src="ex.gifUrl"
                          :alt="ex.name"
                          class="w-100 h-100 object-fit-cover rounded-3"
                          loading="lazy"
                          @error="handleImageError"
                        />
                      </div>

                      <!-- جزئیات حرکت -->
                      <div class="flex-grow-1">
                        <h5 class="h6 fw-bold mb-2">{{ ex.name }}</h5>

                        <div class="exercise-meta d-flex gap-3 small mb-2">
                          <span class="badge bg-primary">
                            <i class="bi bi-bar-chart me-1"></i> ست: {{ ex.sets }}
                          </span>
                          <span class="badge bg-success">
                            <i class="bi bi-arrow-repeat me-1"></i> تکرار: {{ ex.reps }}
                          </span>
                          <span class="badge bg-warning text-dark">
                            <i class="bi bi-hourglass me-1"></i> استراحت: {{ ex.restTime }}
                          </span>
                        </div>

                        <!-- دکمه ثبت انجام تمرین -->
                        <button
                          v-if="!ex.completed"
                          class="btn btn-sm btn-outline-success mt-2"
                          @click="markExerciseCompleted(day, index)"
                        >
                          <i class="bi bi-check-circle me-1"></i>
                          انجام شد
                        </button>
                        <span v-else class="text-success small">
                          <i class="bi bi-check-circle-fill me-1"></i>
                          انجام شده
                        </span>
                      </div>
                    </div>

                    <!-- نکات حرکت -->
                    <div
                      v-if="ex.description"
                      class="exercise-notes small text-secondary mt-2 pt-2 border-top"
                    >
                      <i class="bi bi-info-circle me-1"></i>
                      {{ ex.description }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- سایر هفته‌ها (آکاردئونی) -->
      <div class="other-weeks-section">
        <h3 class="h5 fw-bold mb-3">سایر هفته‌ها</h3>

        <div class="accordion" id="weeksAccordion">
          <div
            v-for="week in otherWeeks"
            :key="week.weekNumber"
            class="accordion-item border-0 mb-2"
          >
            <h2 class="accordion-header">
              <button
                class="accordion-button collapsed bg-light rounded-3"
                type="button"
                @click="toggleWeek(week.weekNumber)"
              >
                <span class="fw-bold">هفته {{ week.weekNumber }}</span>
                <span class="badge bg-secondary ms-2">{{ getWeekExercisesCount(week) }} حرکت</span>
                <span v-if="week.focus" class="ms-3 small text-secondary">{{ week.focus }}</span>
              </button>
            </h2>
            <div
              class="accordion-collapse collapse"
              :class="{ show: expandedWeek === week.weekNumber }"
            >
              <div class="accordion-body">
                <div v-for="day in week.days" :key="day.id" class="day-preview mb-3">
                  <h6 class="fw-bold mb-2">{{ getDayName(day.dayName) }}</h6>
                  <div class="exercises-preview">
                    <div
                      v-for="ex in day.exercises.slice(0, 2)"
                      :key="ex.id"
                      class="exercise-preview-item d-flex gap-2 align-items-center mb-1"
                    >
                      <img
                        :src="ex.gifUrl"
                        :alt="ex.name"
                        style="width: 30px; height: 30px"
                        class="rounded-2 object-fit-cover"
                      />
                      <span class="small">{{ ex.name }}</span>
                    </div>
                    <div v-if="day.exercises.length > 2" class="small text-secondary">
                      + {{ day.exercises.length - 2 }} حرکت دیگر
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- حالت خالی -->
    <div v-else-if="!loading && workouts.length === 0" class="text-center py-5">
      <i class="bi bi-calendar-x display-1 text-secondary opacity-25"></i>
      <p class="text-secondary mt-3">برنامه تمرینی برای شما ثبت نشده است</p>
    </div>

    <!-- لودینگ -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="text-secondary mt-3">در حال دریافت برنامه...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { workoutService } from '../../services/workouts'

const props = defineProps({
  studentId: {
    type: Number,
    required: true,
  },
})

// State
const loading = ref(false)
const workouts = ref([])
const selectedWorkoutId = ref(null)
const currentWorkout = ref(null)
const expandedWeek = ref(null)

// محاسبه هفته جاری بر اساس تاریخ
const currentWeekNumber = computed(() => {
  if (!currentWorkout.value) return 1

  const startDate = new Date(currentWorkout.value.startDate)
  const today = new Date()
  const diffTime = today - startDate
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  const weekNumber = Math.floor(diffDays / 7) + 1

  return Math.min(weekNumber, currentWorkout.value.durationWeeks)
})

// هفته جاری
const currentWeek = computed(() => {
  if (!currentWorkout.value) return null
  return currentWorkout.value.weeks.find((w) => w.weekNumber === currentWeekNumber.value)
})

// سایر هفته‌ها
const otherWeeks = computed(() => {
  if (!currentWorkout.value) return []
  return currentWorkout.value.weeks.filter((w) => w.weekNumber !== currentWeekNumber.value)
})

// ========== توابع کمکی ==========
const getDayName = (dayName) => {
  const days = {
    saturday: 'شنبه',
    sunday: 'یکشنبه',
    monday: 'دوشنبه',
    tuesday: 'سه‌شنبه',
    wednesday: 'چهارشنبه',
    thursday: 'پنج‌شنبه',
    friday: 'جمعه',
  }
  return days[dayName] || dayName
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('fa-IR')
}

const formatPersianDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('fa-IR', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

const isToday = (dayNumber) => {
  if (!currentWorkout.value) return false

  const startDate = new Date(currentWorkout.value.startDate)
  const today = new Date()
  const diffTime = today - startDate
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))
  const currentDayOfWeek = (diffDays % 7) + 1 // 1 = saturday, 7 = friday

  return currentDayOfWeek === dayNumber
}

const getWeekExercisesCount = (week) => {
  return week.days.reduce((total, day) => total + day.exercises.length, 0)
}

const getWeekProgress = () => {
  if (!currentWorkout.value) return 0

  const totalExercises = currentWeek.value.days.reduce(
    (total, day) => total + day.exercises.length,
    0,
  )
  const completedExercises = currentWeek.value.days.reduce(
    (total, day) => total + day.exercises.filter((ex) => ex.completed).length,
    0,
  )

  return totalExercises > 0 ? Math.round((completedExercises / totalExercises) * 100) : 0
}

const handleImageError = (e) => {
  // اگه عکس لود نشد، از آدرس اصلی استفاده کن
  const originalUrl = e.target.getAttribute('data-original') || e.target.src

  if (originalUrl.includes('localhost') || originalUrl.includes('127.0.0.1')) {
    // اگه آدرس لوکال بود و کار نکرد، برو سراغ آدرس اصلی
    const fileName = originalUrl.split('/').pop()
    e.target.src = `https://static.exercisedb.dev/media/${fileName}`
  } else {
    // آدرس پیش‌فرض
    e.target.src = 'https://via.placeholder.com/60?text=No+Image'
  }
}

// ========== توابع اصلی ==========
const loadWorkouts = async () => {
  loading.value = true
  try {
    const response = await workoutService.getWorkouts()
    if (response.success) {
      workouts.value = response.programs || []

      if (workouts.value.length > 0) {
        // اگر چندتا برنامه داره، اولین برنامه رو انتخاب کن
        selectedWorkoutId.value = workouts.value[0].id
        await loadWorkout()
      }
    }
  } catch (error) {
    console.error('Error loading workouts:', error)
  } finally {
    loading.value = false
  }
}

const loadWorkout = async () => {
  if (!selectedWorkoutId.value) return

  loading.value = true
  try {
    const response = await workoutService.getWorkoutById(selectedWorkoutId.value)
    if (response.success) {
      currentWorkout.value = response.program

      // اضافه کردن فیلد completed به حرکات (برای ثبت انجام)
      currentWorkout.value.weeks.forEach((week) => {
        week.days.forEach((day) => {
          day.exercises.forEach((ex) => {
            if (ex.completed === undefined) {
              ex.completed = false
            }
          })
        })
      })
    }
  } catch (error) {
    console.error('Error loading workout:', error)
  } finally {
    loading.value = false
  }
}

const markExerciseCompleted = (day, exerciseIndex) => {
  day.exercises[exerciseIndex].completed = true

  // اینجا می‌تونی به API بزنی برای ثبت
  console.log('Exercise completed:', day.exercises[exerciseIndex].name)
}

const toggleWeek = (weekNumber) => {
  expandedWeek.value = expandedWeek.value === weekNumber ? null : weekNumber
}

// Lifecycle
onMounted(() => {
  loadWorkouts()
})
</script>

<style scoped>
.object-fit-cover {
  object-fit: cover;
}

.day-card.today {
  border-right: 4px solid #0d6efd;
}

.accordion-button:not(.collapsed) {
  background-color: #f8f9fa;
  color: inherit;
}

.accordion-button:focus {
  box-shadow: none;
  border-color: #dee2e6;
}

.workout-gif {
  transition: transform 0.2s ease;
}

.workout-gif:hover {
  transform: scale(1.02);
}
</style>
