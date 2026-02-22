import { defineStore } from 'pinia'
import { studentService } from '../services/students'

export const useStudentsStore = defineStore('students', {
  state: () => ({
    students: [],
    loading: false,
    error: null,
  }),

  actions: {
    async fetchStudents() {
      this.loading = true
      try {
        const response = await studentService.getStudents()
        this.students = response.students
      } catch (error) {
        this.error = error.response?.data?.message || 'خطا در دریافت لیست شاگردان'
      } finally {
        this.loading = false
      }
    },

    async addStudent(studentData) {
      try {
        const response = await studentService.createStudent(studentData)
        if (response.success) {
          await this.fetchStudents()
        }
        return response
      } catch (error) {
        this.error = error.response?.data?.message || 'خطا در ایجاد شاگرد'
        throw error
      }
    },
  },
})
