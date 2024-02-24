import { useToast } from '@/components/ui/toast/use-toast'

export default defineNuxtPlugin({
  setup() {
    const { toast } = useToast()
    const router = useRouter()
    const authUser = useAuthStore()
    const api = $fetch.create({
      baseURL: useRuntimeConfig().public.BASE_URL,
      onRequest({ options }) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${authUser.authUser?.token}`,
          'Content-Type': 'application/json',
          Accept: 'application/json',
          credentials: 'include'
        }
      },
      onResponseError({ request, response, options }) {
        if (response._data.error === 'UserAlreadyLoggedIn') {
          router.push('/dashboard')
        } else {
          toast({
            description: response._data.error,
            variant: 'destructive'
          })
        }
      }
    })

    return {
      provide: {
        api
      }
    }
  }
})
