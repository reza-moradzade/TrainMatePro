<template>
  <div class="edit-workout">
    <!-- هدر -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h5 class="fw-bold mb-0">
        <i class="bi bi-pencil-square text-primary me-2"></i>
        ویرایش برنامه تمرینی
      </h5>
      <!-- اصلاح: استفاده از form.title به جای workout?.title -->
      <span class="badge bg-primary">{{ form.title || 'بدون عنوان' }}</span>
    </div>

    <!-- فرم ویرایش (مشابه CreateWorkout با داده‌های پر شده) -->
    <form @submit.prevent="handleSubmit">
      <!-- اطلاعات پایه برنامه -->
      <div class="mb-4">
        <h6 class="fw-bold mb-3">اطلاعات برنامه</h6>

        <div class="mb-3">
          <label class="form-label small fw-bold">عنوان برنامه</label>
          <input
            type="text"
            class="form-control"
            v-model="form.title"
            placeholder="مثال: برنامه افزایش قدرت - هفته اول"
            required
          />
        </div>

        <div class="mb-3">
          <label class="form-label small fw-bold">توضیحات</label>
          <textarea
            class="form-control"
            rows="2"
            v-model="form.description"
            placeholder="توضیحات کلی برنامه..."
          ></textarea>
        </div>

        <div class="row">
          <div class="col-6 mb-3">
            <label class="form-label small fw-bold">تاریخ شروع</label>
            <input type="date" class="form-control" v-model="form.startDate" required />
          </div>
          <div class="col-6 mb-3">
            <label class="form-label small fw-bold">تاریخ پایان</label>
            <input type="date" class="form-control" v-model="form.endDate" readonly />
          </div>
        </div>

        <div class="mb-3">
          <label class="form-label small fw-bold">مدت (هفته)</label>
          <input
            type="number"
            class="form-control"
            v-model.number="form.durationWeeks"
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
          <button type="button" class="btn btn-sm btn-outline-primary" @click="addWeek">
            <i class="bi bi-plus-circle me-1"></i>
            هفته جدید
          </button>
        </div>

        <!-- هفته‌ها -->
        <div
          v-for="(week, weekIndex) in form.weeks"
          :key="weekIndex"
          class="card mb-3 border-0 shadow-sm"
        >
          <div
            class="card-header bg-white border-0 d-flex justify-content-between align-items-center"
          >
            <h6 class="fw-bold mb-0">هفته {{ week.weekNumber }}</h6>
            <button
              type="button"
              class="btn btn-sm btn-light text-danger"
              @click="removeWeek(weekIndex)"
              :disabled="form.weeks.length <= 1"
            >
              <i class="bi bi-trash"></i>
            </button>
          </div>

          <div class="card-body">
            <!-- هدف هفته -->
            <div class="mb-3">
              <label class="form-label small fw-bold">هدف هفته</label>
              <input
                type="text"
                class="form-control"
                v-model="week.focus"
                placeholder="مثال: افزایش قدرت پایه"
              />
            </div>

            <!-- روزها -->
            <div v-for="(day, dayIndex) in week.days" :key="dayIndex" class="mb-4">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="badge bg-light text-secondary px-3 py-2">
                  {{ getFullDayName(day.dayName) }}
                </span>
                <button
                  type="button"
                  class="btn btn-sm btn-link text-danger"
                  @click="removeDay(weekIndex, dayIndex)"
                  :disabled="week.days.length <= 1"
                >
                  <i class="bi bi-x-lg"></i>
                </button>
              </div>

              <!-- تمرکز روز -->
              <div class="mb-2">
                <input
                  type="text"
                  class="form-control form-control-sm"
                  v-model="day.focus"
                  placeholder="تمرکز روز (مثال: تمرین سینه)"
                />
              </div>

              <!-- مدت زمان -->
              <div class="mb-2">
                <input
                  type="number"
                  class="form-control form-control-sm"
                  v-model.number="day.duration"
                  placeholder="مدت زمان (دقیقه)"
                />
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
                          :src="exercise.gifUrl"
                          class="w-100 h-100 object-fit-cover rounded"
                          :alt="exercise.name"
                          @error="handleImageError"
                        />
                      </div>

                      <!-- جزئیات حرکت -->
                      <div class="flex-grow-1">
                        <div class="d-flex justify-content-between">
                          <div class="flex-grow-1">
                            <!-- نام حرکت قابل ویرایش -->
                            <input
                              type="text"
                              class="form-control form-control-sm mb-2"
                              :value="exercise.displayName || exercise.name"
                              @input="updateExerciseName(exercise, $event.target.value)"
                              placeholder="نام حرکت"
                            />
                            <small
                              v-if="exercise.displayName && exercise.displayName !== exercise.name"
                              class="text-muted d-block mb-2"
                            >
                              نام اصلی: {{ exercise.name }}
                            </small>

                            <div class="d-flex gap-2 flex-wrap">
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
                            type="button"
                            class="btn btn-sm btn-link text-danger ms-2"
                            @click="removeExercise(weekIndex, dayIndex, exIndex)"
                          >
                            <i class="bi bi-trash"></i>
                          </button>
                        </div>

                        <!-- توضیحات حرکت -->
                        <textarea
                          v-model="exercise.description"
                          class="form-control form-control-sm mt-2"
                          rows="2"
                          placeholder="نکات فنی حرکت (اختیاری)"
                        ></textarea>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- دکمه افزودن حرکت -->
                <button
                  type="button"
                  class="btn btn-sm btn-outline-primary w-100 mt-2"
                  @click="openExerciseSelector(weekIndex, dayIndex)"
                >
                  <i class="bi bi-plus-circle me-1"></i>
                  افزودن حرکت
                </button>
              </div>
            </div>

            <!-- دکمه افزودن روز -->
            <button
              type="button"
              class="btn btn-sm btn-outline-secondary w-100 mt-3"
              @click="addDay(weekIndex)"
            >
              <i class="bi bi-plus-circle me-1"></i>
              روز جدید
            </button>
          </div>
        </div>
      </div>

      <!-- دکمه‌ها -->
      <div class="d-flex gap-2 mt-4">
        <button type="button" class="btn btn-outline-secondary flex-grow-1" @click="$emit('close')">
          انصراف
        </button>
        <button type="submit" class="btn btn-primary flex-grow-1" :disabled="loading">
          <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
          {{ loading ? 'در حال ذخیره...' : 'ذخیره تغییرات' }}
        </button>
      </div>
    </form>

    <!-- مودال انتخابگر حرکت -->
    <div
      class="modal fade"
      id="editExerciseSelectorModal"
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
            <ExerciseSelector
              @select="onExerciseSelected"
              @close="closeSelector"
              ref="exerciseSelectorRef"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import * as bootstrap from 'bootstrap'
