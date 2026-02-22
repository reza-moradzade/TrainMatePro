<template>
  <div
    class="toast-container position-fixed top-0 start-50 translate-middle-x p-3"
    style="z-index: 9999"
  >
    <div
      v-for="(toast, index) in toasts"
      :key="index"
      class="toast show align-items-center border-0"
      :class="`bg-${toast.type} text-white`"
      role="alert"
    >
      <div class="d-flex">
        <div class="toast-body">
          <i :class="getIcon(toast.type)" class="me-2"></i>
          {{ toast.message }}
        </div>
        <button
          type="button"
          class="btn-close btn-close-white me-2 m-auto"
          @click="removeToast(index)"
        ></button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const toasts = ref([])

const showToast = (message, type = 'success') => {
  const id = Date.now()
  toasts.value.push({ id, message, type })

  // Auto remove after 3 seconds
  setTimeout(() => {
    removeToast(toasts.value.findIndex((t) => t.id === id))
  }, 3000)
}

const removeToast = (index) => {
  if (index !== -1) {
    toasts.value.splice(index, 1)
  }
}

const getIcon = (type) => {
  const icons = {
    success: 'bi bi-check-circle-fill',
    danger: 'bi bi-exclamation-triangle-fill',
    warning: 'bi bi-exclamation-circle-fill',
    info: 'bi bi-info-circle-fill',
  }
  return icons[type] || icons.info
}

defineExpose({ showToast })
</script>

<style scoped>
.toast {
  min-width: 300px;
  opacity: 1 !important;
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

.toast-container {
  pointer-events: none;
}

.toast {
  pointer-events: auto;
}
</style>
