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
import { useToast } from '@/components/ui/toast/use-toast'

interface GuestEmial {
  email: string
}

const { toast } = useToast()
const runTimeConfig = useRuntimeConfig()
const GUEST_EMAIL_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/Link/SendLinkViewsEmailForGuestUserCommand`

const formSchema = toTypedSchema(
  z.object({
    email: z.string().email()
  })
)

const { handleSubmit, errors } = useForm<GuestEmial>({
  validationSchema: formSchema
})

const isLoading = ref(false)
const guestStore = useGuestStore()

const sendLinksToEmail = async (values: GuestEmial) => {
  isLoading.value = true

  await useAsyncData(
    'guestEmailForLink',
    async () =>
      await $fetch(GUEST_EMAIL_ENDPOINT_API, {
        method: 'POST',
        body: {
          email: values.email
        },
        onResponseError({ request, response, options }) {
          toast({
            description: response._data.error,
            variant: 'destructive'
          })
        },
        onResponse({ request, response, options }) {
          if (response._data) {
            guestStore.isSendEmailForLink = true
          }
        }
      })
  ).finally(() => {
    isLoading.value = false
  })
}

const onSubmitEmail = handleSubmit((values, action) => {
  sendLinksToEmail(values)
  action.resetForm()
})
</script>
