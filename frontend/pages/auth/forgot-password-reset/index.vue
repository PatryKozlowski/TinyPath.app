<template>
  <div class="flex flex-col w-full space-y-6">
    <h1 class="text-center text-2xl text-white">Reset password</h1>
    <form
      class="flex flex-col space-y-6"
      @submit.prevent="handleResetPassword"
      v-if="!message"
    >
      <FormField v-slot="{ componentField }" name="password" class="mb-4">
        <FormItem>
          <FormControl>
            <div class="flex rounded-md border border-slate-200">
              <Input
                :type="hidden.password ? 'password' : 'text'"
                placeholder="New password"
                v-bind="componentField"
                class="border-none rounded-none focus-visible:outline-none focus-visible:ring-0 focus-visible:ring-transparent focus-visible:ring-offset-0"
              />
              <Button
                class="flex h-10 px-3 py-2 text-sm rounded-none bg-slate-200"
                type="button"
                variant="secondary"
                @click="toggleHidden('password')"
              >
                <Icon
                  :name="hidden.password ? 'lucide:eye' : 'lucide:eye-off'"
                  class="w-6 h-6"
                />
              </Button>
            </div>
          </FormControl>
          <div class="text-red-500 h-6 text-sm">
            {{ errors.password }}
          </div>
        </FormItem>
      </FormField>
      <FormField v-slot="{ componentField }" name="repeatPassword" class="mb-4">
        <FormItem>
          <FormControl>
            <div class="flex rounded-md border border-slate-200">
              <Input
                type="password"
                placeholder="Repeat password"
                v-bind="componentField"
                class="border-none rounded-none focus-visible:outline-none focus-visible:ring-0 focus-visible:ring-transparent focus-visible:ring-offset-0"
              />
              <Button
                class="flex h-10 px-3 py-2 text-sm rounded-none bg-slate-200"
                type="button"
                variant="secondary"
                @click="toggleHidden('repeatPassword')"
              >
                <Icon
                  :name="
                    hidden.repeatPassword ? 'lucide:eye' : 'lucide:eye-off'
                  "
                  class="w-6 h-6"
                />
              </Button>
            </div>
          </FormControl>
          <div class="text-red-500 h-6 text-sm">
            {{ errors.repeatPassword }}
          </div>
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
        <template v-else> Reset password </template>
      </Button>
    </form>
    <div v-else>
      <p class="text-center text-white">{{ message }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'

definePageMeta({
  layout: 'auth',
  middleware: ['check-token']
})

useHead({
  title: 'TinyPath - Reset password',
  meta: [
    {
      name: 'description',
      content: 'Reset password page'
    }
  ]
})
const route = useRoute()
const token = route.query.Token as string
const message = ref('')
const { $api } = useNuxtApp()
const useRepository = repository($api)

const isLoading = ref(false)

const formSchema = toTypedSchema(
  z
    .object({
      password: z
        .string()
        .min(1, 'Password is required')
        .min(8, 'Password must be at least 8 characters long')
        .regex(/[A-Z]/, 'Password must contain an uppercase letter')
        .regex(/[0-9]/, 'Password must contain a number')
        .regex(
          /[!@#$%^&*()_+}{":;'?/>.<,]/,
          'Password must contain a special character'
        ),
      repeatPassword: z
        .string()
        .min(1, 'Repeat password is required')
        .min(8, 'Password must be at least 8 characters long')
        .regex(/[A-Z]/, 'Password must contain an uppercase letter')
        .regex(/[0-9]/, 'Password must contain a number')
        .regex(
          /[!@#$%^&*()_+}{":;'?/>.<,]/,
          'Password must contain a special character'
        )
    })
    .refine((data) => data.password === data.repeatPassword, {
      message: 'Passwords do not match',
      path: ['repeatPassword']
    })
)

const hidden = ref<Hidden>({
  password: true,
  repeatPassword: true
})

const toggleHidden = (field: keyof Hidden) => {
  hidden.value[field] = !hidden.value[field]
}

const { handleSubmit, errors } = useForm<ResetPasswordForm>({
  validationSchema: formSchema
})

const handleResetPassword = handleSubmit(async (values) => {
  const resetPasswordForm = {
    password: values.password,
    repeatPassword: values.repeatPassword,
    token
  }
  console.log(resetPasswordForm)

  isLoading.value = true
  await useRepository
    .resetPassword(resetPasswordForm)
    .then(() => {
      message.value = 'Password has been reset successfully'
    })
    .finally(() => {
      isLoading.value = false
    })
})
</script>
