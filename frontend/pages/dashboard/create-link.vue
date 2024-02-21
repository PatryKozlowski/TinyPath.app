<template>
  <DashboardWrapper>
    <DashboardTitle title="Create new link" />
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
  title: 'TinyPath - Dashboard | Create',
  meta: [
    {
      name: 'description',
      content: 'Dashboard page'
    }
  ]
})

const createdLink = ref('')
const isLoading = ref(false)
const { $api } = useNuxtApp()
const useRepository = repository($api)

const formSchema = toTypedSchema(
  z
    .object({
      url: z
        .string()
        .min(1, 'Link is required')
        .url('Link must be a valid URL'),
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

const { handleSubmit, errors } = useForm<CreateLinkForm>({
  validationSchema: formSchema
})

const create = async (values: CreateLinkForm) => {
  isLoading.value = true
  const { title, ...rest } = values

  const cleanedValues = title === undefined ? rest : values

  const { data } = await useAsyncData(() =>
    useRepository.createShortLink(cleanedValues)
  ).finally(() => {
    isLoading.value = false
  })

  if (data.value) {
    createdLink.value = data.value.link
  } else {
    createdLink.value = ''
  }
}

const onSubmit = handleSubmit((values, action) => {
  create(values)
  action.resetForm()
})
</script>
