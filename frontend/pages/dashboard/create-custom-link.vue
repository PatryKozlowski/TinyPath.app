<template>
  <DashboardWrapper>
    <template v-if="authStore.user?.isSubscribed || authStore.user?.isAdmin">
      <DashboardTitle title="Create custom link" />
      <form
        v-if="!isLoading && createdLink.length === 0"
        class="space-y-6"
        @submit.prevent="onSubmit"
      >
        <FormField v-slot="{ componentField }" name="url" class="mb-4">
          <FormItem class="flex flex-col">
            <FormLabel class="mb-4 text-md font-normal text-black">
              Destination
            </FormLabel>
            <FormControl>
              <Input
                type="text"
                placeholder="https://example.com/your-link"
                v-bind="componentField"
              />
            </FormControl>
            <div class="text-red-500 h-6">{{ errors.url }}</div>
          </FormItem>
        </FormField>
        <FormField v-slot="{ componentField }" name="customCode" class="mb-4">
          <FormItem class="flex flex-col">
            <FormLabel class="mb-4 text-md font-normal text-violet-600">
              Custom code
            </FormLabel>
            <FormControl>
              <Input
                type="text"
                placeholder="https://example.com/your-link"
                v-bind="componentField"
              />
            </FormControl>
            <div class="text-red-500 h-6">{{ errors.customCode }}</div>
          </FormItem>
        </FormField>
        <FormField v-slot="{ componentField }" name="title" class="mb-4">
          <FormItem class="flex flex-col">
            <FormLabel class="mb-4 text-sm text-black font-light">
              Title (optional)
            </FormLabel>
            <FormControl>
              <Input type="text" v-bind="componentField" />
            </FormControl>
            <div class="text-red-500 h-6">{{ errors.title }}</div>
          </FormItem>
        </FormField>
        <Separator />
        <div class="flex w-full justify-center flex-col md:flex-row">
          <Button
            type="submit"
            :disabled="isLoading"
            class="w-full bg-violet-500 hover:bg-gray-700 transition-colors duration-300"
          >
            <template v-if="isLoading">
              <Spinner />
            </template>
            <template v-else> Create ! </template>
          </Button>
        </div>
      </form>
      <Spinner v-if="isLoading" class="text-center" />
      <ShortLinkBox
        :createdLink="createdLink"
        v-else-if="!isLoading && createdLink.length > 0"
      />
    </template>
    <template v-else>
      <div class="flex justify-center items-center h-full flex-col">
        <div class="text-center">
          <h1 class="text-3xl font-bold mb-4">You are not premium user</h1>
          <p class="text-lg font-light">
            You are not authorized to create custom links
          </p>
          <p>Clilck below and buy subscribed</p>
        </div>
        <Button class="min-w-[300px] mt-6" @click="handleCheckout">
          <Icon name="lucide:gem" class="mr-2 w-4 h-4" />
          Buy !
        </Button>
      </div>
    </template>
  </DashboardWrapper>
</template>
<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

useHead({
  title: 'TinyPath - Dashboard | Create custom link',
  meta: [
    {
      name: 'description',
      content: 'Dashboard page'
    }
  ]
})

const runTimeConfig = useRuntimeConfig()
const createdLink = ref('')
const isLoading = ref(false)
const authStore = useAuthStore()
const maxCustomCodeLength = runTimeConfig.public
  .MAX_LENGTH_CUSTOM_CODE as unknown as number
const { $api } = useNuxtApp()
const useRepository = repository($api)
const formSchema = toTypedSchema(
  z
    .object({
      url: z
        .string()
        .min(1, 'Link is required')
        .url('Link must be a valid URL'),
      customCode: z
        .string()
        .min(1, 'Custom code is required')
        .max(
          maxCustomCodeLength,
          `Custom code must be less than ${maxCustomCodeLength} characters`
        ),
      title: z.string().optional()
    })
    .refine((data) => !data.url.includes('localhost'), {
      message: 'Link cannot be localhost',
      path: ['link']
    })
    .refine((data) => !data.url.includes('tinypath'), {
      message: 'Link cannot be tinypath',
      path: ['link']
    })
    .refine((data) => !data.url.includes('tiny-path'), {
      message: 'Link cannot be tinypath',
      path: ['link']
    })
)

const { handleSubmit, errors } = useForm<CreateCustomLinkForm>({
  validationSchema: formSchema
})

const create = async (values: CreateCustomLinkForm) => {
  isLoading.value = true
  const { title, ...rest } = values

  const cleanedValues = title === undefined ? rest : values

  const data = await useRepository
    .createCustomShortLink(cleanedValues)
    .finally(() => {
      isLoading.value = false
    })

  if (data.link) {
    createdLink.value = data.link
  } else {
    createdLink.value = ''
  }
}

const onSubmit = handleSubmit((values, action) => {
  create(values)
  action.resetForm()
})

const handleCheckout = async () => {
  isLoading.value = true
  const checkoutPrice = {
    priceCode: 'price_1Ocx5WAx2Zy2VacOjBvFRwak'
  }

  const data = await useRepository
    .createCheckoutSession(checkoutPrice)
    .finally(() => {
      isLoading.value = false
    })

  if (data.link) {
    window.location.href = data.link
  }
}
</script>
