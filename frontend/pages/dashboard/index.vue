<template>
  <DashboardWrapper>
    <DashboardTitle title="Dashboard" />
    <div
      class="w-32 h-32 bg-slate-500 rounded-xl hover:bg-slate-400 transition-colors duration-300 cursor-pointer"
    >
      <NuxtLink
        to="/dashboard/links"
        class="flex flex-col items-center justify-center w-full h-full text-white"
      >
        <Icon name="lucide:link" class="w-8 h-8 mb-4" />
        Latest link
      </NuxtLink>
    </div>
  </DashboardWrapper>
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
