<template>
  <div>
    <!-- اطلاعات پایه برنامه -->
    <div class="mb-4">
      <h6 class="fw-bold mb-3">اطلاعات برنامه</h6>

      <!-- انتخاب شاگرد (مخفی وقتی از داشبورد میاد) -->
      <div class="mb-3" v-if="!studentId">
        <label class="form-label small fw-bold">انتخاب شاگرد</label>
        <select class="form-select" v-model="workout.studentId" required>
          <option value="" disabled>انتخاب کنید</option>
          <option
            v-for="student in students"
            :key="student.studentId || student.userId"
            :value="student.studentId || student.userId"
          >
            {{ student.fullName }} - {{ student.email }}
          </option>
        </select>
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">عنوان برنامه</label>
        <input
          type="text"
          class="form-control"
          v-model="workout.title"
          placeholder="مثال: برنامه افزایش قدرت - هفته اول"
          required
        />
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">توضیحات</label>
        <textarea
          class="form-control"
          rows="2"
          v-model="workout.description"
          placeholder="توضیحات کلی برنامه..."
        ></textarea>
      </div>

      <div class="row">
        <div class="col-6 mb-3">
          <label class="form-label small fw-bold">تاریخ شروع</label>
          <input type="date" class="form-control" v-model="workout.startDate" required />
        </div>
        <div class="col-6 mb-3">
          <label class="form-label small fw-bold">تاریخ پایان</label>
          <input type="date" class="form-control" v-model="workout.endDate" readonly />
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">مدت (هفته)</label>
        <input
          type="number"
          class="form-control"
          v-model="workout.durationWeeks"
          min="1"
          max="12"
          required
          @change="calculateEndDate"
        />
      </div>
    </div>

    <!-- هفته‌ها و روزها -->
    <div class="mb-4">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold mb-0">برنامه هفتگی</h6>
        <button class="btn btn-sm btn-outline-primary" @click="addWeek">
          <i class="bi bi-plus-circle me-1"></i>
          هفته جدید
        </button>
      </div>

      <!-- هفته‌ها -->
      <div
        v-for="(week, weekIndex) in workout.weeks"
        :key="weekIndex"
        class="card mb-3 border-0 shadow-sm"
      >
        <div
          class="card-header bg-white border-0 d-flex justify-content-between align-items-center"
        >
          <h6 class="fw-bold mb-0">هفته {{ week.weekNumber }}</h6>
          <button class="btn btn-sm btn-light text-danger" @click="removeWeek(weekIndex)">
            <i class="bi bi-trash"></i>
          </button>
        </div>

        <div class="card-body">
          <!-- روزها -->
          <div v-for="(day, dayIndex) in week.days" :key="dayIndex" class="mb-4">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="badge bg-light text-secondary px-3 py-2">
                {{ translateDay(day.dayName) }}
              </span>
              <button
                class="btn btn-sm btn-link text-danger"
                @click="removeDay(weekIndex, dayIndex)"
              >
                <i class="bi bi-x-lg"></i>
              </button>
              <!-- مطمئن شو این بسته شده -->
            </div>

            <!-- حرکات روز -->
            <div class="ms-3">
              <div
                v-for="(exercise, exIndex) in day.exercises"
                :key="exIndex"
                class="card bg-light border-0 mb-2"
              >
                <div class="card-body p-3">
                  <div class="d-flex gap-3">
                    <!-- GIF حرکت -->
                    <div style="width: 60px; height: 60px">
                      <img
                        :src="getGifUrl(exercise.gifUrl)"
                        :alt="exercise.name"
                        class="w-100 h-100 object-fit-cover rounded"
                        loading="lazy"
                        @error="handleImageError"
                      />
                    </div>

                    <!-- جزئیات حرکت -->
                    <div class="flex-grow-1">
                      <div class="d-flex justify-content-between">
                        <div>
                          <!-- نام حرکت با قابلیت ویرایش -->
                          <input
                            type="text"
                            class="form-control form-control-sm mb-2"
                            :value="exercise.displayName || exercise.name"
                            @input="updateExerciseName(exercise, $event.target.value)"
                            placeholder="نام حرکت"
                          />
                          <!-- نمایش نام اصلی (اختیاری) -->
                          <small
                            v-if="exercise.displayName && exercise.displayName !== exercise.name"
                            class="text-muted d-block mb-2"
                          >
                            نام اصلی: {{ exercise.name }}
                          </small>

                          <div class="d-flex gap-2">
                            <input
                              type="number"
                              class="form-control form-control-sm"
                              style="width: 70px"
                              v-model.number="exercise.sets"
                              placeholder="ست"
                            />
                            <input
                              type="text"
                              class="form-control form-control-sm"
                              style="width: 100px"
                              v-model="exercise.reps"
                              placeholder="تکرار"
                            />
                            <input
                              type="text"
                              class="form-control form-control-sm"
                              style="width: 100px"
                              v-model="exercise.restTime"
                              placeholder="استراحت"
                            />
                          </div>
                        </div>

                        <button
                          class="btn btn-sm btn-link text-danger"
                          @click="removeExercise(weekIndex, dayIndex, exIndex)"
                        >
                          <i class="bi bi-trash"></i>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- دکمه افزودن حرکت -->
              <button
                class="btn btn-sm btn-outline-primary w-100"
                @click="openExerciseSelector(weekIndex, dayIndex)"
              >
                <i class="bi bi-plus-circle me-1"></i>
                افزودن حرکت
              </button>
            </div>
          </div>

          <!-- دکمه افزودن روز -->
          <button class="btn btn-sm btn-outline-secondary w-100 mt-3" @click="addDay(weekIndex)">
            <i class="bi bi-plus-circle me-1"></i>
            روز جدید
          </button>
        </div>
      </div>
    </div>

    <!-- مودال انتخابگر حرکت (با فیلترهای پیشرفته) -->
    <div
      class="modal fade"
      id="exerciseSelectorModal"
      tabindex="-1"
      ref="exerciseSelectorModal"
      data-bs-backdrop="static"
    >
      <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title">
              <i class="bi bi-search me-2"></i>
              انتخاب حرکت ورزشی
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              @click="closeSelector"
            ></button>
          </div>
          <div class="modal-body">
            <!-- کامپوننت ExerciseSelector با فیلترهای پیشرفته -->
            <ExerciseSelector
              @select="onExerciseSelected"
              @close="closeSelector"
              ref="exerciseSelectorRef"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- دکمه‌ها -->
    <div class="d-flex gap-2 mt-4">
      <button type="button" class="btn btn-outline-secondary flex-grow-1" @click="$emit('close')">
        انصراف
      </button>
      <button
        type="button"
        class="btn btn-primary flex-grow-1"
        @click="saveWorkout"
        :disabled="loading"
      >
        <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
        {{ loading ? 'در حال ثبت...' : 'ثبت برنامه' }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import * as bootstrap from 'bootstrap'
import ExerciseSelector from './ExerciseSelector.vue'
import { workoutService } from '../../services/workouts'
import { studentService } from '../../services/students'

const props = defineProps({
  studentId: {
    type: Number,
    default: null,
  },
})

const emit = defineEmits(['success', 'close'])

// State
const loading = ref(false)
const exerciseSelectorModal = ref(null)
const exerciseSelectorRef = ref(null)
const selectedWeek = ref(null)
const selectedDay = ref(null)
const students = ref([])

// داده‌های برنامه
const workout = reactive({
  studentId: props.studentId || '',
  title: '',
  description: '',
  startDate: new Date().toISOString().split('T')[0],
  endDate: '',
  durationWeeks: 4,
  weeks: [
    {
      weekNumber: 1,
      title: 'هفته اول',
      focus: '',
      days: [],
    },
  ],
})

// ========== متدهای کمکی ==========
const translateDay = (day) => {
  const days = {
    saturday: 'شنبه',
    sunday: 'یکشنبه',
    monday: 'دوشنبه',
    tuesday: 'سه‌شنبه',
    wednesday: 'چهارشنبه',
    thursday: 'پنج‌شنبه',
    friday: 'جمعه',
  }
  return days[day] || day
}

const getDayName = (dayNumber) => {
  const days = ['saturday', 'sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday']
  return days[(dayNumber - 1) % 7]
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
const getGifUrl = (url) => {
  // اگه آدرس لوکال هست، تبدیلش کن به CDN
  if (url.includes('localhost') || url.includes('https://localhost')) {
    const fileName = url.split('/').pop()
    return `https://static.exercisedb.dev/media/${fileName}`
  }
  return url
}
const updateExerciseName = (exercise, newName) => {
  if (!exercise.displayName) {
    // اگر displayName نداره، ایجادش کن
    exercise.displayName = newName
  } else {
    exercise.displayName = newName
  }
}
// ========== توابع تاریخ ==========
const calculateEndDate = () => {
  if (workout.startDate && workout.durationWeeks) {
    const startDate = new Date(workout.startDate)
    const endDate = new Date(startDate)
    endDate.setDate(endDate.getDate() + workout.durationWeeks * 7 - 1)
    workout.endDate = endDate.toISOString().split('T')[0]
  }
}

// ========== توابع شاگرد ==========
const fetchStudents = async () => {
  try {
    const response = await studentService.getStudents()
    if (response.success) {
      students.value = response.students
      // ⚠️ مهم: اینجا workout.studentId رو تغییر نده
      console.log('Students loaded, current workout.studentId:', workout.studentId)
    }
  } catch (err) {
    console.error('Error fetching students:', err)
  }
}

// ========== توابع هفته و روز ==========
const addWeek = () => {
  workout.weeks.push({
    weekNumber: workout.weeks.length + 1,
    title: `هفته ${workout.weeks.length + 1}`,
    focus: '',
    days: [],
  })
}

const removeWeek = (index) => {
  workout.weeks.splice(index, 1)
  // آپدیت شماره هفته‌ها
  workout.weeks.forEach((week, i) => {
    week.weekNumber = i + 1
  })
}

const addDay = (weekIndex) => {
  const week = workout.weeks[weekIndex]
  const dayNumber = week.days.length + 1
  week.days.push({
    dayNumber,
    dayName: getDayName(dayNumber),
    title: `روز ${dayNumber}`,
    focus: '',
    duration: 60,
    exercises: [],
  })
}

const removeDay = (weekIndex, dayIndex) => {
  workout.weeks[weekIndex].days.splice(dayIndex, 1)
}

const removeExercise = (weekIndex, dayIndex, exIndex) => {
  workout.weeks[weekIndex].days[dayIndex].exercises.splice(exIndex, 1)
}

// ========== توابع انتخابگر حرکت ==========
const openExerciseSelector = (weekIndex, dayIndex) => {
  selectedWeek.value = weekIndex
  selectedDay.value = dayIndex

  // اگر کامپوننت ExerciseSelector متدی برای پاک کردن فیلترها دارد
  if (exerciseSelectorRef.value?.resetFilters) {
    exerciseSelectorRef.value.resetFilters()
  }

  const modal = new bootstrap.Modal(exerciseSelectorModal.value)
  modal.show()
}

const closeSelector = () => {
  const modal = bootstrap.Modal.getInstance(exerciseSelectorModal.value)
  if (modal) {
    modal.hide()
  }
}

const onExerciseSelected = (exercise) => {
  if (selectedWeek.value !== null && selectedDay.value !== null) {
    workout.weeks[selectedWeek.value].days[selectedDay.value].exercises.push({
      exerciseId: exercise.exerciseId,
      name: exercise.name,
      displayName: exercise.name,
      gifUrl: exercise.gifUrl,
      sets: 3,
      reps: '10-12',
      restTime: '60-90 ثانیه',
      notes: '',
      description: exercise.instructions?.[0] || '',
      order: workout.weeks[selectedWeek.value].days[selectedDay.value].exercises.length + 1,
    })
  }
  closeSelector()
}
// ========== ذخیره برنامه ==========
const saveWorkout = async () => {
  console.log('📤 Before validation - workout.studentId:', workout.studentId)
  console.log('📤 props.studentId:', props.studentId)

  // اگه workout.studentId خالی بود، از props استفاده کن
  if (!workout.studentId && props.studentId) {
    workout.studentId = props.studentId
    console.log('📤 Fixed - using props.studentId:', workout.studentId)
  }

  // اگه بازم خالی بود، از localStorage استفاده کن
  if (!workout.studentId) {
    const tempId = localStorage.getItem('tempStudentId')
    if (tempId) {
      workout.studentId = parseInt(tempId)
      localStorage.removeItem('tempStudentId')
      console.log('📤 Fixed - using tempStudentId:', workout.studentId)
    }
  }

  // اعتبارسنجی نهایی
  if (!workout.studentId) {
    alert('لطفاً شاگرد را انتخاب کنید')
    return
  }

  if (!workout.title) {
    alert('لطفاً عنوان برنامه را وارد کنید')
    return
  }

  // بررسی حداقل یک حرکت
  const hasExercise = workout.weeks.some((week) =>
    week.days.some((day) => day.exercises.length > 0),
  )

  if (!hasExercise) {
    alert('حداقل یک حرکت به برنامه اضافه کنید')
    return
  }

  loading.value = true
  try {
    // محاسبه تاریخ پایان اگر محاسبه نشده
    if (!workout.endDate) {
      calculateEndDate()
    }

    // ✅ تبدیل تاریخ‌ها به UTC
    const workoutData = {
      studentId: Number(workout.studentId), // این باید studentId باشه (آیدی پروفایل)
      title: workout.title,
      description: workout.description,
      startDate: new Date(workout.startDate).toISOString(),
      endDate: new Date(workout.endDate).toISOString(),
      durationWeeks: workout.durationWeeks,
      weeks: workout.weeks.map((week) => ({
        weekNumber: week.weekNumber,
        title: week.title,
        focus: week.focus,
        days: week.days.map((day) => ({
          dayNumber: day.dayNumber,
          dayName: day.dayName,
          title: day.title,
          focus: day.focus,
          duration: day.duration,
          exercises: day.exercises.map((ex) => ({
            exerciseId: ex.exerciseId,
            name: ex.displayName || ex.name,
            description: ex.description || '',
            sets: ex.sets || 3,
            reps: ex.reps || '10-12',
            restTime: ex.restTime || '60-90 ثانیه',
            gifUrl: ex.gifUrl,
          })),
        })),
      })),
    }

    console.log('📤 Sending workout data with studentId:', workoutData.studentId)
    console.log('📤 Full data:', JSON.stringify(workoutData, null, 2))

    const response = await workoutService.createWorkout(workoutData)

    if (response.success) {
      emit('success', response)
    }
  } catch (error) {
    console.error('❌ Error saving workout:', error)
    console.error('❌ Response data:', error.response?.data)
    console.error('❌ Response status:', error.response?.status)
    alert(`خطا در ذخیره برنامه: ${error.response?.data?.message || 'خطای ناشناخته'}`)
  } finally {
    loading.value = false
  }
}
// ========== Lifecycle ==========
onMounted(() => {
  // اولویت با localStorage
  const tempId = localStorage.getItem('tempStudentId')
  if (tempId) {
    workout.studentId = parseInt(tempId)
    console.log('📌 Using tempStudentId:', workout.studentId)
    localStorage.removeItem('tempStudentId')
  }
  // بعد props
  else if (props.studentId) {
    workout.studentId = props.studentId
    console.log('📌 Using props.studentId:', workout.studentId)
  }

  // اگر studentId نداریم، لیست شاگردان را بارگذاری کن
  // ❌ این رو به بعد از مقداردهی منتقل کن
  if (!props.studentId && !tempId) {
    fetchStudents()
  } else {
    // حتی اگر studentId داریم، بازم fetchStudents رو صدا بزن
    // ولی مقدار workout.studentId رو تغییر نده
    fetchStudents()
  }

  // ایجاد روزهای پیش‌فرض برای هفته اول
  const firstWeek = workout.weeks[0]
  for (let i = 1; i <= 3; i++) {
    firstWeek.days.push({
      dayNumber: i,
      dayName: getDayName(i),
      title: `روز ${i}`,
      focus: '',
      duration: 60,
      exercises: [],
    })
  }

  calculateEndDate()
})
const resetForm = () => {
  workout.studentId = props.studentId || ''
  workout.title = ''
  workout.description = ''
  workout.startDate = new Date().toISOString().split('T')[0]
  workout.endDate = ''
  workout.durationWeeks = 4
  workout.weeks = [
    {
      weekNumber: 1,
      title: 'هفته اول',
      focus: '',
      days: [],
    },
  ]
  const firstWeek = workout.weeks[0]
  for (let i = 1; i <= 3; i++) {
    firstWeek.days.push({
      dayNumber: i,
      dayName: getDayName(i),
      title: `روز ${i}`,
      focus: '',
      duration: 60,
      exercises: [],
    })
  }
  const tempId = localStorage.getItem('tempStudentId')
  if (tempId) {
    workout.studentId = parseInt(tempId)
    localStorage.removeItem('tempStudentId')
  }

  calculateEndDate()
}
</script>

<style scoped>
.object-fit-cover {
  object-fit: cover;
}

/* استایل برای مودال */
:deep(.modal-xl) {
  max-width: 800px;
}

@media (min-width: 992px) {
  :deep(.modal-xl) {
    max-width: 900px;
  }
}
</style>
