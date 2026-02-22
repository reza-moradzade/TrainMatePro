import api from './api'

export const exerciseService = {
  // دریافت لیست حرکات با فیلتر
  async getExercises(params = {}) {
    const response = await api.get('/exercises', { params })
    return response.data
  },

  // جستجوی حرکات
  async searchExercises(query) {
    const response = await api.get(`/exercises/search?q=${query}`)
    return response.data
  },

  // دریافت یک حرکت با ID
  async getExerciseById(id) {
    const response = await api.get(`/exercises/${id}`)
    return response.data
  },

  // دریافت لیست اعضای بدن
  async getBodyParts() {
    const response = await api.get('/exercises/body-parts')
    return response.data
  },

  // دریافت لیست تجهیزات
  async getEquipments() {
    const response = await api.get('/exercises/equipments')
    return response.data
  },

  // دریافت لیست عضلات
  async getMuscles() {
    const response = await api.get('/exercises/muscles')
    return response.data
  },
}
