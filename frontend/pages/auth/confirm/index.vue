<template>
  <Spinner v-if="isLoading" class="text-center" />
  <div
    class="flex items-center space-x-4 text-white"
    v-else-if="ConfirmEmailCode && message"
  >
    <div class="flex flex-col items-center mb-4 space-y-2">
      <p class="text-center">Email confirmation failed</p>
      <p>{{ message }}</p>
    </div>
  </div>
  <div v-else-if="!ConfirmEmailCode">
    <div class="flex items-center space-x-4 text-white">
      <div class="flex flex-col items-center mb-4 space-y-2">
        <p class="text-center">
          Please check your email and confirm your email address
        </p>
        <p>Your link is active for 15 minutes</p>
      </div>
    </div>
    <NuxtLink
      to="/auth/resend-email"
      class="flex items-center p-2 bg-violet-500 rounded-md text-white hover:bg-violet-400 transition-colors duration-300 mt-4 w-full justify-center"
    >
      <Icon name="lucide:mail" class="w-4 h-4 mr-2" />
      Resend email
    </NuxtLink>
  </div>
</template>
<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})

useHead({
  title: 'TinyPath - Confirmation',
  meta: [
    {
      name: 'description',
      content: 'Confirmation page'
    }
  ]
})

const ConfirmEmailCode = ref('')
const route = useRoute()
const runTimeConfig = useRuntimeConfig()
const message = ref('')
const isLoading = ref(true)
let intervalId: NodeJS.Timeout
const REDIRETECT_ENDPOINT = '/auth/welcome'

onMounted(async () => {
  isLoading.value = false
  ConfirmEmailCode.value = route.query.ConfirmEmailCode as string
  if (ConfirmEmailCode.value) {
    isLoading.value = true
    await $fetch(
      `${runTimeConfig.public.BASE_URL}/api/User/ConfirmEmailCommand/?ConfirmEmailCode=${ConfirmEmailCode.value}`,
      {
        method: 'GET',
        onResponse({ request, response, options }) {
          message.value = response._data.message
          if (process.client) {
            intervalId = setInterval(() => {
              clearInterval(intervalId)
              navigateTo(REDIRETECT_ENDPOINT)
            }, 2000)
          }
        },
        onResponseError({ request, response, options }) {
          message.value = response._data.error
        }
      }
    ).finally(() => {
      isLoading.value = false
    })
  }
})

onUnmounted(() => {
  clearInterval(intervalId)
})
</script>
