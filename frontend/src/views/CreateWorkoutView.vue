<template>
  <div class="min-vh-100 bg-light">
    <!-- نوار بالایی -->
    <nav class="navbar navbar-light bg-white shadow-sm sticky-top">
      <div class="container-fluid px-3">
        <button class="btn btn-link text-dark p-0 border-0" @click="goBack">
          <i class="bi bi-arrow-right fs-5"></i>
        </button>
        <span class="fw-bold text-primary">ایجاد برنامه جدید</span>
        <div style="width: 40px"></div>
        <!-- spacer -->
      </div>
    </nav>

    <!-- محتوای اصلی -->
    <main class="container-fluid px-3 py-4">
      <CreateWorkout :student-id="studentId" @success="onWorkoutCreated" @close="goBack" />
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import CreateWorkout from '../components/coach/CreateWorkout.vue'

const router = useRouter()
const route = useRoute()
const studentId = ref(route.params.studentId ? parseInt(route.params.studentId) : null)

const goBack = () => {
  router.push('/dashboard')
}

const onWorkoutCreated = (response) => {
  // بعد از ایجاد موفق، به لیست برنامه‌ها برو
  router.push('/coach/workouts')
}
</script>
