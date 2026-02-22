<template>
  <div class="min-vh-100 bg-light">
    <!-- نوار بالایی -->
    <nav class="navbar navbar-light bg-white shadow-sm sticky-top">
      <div class="container-fluid px-3">
        <button class="btn btn-link text-dark p-0 border-0" @click="goBack">
          <i class="bi bi-arrow-right fs-5"></i>
        </button>
        <span class="fw-bold text-primary">برنامه تمرینی</span>
        <div style="width: 40px"></div>
        <!-- spacer -->
      </div>
    </nav>

    <!-- محتوای اصلی -->
    <main class="container-fluid px-3 py-4">
      <WorkoutViewer :student-id="studentId" />
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import WorkoutViewer from '../components/student/WorkoutViewer.vue'

const router = useRouter()
const authStore = useAuthStore()
const studentId = ref(authStore.user?.id)

const goBack = () => {
  router.push('/student-dashboard')
}
</script>
