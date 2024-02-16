<template>
  <div class="flex w-full flex-col">
    <div class="w-full flex md:justify-center">
      <div
        class="md:px-24 md:py-9 md:w-3/4 md:max-w-screen-lg w-full space-y-4"
      >
        <div class="md:text-4xl font-semibold mb-14">
          <h1>Create a new link</h1>
        </div>
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
            <div class="text-slate-400 flex">
              <p>You can create</p>
              <p class="mx-1 text-black font-semibold">12</p>
              <p>more links this month</p>
            </div>
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
              class="w-full bg-violet-500 hover:bg-gray-700 transition-colors duration-300"
            >
              <template v-if="false">
                <Spinner />
              </template>
              <template v-else> Create ! </template>
            </Button>
          </div>
        </form>
        <Spinner v-if="isLoading" class="text-center" />
        <div
          v-else-if="!isLoading && createdLink.length > 0"
          class="bg-green-300/80 border-2 border-green-500 p-4 rounded-md"
        >
          <div class="flex items-center space-x-4">
            <p>Your short link:</p>
            <div class="flex space-x-2">
              <p class="text-lg font-semibold">
                {{ createdLink }}
              </p>
              <a
                :href="createdLink"
                target="_blank"
                rel="noopener noreferrer"
                class="text-green-500 cursor-pointer hover:text-slate-500 transition-all duration-300"
              >
                <Icon class="w-5 h-5" name="lucide:external-link" />
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'
import { useToast } from '@/components/ui/toast/use-toast'

interface CreateLink {
  url: string
  title?: string
}

interface CreateLinkResponse {
  link: string
}

definePageMeta({
  layout: 'dashboard'
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

const runTimeConfig = useRuntimeConfig()
const { toast } = useToast()
const createdLink = ref('')
const isLoading = ref(false)

const CREATE_ENDPOINT_API = `${runTimeConfig.public.BASE_URL}/api/Link/CreateShortLinkCommand`

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

const { handleSubmit, errors } = useForm<CreateLink>({
  validationSchema: formSchema
})

const create = async (values: CreateLink) => {
  isLoading.value = true
  const { title, ...rest } = values

  const cleanedValues = title === undefined ? rest : values

  await useAsyncData<CreateLinkResponse>(
    'createLink',
    async () =>
      await $fetch(CREATE_ENDPOINT_API, {
        method: 'POST',
        body: {
          url: cleanedValues.url
        },
        onResponseError({ request, response, options }) {
          createdLink.value = ''
          toast({
            description: response._data.error,
            variant: 'destructive'
          })
        },
        onResponse({ request, response, options }) {
          createdLink.value = response._data.link
        }
      })
  ).finally(() => {
    isLoading.value = false
  })
}

const onSubmit = handleSubmit((values, action) => {
  create(values)
  action.resetForm()
})
</script>
