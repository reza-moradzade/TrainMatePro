<template>
  <div class="workout-list">
    <!-- لیست برنامه‌ها -->
    <div v-if="workouts.length > 0" class="list-group">
      <div
        v-for="workout in workouts"
        :key="workout.id"
        class="list-group-item border-0 shadow-sm mb-2 rounded-4 p-3"
      >
        <div class="d-flex justify-content-between align-items-start mb-2">
          <div>
            <h6 class="fw-bold mb-1">{{ workout.title }}</h6>
            <p class="small text-secondary mb-1">{{ workout.student?.fullName }}</p>
          </div>
          <span class="badge" :class="getStatusBadge(workout.status)">
            {{ getStatusText(workout.status) }}
          </span>
        </div>

        <!-- تاریخ برنامه -->
        <div class="small text-secondary mb-2">
          <i class="bi bi-calendar3 me-1"></i>
          {{ formatDate(workout.startDate) }} - {{ formatDate(workout.endDate) }}
        </div>

        <!-- آمار سریع -->
        <div class="d-flex gap-3 small text-secondary mb-3">
          <span>
            <i class="bi bi-calendar-week me-1"></i>
            {{ workout.durationWeeks }} هفته
          </span>
          <span>
            <i class="bi bi-activity me-1"></i>
            {{ getTotalExercises(workout) }} حرکت
          </span>
        </div>

        <!-- دکمه‌ها -->
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary flex-grow-1" @click="viewWorkout(workout)">
            <i class="bi bi-eye"></i>
            <span class="d-none d-sm-inline ms-1">مشاهده</span>
          </button>
          <button class="btn btn-sm btn-outline-warning flex-grow-1" @click="editWorkout(workout)">
            <i class="bi bi-pencil"></i>
            <span class="d-none d-sm-inline ms-1">ویرایش</span>
          </button>
          <button class="btn btn-sm btn-outline-danger flex-grow-1" @click="deleteWorkout(workout)">
            <i class="bi bi-trash"></i>
            <span class="d-none d-sm-inline ms-1">حذف</span>
          </button>
        </div>
      </div>
    </div>

    <!-- حالت خالی -->
    <div v-else class="text-center py-4 bg-white rounded-4">
      <i class="bi bi-calendar-x fs-1 text-secondary opacity-25"></i>
      <p class="text-secondary small mt-2">برنامه‌ای وجود ندارد</p>
      <button class="btn btn-sm btn-outline-primary">ایجاد برنامه جدید</button>
    </div>

    <!-- مودال ویرایش برنامه -->
    <div class="modal fade" id="editWorkoutModal" tabindex="-1" ref="editWorkoutModal">
      <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title">
              <i class="bi bi-pencil-square me-2 text-primary"></i>
              ویرایش برنامه تمرینی
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <EditWorkout
              v-if="selectedWorkout"
              :workout-id="selectedWorkout.id"
              @success="onWorkoutUpdated"
              @close="closeEditModal"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- مودال مشاهده برنامه -->
    <div class="modal fade" id="viewWorkoutModal" tabindex="-1" ref="viewWorkoutModal">
      <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title">
              <i class="bi bi-eye me-2 text-primary"></i>
              مشاهده برنامه تمرینی
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <div v-if="selectedWorkout" class="workout-detail">
              <!-- محتوای برنامه برای مشاهده -->
              <div class="mb-4">
                <h6 class="fw-bold mb-3">اطلاعات برنامه</h6>
                <div class="bg-light p-3 rounded-3">
                  <p><strong>شاگرد:</strong> {{ selectedWorkout.student?.fullName }}</p>
                  <p><strong>عنوان:</strong> {{ selectedWorkout.title }}</p>
                  <p><strong>توضیحات:</strong> {{ selectedWorkout.description || 'ندارد' }}</p>
                  <p><strong>مدت:</strong> {{ selectedWorkout.durationWeeks }} هفته</p>
                  <p>
                    <strong>تاریخ:</strong> {{ formatDate(selectedWorkout.startDate) }} تا
                    {{ formatDate(selectedWorkout.endDate) }}
                  </p>
                </div>
              </div>

              <div v-for="week in selectedWorkout.weeks" :key="week.weekNumber" class="mb-4">
                <h6 class="fw-bold mb-2">هفته {{ week.weekNumber }}</h6>
                <div v-if="week.focus" class="small text-secondary mb-2">
                  <i class="bi bi-bullseye me-1"></i> {{ week.focus }}
                </div>

                <div v-for="day in week.days" :key="day.id" class="mb-3">
                  <div class="d-flex justify-content-between align-items-center">
                    <span class="badge bg-light text-secondary px-3 py-2">
                      {{ getDayName(day.dayName) }}
                    </span>
                    <span class="small text-secondary">{{ day.exercises.length }} حرکت</span>
                  </div>

                  <div v-if="day.exercises.length > 0" class="mt-2">
                    <div v-for="(ex, idx) in day.exercises" :key="idx" class="small text-secondary">
                      {{ idx + 1 }}. {{ ex.name }} - {{ ex.sets }}×{{ ex.reps }}
                    </div>
                  </div>
                  <div v-else class="small text-secondary mt-2">بدون حرکت</div>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer border-0">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">بستن</button>
          </div>
        </div>
      </div>
    </div>

    <!-- مودال تأیید حذف -->
    <div class="modal fade" id="deleteConfirmModal" tabindex="-1" ref="deleteConfirmModal">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title text-danger">
              <i class="bi bi-exclamation-triangle-fill me-2"></i>
              تأیید حذف
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <p>آیا از حذف برنامه "{{ selectedWorkout?.title }}" اطمینان دارید؟</p>
            <p class="small text-secondary">این عمل قابل بازگشت نیست.</p>
          </div>
          <div class="modal-footer border-0">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">انصراف</button>
            <button
              type="button"
              class="btn btn-danger"
              @click="confirmDelete"
              :disabled="deleting"
            >
              <span v-if="deleting" class="spinner-border spinner-border-sm me-2"></span>
              {{ deleting ? 'در حال حذف...' : 'حذف برنامه' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import * as bootstrap from 'bootstrap'
import EditWorkout from './EditWorkout.vue'
import { workoutService } from '../../services/workouts'

const props = defineProps({
  workouts: {
    type: Array,
    required: true,
  },
})

const emit = defineEmits(['refresh'])

// State
const selectedWorkout = ref(null)
const deleting = ref(false)
const editWorkoutModal = ref(null)
const viewWorkoutModal = ref(null)
const deleteConfirmModal = ref(null)

// توابع کمکی
const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('fa-IR')
}

const getDayName = (dayName) => {
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

const getStatusBadge = (status) => {
  const badges = {
    active: 'bg-success',
    completed: 'bg-secondary',
    cancelled: 'bg-danger',
  }
  return badges[status] || 'bg-primary'
}

const getStatusText = (status) => {
  const texts = {
    active: 'فعال',
    completed: 'تکمیل شده',
    cancelled: 'لغو شده',
  }
  return texts[status] || status
}

const getTotalExercises = (workout) => {
  return workout.weeks.reduce(
    (total, week) =>
      total + week.days.reduce((dayTotal, day) => dayTotal + day.exercises.length, 0),
    0,
  )
}

// توابع اصلی
const viewWorkout = (workout) => {
  selectedWorkout.value = workout
  const modal = new bootstrap.Modal(viewWorkoutModal.value)
  modal.show()
}

const editWorkout = (workout) => {
  selectedWorkout.value = workout
  const modal = new bootstrap.Modal(editWorkoutModal.value)
  modal.show()
}

const deleteWorkout = (workout) => {
  selectedWorkout.value = workout
  const modal = new bootstrap.Modal(deleteConfirmModal.value)
  modal.show()
}

const confirmDelete = async () => {
  deleting.value = true
  try {
    await workoutService.deleteWorkout(selectedWorkout.value.id)

    // بستن مودال
    const modal = bootstrap.Modal.getInstance(deleteConfirmModal.value)
    if (modal) modal.hide()

    // رفرش لیست
    emit('refresh')
  } catch (error) {
    console.error('Error deleting workout:', error)
    alert('خطا در حذف برنامه')
  } finally {
    deleting.value = false
  }
}

const onWorkoutUpdated = (response) => {
  const modal = bootstrap.Modal.getInstance(editWorkoutModal.value)
  if (modal) modal.hide()
  emit('refresh')
}

const closeEditModal = () => {
  const modal = bootstrap.Modal.getInstance(editWorkoutModal.value)
  if (modal) modal.hide()
}
</script>
