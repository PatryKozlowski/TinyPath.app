<template>
  <div>
    <h1>Dashboard</h1>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

useHead({
  title: 'TinyPath - Dashboard',
  meta: [
    {
      name: 'description',
      content: 'Dashboard page'
    }
  ]
})

const authStore = useAuthStore()

onMounted(async () => {
  if (!authStore.user?.email) {
    await authStore.loadedUserData()
  }
})
</script>
