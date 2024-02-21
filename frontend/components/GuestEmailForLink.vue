<template>
  <div class="shadow-md p-4 rounded-md space-y-4 text-sm md:text-base">
    <div class="text-black p-1 space-y-1">
      <p>You can type your email to save magic link to your email</p>
      <p>Magic link show you how many clicks you have on your link</p>
    </div>
    <form class="flex flex-col" @submit.prevent="onSubmitEmail">
      <FormField v-slot="{ componentField }" name="email" class="mb-4">
        <FormItem class="flex flex-col">
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
      <Button class="w-full">
        <template v-if="isLoading">
          <Spinner />
        </template>
        <template v-else> Save link </template>
      </Button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'

const { $api } = useNuxtApp()
const useRepository = repository($api)

const formSchema = toTypedSchema(
  z.object({
    email: z.string().email()
  })
)

const { handleSubmit, errors } = useForm<GuestEmailForm>({
  validationSchema: formSchema
})

const isLoading = ref(false)
const guestStore = useGuestStore()

const sendLinksToEmail = async (values: GuestEmailForm) => {
  isLoading.value = true

  const data = await useRepository
    .sendEmailForTrackingLink(values)
    .finally(() => {
      isLoading.value = false
    })

  if (data) {
    guestStore.isSendEmailForLink = true
  }
}

const onSubmitEmail = handleSubmit((values, action) => {
  sendLinksToEmail(values)
  action.resetForm()
})
</script>
