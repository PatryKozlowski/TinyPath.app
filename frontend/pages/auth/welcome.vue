<template>
  <div
    class="w-full h-screen flex flex-col justify-center items-center bg-gray-700"
  >
    <div class="flex flex-col justify-center items-center">
      <Logo />
      <div class="flex items-center space-x-4 text-white">
        <div class="flex flex-col items-center mb-4">
          <div class="flex space-x-1 text-green-500 items-center">
            <Icon name="lucide:mail-check" class="w-5 h-5" />
            <p>Your email confirmation was successful</p>
          </div>
          <p>
            You will be redirected to the login page in {{ counter }} seconds
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
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
const router = useRouter()

let intervalId: NodeJS.Timeout

onMounted(() => {
  if (process.client) {
    intervalId = setInterval(() => {
      counter.value--
      if (counter.value === 0) {
        clearInterval(intervalId)
        router.push(REDIRETECT_ENDPOINT)
      }
    }, 1000)
  }
})

onUnmounted(() => {
  clearInterval(intervalId)
})
</script>
