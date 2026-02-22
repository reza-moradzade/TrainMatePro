import { defineStore } from 'pinia'
import { authService } from '../services/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: authService.getStoredUser(),
    token: authService.getToken(),
    loading: false,
    error: null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isCoach: (state) => state.user?.role === 'coach',
    isStudent: (state) => state.user?.role === 'student',
    userName: (state) => state.user?.fullName || '',
  },

  actions: {
    async login(email, password) {
      this.loading = true
      this.error = null
      try {
        const response = await authService.login(email, password)
        if (response.success) {
          this.user = response.user
          this.token = response.token
        } else {
          this.error = 'ایمیل یا رمز عبور اشتباه است'
        }
        return response
      } catch (error) {
        this.error = error.response?.data?.message || 'خطا در ورود'
        throw error
      } finally {
        this.loading = false
      }
    },

    async logout() {
      await authService.logout()
      this.user = null
      this.token = null
    },

    async getCurrentUser() {
      try {
        const response = await authService.getCurrentUser()
        this.user = response.user
        return response
      } catch (error) {
        console.error('Error getting current user:', error)
      }
    },
  },

  persist: true,
})
