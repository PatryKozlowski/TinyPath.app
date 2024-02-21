export const useAuthStore = defineStore('auth', {
  state: () => ({
    authUser: {
      email: '',
      token: '',
      isSubscribed: false,
      isAdmin: false
    } as AuthUser | null,
    isLoading: false as boolean,
    isAuthenticated: false as boolean
  }),
  persist: true,
  getters: {
    user: (state) => state.authUser
  },
  actions: {
    async login(formValue: LoginForm) {
      this.isLoading = true
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      const data = await useRepository.login(formValue).finally(() => {
        this.isLoading = false
      })

      if (data) {
        this.authUser = {
          email: '',
          token: data.token,
          isSubscribed: false,
          isAdmin: false
        }
        this.isAuthenticated = true
        navigateTo('/dashboard')
      }
    },
    async loadedUserData() {
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      const data = await useRepository.loadedUserData()

      if (data) {
        this.authUser = {
          email: data.email,
          token: this.authUser?.token!,
          isSubscribed: data.isSubscribed,
          isAdmin: data.isAdmin
        }
        this.isAuthenticated = true
      }
    },
    async logout() {
      const { $api } = useNuxtApp()
      const useRepository = repository($api)
      await useRepository.logout().finally(() => {
        this.$reset()
        navigateTo('/')
      })
    }
  }
})
