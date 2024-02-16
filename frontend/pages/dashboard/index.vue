<template>
  <div>
    <h1>Dashboard</h1>
  </div>
</template>

<script setup lang="ts">
const isLogged = ref(true)

const router = useRouter()

definePageMeta({
  layout: 'dashboard'
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

onMounted(() => {
  if (!isLogged.value) {
    router.push('/auth/login')
  }
})
</script>
