<template>
  <div class="workout-programs">
    <div v-for="program in programs" :key="program.id" class="card mb-3">
      <!-- ... بقیه محتوا ... -->

      <div class="card-footer bg-white border-0 d-flex gap-2">
        <button class="btn btn-sm btn-outline-primary flex-grow-1">
          <i class="bi bi-eye"></i> مشاهده
        </button>
        <button class="btn btn-sm btn-outline-warning flex-grow-1" @click="editWorkout(program)">
          <i class="bi bi-pencil"></i> ویرایش
        </button>
        <button class="btn btn-sm btn-outline-danger flex-grow-1">
          <i class="bi bi-trash"></i> حذف
        </button>
      </div>
    </div>

    <!-- مودال ویرایش -->
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
  </div>
</template>

<script setup>
import { ref } from 'vue'
import * as bootstrap from 'bootstrap'
import EditWorkout from './EditWorkout.vue'

const props = defineProps({
  programs: {
    type: Array,
    required: true,
  },
})

const emit = defineEmits(['refresh'])

const selectedWorkout = ref(null)
const editWorkoutModal = ref(null)

const editWorkout = (program) => {
  selectedWorkout.value = program
  const modal = new bootstrap.Modal(editWorkoutModal.value)
  modal.show()
}

const closeEditModal = () => {
  const modal = bootstrap.Modal.getInstance(editWorkoutModal.value)
  if (modal) {
    modal.hide()
  }
}

const onWorkoutUpdated = (response) => {
  closeEditModal()
  emit('refresh')
  // می‌تونی یه نوتیفیکیشن موفقیت نشون بدی
}
</script>
