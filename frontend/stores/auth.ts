import { useToast } from '@/components/ui/toast/use-toast'

interface AuthUser {
  email: string
  token?: string
  isSubscribed?: boolean
  isAdmin?: boolean
}

interface LoginResponse {
  token: string
}

interface LoginForm {
  email: string
  password: string
}

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
    async login(data: LoginForm) {
      this.isLoading = true
      const runTimeConfig = useRuntimeConfig()
      const { toast } = useToast()
      const router = useRouter()
      const LOGIN_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/User/LoginCommand`
      const REDIRETECT_ENDPOINT = '/dashboard'
      await $fetch<LoginResponse>(LOGIN_ENDPOINT_API, {
        method: 'POST',
        credentials: 'include',
        body: {
          email: data.email,
          password: data.password
        },
        onResponseError({ response }) {
          toast({
            description: response._data.error,
            variant: 'destructive'
          })
        }
      })
        .then((response) => {
          this.$patch((state) => {
            state.authUser = {
              email: 'data.email',
              token: response.token
            }
            state.isAuthenticated = true
          })
          router.push(REDIRETECT_ENDPOINT)
        })
        .finally(() => {
          this.isLoading = false
        })
    },
    async logout() {
      const runTimeConfig = useRuntimeConfig()
      const router = useRouter()
      const LOGOUT_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/User/Logout`
      await $fetch(LOGOUT_ENDPOINT_API, {
        method: 'POST',
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${this.authUser?.token}`
        }
      }).finally(() => {
        this.$patch((state) => {
          state.authUser = null
          state.isAuthenticated = false
        })
        router.push('/')
      })
    },
    async loadLoggedInUser() {
      const runTimeConfig = useRuntimeConfig()
      const LOAD_LOGGED_IN_USER_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/User/GetAuthenticatedUserCommand`
      await $fetch<AuthUser>(LOAD_LOGGED_IN_USER_ENDPOINT_API, {
        method: 'GET',
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${this.authUser?.token}`
        }
      }).then((response) => {
        this.authUser = {
          email: response.email,
          isSubscribed: response.isSubscribed,
          isAdmin: response.isAdmin
        }
      })
    }
  }
})
