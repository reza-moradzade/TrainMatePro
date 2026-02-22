<template>
  <div class="min-vh-100 bg-light">
    <!-- نوار بالایی -->
    <nav class="navbar navbar-light bg-white shadow-sm sticky-top">
      <div class="container-fluid px-3">
        <button class="btn btn-link text-dark p-0 border-0" @click="goBack">
          <i class="bi bi-arrow-right fs-5"></i>
        </button>
        <span class="fw-bold text-primary">ویرایش برنامه تمرینی</span>
        <div style="width: 40px"></div>
        <!-- spacer -->
      </div>
    </nav>

    <!-- محتوای اصلی -->
    <main class="container-fluid px-3 py-4">
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="text-secondary mt-3">در حال بارگذاری برنامه...</p>
      </div>

      <div v-else-if="error" class="text-center py-5">
        <i class="bi bi-exclamation-triangle display-1 text-danger opacity-50"></i>
        <p class="text-danger mt-3">{{ error }}</p>
        <button class="btn btn-primary mt-3" @click="goBack">
          <i class="bi bi-arrow-right me-2"></i>
          بازگشت
        </button>
      </div>

      <EditWorkout v-else :workout-id="workoutId" @success="onWorkoutUpdated" @close="goBack" />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import EditWorkout from '../components/coach/EditWorkout.vue'
import { workoutService } from '../services/workouts'

const router = useRouter()
const route = useRoute()
const workoutId = ref(parseInt(route.params.id))
const loading = ref(true)
const error = ref('')

const goBack = () => {
  router.push('/dashboard')
}

const onWorkoutUpdated = (response) => {
  // می‌تونی یه نوتیفیکیشن موفقیت نشون بدی
  console.log('Workout updated successfully:', response)
  router.push('/dashboard')
}

// بررسی وجود برنامه قبل از بارگذاری
onMounted(async () => {
  try {
    const response = await workoutService.getWorkoutById(workoutId.value)
    if (!response.success) {
      error.value = 'برنامه مورد نظر یافت نشد'
    }
  } catch (err) {
    error.value = 'خطا در دریافت اطلاعات برنامه'
    console.error('Error loading workout:', err)
  } finally {
    loading.value = false
  }
})
</script>
