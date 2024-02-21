<template>
  <form class="space-y-3 p-4 w-full" @submit.prevent="onSubmit">
    <FormField v-slot="{ componentField }" name="email" class="mb-4">
      <FormItem>
        <FormControl>
          <Input
            type="text"
            placeholder="Type your email"
            v-bind="componentField"
          />
        </FormControl>
        <div class="text-red-500 h-6">{{ errors.email }}</div>
      </FormItem>
    </FormField>
    <Button
      :disabled="isLoading"
      type="submit"
      class="w-full bg-violet-500 hover:bg-gray-700 transition-colors duration-300"
    >
      <template v-if="isLoading">
        <Spinner />
      </template>
      <template v-else> Go ! </template>
    </Button>
  </form>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'
import { useToast } from '@/components/ui/toast/use-toast'

interface Resend {
  email: string
}

definePageMeta({
  layout: 'auth'
})

useHead({
  title: 'TinyPath - Resend confirmation email',
  meta: [
    {
      name: 'description',
      content: 'Resend confirmation email page'
    }
  ]
})

const { toast } = useToast()
const router = useRouter()
const runTimeConfig = useRuntimeConfig()

const RESEND_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/User/RegenerateConfirmEmailCommand`
const REDIRETECT_ENDPOINT = '/auth/confirmation'

const formSchemaLogin = toTypedSchema(
  z.object({
    email: z
      .string()
      .min(1, 'Email is required')
      .email('Please enter a valid email address')
  })
)

const { handleSubmit, errors } = useForm<Resend>({
  validationSchema: formSchemaLogin
})

const onSubmit = handleSubmit((values, action) => {
  resend(values)
  action.resetForm()
})

const isLoading = ref<boolean>(false)

const resend = async (formValue: Resend) => {
  isLoading.value = true

  await useAsyncData(
    'register',
    async () =>
      await $fetch(RESEND_ENDPOINT_API, {
        method: 'POST',
        body: {
          email: formValue.email
        },
        onResponseError({ request, response, options }) {
          toast({
            description: response._data.error,
            variant: 'destructive'
          })
        },
        onResponse({ request, response, options }) {
          if (response.status === 200) {
            router.push(REDIRETECT_ENDPOINT)
          }
        }
      })
  ).finally(() => {
    isLoading.value = false
  })
}
</script>
