import api from './api'

export const studentService = {
  async getStudents() {
    const response = await api.get('/students')
    return response.data
  },

  async createStudent(studentData) {
    const response = await api.post('/students', studentData)
    return response.data
  },
}
