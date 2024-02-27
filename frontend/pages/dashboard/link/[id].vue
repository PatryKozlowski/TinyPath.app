<template>
  <DashboardWrapper class="h-full overflow-y-auto">
    <LinkDetails />
    <Separator class="my-6" />
    <LinkStatistics />
  </DashboardWrapper>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})
</script>
