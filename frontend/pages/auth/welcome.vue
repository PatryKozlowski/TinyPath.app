<template>
  <div class="flex items-center space-x-4 text-white">
    <div class="flex flex-col items-center mb-4">
      <div
        class="flex space-x-1 text-green-500 items-center text-sm md:text-base"
      >
        <Icon name="lucide:mail-check" class="w-5 h-5" />
        <p>Your email confirmation was successful</p>
      </div>
      <p class="text-center text-sm md:text-base">
        You will be redirected to the login page in {{ counter }} seconds
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})

useHead({
  title: 'TinyPath - Welcome',
  meta: [
    {
      name: 'description',
      content: 'Welcome page'
    }
  ]
})

const REDIRETECT_ENDPOINT = '/auth/login'

const tiemToRedirect = 5

const counter = ref(tiemToRedirect)

let intervalId: NodeJS.Timeout

onMounted(() => {
  if (process.client) {
    intervalId = setInterval(() => {
      counter.value--
      if (counter.value === 0) {
        clearInterval(intervalId)
        navigateTo(REDIRETECT_ENDPOINT)
      }
    }, 1000)
  }
})

onUnmounted(() => {
  clearInterval(intervalId)
})
</script>
