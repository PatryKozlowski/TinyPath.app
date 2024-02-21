<template>
  <div class="flex flex-col items-center justify-center h-full">
    <p>Success</p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: ['auth']
})

useHead({
  title: 'TinyPath - Checkout | Success',
  meta: [
    {
      name: 'description',
      content: 'Checkout page'
    }
  ]
})

const authStore = useAuthStore()

onMounted(async () => {
  if (authStore.isAuthenticated) {
    await authStore.loadedUserData().finally(() => {
      navigateTo('/dashboard')
    })
  }
})
</script>
