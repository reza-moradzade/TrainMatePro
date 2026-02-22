<template>
  <div class="min-vh-100 d-flex align-items-center justify-content-center bg-light py-5">
    <div class="card shadow-lg" style="max-width: 400px; width: 100%">
      <div class="card-body p-5">
        <div class="text-center mb-4">
          <i class="bi bi-bar-chart-steps text-primary" style="font-size: 3rem"></i>
          <h2 class="h3 fw-bold mt-3">ورود به TrainMatePro</h2>
          <p class="text-muted">برای مربیان و ورزشکاران</p>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="mb-3">
            <label for="email" class="form-label">ایمیل</label>
            <input
              type="email"
              class="form-control"
              id="email"
              v-model="email"
              placeholder="example@gmail.com"
              required
            />
          </div>

          <div class="mb-3">
            <label for="password" class="form-label">رمز عبور</label>
            <input
              type="password"
              class="form-control"
              id="password"
              v-model="password"
              placeholder="••••••••"
              required
            />
          </div>

          <div v-if="error" class="alert alert-danger py-2">
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            {{ error }}
          </div>

          <button type="submit" class="btn btn-primary w-100 py-2" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            {{ loading ? 'در حال ورود...' : 'ورود' }}
          </button>
        </form>

        <hr class="my-4" />

        <div class="text-center">
          <p class="text-muted small mb-2">حساب‌های تست:</p>
          <button @click="fillCoachAccount" class="btn btn-sm btn-outline-success">
            <i class="bi bi-person-badge me-1"></i>
            مربی: qc.reza@gmail.com
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

const handleSubmit = async () => {
  loading.value = true
  error.value = ''

  try {
    const response = await authStore.login(email.value, password.value)

    if (response.success) {
      if (authStore.isCoach) {
        router.push('/dashboard')
      } else {
        router.push('/student-dashboard')
      }
    } else {
      error.value = response.message || 'ایمیل یا رمز عبور اشتباه است'
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'خطا در برقراری ارتباط با سرور'
  } finally {
    loading.value = false
  }
}

const fillCoachAccount = () => {
  email.value = 'qc.reza@gmail.com'
  password.value = 'reza123456'
}
</script>
