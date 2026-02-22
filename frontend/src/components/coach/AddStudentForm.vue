<template>
  <form @submit.prevent="handleSubmit">
    <!-- اطلاعات پایه -->
    <div class="mb-4">
      <h6 class="fw-bold mb-3">اطلاعات پایه</h6>

      <div class="mb-3">
        <label class="form-label small fw-bold">نام کامل</label>
        <input
          type="text"
          class="form-control"
          v-model="form.fullName"
          placeholder="مثال: علی محمدی"
          required
        />
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">ایمیل</label>
        <input
          type="email"
          class="form-control"
          v-model="form.email"
          placeholder="example@gmail.com"
          required
          dir="ltr"
        />
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">رمز عبور</label>
        <input
          type="password"
          class="form-control"
          v-model="form.password"
          placeholder="حداقل ۶ کاراکتر"
          required
          minlength="6"
          dir="ltr"
        />
      </div>
    </div>

    <!-- اطلاعات فیزیکی -->
    <div class="mb-4">
      <h6 class="fw-bold mb-3">اطلاعات فیزیکی</h6>

      <div class="row">
        <div class="col-md-6 mb-3">
          <label class="form-label small fw-bold">سن</label>
          <input type="number" class="form-control" v-model="form.age" placeholder="مثال: ۲۵" />
        </div>

        <div class="col-md-6 mb-3">
          <label class="form-label small fw-bold">جنسیت</label>
          <select class="form-select" v-model="form.gender">
            <option value="">انتخاب کنید</option>
            <option value="male">مرد</option>
            <option value="female">زن</option>
          </select>
        </div>
      </div>

      <div class="row">
        <div class="col-md-6 mb-3">
          <label class="form-label small fw-bold">قد (سانتیمتر)</label>
          <input type="number" class="form-control" v-model="form.height" placeholder="مثال: ۱۷۵" />
        </div>

        <div class="col-md-6 mb-3">
          <label class="form-label small fw-bold">وزن (کیلوگرم)</label>
          <input type="number" class="form-control" v-model="form.weight" placeholder="مثال: ۷۰" />
        </div>
      </div>
    </div>

    <!-- سطح و اهداف -->
    <div class="mb-4">
      <h6 class="fw-bold mb-3">سطح و اهداف</h6>

      <div class="mb-3">
        <label class="form-label small fw-bold">سطح آمادگی</label>
        <select class="form-select" v-model="form.fitnessLevel">
          <option value="beginner">مبتدی</option>
          <option value="intermediate">متوسط</option>
          <option value="advanced">پیشرفته</option>
        </select>
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">اهداف تمرینی</label>
        <textarea
          class="form-control"
          rows="2"
          v-model="form.goals"
          placeholder="مثال: کاهش وزن و افزایش قدرت"
        ></textarea>
      </div>

      <div class="mb-3">
        <label class="form-label small fw-bold">یادداشت‌ها</label>
        <textarea
          class="form-control"
          rows="2"
          v-model="form.notes"
          placeholder="هر نکته دیگری..."
        ></textarea>
      </div>
    </div>

    <!-- پیام خطا -->
    <div v-if="error" class="alert alert-danger py-2">
      <i class="bi bi-exclamation-triangle-fill me-2"></i>
      {{ error }}
    </div>

    <!-- دکمه‌ها -->
    <div class="d-flex gap-2">
      <button type="button" class="btn btn-outline-secondary flex-grow-1" data-bs-dismiss="modal">
        انصراف
      </button>
      <button type="submit" class="btn btn-primary flex-grow-1" :disabled="loading">
        <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
        {{ loading ? 'در حال ثبت...' : 'ثبت شاگرد' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { studentService } from '../../services/students'

const emit = defineEmits(['success', 'close'])

const loading = ref(false)
const error = ref('')

const form = reactive({
  fullName: '',
  email: '',
  password: '',
  age: null,
  gender: '',
  height: null,
  weight: null,
  fitnessLevel: 'beginner',
  goals: '',
  notes: '',
})

const handleSubmit = async () => {
  loading.value = true
  error.value = ''

  try {
    const response = await studentService.createStudent(form)

    if (response.success) {
      emit('success', response.student)
      // ریست فرم
      Object.assign(form, {
        fullName: '',
        email: '',
        password: '',
        age: null,
        gender: '',
        height: null,
        weight: null,
        fitnessLevel: 'beginner',
        goals: '',
        notes: '',
      })
    } else {
      error.value = response.message || 'خطا در ثبت شاگرد'
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'خطا در ارتباط با سرور'
  } finally {
    loading.value = false
  }
}
</script>
