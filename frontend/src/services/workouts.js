import api from './api'

export const workoutService = {
  // دریافت برنامه‌ها (برای مربی با studentId اختیاری)
  async getWorkouts(studentId = null) {
    const params = studentId ? { studentId } : {}
    console.log('📥 Fetching workouts with params:', params)
    const response = await api.get('/workouts', { params })
    return response.data
  },

  // دریافت برنامه‌های یک شاگرد خاص (با studentId پروفایل)
  async getStudentWorkouts(studentProfileId) {
    console.log('📥 Fetching student workouts for profile ID:', studentProfileId)
    const response = await api.get(`/workouts?studentId=${studentProfileId}`)
    return response.data
  },

  // دریافت یک برنامه
  async getWorkoutById(id) {
    console.log('📥 Fetching workout by ID:', id)
    const response = await api.get(`/workouts/${id}`)
    return response.data
  },

  // ایجاد برنامه جدید
  async createWorkout(workoutData) {
    console.log('📤 Creating workout with data:', workoutData)
    const response = await api.post('/workouts', workoutData)
    return response.data
  },

  // ویرایش برنامه
  async updateWorkout(id, workoutData) {
    console.log('📤 Updating workout ID:', id)
    console.log('📤 With data:', workoutData)
    const response = await api.put(`/workouts/${id}`, workoutData)
    return response.data
  },

  // حذف برنامه
  async deleteWorkout(id) {
    console.log('🗑️ Deleting workout ID:', id)
    const response = await api.delete(`/workouts/${id}`)
    return response.data
  },
}
