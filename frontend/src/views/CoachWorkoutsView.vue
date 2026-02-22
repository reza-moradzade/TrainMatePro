<template>
  <div class="min-vh-100 bg-light">
    <nav class="navbar navbar-light bg-white shadow-sm sticky-top">
      <div class="container-fluid px-3">
        <button class="btn btn-link text-dark p-0 border-0" @click="goBack">
          <i class="bi bi-arrow-right fs-5"></i>
        </button>
        <span class="fw-bold text-primary">برنامه‌های تمرینی</span>
        <div style="width: 40px"></div>
      </div>
    </nav>

    <main class="container-fluid px-3 py-4">
      <h5 class="mb-3">لیست برنامه‌ها</h5>
      <p class="text-secondary">در حال توسعه...</p>
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()
const goBack = () => router.push('/dashboard')
</script>