import ExerciseSelector from './ExerciseSelector.vue'
import { workoutService } from '../../services/workouts'

const props = defineProps({
  workoutId: {
    type: Number,
    required: true,
  },
})

const emit = defineEmits(['success', 'close'])

// State
const loading = ref(false)
const originalWorkout = ref(null)
const exerciseSelectorModal = ref(null)
const exerciseSelectorRef = ref(null)
const selectedWeek = ref(null)
const selectedDay = ref(null)
const error = ref('')

// فرم ویرایش
const form = reactive({
  studentId: null,
  title: '',
  description: '',
  startDate: '',
  endDate: '',
  durationWeeks: 4,
  weeks: [],
})

// ========== توابع کمکی ==========
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

const getDayName = (dayNumber) => {
  const days = ['saturday', 'sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday']
  return days[(dayNumber - 1) % 7]
}

const handleImageError = (e) => {
  e.target.src = 'https://via.placeholder.com/60?text=No+Image'
}

const updateExerciseName = (exercise, newName) => {
  if (!exercise.displayName) {
    exercise.displayName = newName
  } else {
    exercise.displayName = newName
  }
}

// ========== توابع تاریخ ==========
const calculateEndDate = () => {
  if (form.startDate && form.durationWeeks) {
    const startDate = new Date(form.startDate)
    const endDate = new Date(startDate)
    endDate.setDate(endDate.getDate() + form.durationWeeks * 7 - 1)
    form.endDate = endDate.toISOString().split('T')[0]
  }
}

// ========== توابع هفته و روز ==========
const addWeek = () => {
  const newWeekNumber = form.weeks.length + 1
  const firstWeek = form.weeks[0]

  // ایجاد هفته جدید بر اساس هفته اول
  const newWeek = {
    weekNumber: newWeekNumber,
    title: `هفته ${newWeekNumber}`,
    focus: '',
    days: firstWeek
      ? firstWeek.days.map((day) => ({
          dayNumber: day.dayNumber,
          dayName: day.dayName,
          title: day.title,
          focus: '',
          duration: day.duration || 60,
          exercises: [],
        }))
      : [],
    customized: false,
  }

  form.weeks.push(newWeek)
}

const removeWeek = (index) => {
  if (form.weeks.length <= 1) return
  form.weeks.splice(index, 1)
  // آپدیت شماره هفته‌ها
  form.weeks.forEach((week, i) => {
    week.weekNumber = i + 1
  })
}

const addDay = (weekIndex) => {
  const week = form.weeks[weekIndex]
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
  const week = form.weeks[weekIndex]
  if (week.days.length <= 1) return
  week.days.splice(dayIndex, 1)
}

const removeExercise = (weekIndex, dayIndex, exIndex) => {
  form.weeks[weekIndex].days[dayIndex].exercises.splice(exIndex, 1)
}

