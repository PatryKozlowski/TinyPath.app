<template>
  <div class="md:text-4xl font-semibold mb-14">
    <h1>{{ title }}</h1>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  title: String
})

const { title } = props
</script>
