<template>
  <div
    class="w-full h-screen flex flex-col justify-center items-center bg-gray-700"
  >
    <Logo />
    <div class="md:w-[400px] w-[350px] bg-[#111827] p-4 rounded-lg">
      <div
        class="text-white flex justify-center items-center space-x-4 text-xl mb-4"
      >
        <Icon name="lucide:user" class="w-6 h-6" />
        <h2>Login</h2>
      </div>
      <form class="space-y-3 p-4" @submit.prevent="onSubmit">
        <FormField v-slot="{ componentField }" name="email" class="mb-4">
          <FormItem class="flex flex-col">
            <FormLabel class="text-sm mb-2 text-white"> Email </FormLabel>
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
        <FormField v-slot="{ componentField }" name="password" class="mb-4">
          <FormItem>
            <FormLabel class="text-sm mb-2 text-white">Password</FormLabel>
            <FormControl>
              <div class="flex rounded-md border border-slate-200">
                <Input
                  :type="hiddenPassword ? 'password' : 'text'"
                  placeholder="Type your password"
                  v-bind="componentField"
                  class="border-none rounded-none focus-visible:outline-none focus-visible:ring-0 focus-visible:ring-transparent focus-visible:ring-offset-0"
                />
                <Button
                  class="flex h-10 px-3 py-2 text-sm rounded-none bg-slate-200"
                  type="button"
                  variant="secondary"
                  @click="toggleHidden"
                >
                  <Icon
                    :name="hiddenPassword ? 'lucide:eye' : 'lucide:eye-off'"
                    class="w-6 h-6"
                  />
                </Button>
              </div>
            </FormControl>
            <div class="text-red-500 h-6">
              {{ errors.password }}
            </div>
          </FormItem>
        </FormField>
        <Button
          :disbaled="isLoading"
          type="submit"
          class="w-full bg-violet-500 hover:bg-gray-700 transition-colors duration-300"
        >
          {{ isLoading ? 'Loading' : 'Go !' }}
        </Button>
      </form>
      <div class="flex w-full justify-center mt-4 flex-col md:flex-row">
        <Button variant="link" size="sm" class="text-white text-sm">
          <NuxtLink to="/auth/register" class="text-white text-sm">
            Do you need account?
          </NuxtLink>
        </Button>
        <Button variant="link" size="sm">
          <NuxtLink to="/auth/forgotpassword" class="text-white text-sm">
            I dont remeber my password
          </NuxtLink>
        </Button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'
import { useToast } from '@/components/ui/toast/use-toast'

interface Login {
  email: string
  password: string
}

const { toast } = useToast()

const formSchemaLogin = toTypedSchema(
  z.object({
    email: z
      .string()
      .min(1, 'Email is required')
      .email('Please enter a valid email address'),
    password: z
      .string()
      .min(1, 'Password is required')
      .min(8, 'Password must be at least 8 characters long')
      .regex(/[A-Z]/, 'Password must contain an uppercase letter')
      .regex(/[0-9]/, 'Password must contain a number')
      .regex(
        /[!@#$%^&*()_+}{":;'?/>.<,]/,
        'Password must contain a special character'
      )
  })
)

const { handleSubmit, errors } = useForm<Login>({
  validationSchema: formSchemaLogin
})

const hiddenPassword = ref<boolean>(true)

const toggleHidden = () => {
  hiddenPassword.value = !hiddenPassword.value
}

const onSubmit = handleSubmit((values, action) => {
  login(values)
  action.resetForm()
})

const isLoading = ref<boolean>(false)

const login = (formValue: Login) => {
  isLoading.value = true

  useWebApiFetch('/auth/login', {
    method: 'POST',
    body: {
      email: formValue.email,
      password: formValue.password
    },
    onResponseError({ request, response, options }) {
      toast({
        title: 'Information',
        description: response._data.error
      })
    }
  }).finally(() => {
    isLoading.value = false
  })
}
</script>