// ========== توابع انتخابگر حرکت ==========
const openExerciseSelector = (weekIndex, dayIndex) => {
  selectedWeek.value = weekIndex
  selectedDay.value = dayIndex

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
    form.weeks[selectedWeek.value].days[selectedDay.value].exercises.push({
      exerciseId: exercise.exerciseId,
      name: exercise.name,
      displayName: exercise.name,
      gifUrl: exercise.gifUrl,
      sets: 3,
      reps: '10-12',
      restTime: '60-90 ثانیه',
      notes: '',
      description: exercise.instructions?.[0] || '',
      order: form.weeks[selectedWeek.value].days[selectedDay.value].exercises.length + 1,
    })
  }
  closeSelector()
}

// ========== بارگذاری داده‌های برنامه ==========
const loadWorkoutData = async () => {
  loading.value = true
  error.value = ''
  try {
    console.log('📥 Loading workout with ID:', props.workoutId)
    const response = await workoutService.getWorkoutById(props.workoutId)
    console.log('📥 Workout data response:', response)

    if (response.success) {
      const program = response.program
      originalWorkout.value = program

      // پر کردن فرم با داده‌های موجود
      form.studentId = program.student?.id || null // این رو نگه می‌داریم ولی استفاده نمی‌کنیم
      form.title = program.title || ''
      form.description = program.description || ''
      form.startDate = program.startDate ? program.startDate.split('T')[0] : ''
      form.endDate = program.endDate ? program.endDate.split('T')[0] : ''
      form.durationWeeks = program.durationWeeks || 4

      // لاگ برای دیباگ
      console.log('📥 Program student data:', program.student)
      console.log('📥 StudentProfile ID from API:', program.student?.id)

      // تبدیل داده‌های هفته‌ها
      form.weeks = (program.weeks || []).map((week) => ({
        weekNumber: week.weekNumber,
        title: week.title || `هفته ${week.weekNumber}`,
        focus: week.focus || '',
        days: (week.days || []).map((day) => ({
          dayNumber: day.dayNumber,
          dayName: day.dayName,
          title: day.title || '',
          focus: day.focus || '',
          duration: day.duration || 60,
          exercises: (day.exercises || []).map((ex) => ({
            exerciseId: ex.exerciseId,
            name: ex.name,
            displayName: ex.name,
            gifUrl: ex.gifUrl,
            sets: ex.sets || 3,
            reps: ex.reps || '10-12',
            restTime: ex.restTime || '60-90 ثانیه',
            description: ex.description || '',
          })),
        })),
        customized: week.customized || false,
      }))

      console.log('✅ Form populated:', form)
    } else {
      error.value = 'خطا در دریافت اطلاعات برنامه'
    }
  } catch (error) {
    console.error('❌ Error loading workout:', error)
    error.value = 'خطا در ارتباط با سرور'
  } finally {
    loading.value = false
  }
}

// ========== ذخیره تغییرات ==========
const handleSubmit = async () => {
  // اعتبارسنجی
  if (!form.title) {
    alert('لطفاً عنوان برنامه را وارد کنید')
    return
  }

  // بررسی حداقل یک حرکت
  const hasExercise = form.weeks.some((week) => week.days.some((day) => day.exercises.length > 0))

  if (!hasExercise) {
    alert('حداقل یک حرکت به برنامه اضافه کنید')
    return
  }

  loading.value = true
  try {
    // ⚠️ IMPORTANT: از student.id استفاده کن که تو originalWorkout ذخیره شده
    // این student.id همون studentProfileId هست (در مثال ما = 2 برای شاگرد دوم)
    const correctStudentId = originalWorkout.value?.student?.id

    if (!correctStudentId) {
      console.error('Student ID not found in original workout')
      alert('خطا: شناسه شاگرد یافت نشد')
      return
    }

    console.log('✅ Using studentProfileId for update:', correctStudentId)
    console.log('❌ Not using form.studentId which is:', form.studentId)

    // آماده‌سازی داده‌ها برای ارسال
    const workoutData = {
      studentId: correctStudentId, // استفاده از studentProfileId درست
      title: form.title,
      description: form.description,
      startDate: new Date(form.startDate).toISOString(),
      endDate: new Date(form.endDate).toISOString(),
      durationWeeks: form.durationWeeks,
      weeks: form.weeks.map((week) => ({
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
            sets: ex.sets,
            reps: ex.reps,
            restTime: ex.restTime,
            gifUrl: ex.gifUrl,
          })),
        })),
      })),
    }

    console.log('📤 Updating workout with data:', workoutData)
    const response = await workoutService.updateWorkout(props.workoutId, workoutData)
    console.log('📥 Update response:', response)

    if (response.success) {
      emit('success', response)
    } else {
      alert('خطا در ویرایش برنامه')
    }
  } catch (error) {
    console.error('❌ Error updating workout:', error)
    console.error('❌ Error response:', error.response?.data)
    alert(`خطا در ویرایش برنامه: ${error.response?.data?.message || 'خطای ناشناخته'}`)
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadWorkoutData()
})
</script>

<style scoped>
.object-fit-cover {
  object-fit: cover;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
}

.card-header {
  background-color: #f8f9fa;
}
</style>
