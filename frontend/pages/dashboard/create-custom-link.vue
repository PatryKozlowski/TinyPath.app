<template>
  <DashboardWrapper>
    <DashboardTitle title="Create custom link" />
  </DashboardWrapper>
</template>
<script setup lang="ts">
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
</script>
