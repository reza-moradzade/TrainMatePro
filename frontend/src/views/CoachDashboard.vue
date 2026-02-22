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
        <h5 class="offcanvas-title text-primary">منوی مربی</h5>
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
          <router-link to="/dashboard" class="list-group-item list-group-item-action active">
            <i class="bi bi-speedometer2 me-2"></i>داشبورد
          </router-link>
          <router-link to="/coach/students" class="list-group-item list-group-item-action">
            <i class="bi bi-people me-2"></i>مدیریت شاگردان
          </router-link>
          <router-link to="/coach/workouts" class="list-group-item list-group-item-action">
            <i class="bi bi-calendar-check me-2"></i>همه برنامه‌ها
          </router-link>
          <router-link to="/coach/reports" class="list-group-item list-group-item-action">
            <i class="bi bi-bar-chart me-2"></i>گزارشات
          </router-link>
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

    <!-- کامپوننت Toast برای پیام‌ها -->
    <Toast ref="toastRef" />

    <!-- محتوای اصلی -->
    <main class="container-fluid px-3 py-4">
      <!-- خوش‌آمدگویی -->
      <div class="mb-4">
        <h1 class="h4 fw-bold mb-1">سلام، {{ authStore.user?.fullName }}</h1>
        <p class="text-secondary small">مدیریت برنامه‌های تمرینی شاگردان</p>
      </div>

      <!-- بخش انتخاب شاگرد با کامبو باکس -->
      <div class="student-selector-section mb-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h2 class="h5 fw-bold mb-0">
            <i class="bi bi-person-badge text-primary me-2"></i>
            انتخاب شاگرد
          </h2>
          <button
            class="btn btn-sm btn-primary"
            data-bs-toggle="modal"
            data-bs-target="#addStudentModal"
          >
            <i class="bi bi-plus-circle me-1"></i>
            شاگرد جدید
          </button>
        </div>

        <!-- حالت بارگذاری -->
        <div v-if="loading" class="text-center py-4">
          <div class="spinner-border text-primary" role="status"></div>
        </div>

        <!-- کامبو باکس انتخاب شاگرد -->
        <div v-else-if="students.length > 0" class="row">
          <div class="col-12">
            <select
              class="form-select form-select-lg rounded-4 border-0 shadow-sm"
              v-model="selectedStudentId"
              @change="onStudentSelect"
            >
              <option value="" disabled selected>انتخاب شاگرد...</option>
              <option v-for="student in students" :key="student.userId" :value="student.userId">
                {{ student.fullName }} ({{ student.email }})
              </option>
            </select>
          </div>
        </div>

        <!-- حالت خالی شاگرد -->
        <div v-else class="text-center py-4 bg-white rounded-4">
          <i class="bi bi-people fs-1 text-secondary opacity-25"></i>
          <p class="text-secondary mt-2">هنوز شاگردی ثبت نکرده‌اید</p>
          <button
            class="btn btn-sm btn-primary mt-2"
            data-bs-toggle="modal"
            data-bs-target="#addStudentModal"
          >
            <i class="bi bi-plus-circle me-1"></i>
            ثبت اولین شاگرد
          </button>
        </div>
      </div>

      <!-- بخش برنامه‌های شاگرد انتخاب شده -->
      <div v-if="selectedStudent" class="student-workouts-section">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h2 class="h5 fw-bold mb-0">
            <i class="bi bi-calendar-check text-primary me-2"></i>
            برنامه‌های {{ selectedStudent.fullName }}
          </h2>
          <button class="btn btn-sm btn-success" @click="createWorkoutForSelectedStudent">
            <i class="bi bi-plus-circle me-1"></i>
            برنامه جدید
          </button>
        </div>

        <!-- اطلاعات شاگرد (خلاصه) -->
        <div class="student-info-card bg-light p-3 rounded-4 mb-3">
          <div class="row g-2">
            <div class="col-6">
              <span class="small text-secondary d-block">سن</span>
              <span class="fw-bold">{{ selectedStudent.age || '—' }}</span>
            </div>
            <div class="col-6">
              <span class="small text-secondary d-block">قد/وزن</span>
              <span class="fw-bold"
                >{{ selectedStudent.height || '?' }}cm / {{ selectedStudent.weight || '?' }}kg</span
              >
            </div>
            <div class="col-6">
              <span class="small text-secondary d-block">سطح</span>
              <span class="fw-bold">{{ getLevelText(selectedStudent.fitnessLevel) }}</span>
            </div>
            <div class="col-6">
              <span class="small text-secondary d-block">اهداف</span>
              <span class="fw-bold text-truncate">{{ selectedStudent.goals || '—' }}</span>
            </div>
          </div>
        </div>

        <!-- لیست برنامه‌ها -->
        <div v-if="studentWorkouts.length > 0" class="workouts-list">
          <div
            v-for="workout in studentWorkouts"
            :key="workout.id"
            class="workout-card card border-0 shadow-sm mb-3"
          >
            <div class="card-body">
              <!-- هدر برنامه -->
              <div class="d-flex justify-content-between align-items-start mb-2">
                <div>
                  <h6 class="fw-bold mb-1">{{ workout.title }}</h6>
                  <div class="small text-secondary">
                    <i class="bi bi-calendar3 me-1"></i>
                    {{ formatDate(workout.startDate) }} - {{ formatDate(workout.endDate) }}
                  </div>
                </div>
                <span class="badge" :class="getStatusBadge(workout.status)">
                  {{ getStatusText(workout.status) }}
                </span>
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

              <!-- دکمه‌های عملیات -->
              <div class="d-flex gap-2">
                <button
                  class="btn btn-sm btn-outline-primary flex-grow-1"
                  @click="viewWorkout(workout)"
                >
                  <i class="bi bi-eye"></i>
                  <span class="d-none d-sm-inline ms-1">مشاهده</span>
                </button>
                <button
                  class="btn btn-sm btn-outline-warning flex-grow-1"
                  @click="editWorkout(workout)"
                >
                  <i class="bi bi-pencil"></i>
                  <span class="d-none d-sm-inline ms-1">ویرایش</span>
                </button>
                <button
                  class="btn btn-sm btn-outline-danger flex-grow-1"
                  @click="deleteWorkout(workout)"
                >
                  <i class="bi bi-trash"></i>
                  <span class="d-none d-sm-inline ms-1">حذف</span>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- حالت خالی برنامه -->
        <div v-else class="text-center py-5 bg-white rounded-4">
          <i class="bi bi-calendar-x fs-1 text-secondary opacity-25"></i>
          <p class="text-secondary mt-2">برنامه‌ای برای این شاگرد وجود ندارد</p>
          <button class="btn btn-sm btn-primary mt-2" @click="createWorkoutForSelectedStudent">
            <i class="bi bi-plus-circle me-1"></i>
            ایجاد اولین برنامه
          </button>
        </div>
      </div>

      <!-- وقتی شاگردی انتخاب نشده -->
      <div v-else class="no-student-selected text-center py-5">
        <i class="bi bi-person-up display-1 text-secondary opacity-25"></i>
        <p class="text-secondary mt-3">لطفاً یک شاگرد را انتخاب کنید</p>
      </div>
    </main>

    <!-- Modal افزودن شاگرد -->
    <div
      class="modal fade"
      id="addStudentModal"
      tabindex="-1"
      ref="addStudentModal"
      data-bs-backdrop="static"
    >
      <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0 pb-0">
            <h5 class="modal-title">
              <i class="bi bi-person-plus-fill me-2 text-primary"></i>
              شاگرد جدید
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              @click="resetAddStudentForm"
            ></button>
          </div>
          <div class="modal-body">
            <AddStudentForm
              :key="addStudentFormKey"
              @success="onStudentAdded"
              @close="closeModal"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Modal ایجاد برنامه جدید -->
    <div
      class="modal fade"
      id="createWorkoutModal"
      tabindex="-1"
      ref="createWorkoutModal"
      data-bs-backdrop="static"
    >
      <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title">
              <i class="bi bi-calendar-plus me-2 text-primary"></i>
              برنامه تمرینی جدید برای {{ selectedStudent?.fullName }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              @click="resetWorkoutForm"
            ></button>
          </div>
          <div class="modal-body">
            <Transition name="fade" mode="out-in">
              <CreateWorkout
                v-if="selectedStudent"
                :key="workoutFormKey"
                :student-id="selectedStudent.studentId"
                @success="onWorkoutCreated"
                @close="closeWorkoutModal"
              />
            </Transition>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal مشاهده برنامه -->
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
                  <p><strong>شاگرد:</strong> {{ selectedStudent?.fullName }}</p>
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
                      <div class="d-flex align-items-center gap-2 mb-1">
                        <img
                          :src="ex.gifUrl"
                          class="rounded"
                          style="width: 30px; height: 30px; object-fit: cover"
                        />
                        <span>{{ idx + 1 }}. {{ ex.name }} - {{ ex.sets }}×{{ ex.reps }}</span>
                      </div>
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

    <!-- Modal ویرایش برنامه -->
    <div
      class="modal fade"
      id="editWorkoutModal"
      tabindex="-1"
      ref="editWorkoutModal"
      data-bs-backdrop="static"
    >
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
              :key="editWorkoutFormKey"
              :workout-id="selectedWorkout.id"
              @success="onWorkoutUpdated"
              @close="closeEditModal"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Modal تأیید حذف -->
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { studentService } from '../services/students'
import { workoutService } from '../services/workouts'
import AddStudentForm from '../components/coach/AddStudentForm.vue'
import CreateWorkout from '../components/coach/CreateWorkout.vue'
import EditWorkout from '../components/coach/EditWorkout.vue'
import Toast from '../components/common/Toast.vue'
import * as bootstrap from 'bootstrap'

const router = useRouter()
const authStore = useAuthStore()

// State
const students = ref([])
const workouts = ref([])
const loading = ref(true)
const sidebar = ref(null)
const selectedStudentId = ref('')
const selectedStudent = ref(null)
const studentWorkouts = ref([])
const selectedWorkout = ref(null)
const deleting = ref(false)

// Modal refs
const addStudentModal = ref(null)
const createWorkoutModal = ref(null)
const viewWorkoutModal = ref(null)
const editWorkoutModal = ref(null)
const deleteConfirmModal = ref(null)

// Keys for form resets
const workoutFormKey = ref(0)
const editWorkoutFormKey = ref(0)
const addStudentFormKey = ref(0)

// Toast ref
const toastRef = ref(null)

// ========== توابع احراز هویت ==========
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

// ========== توابع نمایش پیام ==========
const showSuccess = (message) => {
  toastRef.value?.showToast(message, 'success')
}

const showError = (message) => {
  toastRef.value?.showToast(message, 'danger')
}

const showInfo = (message) => {
  toastRef.value?.showToast(message, 'info')
}

// ========== توابع ریست فرم ==========
const resetWorkoutForm = () => {
  workoutFormKey.value += 1
  localStorage.removeItem('tempStudentId')
}

const resetEditWorkoutForm = () => {
  editWorkoutFormKey.value += 1
}

const resetAddStudentForm = () => {
  addStudentFormKey.value += 1
}

// ========== توابع شاگردان ==========
const loadStudents = async () => {
  try {
    const response = await studentService.getStudents()
    students.value = response.students
  } catch (error) {
    console.error('Error loading students:', error)
    showError('خطا در بارگذاری لیست شاگردان')
  } finally {
    loading.value = false
  }
}

const onStudentAdded = (newStudent) => {
  const modal = bootstrap.Modal.getInstance(addStudentModal.value)
  if (modal) modal.hide()

  resetAddStudentForm()
  loadStudents()
  showSuccess(`شاگرد "${newStudent.fullName}" با موفقیت اضافه شد`)
}

const closeModal = () => {
  const modal = bootstrap.Modal.getInstance(addStudentModal.value)
  if (modal) modal.hide()
  resetAddStudentForm()
}

// ========== انتخاب شاگرد ==========
const onStudentSelect = () => {
  console.log('Selected student ID (userId):', selectedStudentId.value)

  if (!selectedStudentId.value) {
    selectedStudent.value = null
    studentWorkouts.value = []
    return
  }

  // پیدا کردن شاگرد از لیست
  const student = students.value.find((s) => s.userId === parseInt(selectedStudentId.value))
  console.log('Found student:', student)

  if (student) {
    selectedStudent.value = student

    // ⚠️ مهم: برای دریافت برنامه‌ها باید از studentId استفاده کنیم
    // studentId = آیدی پروفایل دانشجو (تو دیتابیس)
    // userId = آیدی کاربر (برای لاگین)

    // اگه studentId وجود داشت از اون استفاده کن، وگرنه از userId استفاده کن
    const studentProfileId = student.studentId || student.userId
    console.log('Loading workouts with studentProfileId:', studentProfileId)

    loadStudentWorkouts(studentProfileId)
  } else {
    selectedStudent.value = null
    studentWorkouts.value = []
  }
}

// ========== توابع برنامه‌ها ==========
const loadAllWorkouts = async () => {
  try {
    const response = await workoutService.getWorkouts()
    if (response.success) {
      workouts.value = response.programs || []
    }
  } catch (error) {
    console.error('Error loading workouts:', error)
  }
}

const loadStudentWorkouts = async (studentProfileId) => {
  try {
    console.log('📥 Loading workouts for studentProfileId:', studentProfileId)

    // مطمئن شو که studentProfileId عدد هست
    const id = parseInt(studentProfileId)

    const response = await workoutService.getStudentWorkouts(id)
    console.log('📥 API Response:', response)

    if (response.success) {
      studentWorkouts.value = response.programs || []
      console.log(`📥 Loaded ${studentWorkouts.value.length} workouts`)
    } else {
      console.log('📥 No workouts found')
      studentWorkouts.value = []
    }
  } catch (error) {
    console.error('❌ Error loading student workouts:', error)
    showError('خطا در بارگذاری برنامه‌ها')
    studentWorkouts.value = []
  }
}

// ========== توابع کمکی ==========
const getLevelText = (level) => {
  const levels = {
    beginner: 'مبتدی',
    intermediate: 'متوسط',
    advanced: 'پیشرفته',
  }
  return levels[level] || level
}

const formatDate = (dateString) => {
  if (!dateString) return 'تاریخ نامشخص'
  try {
    const date = new Date(dateString)
    // بررسی کن که تاریخ معتبر باشه
    if (isNaN(date.getTime())) {
      return dateString
    }
    return date.toLocaleDateString('fa-IR')
  } catch (error) {
    console.error('Error formatting date:', error)
    return dateString
  }
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

// ========== توابع عملیات روی برنامه ==========
const createWorkoutForSelectedStudent = () => {
  if (!selectedStudent.value) {
    showError('لطفاً ابتدا یک شاگرد را انتخاب کنید')
    return
  }

  console.log('Creating workout for student:', selectedStudent.value)
  console.log('With studentId:', selectedStudent.value.studentId)

  // ذخیره در localStorage قبل از باز کردن مودال
  localStorage.setItem('tempStudentId', selectedStudent.value.studentId.toString())
  console.log('✅ Saved tempStudentId to localStorage:', selectedStudent.value.studentId)

  const modal = new bootstrap.Modal(createWorkoutModal.value)
  modal.show()
}

const viewWorkout = (workout) => {
  selectedWorkout.value = workout
  const modal = new bootstrap.Modal(viewWorkoutModal.value)
  modal.show()
}

const editWorkout = (workout) => {
  selectedWorkout.value = workout
  resetEditWorkoutForm()
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

    const modal = bootstrap.Modal.getInstance(deleteConfirmModal.value)
    if (modal) modal.hide()

    showSuccess(`برنامه "${selectedWorkout.value.title}" با موفقیت حذف شد`)

    // رفرش لیست برنامه‌های شاگرد
    if (selectedStudent.value) {
      loadStudentWorkouts(selectedStudent.value.studentId || selectedStudent.value.userId)
    }

    selectedWorkout.value = null
  } catch (error) {
    console.error('Error deleting workout:', error)
    showError('خطا در حذف برنامه')
  } finally {
    deleting.value = false
  }
}

const closeWorkoutModal = () => {
  const modal = bootstrap.Modal.getInstance(createWorkoutModal.value)
  if (modal) modal.hide()
  resetWorkoutForm()
}

const closeEditModal = () => {
  const modal = bootstrap.Modal.getInstance(editWorkoutModal.value)
  if (modal) modal.hide()
  selectedWorkout.value = null
}

const onWorkoutCreated = (response) => {
  console.log('✅ Workout created successfully', response)

  closeWorkoutModal()
  showSuccess('برنامه تمرینی با موفقیت ایجاد شد')

  setTimeout(() => {
    if (selectedStudent.value) {
      const profileId = selectedStudent.value.studentId
      console.log('🔄 Reloading workouts for profile ID:', profileId)
      loadStudentWorkouts(profileId)
    }
  }, 100)
}

const onWorkoutUpdated = (response) => {
  console.log('✅ Workout updated successfully', response)

  closeEditModal()
  showSuccess('برنامه تمرینی با موفقیت ویرایش شد')

  setTimeout(() => {
    if (selectedStudent.value) {
      const profileId = selectedStudent.value.studentId
      console.log('🔄 Reloading workouts for profile ID:', profileId)
      loadStudentWorkouts(profileId)
    }
  }, 100)
}

// ========== Lifecycle ==========
onMounted(() => {
  loadStudents()
  loadAllWorkouts()
})
</script>

<style scoped>
.student-info-card {
  border-right: 4px solid #0d6efd;
}

.workout-card {
  transition: all 0.2s ease;
}

.workout-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15) !important;
}

.form-select-lg {
  padding: 0.75rem 1rem;
  font-size: 1rem;
}

/* انیمیشن برای ریست فرم */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* استایل برای toast */
:global(.toast) {
  direction: rtl;
  font-family: inherit;
}

:global(.toast .toast-body) {
  display: flex;
  align-items: center;
}
</style>
