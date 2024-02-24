<template>
  <div class="flex flex-col w-full space-y-6">
    <h1 class="text-center text-2xl text-white">Reset password</h1>
    <form
      class="flex flex-col space-y-6"
      @submit.prevent="handleEmailToResetPassword"
    >
      <FormField v-slot="{ componentField }" name="email" class="mb-4">
        <FormItem>
          <FormControl>
            <Input
              type="text"
              placeholder="Type your email"
              v-bind="componentField"
            />
          </FormControl>
          <div class="text-red-500 h-6 text-sm">
            {{ errors.email }}
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
        <template v-else> Go ! </template>
      </Button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'

definePageMeta({
  layout: 'auth'
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

const isLoading = ref(false)

const formSchema = toTypedSchema(
  z.object({
    email: z
      .string()
      .min(1, 'Email is required')
      .email('Please enter a valid email address')
  })
)

const { handleSubmit, errors } = useForm<{ email: string }>({
  validationSchema: formSchema
})

const handleEmailToResetPassword = handleSubmit(async (values) => {
  console.log(values)
})
</script>
